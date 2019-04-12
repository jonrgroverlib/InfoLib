//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InfoLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;                         // for Guid
using System.Collections.Generic;     // for List<>
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- BigNumber -->
    /// <summary>
    ///      The BigNumber class manages really really large numbers (somewhat inefficiently)
    ///      When used statically it also contains long number base conversion methods (more efficient)
    ///      At some  point I will increase efficiency for small numbers (less than LongMax)
    /// </summary>
    /// <remarks>
    ///      Some odd things about this class:
    ///      1. Literal digits:
    ///         a. Negative digits are literals, number base conversions wrap up at each literal
    ///         b. Each literal provides an arithmetic and encoding break, for example
    ///             FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF  translates to base 56 as:
    ///             Hie3a4-UoP-UoP-UoP-CosabjXBH          where the dashes are literals stored
    ///                                                   as negative digits
    ///         c. Literals are also digits that are too high for their radix (number base)
    ///         d. Leading zeros are preserved at each literal break
    ///         e. This makes addition and subtraction arithmetic of formatted encodings complicated
    ///            for the programmer, simple for the user
    ///      2. Leading zeros:
    ///         Leading Zeros are preserved so that a string encoded then decoded will not lose its
    ///         leading zeros, this also means that leading zeros are preserved arithmetically
    ///         although addition and multiplication can fill them up, division and subtraction can
    ///         leave lots of leading zeros
    ///      3. Signs:
    ///         The sign of the number is kept in the 'Sign' field rather than the digits list,
    ///         If a Number is compiled out of a bunch of appended digits, each negative one
    ///         toggles the sign
    ///      4. Digit order:
    ///         the digit at the 'one's place is at position zero, the 'tens's place is at
    ///         position 1 etc. which is backwards from the order of digits in an encoding,
    ///         This means that encodings are little-endian but internally the digit list is
    ///         big-endian
    ///      TODO:
    ///      - base 1 is a special format called 'Roman Numerals': IVXLCDM
    ///      - fractions
    ///      - decimal
    ///      - powers
    ///      - imaginary numbers, compex numbers
    ///      
    /// 
    ///      beta code - heavily tested alpha code
    /// </remarks>
    public class BigNumber
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- _digit -->
        /// <summary>
        ///      Where the actual number is kept
        /// </summary>
        private List<int> _numerator;   // whole number if _denominator == 1
        private List<int> _denominator; // NOT YET IMPLEMENTED == 1 for whole nubmers


        // ----------------------------------------------------------------------------------------
        /// <!-- Encoding -->
        private string _encoding;
        /// <summary>
        ///      If you want to use something less than base 99, you should have an encoding
        /// </summary>
        public string Encoding
        {
            get { return _encoding; }
            //set { _encoding = value; }
        }
        public string RelevantEncoding
        {
            get
            {
                if (_radix >= _encoding.Length)  return "";
                else  return _encoding.Substring(0, _radix);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Radix -->
        private int _radix;
        /// <summary>The 'base' of the number</summary>
        public int Radix { get { return _radix; } //set { _radix = value; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Sign -->
        private int _sign;
        /// <summary>The 'sign' of the number</summary>
        public int Sign { get { return _sign; } set { _sign = value; } }


        // ----------------------------------------------------------------------------------------
        //  Simple Constructors             beta code - heavily tested alpha code
        // ----------------------------------------------------------------------------------------
        public BigNumber()                                     { Init(10,    Base16StyleEncoding, 1);               }
        public BigNumber(long num)                             { Init(10,    Base16StyleEncoding, 1);    Init(num); }
        public BigNumber(long num, int radix)                  { Init(radix, DefaultEncoding(radix), 1); Init(num); }
        public BigNumber(int radix, string encoding)           { Init(radix, encoding, 1);                          }
        public BigNumber(long num, int radix, string encoding) { Init(radix, encoding, 1);               Init(num); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Number constructor -->
        /// <summary>
        ///      Creates one number as a copy of another
        /// </summary>
        /// <param name="num">the number to be copied</param>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber(BigNumber num)
        {
            Init(num._radix, num._encoding, num._sign);
            foreach (int digit in num._numerator)    _numerator.Add(digit);
            foreach (int digit in num._denominator)  _denominator.Add(digit);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Number constructor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="radix"></param>
        /// <param name="encoding"></param>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber(string num, int radix, string encoding)
        {
            Init(radix, encoding, 1);
            num = TakeSignFrom(num);
            char[] chars = num.ToCharArray();
            for (int i = chars.Length - 1; i >= 0; --i)
                Add(Digit(chars[i]));
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AbsEquals -->
        /// <summary>
        ///      Sets the number to is absolut value and returns it
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber AbsEquals()
        {
            _sign = 1;
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Appends a digit to the Number's digits list, including various sign manipulations:
        ///      1. Negative digits change the sign and are set to positive
        ///      2. Any digits that exceed the radix (number base) are set to negative, this means
        ///         they are handled as formatting characters rather than as part of of the number,
        ///         This means that they break the number into pieces for base conversion purposes
        /// </summary>
        /// <param name="digit"></param>
        public BigNumber Add(int digit)
        {
            // --------------------------------------------------------------------------
            //  1. positive means positive, negative means negative
            //  2. positive means encoded, negative means unencoded
            // --------------------------------------------------------------------------
            if (digit < 0) { _sign = -_sign;  digit = -digit; }
            if (digit < _radix)
                 _numerator.Add(digit);
            else _numerator.Add(-digit);
            return this;
        }
        public BigNumber Add(long digit) { return Add((int)digit); }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Appends the digits from the input number to the Number's digits list as is,
        ///      without sign manipulations
        /// </summary>
        /// <param name="num">
        ///      Appends the digits from the input number to the Number's digits list as is,
        ///      without sign manipulations
        /// </param>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public void Add(BigNumber num)
        {
            foreach (int digit in num._numerator)
                _numerator.Add(digit);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Base -->
        /// <summary>
        ///      Returns the number in a new base, positive digits are converted to the new base,
        ///      negative digits are taken as literal, leading zeros are ignored
        /// </summary>
        /// <param name="newNumRadix"></param>
        /// <param name="newNumEncoding"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber Base(int newNumRadix, string newNumEncoding)
        {
            if (newNumEncoding == "")
                newNumEncoding = DefaultEncoding(newNumRadix);
            BigNumber output;


            string keep = "";


            // --------------------------------------------------------------------------
            //  Start a new number
            // --------------------------------------------------------------------------
            output = new BigNumber(0, newNumRadix, newNumEncoding);
            int leadingZeros = 0;
            bool leading = true;


            if (_numerator.Count == 1 && _numerator[0] == 0)  return output;


            for (int i = _numerator.Count - 1; i >= 0; --i)
            {
                int digit = _numerator[i];
                if (digit >= 0)
                {
                    // ------------------------------------------------------------------
                    //  Positive digits mean add on to the number
                    // ------------------------------------------------------------------
                    if (leading && digit == 0)  leadingZeros++;  else  leading = false;
                    if (i == 0 || _numerator[i - 1] < 0)  output.PlusEquals(digit);
                    else
                    {
                        if (output.Count > 1 || output._numerator[0] > 0)  output.TimesEquals(_radix);
                        output.PlusEquals(digit * _radix);
                    }
                }
                else
                {
                    // ------------------------------------------------------------------
                    //  Negative digits mean finish the number and start a new one
                    // ------------------------------------------------------------------
                    if (-digit < output.RelevantEncoding.Length)
                        throw new ArithmeticException("literal digit"
                            + " which was outside old base encoding"
                            + " is inside new base encoding!");
                    for (int j = 0; j < leadingZeros; ++j)  output.Add(0);
                    keep = keep + output.ToString() + Code(-digit, output._encoding);


                    // ------------------------------------------------------------------
                    //  Start a new number
                    // ------------------------------------------------------------------
                    output = new BigNumber(0, newNumRadix, newNumEncoding);
                    leadingZeros = 0;
                    leading = true;
                }
            }


            // --------------------------------------------------------------------------
            //  Handle leading zeros
            // --------------------------------------------------------------------------
            if (leadingZeros == _numerator.Count)
                --leadingZeros;
            for (int i = 0; i < leadingZeros; ++i)
                output.Add(0);


            // --------------------------------------------------------------------------
            //  Add on the previous numbers kept if any
            // --------------------------------------------------------------------------
            if (!string.IsNullOrEmpty(keep))
            {
                if (output.Count > 1 || output._numerator[0] > 0)
                    keep = keep + output.ToString();
                output = new BigNumber(keep, newNumRadix, newNumEncoding);
            }


            output._sign = _sign;
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Count -->
        /// <summary>
        ///      Returns the number of digits int he number
        /// </summary>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public int Count
        {
            get { return _numerator.Count; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Dec -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal Dec()
        {
            if (_denominator.Count == 1 && _denominator[0] == 1)
            {
                //  Nothing after the decimal
            }
            else
            {
                //  Something after the decimal
            }
            return 1.7M;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DefaultEncoding -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public static string DefaultEncoding(int radix)
        {
            string encoding = "";
            if (radix <= 18)
                encoding = Base16StyleEncoding;
            else
            {
                if (radix <= ZeroAt35Encoding.Length)
                    encoding = ZeroAt35Encoding;
                else
                    encoding = "";
            }
            return encoding;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Denominator -->
        /// <summary>
        ///      Returns the denominator as a long
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public long Denominator()
        {
            long max = long.MaxValue;
            long num = 0;
            int digit;
            for (int i = Count - 1; i >= 0 && num < max; --i)
            {
                digit = _denominator[i];
                if (num < (max - digit)/_radix)
                    num = num * _radix + digit;
                else  num = max;
            }
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Digit -->
        /// <summary>
        ///      Returns the digit at the specified place, or zero if it is left of the number
        /// </summary>
        /// <param name="i">place</param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public int Digit(int i)
        {
            if (Count > i) return _numerator[i]; else return 0;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Digit -->
        /// <summary>
        ///      Returns what the encoding says the digit is
        ///      try to get digit (including case toggle)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private int Digit(char c)
        {
            int digit = Code(c, _encoding);
            if (digit >= _radix)
            {
                // ----------------------------------------------------------------------
                //  Try again by changing case
                // ----------------------------------------------------------------------
                string re = RelevantEncoding;
                char c2 = ToggleCase(c);
                if (re.Contains(c2.ToString()) && !re.Contains(c.ToString()))
                    digit = Code(c2, _encoding);
            }
            return digit;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Equals -->
        /// <summary>
        ///      Test for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public override bool Equals(object obj)
        {
            if (obj == this)  return true;
            if (obj == null || obj.GetType() != this.GetType())  return false;


            // --------------------------------------------------------------------------
            //  Check two Numbers for equality
            // --------------------------------------------------------------------------
            BigNumber num = (BigNumber)obj;
            if ((num._sign == _sign) && (num._radix == _radix)
                && (num.Count == this.Count) && (num._encoding == _encoding))
            {
                for (int i = 0; i < _numerator.Count; ++i)
                    if (_numerator[i] != num._numerator[i])  return false;
                return true;
            }

            else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetHashCode -->
        /// <summary>
        ///      Very similar to the Int functionality
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public override int GetHashCode()
        {
            int max = int.MaxValue;
            int num = 0;
            int digit;
            for (int i = Count - 1; i >= 0 && num < max; --i)
            {
                digit = _numerator[i];
                if (num < (max - digit)/_radix)
                    num = num * _radix + digit;
                else  num >>= 16;
            }
            return _sign * num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      This needs to have radix set first
        /// </summary>
        /// <param name="num"></param>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private BigNumber Init(long num)
        {
            _numerator = new List<int>();
            if (num >= 0) _sign = 1;
            else { num = -num; _sign = -1; }

            
            // --------------------------------------------------------------------------
            //  Add the number to the digits
            // --------------------------------------------------------------------------
            if (_radix > 1)
                if (num == 0) Add(0);
                else while (num > 0)
                    num = TakeAddFrom(num);
            else throw new Exception("number base (Radix) must be 2 or higher");


            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Core initialization, sets radix, encoding, sign, and digit list
        /// </summary>
        /// <param name="radix"></param>
        /// <param name="encoding"></param>
        /// <param name="sign">1 or -1</param>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private void Init(int radix, string encoding, int sign)
        {
            _numerator = new List<int>();
            _denominator = new List<int>();
            _denominator.Add(1);
            _encoding = encoding;


            if (radix > 1)
                _radix = radix;
            else
                throw new Exception("radix (number base) must exceed 1");


            if (sign == 1 || sign == -1)
                _sign = sign;
            else
                throw new Exception("sign must be 1 or -1");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Integer -->
        /// <summary>
        ///      Returns an integer if it can
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public int Integer()
        {
            long longNum = LongInt();
            if (int.MinValue < longNum && longNum < int.MaxValue)
                             return (int)longNum;
            if (longNum < 0) return int.MinValue;
            else             return int.MaxValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LongInt -->
        /// <summary>
        ///      Returns an long if it can
        /// </summary>
        /// <returns></returns>
        public long LongInt()
        {
            long max = long.MaxValue;
            long num = 0;
            int digit;
            for (int i = Count - 1; i >= 0 && num < max; --i)
            {
                digit = _numerator[i];
                if (num < (max - digit)/_radix)
                    num = num * _radix + digit;
                else  num = max;
            }
            return _sign * num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MagnitudeExceeds -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public bool MagnitudeExceeds(BigNumber num)
        {
            int count = Math.Max(this.Count, num.Count);
            bool exceeds = false;
            int digit = 0;
            int digit2 = 0;
            for (int i = count - 1; digit == digit2 && i >= 0; --i)
            {
                digit = this.Digit(i);
                digit2 = num.Digit(i);
                if (digit > digit2)  exceeds = true;
            }
            return exceeds;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Minus -->
        /// <summary>
        ///      Subtracts one Number from another, could be made a bit more efficient
        /// </summary>
        /// <remarks>
        ///      Example Process:
        ///       542 - 378
        ///       5.4.-6 - 3.7.0
        ///       5.-3.-6 - 3.0.0
        ///       2.-3.-6 - 0.0.0
        ///       1.7.-6
        ///       1.6.4
        /// </remarks>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber Minus(BigNumber num)
        {
            // --------------------------------------------------------------------------
            //  Plus situations
            // ---------------------------------------------------------------------90
            if (this._sign == 1 && num._sign == -1)
                return Plus(num.Times(-1));
            if (this._sign == -1 && num._sign == 1)
                return Plus(num.Times(-1));


            if (num.MagnitudeExceeds(this))
                return num.Minus(this).Times(-1);


            // ---------------------------------------------------------------------90
            //  Normal situation
            // ---------------------------------------------------------------------90
            if (this._radix != num._radix)
                throw new Exception("can not add two Numbers with different bases");
            BigNumber z = new BigNumber(this);
            int count;


            // ---------------------------------------------------------------------90
            //  Raw Calculations
            // ---------------------------------------------------------------------90
            count = Math.Max(this.Count, num.Count);
            for (int i = 0; i < count; ++i)
                z._numerator[i] = Digit(i) - num.Digit(i);


            // ---------------------------------------------------------------------90
            //  Process Calculations
            // ---------------------------------------------------------------------90
            bool goAgain = true;
            while (goAgain)
            {
                goAgain = false;
                for (int i = count - 1; i > 0; --i)
                {
                    if (z._numerator[i] >= 0 && z._numerator[i - 1] < 0)
                    {
                        if (z._numerator[i] == 0)
                            goAgain = true;
                        z._numerator[i]--;
                        z._numerator[i - 1] = z._radix + z._numerator[i - 1];
                    }
                }
            }


            return z;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Numerator -->
        /// <summary>
        ///      Returns the numerator as a long
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public long Numerator()
        {
            long max = long.MaxValue;
            long num = 0;
            int digit;
            for (int i = Count - 1; i >= 0 && num < max; --i)
            {
                digit = _numerator[i];
                if (0 <= digit && digit < _radix)
                {
                    if (num < (max - digit) / _radix)
                        num = num * _radix + digit;
                    else num = max;
                }
                else
                {
                    num = 0;
                }
            }
            return num;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Plus -->
        /// <summary>
        ///      Adds an integer to the number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public BigNumber PlusEquals(int num)
        {
            for (int i = 0; i < Count; ++i)
                num = TakeSetFrom(i, _numerator[i] + num);
            while (num > 0)
                num = TakeAddFrom(num);
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Plus -->
        /// <summary>
        ///      Adds two Numbers using long addition returning a Number
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>x + y</returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber Plus(BigNumber num)
        {
            // ---------------------------------------------------------------------90
            //  Minus situations
            // ---------------------------------------------------------------------90
            if (this._sign == 1 && num._sign == -1)
                return this.Minus(num.Times(-1));
            if (this._sign == -1 && num._sign == 1)
                return num.Minus(this.Times(-1));


            // ---------------------------------------------------------------------90
            //  Normal situation
            // ---------------------------------------------------------------------90
            if (this._radix != num._radix)
                throw new Exception("can not add two Numbers with different bases");
            BigNumber z = new BigNumber(_radix, _encoding);
            int carry = 0;
            int count;


            // ---------------------------------------------------------------------90
            //  Calculate
            // ---------------------------------------------------------------------90
            count = Math.Max(this.Count, num.Count);
            for (int i = 0; i < count; ++i)
                carry = z.TakeAddFrom(this.Digit(i) + num.Digit(i) + carry);
            if (carry > 0)
                carry = z.TakeAddFrom(carry);


            z._sign = _sign;
            return z;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TakeAddFrom -->
        /// <summary>
        ///      Adds the next digit to the number from the integer source number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int TakeAddFrom(int num)
        {
            Add(num % _radix);
            return num / _radix;
        }
        private long TakeAddFrom(long num)
        {
            Add(num % _radix);
            return num / _radix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TakeSetFrom -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="carry"></param>
        /// <param name="i"></param>
        /// <param name="addNum"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private int TakeSetFrom(int i, int num)
        {
            _numerator[i] = num % _radix;
            return num / _radix;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TakeSignFrom -->
        /// <summary>
        ///      Strips an initial '-' of a number in string form setting the Number's sign
        /// </summary>
        /// <param name="num"></param>
        /// <returns>number with the sign stripped</returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private string TakeSignFrom(string num1)
        {
            string num2 = num1;
            string encoding = RelevantEncoding;
            if (!encoding.Contains("-"))    num2 = Regex.Replace(num1, "^-", "");
            if (num2.Length < num1.Length)  _sign = -1;  else  _sign = 1;
            if (!encoding.Contains("+"))    num2 = Regex.Replace(num2, "^[+]", "");
            return num2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Times -->
        /// <summary>
        ///      Do a number of long additions, shifting and multiplying by each digit
        /// </summary>
        /// <param name="num"></param>
        /// <returns>a new number without affecting the original</returns>
        public BigNumber Times(BigNumber num)
        {
            if (num._radix != this._radix)
                throw new ArithmeticException("can not multiply two Numbers with different bases");


            // --------------------------------------------------------------------------
            //  Initialize array of rows to add together
            // --------------------------------------------------------------------------
            List<BigNumber> row = new List<BigNumber>(num.Count);
            for (int i = 0; i < this.Count + num.Count; ++i)
                row.Add(new BigNumber(this._radix, this._encoding));


            // --------------------------------------------------------------------------
            //  Multiply each digit times the object to create a row
            // --------------------------------------------------------------------------
            for (int i = 0; i < num.Count; ++i)
            {
                int digit = num._numerator[i];
                BigNumber temp = this.Times(digit);
                for (int j = 0; j < i; ++j)  row[i].Add(0);
                row[i].Add(temp);
            }


            // --------------------------------------------------------------------------
            //  Add up the rows
            // --------------------------------------------------------------------------
            BigNumber z = new BigNumber(_radix, _encoding);
            z = row[0];
            for (int i = 1; i < row.Count; ++i)
                if (row[i].Count > 0)
                    z = z.Plus(row[i]);


            z._sign = this._sign * num._sign;
            return z;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Times -->
        /// <summary>
        ///      Returns the Number times an integer without affecting the original number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber Times(int num)
        {
            return (new BigNumber(this)).TimesEquals(num);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TimesEquals -->
        /// <summary>
        ///      Multiply number times an integer thus affecting the original number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public BigNumber TimesEquals(int num)
        {
            // --------------------------------------------------------------------------
            //  Handle sign
            // --------------------------------------------------------------------------
            if (num < 0)
            {
                _sign *= -1;
                num = -num;
            }


            // --------------------------------------------------------------------------
            //  Multiply Number value times int value
            // --------------------------------------------------------------------------
            int carry = 0;
            for (int i = 0; i < Count; ++i)
                carry = TakeSetFrom(i, _numerator[i] * num + carry);
            while (carry > 0)
                carry = TakeAddFrom(carry);


            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToBaseString -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="innerDelimiter"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private string ToBaseString(string str, string innerDelimiter)
        {
            string delim = "";
            foreach (int digit in _numerator)
            {
                str = digit + delim + str;
                delim = innerDelimiter;
            }
            if (_sign == -1)
                str = "-" + innerDelimiter + str;
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToggleCase -->
        /// <summary>
        ///      Toggles a character's case
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static char ToggleCase(char c)
        {
            char c2 = c;
            if (Char.IsUpper(c)) c2 = Char.ToLower(c);
            if (Char.IsLower(c)) c2 = Char.ToUpper(c);
            return c2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public override string ToString()
        {
            string str = "";
            if (!string.IsNullOrEmpty(_encoding) && _radix <= _encoding.Length)
            {
                // ----------------------------------------------------------------------
                //  Encoded string
                // ----------------------------------------------------------------------
                foreach (int digit in _numerator)
                    str = Code(digit, _encoding) + str;
                if (_sign == -1 && !RelevantEncoding.Contains("-"))
                    str = "-" + str;
            }
            else
            {
                // ----------------------------------------------------------------------
                //  Base represenation string
                // ----------------------------------------------------------------------
                string innerDelimiter = "|";
                str = ToBaseString(str, innerDelimiter);
            }
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EncodeLong -->
        /// <summary>
        ///      Returns a base x number with an encoding friendly for strings, xml, numbers, sql etc.
        /// </summary>
        /// <param name="radix">the number base to encode into</param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string EncodeLong(int radix, long num)
        {
            if (radix > 18)  return EncodeLong(radix, num, ZeroAt35Encoding);
            else             return EncodeLong(radix, num, Base16StyleEncoding);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EncodeLong -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix">the number base to encode into</param>
        /// <param name="num"></param>
        /// <param name="encoding">the encoding to use</param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public static string EncodeLong(int radix, long num, string encoding)
        {
            string str = "";
            for (int i = 0; i < 100 && num > 0; ++i)
            {
                long n2 = num / radix;
                str = Code((int)(num - n2 * radix), encoding) + str;
                num = n2;
            }
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DecodeLong -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix">the number base to encode out of</param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public static long DecodeLong(int radix, string str) // longs are 63 bits
        {
            if (radix > 18)  return DecodeLong(radix, str, ZeroAt35Encoding);
            else             return DecodeLong(radix, str, Base16StyleEncoding);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DecodeLong -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radix">the number base to encode out of</param>
        /// <param name="str">the encoded string</param>
        /// <param name="encoding">the encoding used in the encoded string</param>
        /// <returns></returns>
        public static long DecodeLong(int radix, string str, string encoding)
        {
            long n = 0;
            char[] c = str.ToCharArray();
            int len = c.Length;
            for (int i = 0; i < len; ++i)
                n = n * radix + Code(c[i], encoding);
            return n;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Base16 -->
        /// <summary>
        ///      Converts an number to a Base 16 string
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string Base16(int num)
        {
            return num.ToString("X");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Code -->
        /// <summary>
        ///      Decodes characters
        /// </summary>
        /// <param name="c"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int Code(char c, string encoding) { return encoding.IndexOf(c); }

        // ----------------------------------------------------------------------------------------
        /// <!-- Code -->
        /// <summary>
        ///      Encodes numbers
        /// </summary>
        /// <param name="num"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        public static char Code(int num, string encoding)
        {
            char[] ca = encoding.ToCharArray();
            return ca[Math.Abs(num)];
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Base16StyleEncoding -->
        /// <remarks>
        ///      base ranges:
        ///       2-18:   signed case insensitive Base16 encoding - 0-9A-Za-z
        ///      19-36:   signed case insensitive Base64 encoding - A-Z9-0a-z
        ///      37-85:   signed case   sensitive Base64 encoding - A-Z9-0a-z...
        ///      86-98: unsigned case   sensitive Base64 encoding - A-Z9-0a-z...-+...
        ///      99+:   unicode encoding                                                                                                                                                                                                                                         2 4 6  10    16        26        36                      60  64  68  72  76  80  84  88
        /// </remarks>                                                                                                                                                                                                                   /// <summary>                          0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string Base16StyleEncoding { get { return _base16StyleEncoding; } } private static string _base16StyleEncoding = Numbers + Uppers     + Lowers  +        SymNums + "."      + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>                          ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/_,@`~:;{}$!|().*^-#=?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string Base64StyleEncoding { get { return _base64StyleEncoding; } } private static string _base64StyleEncoding = Uppers  + Lowers     + Numbers + "+/" + SymNums + ".*^-#=" + "?[]%'" + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 25:  ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt26Encoding    { get { return _zeroAt26Encoding;    } } private static string _zeroAt26Encoding    = Uppers  + Numbers    + Lowers  +        SymNums + "."      + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 34:  ABCDEFGHIJKLMNOPQRSTUVWXYZ9876543210abcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt35Encoding    { get { return _zeroAt35Encoding;    } } private static string _zeroAt35Encoding    = Uppers  + Reverse    + Lowers  +        SymNums + "."      + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 51:  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_,@`~:;{}$!|().*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt52Encoding    { get { return _zeroAt52Encoding;    } } private static string _zeroAt52Encoding    = Uppers  + Lowers     + Numbers +        SymNums + "."      + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 60:  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz9876543210_,@`~:;{}$!|().*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt61Encoding    { get { return _zeroAt61Encoding;    } } private static string _zeroAt61Encoding    = Uppers  + Lowers     + Reverse +        SymNums + "."      + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 66:  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().0123456789*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt67Encoding    { get { return _zeroAt67Encoding;    } } private static string _zeroAt67Encoding    = Uppers  + Lowers     + SymNums + "."            + Numbers  + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 75:  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().9876543210*^-#=+/?[]%'&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt76Encoding    { get { return _zeroAt76Encoding;    } } private static string _zeroAt76Encoding    = Uppers  + Lowers     + SymNums + "."            + Reverse  + SymXml  + SymText + " "     + "\t\r\n"; /// <summary>no 1 or 0 up to base 87:  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_,@`~:;{}$!|().*^-#=+/?[]%'9876543210&amp;&lt;&gt;" \t"\r\n</summary>
        public static string ZeroAt88Encoding    { get { return _zeroAt88Encoding;    } } private static string _zeroAt88Encoding    = Uppers  + Lowers     + SymNums + "."            + SymXml   + Reverse + SymText + " "     + "\t\r\n"; /// <summary>                          ABCDEFGHIJKLMNOPQRSTUVWXYZ .abcdefghijklmnopqrstuvwxyz0123456789_,@`~:;{}$!|()*^-#=+/?[]%'&amp;&lt;&gt;"\t"\r\n</summary>
        public static string SimpleTextEncoding  { get { return _simpleTextEncoding;  } } private static string _simpleTextEncoding  = Uppers  + " "        + "."     +        Lowers  + Numbers  + SymNums + SymXml  + SymText + "\t\r\n"; //                                      2 4 6  10    16        26        36                      60  64  68  72  76  80  84  88
        public static string VoiceEncoding       { get { return _voiceEncoding;       } } private static string _voiceEncoding       = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            + "@" // at (sign)
            + "+" // plus
            + "*" // star
            + "|" // pipe
            + "'" // quote
            + "/" // slash
            + "," // comma
            + ":" // colon
            + "^" // caret
            + "~" // tilde
            + "$" // dollar (sign)
            + "=" // equals (sign)
            + "#" // number (sign)
            + "%" // percent (sign)
            + "?" // question (mark)
            + "<" // less than
            + "\\" // back slash
            + "{" // left brace
            + "}" // right brace
            + ">" // greater than
            + "_" // underscore
            + "&" // ampersand (and)
            + "[" // left bracket
            + "]" // right bracket
            + "\"" // double quote
            + ";" // semicolon
            + "!" // exclamation point (bang)
            + "(" // left parenthesis
            + ")" // right parenthesis
            + "`" // back quote (tick)
            + " " // space
            + "." // dot
            + "-" // dash
            + "abcdefghijklmnopqrstuvwxyz\t\r\n";



        // ----------------------------------------------------------------------------------------
        /// <!-- Letters -->
        /// <summary>
        ///      Lowercase letters
        /// </summary>
        private static string Lowers
        {
            get
            {
                return "abcde"   //  base 41 -             24    (GUID friendly - four hex digits become 3 base 41+ digits)
                    + "fghijklm" //  base 49 -             23    
                    + "nopqrstu" //  base 57 -             22    
                    + "vwx"      // *base 60 -             22    (datetime friendly - and 60 can be divided by 1,2,3,4,5,6,10)
                    + "yz"       //  base 62 -             22    (alphanumeric friendly)
                    ;
            }
        }                                                                 /// <summary>Upper case letters</summary>
        private static string Uppers { get { return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Numbers -->
        /// <summary>
        ///      Normal and reverse ordered numbers
        /// </summary>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private static string Numbers { get { return "0123456789"; } }  /// <summary>Reverse ordered numbers</summary>
        private static string Reverse
        {
            get
            {
                return "9876"    //  base 30 (or 56) -     27    (written friendly)
                    + "54"       // *base 32 -             26    (binary friendly) - and 32 can be divided by 1,2,4,8,16)
                    + "32"       //  base 34 (or 60) -     26    spoken friendly (or datetime friendly)
                    + "10";      // *base 36 -             25    (ignore case friendly)
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Symbols -->
        /// <summary>
        ///      Symbols for both base 16 (012...) and base 64 (ABC...) oriented encodings
        ///      An encoding friendly for strings, xml sql, urls, numbers and regex, in that order
        ///      P - printable problems  - space, tab, return, linefeed
        ///      A - string problems     - \'"
        ///      X - xml problems        - &<>'"
        ///      S - sql problems        - []%'
        ///      U - url problems        - #=+/?%& and space
        ///      N - number problems     - ^.-*/+ and space
        ///      R - regex problems      - []\^$.|?*+()
        ///      C - code problems       - :;{}()[]#=/
        ///      I - identifier problems - all symbols except _
        /// </summary>
        /// <remarks>beta code - heavily tested alpha code</remarks>
        private static string Symbols { get { return SymNums + SymXml + SymText; } }

        private static string SymNums
        { get { // NLRCOI:
                return "_"     //  base 63 -              22  I (identifier friendly)
                    + ","      // *base 64 -    I         22  O (octal friendly - and 64 can be divided by 1,2,4,8,16,32)
                    + "@`~"    //  base 67 -    I         22  C (code friendly)
                    + ":;{}"   //  base 71 -   CI         21  R (regex friendly - and 72 can be divided by 1,2,3,4,6,8,9,12)
                    + "$"      //  base 72 -    I   R     21  L (logic friendly  - logic symbols are still special characters)
                    + "!|()";  // *base 76 -   CIL  R     21  N (number friendly - signs, decimal points, operations, powers are still special characters)
        }     }
        private static string SymXml
        { get { // XSBU:
                return "*^-"   // *base 80 -    I N R     21  U (url friendly)
                    + "#="     //  base 82 -   CI N   U   21    
                    + "+/"     // *base 84 -   CI N R U   21  B (base friendly - 84 can be divided by 1,2,3,4,6,7,12,14,18)
                    + "?"      // *base 85 -    I   R U   20  S (sql friendly, 128 bit number friendly - 128 bits fits in 20 characters)
                    + "[]"     //  base 87 -   CI   RS    20  A (string friendly)
                    + "%"      //  base 88 -    I    SU   20  X (xml friendly)
                    + "'";     //  base 89 -  A I    S X  20  H (name friendly)
        }     }
        private static string SymText
        { get { // PHA:
                return "&"     //  base 90 -    IL    UX  20  
                    + "<>"     //  base 92 -    IL     X  20  
                    + "\""     //  base 93 -  A I      X  20  
                    + "\\";    //  base 94 -  A I   R     20  P (printable character friendly)
                    //+ " "    //  base 95 -    INP   U   20    (sentence friendly)
                    //+ "\t";  // *base 96 -      P       20  F (text friendly)
        }     }

                   // "\r\n";  // *base 98 -   F  P       20    (ascii friendly)
    }
}
