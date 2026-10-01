using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FD.Macro;

/// <summary>
/// Number -> string conversions with exact ECMAScript / V8 semantics (backs
/// <see cref="JsMath.Str"/> and <see cref="JsMath.ToFixed"/>).
///
/// Shortest digits: V8 uses Grisu3 with a bignum fallback (DoubleToAscii DTOA_SHORTEST).
/// Its result is fully determined: the shortest digit string inside the rounding interval
/// (interval bounds included iff the significand is even), the one closest to the exact
/// value among those, exact ties broken toward the even digit. Ryu (Ulf Adams, 2018)
/// computes exactly the same thing, so this is a port of the reference ryu/d2s.c
/// (full-table variant, tables generated at startup with the same formulas as
/// d2s_full_table.h).
/// </summary>
internal static class JsNumberFormat
{
    // ======================================================================================
    // Ryu tables (d2s_full_table.h): DOUBLE_POW5_INV_SPLIT[342][2], DOUBLE_POW5_SPLIT[326][2]
    // stored as {low64, high64} pairs.
    // ======================================================================================

    private const int DOUBLE_POW5_INV_BITCOUNT = 125;
    private const int DOUBLE_POW5_BITCOUNT = 125;
    private const int DOUBLE_POW5_INV_TABLE_SIZE = 342;
    private const int DOUBLE_POW5_TABLE_SIZE = 326;

    private static readonly ulong[] Pow5InvSplit = new ulong[DOUBLE_POW5_INV_TABLE_SIZE * 2];
    private static readonly ulong[] Pow5Split = new ulong[DOUBLE_POW5_TABLE_SIZE * 2];

    static JsNumberFormat()
    {
        // Same generator as Ryu's PrintDoubleLookupTable:
        //   POW5_SPLIT[i]     = 5^i shifted to exactly 125 bits
        //   POW5_INV_SPLIT[i] = floor(2^(bitlen(5^i) - 1 + 125) / 5^i) + 1
        BigInteger mask64 = (BigInteger.One << 64) - BigInteger.One;
        BigInteger pow = BigInteger.One;
        for (int i = 0; i < DOUBLE_POW5_INV_TABLE_SIZE; i++)
        {
            int pow5len = (int)pow.GetBitLength();
            if (i < DOUBLE_POW5_TABLE_SIZE)
            {
                BigInteger v = pow5len >= DOUBLE_POW5_BITCOUNT
                    ? pow >> (pow5len - DOUBLE_POW5_BITCOUNT)
                    : pow << (DOUBLE_POW5_BITCOUNT - pow5len);
                Pow5Split[2 * i] = (ulong)(v & mask64);
                Pow5Split[2 * i + 1] = (ulong)(v >> 64);
            }
            BigInteger inv = (BigInteger.One << (pow5len - 1 + DOUBLE_POW5_INV_BITCOUNT)) / pow + BigInteger.One;
            Pow5InvSplit[2 * i] = (ulong)(inv & mask64);
            Pow5InvSplit[2 * i + 1] = (ulong)(inv >> 64);
            pow *= 5;
        }
    }

    // ======================================================================================
    // Ryu helpers (common.h / d2s_intrinsics.h)
    // ======================================================================================

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Pow5Bits(int e) => (int)((((uint)e * 1217359u) >> 19) + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Log10Pow2(int e) => ((uint)e * 78913u) >> 18;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Log10Pow5(int e) => ((uint)e * 732923u) >> 20;

    private static uint Pow5Factor(ulong value)
    {
        const ulong m_inv_5 = 14757395258967641293UL; // 5 * m_inv_5 = 1 (mod 2^64)
        const ulong n_div_5 = 3689348814741910323UL;  // #{ n | n = 0 (mod 2^64) } = 2^64 / 5
        uint count = 0;
        for (;;)
        {
            value = unchecked(value * m_inv_5);
            if (value > n_div_5) break;
            ++count;
        }
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MultipleOfPowerOf5(ulong value, uint p) => Pow5Factor(value) >= p;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MultipleOfPowerOf2(ulong value, uint p) => (value & ((1UL << (int)p) - 1)) == 0;

    /// <summary>(m * mul) >> j with mul a 128-bit table entry, 64 &lt; j &lt; 128 (Ryu mulShift64).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong MulShift64(ulong m, ulong mulLo, ulong mulHi, int j)
    {
        ulong high1 = Math.BigMul(m, mulHi, out ulong low1);
        ulong high0 = Math.BigMul(m, mulLo, out _);
        ulong sum = unchecked(high0 + low1);
        if (sum < high0) ++high1; // overflow into high1
        int dist = j - 64;        // 0 < dist < 64
        return (high1 << (64 - dist)) | (sum >> dist);
    }

    private static int DecimalLength17(ulong v)
    {
        if (v >= 10000000000000000UL) return 17;
        if (v >= 1000000000000000UL) return 16;
        if (v >= 100000000000000UL) return 15;
        if (v >= 10000000000000UL) return 14;
        if (v >= 1000000000000UL) return 13;
        if (v >= 100000000000UL) return 12;
        if (v >= 10000000000UL) return 11;
        if (v >= 1000000000UL) return 10;
        if (v >= 100000000UL) return 9;
        if (v >= 10000000UL) return 8;
        if (v >= 1000000UL) return 7;
        if (v >= 100000UL) return 6;
        if (v >= 10000UL) return 5;
        if (v >= 1000UL) return 4;
        if (v >= 100UL) return 3;
        if (v >= 10UL) return 2;
        return 1;
    }

    // ======================================================================================
    // Ryu d2d: shortest decimal (output * 10^exp10) for a finite, non-zero double.
    // ======================================================================================

    private static void ShortestDecimal(ulong ieeeMantissa, uint ieeeExponent, out ulong output, out int exp10)
    {
        const int DOUBLE_MANTISSA_BITS = 52;
        const int DOUBLE_BIAS = 1023;

        // d2d_small_int: integers in [1, 2^53) are their own shortest representation.
        {
            int e2s = (int)ieeeExponent - DOUBLE_BIAS - DOUBLE_MANTISSA_BITS;
            if (ieeeExponent != 0 && e2s <= 0 && e2s >= -52)
            {
                ulong m2s = (1UL << DOUBLE_MANTISSA_BITS) | ieeeMantissa;
                ulong mask = (1UL << -e2s) - 1;
                if ((m2s & mask) == 0)
                {
                    ulong mant = m2s >> -e2s;
                    int ex = 0;
                    for (;;)
                    {
                        ulong q = mant / 10;
                        uint rem = (uint)(mant - 10 * q);
                        if (rem != 0) break;
                        mant = q;
                        ++ex;
                    }
                    output = mant;
                    exp10 = ex;
                    return;
                }
            }
        }

        int e2;
        ulong m2;
        if (ieeeExponent == 0)
        {
            // We subtract 2 so that the bounds computation has 2 additional bits.
            e2 = 1 - DOUBLE_BIAS - DOUBLE_MANTISSA_BITS - 2;
            m2 = ieeeMantissa;
        }
        else
        {
            e2 = (int)ieeeExponent - DOUBLE_BIAS - DOUBLE_MANTISSA_BITS - 2;
            m2 = (1UL << DOUBLE_MANTISSA_BITS) | ieeeMantissa;
        }
        bool even = (m2 & 1) == 0;
        bool acceptBounds = even;

        // Step 2: Determine the interval of valid decimal representations.
        ulong mv = 4 * m2;
        uint mmShift = (ieeeMantissa != 0 || ieeeExponent <= 1) ? 1u : 0u;
        // mp = 4 * m2 + 2; mm = mv - 1 - mmShift

        // Step 3: Convert to a decimal power base using 128-bit arithmetic.
        ulong vr, vp, vm;
        int e10;
        bool vmIsTrailingZeros = false;
        bool vrIsTrailingZeros = false;
        if (e2 >= 0)
        {
            uint q = Log10Pow2(e2) - (e2 > 3 ? 1u : 0u);
            e10 = (int)q;
            int k = DOUBLE_POW5_INV_BITCOUNT + Pow5Bits((int)q) - 1;
            int i = -e2 + (int)q + k;
            ulong mulLo = Pow5InvSplit[2 * q], mulHi = Pow5InvSplit[2 * q + 1];
            vr = MulShift64(4 * m2, mulLo, mulHi, i);
            vp = MulShift64(4 * m2 + 2, mulLo, mulHi, i);
            vm = MulShift64(4 * m2 - 1 - mmShift, mulLo, mulHi, i);
            if (q <= 21)
            {
                // Only one of mp, mv, and mm can be a multiple of 5, if any.
                uint mvMod5 = (uint)(mv - 5 * (mv / 5));
                if (mvMod5 == 0)
                {
                    vrIsTrailingZeros = MultipleOfPowerOf5(mv, q);
                }
                else if (acceptBounds)
                {
                    vmIsTrailingZeros = MultipleOfPowerOf5(mv - 1 - mmShift, q);
                }
                else
                {
                    vp -= MultipleOfPowerOf5(mv + 2, q) ? 1UL : 0UL;
                }
            }
        }
        else
        {
            uint q = Log10Pow5(-e2) - (-e2 > 1 ? 1u : 0u);
            e10 = (int)q + e2;
            int i = -e2 - (int)q;
            int k = Pow5Bits(i) - DOUBLE_POW5_BITCOUNT;
            int j = (int)q - k;
            ulong mulLo = Pow5Split[2 * i], mulHi = Pow5Split[2 * i + 1];
            vr = MulShift64(4 * m2, mulLo, mulHi, j);
            vp = MulShift64(4 * m2 + 2, mulLo, mulHi, j);
            vm = MulShift64(4 * m2 - 1 - mmShift, mulLo, mulHi, j);
            if (q <= 1)
            {
                // {vr,vp,vm} is trailing zeros if {mv,mp,mm} has at least q trailing 0 bits.
                // mv = 4 * m2, so it always has at least two trailing 0 bits.
                vrIsTrailingZeros = true;
                if (acceptBounds)
                {
                    // mm = mv - 1 - mmShift, so it has 1 trailing 0 bit iff mmShift == 1.
                    vmIsTrailingZeros = mmShift == 1;
                }
                else
                {
                    // mp = mv + 2, so it always has at least one trailing 0 bit.
                    --vp;
                }
            }
            else if (q < 63)
            {
                vrIsTrailingZeros = MultipleOfPowerOf2(mv, q);
            }
        }

        // Step 4: Find the shortest decimal representation in the interval of valid representations.
        int removed = 0;
        uint lastRemovedDigit = 0;
        if (vmIsTrailingZeros || vrIsTrailingZeros)
        {
            // General case, which happens rarely (~0.7%).
            for (;;)
            {
                ulong vpDiv10 = vp / 10;
                ulong vmDiv10 = vm / 10;
                if (vpDiv10 <= vmDiv10) break;
                uint vmMod10 = (uint)(vm - 10 * vmDiv10);
                ulong vrDiv10 = vr / 10;
                uint vrMod10 = (uint)(vr - 10 * vrDiv10);
                vmIsTrailingZeros &= vmMod10 == 0;
                vrIsTrailingZeros &= lastRemovedDigit == 0;
                lastRemovedDigit = vrMod10;
                vr = vrDiv10;
                vp = vpDiv10;
                vm = vmDiv10;
                ++removed;
            }
            if (vmIsTrailingZeros)
            {
                for (;;)
                {
                    ulong vmDiv10 = vm / 10;
                    uint vmMod10 = (uint)(vm - 10 * vmDiv10);
                    if (vmMod10 != 0) break;
                    ulong vpDiv10 = vp / 10;
                    ulong vrDiv10 = vr / 10;
                    uint vrMod10 = (uint)(vr - 10 * vrDiv10);
                    vrIsTrailingZeros &= lastRemovedDigit == 0;
                    lastRemovedDigit = vrMod10;
                    vr = vrDiv10;
                    vp = vpDiv10;
                    vm = vmDiv10;
                    ++removed;
                }
            }
            if (vrIsTrailingZeros && lastRemovedDigit == 5 && vr % 2 == 0)
            {
                // Round even if the exact number is .....50..0.
                lastRemovedDigit = 4;
            }
            // We need to take vr + 1 if vr is outside bounds or we need to round up.
            output = vr + (((vr == vm && (!acceptBounds || !vmIsTrailingZeros)) || lastRemovedDigit >= 5) ? 1UL : 0UL);
        }
        else
        {
            // Specialized for the common case (~99.3%).
            bool roundUp = false;
            ulong vpDiv100 = vp / 100;
            ulong vmDiv100 = vm / 100;
            if (vpDiv100 > vmDiv100)
            {   // Optimization: remove two digits at a time.
                ulong vrDiv100 = vr / 100;
                uint vrMod100 = (uint)(vr - 100 * vrDiv100);
                roundUp = vrMod100 >= 50;
                vr = vrDiv100;
                vp = vpDiv100;
                vm = vmDiv100;
                removed += 2;
            }
            for (;;)
            {
                ulong vpDiv10 = vp / 10;
                ulong vmDiv10 = vm / 10;
                if (vpDiv10 <= vmDiv10) break;
                ulong vrDiv10 = vr / 10;
                uint vrMod10 = (uint)(vr - 10 * vrDiv10);
                roundUp = vrMod10 >= 5;
                vr = vrDiv10;
                vp = vpDiv10;
                vm = vmDiv10;
                ++removed;
            }
            // We need to take vr + 1 if vr is outside bounds or we need to round up.
            output = vr + ((vr == vm || roundUp) ? 1UL : 0UL);
        }
        exp10 = e10 + removed;
    }

    // ======================================================================================
    // Number::toString (radix 10) — V8 DoubleToCString layout (ECMA-262 Number::toString)
    // ======================================================================================

    internal static string ToJsString(double v)
    {
        if (double.IsNaN(v)) return "NaN";
        if (v == 0.0) return "0"; // +0 and -0
        if (double.IsInfinity(v)) return v > 0 ? "Infinity" : "-Infinity";

        Span<char> buf = stackalloc char[40];
        int pos = 0;

        // V8 fast path (IsInt32Double -> IntToCString); same digits as the general path.
        if (v >= -2147483648.0 && v <= 2147483647.0)
        {
            int iv = (int)v;
            if (iv == v)
            {
                pos = WriteInt64(buf, 0, iv);
                return new string(buf.Slice(0, pos));
            }
        }

        ulong bits = (ulong)BitConverter.DoubleToInt64Bits(v);
        bool negative = (long)bits < 0;
        ulong ieeeMantissa = bits & 0x000FFFFFFFFFFFFFUL;
        uint ieeeExponent = (uint)((bits >> 52) & 0x7FF);
        ShortestDecimal(ieeeMantissa, ieeeExponent, out ulong output, out int exp10);

        // Digits of output (k of them, no leading/trailing zeros issues: output has no trailing zeros
        // except in the small-int path where they were stripped too).
        Span<char> digits = stackalloc char[20];
        int k = DecimalLength17(output);
        {
            ulong o = output;
            for (int d = k - 1; d >= 0; d--)
            {
                ulong q = o / 10;
                digits[d] = (char)('0' + (int)(o - q * 10));
                o = q;
            }
        }
        int n = exp10 + k; // value = 0.d1d2...dk * 10^n

        if (negative) buf[pos++] = '-';
        if (k <= n && n <= 21)
        {
            // ECMA-262 Number::toString step 6: digits followed by n-k zeros.
            for (int d = 0; d < k; d++) buf[pos++] = digits[d];
            for (int z = 0; z < n - k; z++) buf[pos++] = '0';
        }
        else if (0 < n && n <= 21)
        {
            // step 7: ddd.ddd
            for (int d = 0; d < n; d++) buf[pos++] = digits[d];
            buf[pos++] = '.';
            for (int d = n; d < k; d++) buf[pos++] = digits[d];
        }
        else if (-6 < n && n <= 0)
        {
            // step 8: 0.000ddd
            buf[pos++] = '0';
            buf[pos++] = '.';
            for (int z = 0; z < -n; z++) buf[pos++] = '0';
            for (int d = 0; d < k; d++) buf[pos++] = digits[d];
        }
        else
        {
            // steps 9-10: d[.ddd]e(+|-)exponent
            buf[pos++] = digits[0];
            if (k != 1)
            {
                buf[pos++] = '.';
                for (int d = 1; d < k; d++) buf[pos++] = digits[d];
            }
            buf[pos++] = 'e';
            int e = n - 1;
            buf[pos++] = e >= 0 ? '+' : '-';
            pos = WriteInt64(buf, pos, e >= 0 ? e : -e);
        }
        return new string(buf.Slice(0, pos));
    }

    /// <summary>Writes a signed integer in decimal (culture-invariant) at buf[pos], returns new pos.</summary>
    private static int WriteInt64(Span<char> buf, int pos, long value)
    {
        ulong u;
        if (value < 0)
        {
            buf[pos++] = '-';
            u = (ulong)(-(value + 1)) + 1; // safe for long.MinValue
        }
        else
        {
            u = (ulong)value;
        }
        int start = pos;
        do
        {
            ulong q = u / 10;
            buf[pos++] = (char)('0' + (int)(u - q * 10));
            u = q;
        } while (u != 0);
        buf.Slice(start, pos - start).Reverse();
        return pos;
    }

    // ======================================================================================
    // Number.prototype.toFixed — V8 DoubleToFixedCString (FastFixedDtoa rounds the exact
    // binary value half-up, i.e. ties pick the larger n, as the spec requires).
    // ======================================================================================

    private static readonly UInt128[] Pow10U128 = BuildPow10U128();

    private static UInt128[] BuildPow10U128()
    {
        var t = new UInt128[18];
        UInt128 p = UInt128.One;
        for (int i = 0; i < t.Length; i++) { t[i] = p; p *= 10; }
        return t;
    }

    internal static string ToFixed(double x, int f)
    {
        if (f < 0 || f > 100)
            throw new ArgumentOutOfRangeException(nameof(f), "toFixed() digits argument must be between 0 and 100");
        if (double.IsNaN(x)) return "NaN";
        if (double.IsInfinity(x)) return x > 0 ? "Infinity" : "-Infinity";

        bool negative = x < 0; // -0 is not < 0 -> no sign
        double a = Math.Abs(x);
        if (a >= 1e21) return ToJsString(x);

        // a = m * 2^e exactly.
        ulong bits = (ulong)BitConverter.DoubleToInt64Bits(a);
        int biased = (int)(bits >> 52);
        ulong m = bits & 0x000FFFFFFFFFFFFFUL;
        int e;
        if (biased == 0) e = -1074;
        else { m |= 1UL << 52; e = biased - 1075; }

        // n = the integer for which n / 10^f - a is closest to zero; ties -> larger n.
        string nDigits;
        if (m == 0)
        {
            nDigits = "0";
        }
        else if (f <= 17)
        {
            // a < 1e21 < 2^70 and 10^17 < 2^57, so everything fits in 128 bits.
            UInt128 n;
            if (e >= 0)
            {
                n = ((UInt128)m << e) * Pow10U128[f];
            }
            else
            {
                int s = -e;
                // m * 10^f < 2^53 * 2^57 = 2^110; if s > 111 the rounded quotient is 0.
                if (s > 111)
                {
                    n = UInt128.Zero;
                }
                else
                {
                    UInt128 num = (UInt128)m * Pow10U128[f];
                    n = (num + (UInt128.One << (s - 1))) >> s;
                }
            }
            nDigits = UInt128ToString(n);
        }
        else
        {
            BigInteger p10 = BigInteger.Pow(10, f);
            BigInteger n;
            if (e >= 0) n = (new BigInteger(m) << e) * p10;
            else
            {
                int s = -e;
                n = (new BigInteger(m) * p10 + (BigInteger.One << (s - 1))) >> s;
            }
            nDigits = n.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        // Layout (ECMA-262 Number.prototype.toFixed steps 10-11).
        var sb = new System.Text.StringBuilder(nDigits.Length + f + 3);
        if (negative) sb.Append('-');
        if (f == 0)
        {
            sb.Append(nDigits);
        }
        else
        {
            int len = nDigits.Length;
            if (len <= f)
            {
                // "0.00ddd"
                sb.Append('0').Append('.');
                sb.Append('0', f - len);
                sb.Append(nDigits);
            }
            else
            {
                sb.Append(nDigits, 0, len - f).Append('.').Append(nDigits, len - f, f);
            }
        }
        return sb.ToString();
    }

    private static string UInt128ToString(UInt128 n)
    {
        if (n <= ulong.MaxValue)
        {
            Span<char> b = stackalloc char[24];
            ulong u = (ulong)n;
            int p = b.Length;
            do
            {
                ulong q = u / 10;
                b[--p] = (char)('0' + (int)(u - q * 10));
                u = q;
            } while (u != 0);
            return new string(b.Slice(p));
        }
        Span<char> buf = stackalloc char[40];
        int pos = buf.Length;
        UInt128 ten = 10;
        do
        {
            UInt128 q = n / ten;
            buf[--pos] = (char)('0' + (int)(ulong)(n - q * ten));
            n = q;
        } while (n != UInt128.Zero);
        return new string(buf.Slice(pos));
    }
}
