using System;
using System.Collections.Generic;

namespace FD.Macro;

/// <summary>
/// Exact port of V8's <c>Array.prototype.sort</c> as shipped in Node 22 (V8 12.4,
/// <c>third_party/v8/builtins/array-sort.tq</c>, a Torque TimSort derived from CPython's
/// listobject.c). It reproduces not only the final order but the exact sequence of
/// comparator calls (same argument pairs, same order), so even inconsistent comparators
/// (random results, NaN) behave exactly as in Node.
///
/// Semantics carried over from V8:
///  - the comparator result goes through <c>SortCompareUserFn</c>: NaN is replaced by +0,
///    and V8 only ever tests it with <c>&lt; 0</c> (and <c>&gt;= 0</c> in CountAndMakeRun),
///    so here every decision is <c>cmp(a, b) &lt; 0</c> (NaN, +0, -0 are all "not less");
///  - elements are copied into a separate work array, sorted there, and written back at the
///    end (the comparator never observes a partially sorted list);
///  - lists with fewer than 2 elements return without calling the comparator;
///  - the C# comparator must return what <c>ToNumber(cmp(a, b))</c> is in JS (a TS comparator
///    returning a boolean becomes 1/0, one returning undefined becomes NaN).
///
/// Not ported: V8's pre-pass that moves <c>undefined</c> / holes to the end without calling
/// the comparator — the C# lists are assumed to contain no JS <c>undefined</c>.
/// Smi-overflow guards in the gallop loops behave identically for lists up to 2^29 elements.
/// </summary>
public static class JsSort
{
    /// <summary>In-place <c>list.sort(cmp)</c> with V8's exact algorithm and comparator call sequence.</summary>
    public static void Sort<T>(List<T> list, Func<T, T, double> cmp)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (cmp == null) throw new ArgumentNullException(nameof(cmp));
        int length = list.Count;
        if (length < 2) return;
        T[] work = list.ToArray();
        new TimSort<T>(work, cmp).Run(length);
        for (int i = 0; i < length; i++) list[i] = work[i];
    }

    /// <summary>In-place <c>array.sort(cmp)</c> for a C# array (same algorithm as the List overload).</summary>
    public static void Sort<T>(T[] array, Func<T, T, double> cmp)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (cmp == null) throw new ArgumentNullException(nameof(cmp));
        int length = array.Length;
        if (length < 2) return;
        T[] work = (T[])array.Clone();
        new TimSort<T>(work, cmp).Run(length);
        Array.Copy(work, array, length);
    }

    /// <summary>TS <c>arr.slice().sort(cmp)</c> (or <c>arr.toSorted(cmp)</c>): copies, sorts, returns the copy.</summary>
    public static List<T> Sorted<T>(IEnumerable<T> src, Func<T, T, double> cmp)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));
        var list = new List<T>(src);
        Sort(list, cmp);
        return list;
    }

    /// <summary>SortState + the TimSort macros of array-sort.tq (names kept from the Torque source).</summary>
    private sealed class TimSort<T>
    {
        // The maximum number of entries in a SortState's pending-runs stack.
        private const int kMaxMergePending = 85;
        // When we get into galloping mode, we stay there until both runs win less
        // often than kMinGallop consecutive times.
        private const int kMinGallopWins = 7;
        // Default size of the temporary array; it only grows.
        private const int kSortStateTempSize = 32;

        private readonly T[] workArray;
        private readonly Func<T, T, double> cmp;
        private T[] tempArray = Array.Empty<T>();
        private int minGallop = kMinGallopWins;
        // pendingRuns[2*i] = base of run i, pendingRuns[2*i+1] = length of run i.
        private readonly int[] pendingRuns = new int[2 * kMaxMergePending];
        private int pendingRunsSize;

        public TimSort(T[] work, Func<T, T, double> cmp)
        {
            workArray = work;
            this.cmp = cmp;
        }

        /// <summary>
        /// sortState.Compare(x, y) &lt; 0. SortCompareUserFn maps NaN to +0 before V8 tests the
        /// sign, so "order &lt; 0" is exactly "cmp(x, y) &lt; 0" and "order &gt;= 0" is its negation.
        /// </summary>
        private bool Less(T x, T y) => cmp(x, y) < 0;

        // ---- pending-runs stack -------------------------------------------------------------

        private int GetPendingRunBase(int run) => pendingRuns[run << 1];
        private void SetPendingRunBase(int run, int value) => pendingRuns[run << 1] = value;
        private int GetPendingRunLength(int run) => pendingRuns[(run << 1) + 1];
        private void SetPendingRunLength(int run, int value) => pendingRuns[(run << 1) + 1] = value;

        private void PushRun(int runBase, int length)
        {
            int stackSize = pendingRunsSize;
            SetPendingRunBase(stackSize, runBase);
            SetPendingRunLength(stackSize, length);
            pendingRunsSize = stackSize + 1;
        }

        // Returns the temporary array and makes sure that it is big enough.
        private T[] GetTempArray(int requestedSize)
        {
            int minSize = Math.Max(kSortStateTempSize, requestedSize);
            if (tempArray.Length >= minSize) return tempArray;
            tempArray = new T[minSize];
            return tempArray;
        }

        // ---- ArrayTimSortImpl ---------------------------------------------------------------

        public void Run(int length)
        {
            if (length < 2) return;
            int remaining = length;

            // March over the array once, left to right, finding natural runs,
            // and extending short natural runs to minrun elements.
            int low = 0;
            int minRunLength = ComputeMinRunLength(remaining);
            while (remaining != 0)
            {
                int currentRunLength = CountAndMakeRun(low, low + remaining);

                // If the run is short, extend it to min(minRunLength, remaining).
                if (currentRunLength < minRunLength)
                {
                    int forcedRunLength = Math.Min(minRunLength, remaining);
                    BinaryInsertionSort(low, low + currentRunLength, low + forcedRunLength);
                    currentRunLength = forcedRunLength;
                }

                // Push run onto pending-runs stack, and maybe merge.
                PushRun(low, currentRunLength);
                MergeCollapse();

                // Advance to find next run.
                low += currentRunLength;
                remaining -= currentRunLength;
            }

            MergeForceCollapse();
        }

        // If n < 64, return n. Else return k, 32 <= k <= 64, such that n/k is close to,
        // but strictly less than, an exact power of 2.
        private static int ComputeMinRunLength(int nArg)
        {
            int n = nArg;
            int r = 0; // Becomes 1 if any 1 bits are shifted off.
            while (n >= 64)
            {
                r |= n & 1;
                n >>= 1;
            }
            return n + r;
        }

        // [low, high) is sorted via binary insertion; [low, startArg) is already sorted.
        private void BinaryInsertionSort(int low, int startArg, int high)
        {
            T[] a = workArray;
            int start = low == startArg ? (startArg + 1) : startArg;

            for (; start < high; ++start)
            {
                // Set left to where a[start] belongs.
                int left = low;
                int right = start;
                T pivot = a[right];

                // Invariants: pivot >= all in [low, left); pivot < all in [right, start).
                while (left < right)
                {
                    int mid = left + ((right - left) >> 1);
                    if (Less(pivot, a[mid]))
                        right = mid;
                    else
                        left = mid + 1;
                }

                // Slide over to make room.
                for (int p = start; p > left; --p) a[p] = a[p - 1];
                a[left] = pivot;
            }
        }

        // Length of the run beginning at lowArg in [lowArg, high): either
        // a[low] <= a[low + 1] <= ... (checked as !(cur < prev)) or strictly descending
        // a[low] > a[low + 1] > ... (checked as cur < prev); descending runs are reversed.
        private int CountAndMakeRun(int lowArg, int high)
        {
            T[] a = workArray;
            int low = lowArg + 1;
            if (low == high) return 1;

            int runLength = 2;

            T elementLow = a[low];
            T elementLowPred = a[low - 1];
            bool isDescending = Less(elementLow, elementLowPred); // order < 0

            T previousElement = elementLow;
            for (int idx = low + 1; idx < high; ++idx)
            {
                T currentElement = a[idx];
                bool lt = Less(currentElement, previousElement);

                if (isDescending)
                {
                    if (!lt) break;  // order >= 0
                }
                else
                {
                    if (lt) break;   // order < 0
                }

                previousElement = currentElement;
                ++runLength;
            }

            if (isDescending) ReverseRange(a, lowArg, lowArg + runLength);

            return runLength;
        }

        private static void ReverseRange(T[] array, int from, int to)
        {
            int low = from;
            int high = to - 1;
            while (low < high)
            {
                T elementLow = array[low];
                T elementHigh = array[high];
                array[low++] = elementHigh;
                array[high--] = elementLow;
            }
        }

        // Returns true iff run_length(n - 2) > run_length(n - 1) + run_length(n).
        private bool RunInvariantEstablished(int n)
        {
            if (n < 2) return true;
            int runLengthN = GetPendingRunLength(n);
            int runLengthNM = GetPendingRunLength(n - 1);
            int runLengthNMM = GetPendingRunLength(n - 2);
            return runLengthNMM > runLengthNM + runLengthN;
        }

        // Merges adjacent runs until the stack invariants are re-established:
        //   1. run_length(i - 3) > run_length(i - 2) + run_length(i - 1)
        //   2. run_length(i - 2) > run_length(i - 1)
        private void MergeCollapse()
        {
            while (pendingRunsSize > 1)
            {
                int n = pendingRunsSize - 2;

                if (!RunInvariantEstablished(n + 1) || !RunInvariantEstablished(n))
                {
                    if (GetPendingRunLength(n - 1) < GetPendingRunLength(n + 1)) --n;
                    MergeAt(n);
                }
                else if (GetPendingRunLength(n) <= GetPendingRunLength(n + 1))
                {
                    MergeAt(n);
                }
                else
                {
                    break;
                }
            }
        }

        // Regardless of invariants, merge all runs on the stack until only one remains.
        private void MergeForceCollapse()
        {
            while (pendingRunsSize > 1)
            {
                int n = pendingRunsSize - 2;
                if (n > 0 && GetPendingRunLength(n - 1) < GetPendingRunLength(n + 1)) --n;
                MergeAt(n);
            }
        }

        // Merges the two runs at stack indices i and i + 1.
        private void MergeAt(int i)
        {
            int stackSize = pendingRunsSize;
            T[] a = workArray;

            int baseA = GetPendingRunBase(i);
            int lengthA = GetPendingRunLength(i);
            int baseB = GetPendingRunBase(i + 1);
            int lengthB = GetPendingRunLength(i + 1);

            // Record the length of the combined runs; if i is the 3rd-last run now,
            // also slide over the last run (which isn't involved in this merge).
            SetPendingRunLength(i, lengthA + lengthB);
            if (i == stackSize - 3)
            {
                int runBase = GetPendingRunBase(i + 2);
                int runLength = GetPendingRunLength(i + 2);
                SetPendingRunBase(i + 1, runBase);
                SetPendingRunLength(i + 1, runLength);
            }
            pendingRunsSize = stackSize - 1;

            // Where does b start in a? Elements in a before that can be ignored.
            T keyRight = a[baseB];
            int k = GallopRight(a, keyRight, baseA, lengthA, 0);

            baseA += k;
            lengthA -= k;
            if (lengthA == 0) return;

            // Where does a end in b? Elements in b after that can be ignored.
            T keyLeft = a[baseA + lengthA - 1];
            lengthB = GallopLeft(a, keyLeft, baseB, lengthB, lengthB - 1);
            if (lengthB == 0) return;

            // Merge what remains of the runs, using a temp array with min(lengthA, lengthB) elements.
            if (lengthA <= lengthB)
                MergeLow(baseA, lengthA, baseB, lengthB);
            else
                MergeHigh(baseA, lengthA, baseB, lengthB);
        }

        // Returns offset in 0..length such that array[base + offset - 1] < key <= array[base + offset]
        // (leftmost position). Comparator calls are Compare(array[...], key).
        private int GallopLeft(T[] array, T key, int runBase, int length, int hint)
        {
            int lastOfs = 0;
            int offset = 1;

            T baseHintElement = array[runBase + hint];

            if (Less(baseHintElement, key))
            {
                // a[base + hint] < key: gallop right, until
                // a[base + hint + lastOfs] < key <= a[base + hint + offset].
                int maxOfs = length - hint;
                while (offset < maxOfs)
                {
                    T offsetElement = array[runBase + hint + offset];
                    // a[base + hint + offset] >= key? Break.
                    if (!Less(offsetElement, key)) break;

                    lastOfs = offset;
                    offset = (offset << 1) + 1;

                    // Integer overflow.
                    if (offset <= 0) offset = maxOfs;
                }

                if (offset > maxOfs) offset = maxOfs;

                // Translate back to positive offsets relative to base.
                lastOfs += hint;
                offset += hint;
            }
            else
            {
                // key <= a[base + hint]: gallop left, until
                // a[base + hint - offset] < key <= a[base + hint - lastOfs].
                int maxOfs = hint + 1;
                while (offset < maxOfs)
                {
                    T offsetElement = array[runBase + hint - offset];
                    if (Less(offsetElement, key)) break;

                    lastOfs = offset;
                    offset = (offset << 1) + 1;

                    // Integer overflow.
                    if (offset <= 0) offset = maxOfs;
                }

                if (offset > maxOfs) offset = maxOfs;

                // Translate back to positive offsets relative to base.
                int tmp = lastOfs;
                lastOfs = hint - offset;
                offset = hint - tmp;
            }

            // Now a[base+lastOfs] < key <= a[base+offset]: binary search with invariant
            // a[base + lastOfs - 1] < key <= a[base + offset].
            lastOfs++;
            while (lastOfs < offset)
            {
                int m = lastOfs + ((offset - lastOfs) >> 1);
                if (Less(array[runBase + m], key))
                    lastOfs = m + 1; // a[base + m] < key.
                else
                    offset = m;      // key <= a[base + m].
            }
            return offset;
        }

        // Like GallopLeft, but returns the position to the right of the rightmost equal element:
        // array[base + offset - 1] <= key < array[base + offset]. Comparator calls are Compare(key, array[...]).
        private int GallopRight(T[] array, T key, int runBase, int length, int hint)
        {
            int lastOfs = 0;
            int offset = 1;

            T baseHintElement = array[runBase + hint];

            if (Less(key, baseHintElement))
            {
                // key < a[base + hint]: gallop left, until
                // a[base + hint - offset] <= key < a[base + hint - lastOfs].
                int maxOfs = hint + 1;
                while (offset < maxOfs)
                {
                    T offsetElement = array[runBase + hint - offset];
                    if (!Less(key, offsetElement)) break;

                    lastOfs = offset;
                    offset = (offset << 1) + 1;

                    // Integer overflow.
                    if (offset <= 0) offset = maxOfs;
                }

                if (offset > maxOfs) offset = maxOfs;

                // Translate back to positive offsets relative to base.
                int tmp = lastOfs;
                lastOfs = hint - offset;
                offset = hint - tmp;
            }
            else
            {
                // a[base + hint] <= key: gallop right, until
                // a[base + hint + lastOfs] <= key < a[base + hint + offset].
                int maxOfs = length - hint;
                while (offset < maxOfs)
                {
                    T offsetElement = array[runBase + hint + offset];
                    // a[base + hint + ofs] <= key.
                    if (Less(key, offsetElement)) break;

                    lastOfs = offset;
                    offset = (offset << 1) + 1;

                    // Integer overflow.
                    if (offset <= 0) offset = maxOfs;
                }

                if (offset > maxOfs) offset = maxOfs;

                // Translate back to positive offsets relative to base.
                lastOfs += hint;
                offset += hint;
            }

            // Now a[base + lastOfs] <= key < a[base + ofs]: binary search.
            lastOfs++;
            while (lastOfs < offset)
            {
                int m = lastOfs + ((offset - lastOfs) >> 1);
                if (Less(key, array[runBase + m]))
                    offset = m;      // key < a[base + m].
                else
                    lastOfs = m + 1; // a[base + m] <= key.
            }
            return offset;
        }

        // Merge the lengthA elements starting at baseA with the lengthB elements starting at
        // baseB (baseA + lengthA == baseB) in a stable way, in-place; lengthA <= lengthB.
        // array[baseB] < array[baseA] and array[baseA + lengthA - 1] belongs at the end.
        private void MergeLow(int baseA, int lengthAArg, int baseB, int lengthBArg)
        {
            int lengthA = lengthAArg;
            int lengthB = lengthBArg;

            T[] work = workArray;
            T[] temp = GetTempArray(lengthA);
            Array.Copy(work, baseA, temp, 0, lengthA);

            int dest = baseA;
            int cursorTemp = 0;
            int cursorB = baseB;

            work[dest++] = work[cursorB++];

            if (--lengthB == 0) goto Succeed;
            if (lengthA == 1) goto CopyB;

            int minGallopLocal = minGallop;
            while (true)
            {
                int nofWinsA = 0; // # of times A won in a row.
                int nofWinsB = 0; // # of times B won in a row.

                // Do the straightforward thing until (if ever) one run appears to win consistently.
                while (true)
                {
                    if (Less(work[cursorB], temp[cursorTemp]))
                    {
                        work[dest++] = work[cursorB++];

                        ++nofWinsB;
                        --lengthB;
                        nofWinsA = 0;

                        if (lengthB == 0) goto Succeed;
                        if (nofWinsB >= minGallopLocal) break;
                    }
                    else
                    {
                        work[dest++] = temp[cursorTemp++];

                        ++nofWinsA;
                        --lengthA;
                        nofWinsB = 0;

                        if (lengthA == 1) goto CopyB;
                        if (nofWinsA >= minGallopLocal) break;
                    }
                }

                // One run is winning so consistently that galloping may be a huge win.
                ++minGallopLocal;
                bool firstIteration = true;
                while (nofWinsA >= kMinGallopWins || nofWinsB >= kMinGallopWins || firstIteration)
                {
                    firstIteration = false;

                    minGallopLocal = Math.Max(1, minGallopLocal - 1);
                    minGallop = minGallopLocal;

                    nofWinsA = GallopRight(temp, work[cursorB], cursorTemp, lengthA, 0);

                    if (nofWinsA > 0)
                    {
                        Array.Copy(temp, cursorTemp, work, dest, nofWinsA);
                        dest += nofWinsA;
                        cursorTemp += nofWinsA;
                        lengthA -= nofWinsA;

                        if (lengthA == 1) goto CopyB;

                        // lengthA == 0 is impossible now if the comparison function is
                        // consistent, but we can't assume that it is.
                        if (lengthA == 0) goto Succeed;
                    }
                    work[dest++] = work[cursorB++];
                    if (--lengthB == 0) goto Succeed;

                    nofWinsB = GallopLeft(work, temp[cursorTemp], cursorB, lengthB, 0);
                    if (nofWinsB > 0)
                    {
                        Array.Copy(work, cursorB, work, dest, nofWinsB); // overlapping, memmove semantics

                        dest += nofWinsB;
                        cursorB += nofWinsB;
                        lengthB -= nofWinsB;

                        if (lengthB == 0) goto Succeed;
                    }
                    work[dest++] = temp[cursorTemp++];
                    if (--lengthA == 1) goto CopyB;
                }
                ++minGallopLocal; // Penalize it for leaving galloping mode
                minGallop = minGallopLocal;
            }

        Succeed:
            if (lengthA > 0) Array.Copy(temp, cursorTemp, work, dest, lengthA);
            return;

        CopyB:
            // The last element of run A belongs at the end of the merge.
            Array.Copy(work, cursorB, work, dest, lengthB);
            work[dest + lengthB] = temp[cursorTemp];
        }

        // Merge the lengthA elements starting at baseA with the lengthB elements starting at
        // baseB, backwards, in a stable way; lengthA >= lengthB.
        private void MergeHigh(int baseA, int lengthAArg, int baseB, int lengthBArg)
        {
            int lengthA = lengthAArg;
            int lengthB = lengthBArg;

            T[] work = workArray;
            T[] temp = GetTempArray(lengthB);
            Array.Copy(work, baseB, temp, 0, lengthB);

            // MergeHigh merges the two runs backwards.
            int dest = baseB + lengthB - 1;
            int cursorTemp = lengthB - 1;
            int cursorA = baseA + lengthA - 1;

            work[dest--] = work[cursorA--];

            if (--lengthA == 0) goto Succeed;
            if (lengthB == 1) goto CopyA;

            int minGallopLocal = minGallop;
            while (true)
            {
                int nofWinsA = 0; // # of times A won in a row.
                int nofWinsB = 0; // # of times B won in a row.

                // Do the straightforward thing until (if ever) one run appears to win consistently.
                while (true)
                {
                    if (Less(temp[cursorTemp], work[cursorA]))
                    {
                        work[dest--] = work[cursorA--];

                        ++nofWinsA;
                        --lengthA;
                        nofWinsB = 0;

                        if (lengthA == 0) goto Succeed;
                        if (nofWinsA >= minGallopLocal) break;
                    }
                    else
                    {
                        work[dest--] = temp[cursorTemp--];

                        ++nofWinsB;
                        --lengthB;
                        nofWinsA = 0;

                        if (lengthB == 1) goto CopyA;
                        if (nofWinsB >= minGallopLocal) break;
                    }
                }

                // One run is winning so consistently that galloping may be a huge win.
                ++minGallopLocal;
                bool firstIteration = true;
                while (nofWinsA >= kMinGallopWins || nofWinsB >= kMinGallopWins || firstIteration)
                {
                    firstIteration = false;

                    minGallopLocal = Math.Max(1, minGallopLocal - 1);
                    minGallop = minGallopLocal;

                    int k = GallopRight(work, temp[cursorTemp], baseA, lengthA, lengthA - 1);
                    nofWinsA = lengthA - k;

                    if (nofWinsA > 0)
                    {
                        dest -= nofWinsA;
                        cursorA -= nofWinsA;
                        Array.Copy(work, cursorA + 1, work, dest + 1, nofWinsA); // overlapping, memmove semantics

                        lengthA -= nofWinsA;
                        if (lengthA == 0) goto Succeed;
                    }
                    work[dest--] = temp[cursorTemp--];
                    if (--lengthB == 1) goto CopyA;

                    k = GallopLeft(temp, work[cursorA], 0, lengthB, lengthB - 1);
                    nofWinsB = lengthB - k;

                    if (nofWinsB > 0)
                    {
                        dest -= nofWinsB;
                        cursorTemp -= nofWinsB;
                        Array.Copy(temp, cursorTemp + 1, work, dest + 1, nofWinsB);

                        lengthB -= nofWinsB;
                        if (lengthB == 1) goto CopyA;

                        // lengthB == 0 is impossible now if the comparison function is
                        // consistent, but we can't assume that it is.
                        if (lengthB == 0) goto Succeed;
                    }
                    work[dest--] = work[cursorA--];
                    if (--lengthA == 0) goto Succeed;
                }
                ++minGallopLocal;
                minGallop = minGallopLocal;
            }

        Succeed:
            if (lengthB > 0) Array.Copy(temp, 0, work, dest - (lengthB - 1), lengthB);
            return;

        CopyA:
            // The first element of run B belongs at the front of the merge.
            dest -= lengthA;
            cursorA -= lengthA;
            Array.Copy(work, cursorA + 1, work, dest + 1, lengthA);
            work[dest] = temp[cursorTemp];
        }
    }
}
