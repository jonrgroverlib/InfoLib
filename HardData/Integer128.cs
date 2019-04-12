using System;
using System.Collections.Generic;
using System.Numerics;

namespace InfoLib.HardData
{
    /// <summary>
    ///     A cheap attempt to build an Int128 using a BigInteger
    /// </summary>
    public class Int128
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private BigInteger Value { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Int128()
        {
            Value = new BigInteger();
        }


        // ----------------------------------------------------------------------------------------
        //  Operators
        // ----------------------------------------------------------------------------------------
        public static implicit operator Int128(BigInteger n) { Int128 num = new Int128(); num.Value = n; if (num.InRange) return num; else throw new OverflowException(); }
        public static implicit operator Int128(long       n) { Int128 num = new Int128(); num.Value = n; if (num.InRange) return num; else throw new OverflowException(); }
        public static implicit operator Int128(int        n) { Int128 num = new Int128(); num.Value = n; if (num.InRange) return num; else throw new OverflowException(); }
        public static implicit operator Int128(short      n) { Int128 num = new Int128(); num.Value = n; if (num.InRange) return num; else throw new OverflowException(); }

        public static Int128 operator +(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value + rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator -(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value - rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator *(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value * rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator /(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value / rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator %(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value % rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator &(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value & rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator |(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value | rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator ^(Int128 lhs, Int128 rhs) { Int128 num = lhs.Value ^ rhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator *(Int128 lhs, short  rhs) { Int128 num = lhs.Value * rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator *(Int128 lhs, int    rhs) { Int128 num = lhs.Value * rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator *(Int128 lhs, long   rhs) { Int128 num = lhs.Value * rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator +(Int128 lhs, short  rhs) { Int128 num = lhs.Value + rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator +(Int128 lhs, int    rhs) { Int128 num = lhs.Value + rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator +(Int128 lhs, long   rhs) { Int128 num = lhs.Value + rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator -(Int128 lhs, short  rhs) { Int128 num = lhs.Value - rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator -(Int128 lhs, int    rhs) { Int128 num = lhs.Value - rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator -(Int128 lhs, long   rhs) { Int128 num = lhs.Value - rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator /(Int128 lhs, short  rhs) { Int128 num = lhs.Value / rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator /(Int128 lhs, int    rhs) { Int128 num = lhs.Value / rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator /(Int128 lhs, long   rhs) { Int128 num = lhs.Value / rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator %(Int128 lhs, short  rhs) { Int128 num = lhs.Value % rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator %(Int128 lhs, int    rhs) { Int128 num = lhs.Value % rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator %(Int128 lhs, long   rhs) { Int128 num = lhs.Value % rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator ^(Int128 lhs, short  rhs) { Int128 num = lhs.Value ^ rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator ^(Int128 lhs, int    rhs) { Int128 num = lhs.Value ^ rhs;       if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator ^(Int128 lhs, long   rhs) { Int128 num = lhs.Value ^ rhs;       if (num.InRange) return num; else throw new OverflowException(); }

        public static Int128 operator ++(Int128 lhs) { Int128 num = ++lhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator --(Int128 lhs) { Int128 num = --lhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator  +(Int128 lhs) { Int128 num =  +lhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator  -(Int128 lhs) { Int128 num =  -lhs.Value; if (num.InRange) return num; else throw new OverflowException(); }
        public static Int128 operator  ~(Int128 lhs) { Int128 num =  ~lhs.Value; if (num.InRange) return num; else throw new OverflowException(); }

        public static bool   operator !=(Int128 lhs, Int128 rhs) { return lhs.Value != rhs.Value; }
        public static bool   operator >=(Int128 lhs, Int128 rhs) { return lhs.Value >= rhs.Value; }
        public static bool   operator <=(Int128 lhs, Int128 rhs) { return lhs.Value <= rhs.Value; }
        public static bool   operator  >(Int128 lhs, Int128 rhs) { return lhs.Value  > rhs.Value; }
        public static bool   operator  <(Int128 lhs, Int128 rhs) { return lhs.Value  < rhs.Value; }
        public static bool   operator ==(Int128 lhs, Int128 rhs) { return lhs.Value == rhs.Value; }


        // ----------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------
        private bool InRange      { get { if (Value < MinValue || Value > MaxValue) return false; else return true; } }
        public  bool IsEven       { get { return Value.IsEven      ; } }
        public  bool IsOne        { get { return Value.IsOne       ; } }
        public  bool IsPowerOfTwo { get { return Value.IsPowerOfTwo; } }
        public  bool IsZero       { get { return Value.IsZero      ; } }
        public  int  Sign         { get { return Value.Sign        ; } }


        // ----------------------------------------------------------------------------------------
        //  Constants
        // ----------------------------------------------------------------------------------------
        public static Int128 MaxValue { get { Int128 max = new BigInteger(long.MaxValue); max = max * max *  2    ; return max; } }
        public static Int128 MinValue { get { Int128 min = new BigInteger(long.MinValue); min = min * min * -2 - 1; return min; } }
        public static Int128 MinusOne { get { return new Int128() { Value = -1 }; } }
        public static Int128 One      { get { return new Int128() { Value =  1 }; } }
        public static Int128 Zero     { get { return new Int128() { Value =  0 }; } }


        // ----------------------------------------------------------------------------------------
        //  Pass-through Methods
        // ----------------------------------------------------------------------------------------
        public static Int128 Add      (Int128 left , Int128 right) { return left + right; }
        public static Int128 Divide   (Int128 left , Int128 right) { return left / right; }
        public static Int128 Multiply (Int128 left , Int128 right) { return left * right; }
        public static Int128 Subtract (Int128 left , Int128 right) { return left - right; }
        public static Int128 Negate   (Int128 value              ) { return -value;       }
        public static Int128 Abs      (Int128 value              ) { return BigInteger.Abs      (value.Value             ); }
        public static int    Compare  (Int128 left , Int128 right) { return BigInteger.Compare  (left .Value, right.Value); }
        public static double Log      (Int128 value              ) { return BigInteger.Log      (value.Value             ); }
        public static double Log10    (Int128 value              ) { return BigInteger.Log10    (value.Value             ); }
        public static Int128 Max      (Int128 left , Int128 right) { return BigInteger.Max      (left .Value, right.Value); }
        public static Int128 Min      (Int128 left , Int128 right) { return BigInteger.Min      (left .Value, right.Value); }
        public static Int128 Parse    (string value              ) { return BigInteger.Parse    (value                   ); }
        public static Int128 Pow      (Int128 value, int exponent) { return BigInteger.Pow      (value.Value, exponent   ); }
        public static Int128 Remainder(Int128 dvdnd, Int128 dvsr ) { return BigInteger.Remainder(dvdnd.Value, dvsr .Value); }
        public static Int128 GrtstCmDv(Int128 left , Int128 right) { return BigInteger.GreatestCommonDivisor(left.Value, right.Value); }
        public static Int128 ModPow   (Int128 value, Int128 exponent, Int128 modulus) { return BigInteger.ModPow(value.Value, exponent.Value, modulus.Value); }


        public static Int128 DivRem(Int128 dividend, Int128 divisor, out Int128 remainder)
        {
            BigInteger rem = new BigInteger();
            BigInteger.DivRem(dividend.Value, divisor.Value, out rem);
            remainder = rem;
            return remainder;
        }

        public static bool TryParse(string value, out Int128 result)
        {
            BigInteger num = new BigInteger();
            bool ok = BigInteger.TryParse(value, out num);
            result = new Int128() { Value = num };
            if (result.InRange) return ok; else { result = new Int32(); return false; }
        }


        // ----------------------------------------------------------------------------------------
        //  InfoLib specific methods
        // ----------------------------------------------------------------------------------------
        public int DigitCount { get { return Value.ToString().Length; } }

        public static Int128 FromEndeme(string endeme)
        {
            Int128 enCompressed = Zero;
            char[] cha = endeme.ToUpper().ToCharArray();

            for (int i = 0; i < cha.Length; ++i)
            {
                int num = cha[i] - 65;
                enCompressed = enCompressed * 24 + num;
            }

            return enCompressed;
        }

        public string ToEndeme()
        {
            List<char> cha = new List<char>();

            Int128 val = this.Copy();

            while (val > 0)
            {
                Int128 rem = DivRem(val, 24, out rem);
                cha.Add(Convert.ToChar(rem.Value + 65));
                val -= rem;
                val /= 24;
            }

            return new string(cha.ToArray());
        }

        public static Int128 FromGuid(Guid guid)
        {
            Int128 output = Zero;
            output.Value = new BigInteger(guid.ToByteArray());
            return output;
        }

        public Guid ToGuid()
        {
            return new Guid(Value.ToByteArray());
        }

        public Int128 Copy() { Int128 val = new Int128() { Value = BigInteger.Parse(this.Value.ToString()) }; return val; }


        // ----------------------------------------------------------------------------------------
        //  Overrides
        // ----------------------------------------------------------------------------------------
        public override string ToString   ()           { return Value.ToString(); }
        public override bool   Equals     (object obj) { if (obj is Int128) return Value == (Int128)obj; else return false; }
        public override int    GetHashCode()           { return Value.GetHashCode(); }
    }
}
