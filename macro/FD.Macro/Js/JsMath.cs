using System;
using System.Runtime.CompilerServices;

namespace FD.Macro;

/// <summary>
/// Bit-exact C# equivalents of the JavaScript number primitives used by the TS simulation:
/// same IEEE-754 bits as Node 22 (V8 12.4) on x64, and identical on every .NET platform
/// (the code only relies on correctly rounded IEEE operations, never on the C runtime).
///
/// Exp / Log / Pow are line-by-line ports of V8's <c>src/base/ieee754.cc</c> (fdlibm
/// e_exp.c / e_log.c / e_pow.c plus V8's own tweaks). They use only IEEE-754 double
/// arithmetic (+ - * /, sqrt, which are correctly rounded everywhere) and integer bit
/// manipulation, never System.Math.Exp/Log/Pow (those call the platform C runtime).
///
/// Everything else (Round, ToInt32/ToUint32, Min/Max, Str, ToFixed) implements the
/// ECMAScript definition exactly; V8 follows the spec for all of them.
///
/// NaN payloads: where V8 returns <c>x + x</c> / <c>y - y</c> we do the same, and where V8
/// returns <c>std::numeric_limits&lt;double&gt;::signaling_NaN()</c> we return the same bit
/// pattern (0x7FF4000000000000), so even NaN bits match V8 on the same CPU architecture.
/// Callers should still treat all NaNs as equal.
/// </summary>
public static class JsMath
{
    // --------------------------------------------------------------------------------------
    // fdlibm word access (EXTRACT_WORDS / GET_HIGH_WORD / SET_LOW_WORD / INSERT_WORDS ...)
    // --------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int HighWord(double d) => (int)(BitConverter.DoubleToInt64Bits(d) >> 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double FromWords(int hi, uint lo) =>
        BitConverter.Int64BitsToDouble((long)(((ulong)(uint)hi << 32) | lo));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double SetHighWord(double d, int hi) =>
        BitConverter.Int64BitsToDouble((long)(((ulong)BitConverter.DoubleToInt64Bits(d) & 0x00000000FFFFFFFFUL) | ((ulong)(uint)hi << 32)));

    /// <summary>SET_LOW_WORD(d, 0): clears the low 32 bits.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double ClearLowWord(double d) =>
        BitConverter.Int64BitsToDouble((long)((ulong)BitConverter.DoubleToInt64Bits(d) & 0xFFFFFFFF00000000UL));

    /// <summary>std::numeric_limits&lt;double&gt;::signaling_NaN() as returned by V8's ieee754.cc.</summary>
    private static readonly double SignalingNaN = BitConverter.Int64BitsToDouble(0x7FF4000000000000L);

    private static readonly double NegativeZero = BitConverter.Int64BitsToDouble(unchecked((long)0x8000000000000000UL));

    // --------------------------------------------------------------------------------------
    // Math.exp — V8 ieee754::exp (fdlibm e_exp.c, plus V8's "exp(1) == E" special case)
    // --------------------------------------------------------------------------------------

    /// <summary>JS <c>Math.exp(x)</c>, bit-exact with V8 (port of fdlibm e_exp.c as in V8 ieee754.cc).</summary>
    public static double Exp(double x)
    {
        const double one = 1.0;
        const double o_threshold = 7.09782712893383973096e+02;  // 0x40862E42, 0xFEFA39EF
        const double u_threshold = -7.45133219101941108420e+02; // 0xC0874910, 0xD52D3051
        const double ln2HI = 6.93147180369123816490e-01;        // 0x3FE62E42, 0xFEE00000 (ln2HI[0]; ln2HI[1] = -ln2HI[0])
        const double ln2LO = 1.90821492927058770002e-10;        // 0x3DEA39EF, 0x35793C76 (ln2LO[0]; ln2LO[1] = -ln2LO[0])
        const double invln2 = 1.44269504088896338700e+00;       // 0x3FF71547, 0x652B82FE
        const double P1 = 1.66666666666666019037e-01;           // 0x3FC55555, 0x5555553E
        const double P2 = -2.77777777770155933842e-03;          // 0xBF66C16C, 0x16BEBD93
        const double P3 = 6.61375632143793436117e-05;           // 0x3F11566A, 0xAF25DE2C
        const double P4 = -1.65339022054652515390e-06;          // 0xBEBBBD41, 0xC5D26BF1
        const double P5 = 4.13813679705723846039e-08;           // 0x3E663769, 0x72BEA4D0
        const double E = 2.718281828459045;                     // 0x4005BF0A, 0x8B145769
        const double huge = 1.0e+300;
        const double twom1000 = 9.33263618503218878990e-302;    // 2**-1000
        const double two1023 = 8.988465674311579539e307;        // 0x1p1023

        double y, hi = 0.0, lo = 0.0, c, t, twopk;
        int k = 0;

        long bits = BitConverter.DoubleToInt64Bits(x);
        uint hx = (uint)(bits >> 32);
        int xsb = (int)((hx >> 31) & 1); // sign bit of x
        hx &= 0x7FFFFFFF;                // high word of |x|

        // filter out non-finite argument
        if (hx >= 0x40862E42)
        {   // |x| >= 709.78...
            if (hx >= 0x7FF00000)
            {
                uint lx = (uint)bits;
                if (((hx & 0xFFFFF) | lx) != 0) return x + x; // NaN
                return (xsb == 0) ? x : 0.0;                   // exp(+-inf) = {inf, 0}
            }
            if (x > o_threshold) return huge * huge;          // overflow  (+Infinity)
            if (x < u_threshold) return twom1000 * twom1000;  // underflow (+0)
        }

        // argument reduction
        if (hx > 0x3FD62E42)
        {   // |x| > 0.5 ln2
            if (hx < 0x3FF0A2B2)
            {   // and |x| < 1.5 ln2
                // V8: special-case exp(1) to return exactly Math.E.
                if (x == 1.0) return E;
                hi = x - (xsb == 0 ? ln2HI : -ln2HI);
                lo = xsb == 0 ? ln2LO : -ln2LO;
                k = 1 - xsb - xsb;
            }
            else
            {
                k = (int)(invln2 * x + (xsb == 0 ? 0.5 : -0.5)); // C cast: truncation toward zero
                t = k;
                hi = x - t * ln2HI; // t*ln2HI is exact here
                lo = t * ln2LO;
            }
            x = hi - lo;
        }
        else if (hx < 0x3E300000)
        {   // when |x| < 2**-28
            if (huge + x > one) return one + x; // trigger inexact
        }
        else
        {
            k = 0;
        }

        // x is now in primary range
        t = x * x;
        if (k >= -1021)
            twopk = FromWords(unchecked(0x3FF00000 + (int)((uint)k << 20)), 0);
        else
            twopk = FromWords(unchecked((int)(0x3FF00000u + ((uint)(k + 1000) << 20))), 0);
        c = x - t * (P1 + t * (P2 + t * (P3 + t * (P4 + t * P5))));
        if (k == 0) return one - ((x * c) / (c - 2.0) - x);
        y = one - ((lo - (x * c) / (2.0 - c)) - hi);
        if (k >= -1021)
        {
            if (k == 1024) return y * 2.0 * two1023;
            return y * twopk;
        }
        return y * twopk * twom1000;
    }

    // --------------------------------------------------------------------------------------
    // Math.log — V8 ieee754::log (fdlibm e_log.c)
    // --------------------------------------------------------------------------------------

    /// <summary>JS <c>Math.log(x)</c> (natural log), bit-exact with V8 (port of fdlibm e_log.c as in V8 ieee754.cc).</summary>
    public static double Log(double x)
    {
        const double ln2_hi = 6.93147180369123816490e-01; // 3fe62e42 fee00000
        const double ln2_lo = 1.90821492927058770002e-10; // 3dea39ef 35793c76
        const double two54 = 1.80143985094819840000e+16;  // 43500000 00000000
        const double Lg1 = 6.666666666666735130e-01;      // 3FE55555 55555593
        const double Lg2 = 3.999999999940941908e-01;      // 3FD99999 9997FA04
        const double Lg3 = 2.857142874366239149e-01;      // 3FD24924 94229359
        const double Lg4 = 2.222219843214978396e-01;      // 3FCC71C5 1D8E78AF
        const double Lg5 = 1.818357216161805012e-01;      // 3FC74664 96CB03DE
        const double Lg6 = 1.531383769920937332e-01;      // 3FC39A09 D078C69F
        const double Lg7 = 1.479819860511658591e-01;      // 3FC2F112 DF3E5244

        double hfsq, f, s, z, R, w, t1, t2, dk;
        int k, hx, i, j;

        long bits = BitConverter.DoubleToInt64Bits(x);
        hx = (int)(bits >> 32);
        uint lx = (uint)bits;

        k = 0;
        if (hx < 0x00100000)
        {   // x < 2**-1022
            if (((uint)(hx & 0x7FFFFFFF) | lx) == 0) return double.NegativeInfinity; // log(+-0) = -inf
            if (hx < 0) return SignalingNaN;                                          // log(-#) = NaN
            k -= 54;
            x *= two54; // subnormal number, scale up x
            hx = HighWord(x);
        }
        if (hx >= 0x7FF00000) return x + x;
        k += (hx >> 20) - 1023;
        hx &= 0x000FFFFF;
        i = (hx + 0x95F64) & 0x100000;
        x = SetHighWord(x, hx | (i ^ 0x3FF00000)); // normalize x or x/2
        k += (i >> 20);
        f = x - 1.0;
        if ((0x000FFFFF & (2 + hx)) < 3)
        {   // -2**-20 <= f < 2**-20
            if (f == 0.0)
            {
                if (k == 0) return 0.0;
                dk = k;
                return dk * ln2_hi + dk * ln2_lo;
            }
            R = f * f * (0.5 - 0.33333333333333333 * f);
            if (k == 0) return f - R;
            dk = k;
            return dk * ln2_hi - ((R - dk * ln2_lo) - f);
        }
        s = f / (2.0 + f);
        dk = k;
        z = s * s;
        i = hx - 0x6147A;
        w = z * z;
        j = 0x6B851 - hx;
        t1 = w * (Lg2 + w * (Lg4 + w * Lg6));
        t2 = z * (Lg1 + w * (Lg3 + w * (Lg5 + w * Lg7)));
        i |= j;
        R = t2 + t1;
        if (i > 0)
        {
            hfsq = 0.5 * f * f;
            if (k == 0) return f - (hfsq - s * (hfsq + R));
            return dk * ln2_hi - ((hfsq - (s * (hfsq + R) + dk * ln2_lo)) - f);
        }
        if (k == 0) return f - s * (f - R);
        return dk * ln2_hi - ((s * (f - R) - dk * ln2_lo) - f);
    }

    // --------------------------------------------------------------------------------------
    // Math.pow / ** — V8 ieee754::pow (fdlibm e_pow.c with V8's modifications:
    // y == 2 -> x*x, y == 0.5 -> sqrt(x) for x >= +0, and (+-1) ** (+-Infinity) == NaN)
    // --------------------------------------------------------------------------------------

    /// <summary>
    /// JS <c>Math.pow(x, y)</c> / <c>x ** y</c>, bit-exact with V8. V8 12.4 uses the fdlibm
    /// e_pow.c port in ieee754.cc in every tier (interpreter, builtins, TurboFan/Maglev and
    /// constant folding); TurboFan's <c>x ** 0.5</c> / <c>x ** 2</c> strength reductions are
    /// value-identical to it.
    /// </summary>
    public static double Pow(double x, double y)
    {
        const double bp1 = 1.5;                         // bp[] = {1.0, 1.5}
        const double dp_h1 = 5.84962487220764160156e-01; // dp_h[1]: 0x3FE2B803, 0x40000000 (dp_h[0] = 0)
        const double dp_l1 = 1.35003920212974897128e-08; // dp_l[1]: 0x3E4CFDEB, 0x43CFD006 (dp_l[0] = 0)
        const double zero = 0.0, one = 1.0, two = 2.0;
        const double two53 = 9007199254740992.0;         // 0x43400000, 0x00000000
        const double huge = 1.0e300, tiny = 1.0e-300;
        // poly coefs for (3/2)*(log(x)-2s-2/3*s**3
        const double L1 = 5.99999999999994648725e-01;      // 0x3FE33333, 0x33333303
        const double L2 = 4.28571428578550184252e-01;      // 0x3FDB6DB6, 0xDB6FABFF
        const double L3 = 3.33333329818377432918e-01;      // 0x3FD55555, 0x518F264D
        const double L4 = 2.72728123808534006489e-01;      // 0x3FD17460, 0xA91D4101
        const double L5 = 2.30660745775561754067e-01;      // 0x3FCD864A, 0x93C9DB65
        const double L6 = 2.06975017800338417784e-01;      // 0x3FCA7E28, 0x4A454EEF
        const double P1 = 1.66666666666666019037e-01;      // 0x3FC55555, 0x5555553E
        const double P2 = -2.77777777770155933842e-03;     // 0xBF66C16C, 0x16BEBD93
        const double P3 = 6.61375632143793436117e-05;      // 0x3F11566A, 0xAF25DE2C
        const double P4 = -1.65339022054652515390e-06;     // 0xBEBBBD41, 0xC5D26BF1
        const double P5 = 4.13813679705723846039e-08;      // 0x3E663769, 0x72BEA4D0
        const double lg2 = 6.93147180559945286227e-01;     // 0x3FE62E42, 0xFEFA39EF
        const double lg2_h = 6.93147182464599609375e-01;   // 0x3FE62E43, 0x00000000
        const double lg2_l = -1.90465429995776804525e-09;  // 0xBE205C61, 0x0CA86C39
        const double ovt = 8.0085662595372944372e-0017;    // -(1024-log2(ovfl+.5ulp))
        const double cp = 9.61796693925975554329e-01;      // 0x3FEEC709, 0xDC3A03FD =2/(3ln2)
        const double cp_h = 9.61796700954437255859e-01;    // 0x3FEEC709, 0xE0000000 =(float)cp
        const double cp_l = -7.02846165095275826516e-09;   // 0xBE3E2FE0, 0x145B01F5 =tail cp_h
        const double ivln2 = 1.44269504088896338700e+00;   // 0x3FF71547, 0x652B82FE =1/ln2
        const double ivln2_h = 1.44269502162933349609e+00; // 0x3FF71547, 0x60000000 =24b 1/ln2
        const double ivln2_l = 1.92596299112661746887e-08; // 0x3E54AE0B, 0xF85DDF44 =1/ln2 tail

        double z, ax, z_h, z_l, p_h, p_l;
        double y1, t1, t2, r, s, t, u, v, w;
        int i, j, k, yisint, n;
        int hx, hy, ix, iy;
        uint lx, ly;

        long xbits = BitConverter.DoubleToInt64Bits(x);
        long ybits = BitConverter.DoubleToInt64Bits(y);
        hx = (int)(xbits >> 32); lx = (uint)xbits;
        hy = (int)(ybits >> 32); ly = (uint)ybits;
        ix = hx & 0x7fffffff;
        iy = hy & 0x7fffffff;

        // y==zero: x**0 = 1
        if (((uint)iy | ly) == 0) return one;

        // +-NaN return x+y
        if (ix > 0x7ff00000 || ((ix == 0x7ff00000) && (lx != 0)) || iy > 0x7ff00000 ||
            ((iy == 0x7ff00000) && (ly != 0)))
        {
            return x + y;
        }

        // determine if y is an odd int when x < 0
        // yisint = 0 ... y is not an integer
        // yisint = 1 ... y is an odd int
        // yisint = 2 ... y is an even int
        yisint = 0;
        if (hx < 0)
        {
            if (iy >= 0x43400000)
            {
                yisint = 2; // even integer y
            }
            else if (iy >= 0x3ff00000)
            {
                k = (iy >> 20) - 0x3ff; // exponent
                if (k > 20)
                {
                    j = (int)(ly >> (52 - k));
                    if ((j << (52 - k)) == (int)ly) yisint = 2 - (j & 1);
                }
                else if (ly == 0)
                {
                    j = iy >> (20 - k);
                    if ((j << (20 - k)) == iy) yisint = 2 - (j & 1);
                }
            }
        }

        // special value of y
        if (ly == 0)
        {
            if (iy == 0x7ff00000)
            {   // y is +-inf
                if (((uint)(ix - 0x3ff00000) | lx) == 0) return y - y;   // (+-1)**+-inf is NaN (V8/ES semantics)
                if (ix >= 0x3ff00000) return (hy >= 0) ? y : zero;       // (|x|>1)**+-inf = inf,0
                return (hy < 0) ? -y : zero;                             // (|x|<1)**-,+inf = inf,0
            }
            if (iy == 0x3ff00000)
            {   // y is +-1
                if (hy < 0) return one / x; // base::Divide == IEEE division
                return x;
            }
            if (hy == 0x40000000) return x * x; // y is 2
            if (hy == 0x3fe00000)
            {   // y is 0.5
                if (hx >= 0) return Math.Sqrt(x); // x >= +0; sqrt is correctly rounded (IEEE-754)
            }
        }

        ax = Math.Abs(x);
        // special value of x
        if (lx == 0)
        {
            if (ix == 0x7ff00000 || ix == 0 || ix == 0x3ff00000)
            {
                z = ax;                     // x is +-0,+-inf,+-1
                if (hy < 0) z = one / z;    // z = (1/|x|)
                if (hx < 0)
                {
                    if (((ix - 0x3ff00000) | yisint) == 0)
                        z = SignalingNaN;   // (-1)**non-int is NaN
                    else if (yisint == 1)
                        z = -z;             // (x<0)**odd = -(|x|**odd)
                }
                return z;
            }
        }

        n = (hx >> 31) + 1;

        // (x<0)**(non-int) is NaN
        if ((n | yisint) == 0) return SignalingNaN;

        s = one; // s (sign of result -ve**odd) = -1 else = 1
        if ((n | (yisint - 1)) == 0) s = -one; // (-ve)**(odd int)

        // |y| is huge
        if (iy > 0x41e00000)
        {   // if |y| > 2**31
            if (iy > 0x43f00000)
            {   // if |y| > 2**64, must o/uflow
                if (ix <= 0x3fefffff) return (hy < 0) ? huge * huge : tiny * tiny;
                if (ix >= 0x3ff00000) return (hy > 0) ? huge * huge : tiny * tiny;
            }
            // over/underflow if x is not close to one
            if (ix < 0x3fefffff) return (hy < 0) ? s * huge * huge : s * tiny * tiny;
            if (ix > 0x3ff00000) return (hy > 0) ? s * huge * huge : s * tiny * tiny;
            // now |1-x| is tiny <= 2**-20, suffice to compute log(x) by x-x^2/2+x^3/3-x^4/4
            t = ax - one; // t has 20 trailing zeros
            w = (t * t) * (0.5 - t * (0.3333333333333333333333 - t * 0.25));
            u = ivln2_h * t; // ivln2_h has 21 sig. bits
            v = t * ivln2_l - w * ivln2;
            t1 = u + v;
            t1 = ClearLowWord(t1);
            t2 = v - (t1 - u);
        }
        else
        {
            double ss, s2, s_h, s_l, t_h, t_l;
            n = 0;
            // take care subnormal number
            if (ix < 0x00100000)
            {
                ax *= two53;
                n -= 53;
                ix = HighWord(ax);
            }
            n += (ix >> 20) - 0x3ff;
            j = ix & 0x000fffff;
            // determine interval
            ix = j | 0x3ff00000; // normalize ix
            if (j <= 0x3988E)
            {
                k = 0; // |x|<sqrt(3/2)
            }
            else if (j < 0xBB67A)
            {
                k = 1; // |x|<sqrt(3)
            }
            else
            {
                k = 0;
                n += 1;
                ix -= 0x00100000;
            }
            ax = SetHighWord(ax, ix);

            double bpk = k == 0 ? 1.0 : bp1;
            double dp_hk = k == 0 ? 0.0 : dp_h1;
            double dp_lk = k == 0 ? 0.0 : dp_l1;

            // compute ss = s_h+s_l = (x-1)/(x+1) or (x-1.5)/(x+1.5)
            u = ax - bpk; // bp[0]=1.0, bp[1]=1.5
            v = one / (ax + bpk);
            ss = u * v;
            s_h = ClearLowWord(ss);
            // t_h=ax+bp[k] High
            t_h = FromWords(((ix >> 1) | 0x20000000) + 0x00080000 + (k << 18), 0);
            t_l = ax - (t_h - bpk);
            s_l = v * ((u - s_h * t_h) - s_h * t_l);
            // compute log(ax)
            s2 = ss * ss;
            r = s2 * s2 * (L1 + s2 * (L2 + s2 * (L3 + s2 * (L4 + s2 * (L5 + s2 * L6)))));
            r += s_l * (s_h + ss);
            s2 = s_h * s_h;
            t_h = 3.0 + s2 + r;
            t_h = ClearLowWord(t_h);
            t_l = r - ((t_h - 3.0) - s2);
            // u+v = ss*(1+...)
            u = s_h * t_h;
            v = s_l * t_h + t_l * ss;
            // 2/(3log2)*(ss+...)
            p_h = u + v;
            p_h = ClearLowWord(p_h);
            p_l = v - (p_h - u);
            z_h = cp_h * p_h; // cp_h+cp_l = 2/(3*log2)
            z_l = cp_l * p_h + p_l * cp + dp_lk;
            // log2(ax) = (ss+..)*2/(3*log2) = n + dp_h + z_h + z_l
            t = n;
            t1 = (((z_h + z_l) + dp_hk) + t);
            t1 = ClearLowWord(t1);
            t2 = z_l - (((t1 - t) - dp_hk) - z_h);
        }

        // split up y into y1+y2 and compute (y1+y2)*(t1+t2)
        y1 = ClearLowWord(y);
        p_l = (y - y1) * t1 + y * t2;
        p_h = y1 * t1;
        z = p_l + p_h;
        long zbits = BitConverter.DoubleToInt64Bits(z);
        j = (int)(zbits >> 32);
        i = (int)(uint)zbits;
        if (j >= 0x40900000)
        {   // z >= 1024
            if (((j - 0x40900000) | i) != 0) return s * huge * huge;     // overflow
            if (p_l + ovt > z - p_h) return s * huge * huge;              // overflow
        }
        else if ((j & 0x7fffffff) >= 0x4090cc00)
        {   // z <= -1075
            // (C: j - 0xc090cc00 is evaluated in unsigned arithmetic)
            if ((unchecked((uint)j - 0xc090cc00u) | (uint)i) != 0) return s * tiny * tiny; // underflow
            if (p_l <= z - p_h) return s * tiny * tiny;                                      // underflow
        }

        // compute 2**(p_h+p_l)
        i = j & 0x7fffffff;
        k = (i >> 20) - 0x3ff;
        n = 0;
        if (i > 0x3fe00000)
        {   // if |z| > 0.5, set n = [z+0.5]
            n = j + (0x00100000 >> (k + 1));
            k = ((n & 0x7fffffff) >> 20) - 0x3ff; // new k for n
            t = FromWords(n & ~(0x000fffff >> k), 0);
            n = ((n & 0x000fffff) | 0x00100000) >> (20 - k);
            if (j < 0) n = -n;
            p_h -= t;
        }
        t = p_l + p_h;
        t = ClearLowWord(t);
        u = t * lg2_h;
        v = (p_l - (t - p_h)) * lg2 + t * lg2_l;
        z = u + v;
        w = v - (z - u);
        t = z * z;
        t1 = z - t * (P1 + t * (P2 + t * (P3 + t * (P4 + t * P5))));
        r = (z * t1) / ((t1 - two) - (w + z * w));
        z = one - (r - z);
        j = HighWord(z);
        j = unchecked(j + (int)((uint)n << 20));
        if ((j >> 20) <= 0)
        {
            z = Scalbn(z, n); // subnormal output
        }
        else
        {
            int tmp = HighWord(z);
            z = SetHighWord(z, unchecked(tmp + (int)((uint)n << 20)));
        }
        return s * z;
    }

    /// <summary>
    /// C99 scalbn(x, n) = x * 2^n with a single correct rounding (what glibc/musl/MSVC return).
    /// Only reached from Pow for results in the subnormal range. Follows musl's scalbn.
    /// </summary>
    private static double Scalbn(double x, int n)
    {
        const double two1023 = 8.98846567431157953865e+307;          // 0x1p1023
        const double twom969 = 2.00416836000897277800e-292;          // 0x1p-969 = 0x1p-1022 * 0x1p53
        double y = x;
        if (n > 1023)
        {
            y *= two1023;
            n -= 1023;
            if (n > 1023)
            {
                y *= two1023;
                n -= 1023;
                if (n > 1023) n = 1023;
            }
        }
        else if (n < -1022)
        {
            // make sure final n < -53 to avoid double rounding in the subnormal range
            y *= twom969;
            n += 1022 - 53;
            if (n < -1022)
            {
                y *= twom969;
                n += 1022 - 53;
                if (n < -1022) n = -1022;
            }
        }
        return y * BitConverter.Int64BitsToDouble((long)(0x3ff + n) << 52);
    }

    // --------------------------------------------------------------------------------------
    // Math.round
    // --------------------------------------------------------------------------------------

    /// <summary>
    /// JS <c>Math.round(x)</c>: nearest integer, ties toward +Infinity; -0 for x in [-0.5, -0];
    /// NaN / ±Infinity / ±0 pass through. Same formula as V8's CSA/TurboFan lowering
    /// (ceil(x), minus 1 if ceil(x) - 0.5 &gt; x), which is exact for every double.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double x)
    {
        // |x| >= 2^52 is already integral; the negated compare also passes NaN through.
        if (!(Math.Abs(x) < 4503599627370496.0)) return x;
        if (x == 0.0) return x; // keeps the sign of zero
        double r = Math.Ceiling(x);
        if (r - 0.5 > x) r -= 1.0; // r - 0.5 is exact for |r| <= 2^52
        if (r == 0.0) return x < 0.0 ? NegativeZero : 0.0; // x in [-0.5, 0) -> -0, x in (0, 0.5) -> +0
        return r;
    }

    // --------------------------------------------------------------------------------------
    // ToInt32 / ToUint32 (x | 0, x >>> 0)
    // --------------------------------------------------------------------------------------

    /// <summary>ECMAScript ToInt32 (<c>x | 0</c>): NaN/±Inf -> 0, truncate, wrap modulo 2^32.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(double x)
    {
        // Fast path: already in range (NaN fails both compares).
        if (x > -2147483649.0 && x < 2147483648.0) return (int)x; // C# cast truncates toward zero
        return unchecked((int)ToUint32Slow(x));
    }

    /// <summary>ECMAScript ToUint32 (<c>x &gt;&gt;&gt; 0</c>): NaN/±Inf -> 0, truncate, wrap modulo 2^32.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUint32(double x)
    {
        if (x > -2147483649.0 && x < 2147483648.0) return unchecked((uint)(int)x);
        return ToUint32Slow(x);
    }

    /// <summary>Exact modulo-2^32 truncation for any double (|x| up to 2^1024).</summary>
    private static uint ToUint32Slow(double x)
    {
        long bits = BitConverter.DoubleToInt64Bits(x);
        int biased = (int)((bits >> 52) & 0x7FF);
        if (biased == 0x7FF) return 0;   // NaN, +-Infinity
        if (biased == 0) return 0;       // +-0 and subnormals (|x| < 1)
        ulong m = ((ulong)bits & 0x000FFFFFFFFFFFFFUL) | 0x0010000000000000UL;
        int e = biased - 1075;           // |x| = m * 2^e, m has 53 bits
        uint r;
        if (e >= 32) r = 0;              // multiple of 2^32
        else if (e >= 0) r = unchecked((uint)(m << e));
        else if (e > -53) r = unchecked((uint)(m >> -e));
        else r = 0;                      // |x| < 1
        return bits < 0 ? unchecked(0u - r) : r;
    }

    // --------------------------------------------------------------------------------------
    // Math.min / Math.max
    // --------------------------------------------------------------------------------------

    /// <summary>JS <c>Math.min(a, b)</c>: NaN if either is NaN; -0 &lt; +0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(double a, double b)
    {
        if (a < b) return a;
        if (b < a) return b;
        if (a == b)
        {
            // Equal: only +0/-0 can differ; prefer the negative zero.
            return BitConverter.DoubleToInt64Bits(a) < 0 ? a : b;
        }
        return double.NaN; // at least one NaN
    }

    /// <summary>JS <c>Math.max(a, b)</c>: NaN if either is NaN; +0 &gt; -0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(double a, double b)
    {
        if (a > b) return a;
        if (b > a) return b;
        if (a == b)
        {
            // Equal: only +0/-0 can differ; prefer the positive zero.
            return BitConverter.DoubleToInt64Bits(a) < 0 ? b : a;
        }
        return double.NaN; // at least one NaN
    }

    /// <summary>JS <c>Math.min(...values)</c>: +Infinity for no arguments, NaN if any argument is NaN.</summary>
    public static double Min(params double[] values)
    {
        double r = double.PositiveInfinity;
        if (values == null) return r;
        bool nan = false;
        for (int i = 0; i < values.Length; i++)
        {
            double v = values[i];
            if (double.IsNaN(v)) nan = true;
            else r = Min(r, v);
        }
        return nan ? double.NaN : r;
    }

    /// <summary>JS <c>Math.max(...values)</c>: -Infinity for no arguments, NaN if any argument is NaN.</summary>
    public static double Max(params double[] values)
    {
        double r = double.NegativeInfinity;
        if (values == null) return r;
        bool nan = false;
        for (int i = 0; i < values.Length; i++)
        {
            double v = values[i];
            if (double.IsNaN(v)) nan = true;
            else r = Max(r, v);
        }
        return nan ? double.NaN : r;
    }

    // --------------------------------------------------------------------------------------
    // Number -> string
    // --------------------------------------------------------------------------------------

    /// <summary>
    /// JS <c>String(x)</c> / <c>Number.prototype.toString()</c> (radix 10): shortest
    /// round-trip digits (ties to even digit, bounds inclusive for even significands, as V8's
    /// Grisu3/bignum dtoa), fixed notation for 1e-6 &lt;= |x| &lt; 1e21, otherwise exponent
    /// form ("1e+21", "1.5e-7"). "NaN", "Infinity", "-Infinity"; -0 gives "0". Culture-invariant.
    /// </summary>
    public static string Str(double x) => JsNumberFormat.ToJsString(x);

    /// <summary>
    /// JS <c>Number.prototype.toFixed(digits)</c> for digits 0..100: exact decimal expansion of
    /// the double, ties pick the larger magnitude; |x| &gt;= 1e21 falls back to <see cref="Str"/>.
    /// Negative values that round to zero keep the sign ("-0.00"); -0 gives "0.00".
    /// </summary>
    public static string ToFixed(double x, int digits) => JsNumberFormat.ToFixed(x, digits);
}
