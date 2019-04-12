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
using InfoLib.Testing;                // for RandomSource
using System;                         // for Random
using System.Collections.Generic;     // for Dictionary
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- StringTests -->
    /// <summary>
    ///      The StringTests class tests the classes in the Mathematics Library
    /// </summary>
    /// <remarks>production ready unit test code</remarks>
    public class StringTests
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private Result _result;

        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            _result = new Result("String tests");


            GrayCode_test               ();
            Number_test                 ();
            Number_Base_test            ();
            Number_Encode_test          ();
            Number_Int_test             ();
            Number_PlusMinus_test       ();
            Number_RelevantEncoding_test();
            Number_Times_test           ();
            Concatenator_test           ();


            return _result.AsciiDetailResults + "\r\n" + _result.SummaryMessage();
        }


        // ----------------------------------------------------------------------------------------
        //  Some test code
        // ----------------------------------------------------------------------------------------
        private static void Concatenator_testcase1()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.Concatenate     ("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            string str = test.ToString();
        }
        private static void Concatenator_testcase2()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.ConcatenateWithBlanks("ABC");
            test.ConcatenateWithBlanks("DEF");
            test.ConcatenateWithBlanks("GHI");
            test.ConcatenateOuter     ("ABC");
            test.ConcatenateWithBlanks("DEF");
            test.ConcatenateWithBlanks("GHI");
            test.ConcatenateOuter     (""   );
            test.ConcatenateWithBlanks("DEF");
            test.ConcatenateWithBlanks("GHI");
            string str = test.ToString();
        }
        private static void Concatenator_testcase3()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            string str = test.ToString();
        }
        private static void Concatenator_testcase4()
        {
            Concatenator_testcase4(" ", ""   ,"ABC","DEF","GHI", "ABC DEF GHI");
            Concatenator_testcase4(" ", "ABC",""   ,"DEF","GHI", "ABC DEF GHI");
            Concatenator_testcase4(" ", "ABC","DEF",""   ,"GHI", "ABC DEF GHI");
            Concatenator_testcase4(" ", "ABC","DEF","GHI",""   , "ABC DEF GHI");
        }
        private static void Concatenator_testcase4(string delim, string str1, string str2, string str3, string str4, string target)
        {
            Concatenator test = new Concatenator(delim);
            test.Concatenate(str1);
            test.Concatenate(str2);
            test.Concatenate(str3);
            test.Concatenate(str4);
            string str = test.ToString();
            bool ok = (str == target);
        }
        private static void Concatenator_testcase6()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     (""   );
            test.Concatenate     (""   );
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            string str = test.ToString();
        }
        private static void Concatenator_testcase5()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     (""   );
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            string str = test.ToString();
        }
        private static void Concatenator_testcase7()
        {
            Concatenator test = new Concatenator(",", "\r\n");
            test.ConcatenateOuter("ABC");
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            test.Concatenate     (""   );
            test.Concatenate     (""   );
            test.ConcatenateOuter(""   );
            test.Concatenate     ("DEF");
            test.Concatenate     ("GHI");
            test.ConcatenateOuter(""   );
            string str = test.ToString();
        }
        private static void Concatenator_testcase8()
        {
            Concatenator test = new Concatenator(" ");
            test.Concatenate("ABC");
            test.Concatenate(""   );
            test.Concatenate(""   );
            test.Concatenate("DEF");
            test.Concatenate("GHI");
            string str = test.ToString();
        }
        public void Concatenator_test()
        {
            Assert.ThingsAbout("Concatenator", "various");

            Concatenator_testcase1();
            Concatenator_testcase2();
            Concatenator_testcase3();
            Concatenator_testcase4();
            Concatenator_testcase5();
            Concatenator_testcase6();
            Concatenator_testcase7();
            Concatenator_testcase8();

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Affix_test -->
        /// <summary>
        /// 
        /// </summary>
        public void _Affix_test()
        {
            Assert.ThingsAbout("__", "Affix");

            string str;
            str = __.ConcatIf("a", "b");                Assert.That(str, Is.equal_to, "ab");
            str = __.ConcatIf("a",  "");                Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf("a", null);               Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf("",  "b");                Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf("",  "");                 Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf("",  null);               Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf(null, "b");               Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf(null, "");                Assert.That(str, Is.equal_to, ""  );
            str = __.ConcatIf(null, null);              Assert.That(str, Is.equal_to, ""  );

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Best_test -->
        /// <summary>
        /// 
        /// </summary>
        public void _Best_test()
        {
            Assert.ThingsAbout("__", "Best");

            string str;
            str = __.Best("", "Davey");              Assert.That(str, Is.equal_to, "Davey");
            str = __.Best("Davey", "");              Assert.That(str, Is.equal_to, "Davey");
            str = __.Best("Davey", "Bednarski");     Assert.That(str, Is.equal_to, "Davey");
            str = __.Best("", "");                   Assert.That(str, Is.equal_to, "");

            _result += Assert.Conclusion;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- Bool_test -->
        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        //public void _Bool_test()
        //{
        //    _result = "UNTESTED";
        //    bool test;
        //    test = __.Bool(true,    false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool(false,   false);          ok &= Assert.That(test, __.equals, false);
        //    test = __.Bool("True",  false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("False", false);          ok &= Assert.That(test, __.equals, false);
        //    test = __.Bool("Yes",   false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("No",    false);          ok &= Assert.That(test, __.equals, false);
        //    test = __.Bool("t",     false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("f",     false);          ok &= Assert.That(test, __.equals, false);
        //    test = __.Bool("y",     false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("n",     false);          ok &= Assert.That(test, __.equals, false);
        //    test = __.Bool("Acceptable", false);     ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("in",    false);          ok &= Assert.That(test, __.equals, true );
        //    test = __.Bool("ok",    false);          ok &= Assert.That(test, __.equals, true );    
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- FileName_test -->
        /// <summary>
        /// 
        /// </summary>
        public void _FileName_test()
        {
            Assert.ThingsAbout("__", "FileName");

            string str;
            str = __.LastSegment("C:\\Program Files\\Business Objects\\BusinessObjects Enterprise 11\\Logging.txt");
            Assert.That(str, Is.equal_to, "Logging.txt");
            str = __.LastSegment("Z:\\Projects\\Some Thing\\Work\\Other Thing\\File.html");
            Assert.That(str, Is.equal_to, "File.html");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _RandomNumericId_test -->
        /// <summary>
        /// 
        /// </summary>
        public void _RandomNumericId_test()
        {
            Assert.ThingsAbout("__", "RandomNumeric");

            Dictionary<char, int> tally = new Dictionary<char,int>();
            for (int i = 0; i < 1000; ++i)
            {
                string id = __.RandomNumericId(i);
                Assert.That(id.Length, Is.equal_to, i);
                if (i > 0)
                {
                    char c = id.ToCharArray()[0];
                    if (!tally.ContainsKey(c))
                        tally.Add(c, 0);
                    tally[c]++;
                }
            }
            Assert.That(tally.Count, Is.equal_to, 10);

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- _Reverse_test -->
        /// <summary>
        /// 
        /// </summary>
        public void _Reverse_test()
        {
            Assert.ThingsAbout("__", "Reverse");

            _Reverse_testcase("ABCD", "DCBA");
            _Reverse_testcase("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ZYXWVUTSRQPONMLKJIHGFEDCBA");
            _Reverse_testcase("", "");

            _result += Assert.Conclusion;
        }
        private static void _Reverse_testcase(string input, string target)
        {
            string output = __.Reverse(input);
            Assert.That(output, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GrayCode_test -->
        /// <summary>
        /// 
        /// </summary>
        public void GrayCode_test()
        {
            Assert.ThingsAbout("GrayCode", "various");

            GrayCode_testcase1();
            GrayCode_testcase2(ulong.MaxValue);
            GrayCode_testcase2(ulong.MaxValue / 2UL + 1UL);
            GrayCode_testcase2(100UL);
            GrayCode_testcase2(100000000000UL);
            GrayCode_testcase2((ulong)(uint.MaxValue) * 64UL);
            GrayCode_testcase2(ulong.MaxValue / 2UL);
            GrayCode_testcase3(); // a test case for distances
            GrayCode_testcase4(); // a test case for distances

            _result += Assert.Conclusion;
        }
        private static void GrayCode_testcase1()
        {
            string code;
            string list = "";
            for (int i = 0; i < 50; ++i)
            {
                GrayCode gc = new GrayCode((ulong)i);
                code = gc.ToString();
                list += "\r\n" + code + " " + i + " " + gc.Count + " " + gc.Code + " " + gc.Decode;
            }
            Assert.That(Regex.IsMatch(list, "00000000 0 0 0 0"   ), Is.equal_to, true);
            Assert.That(Regex.IsMatch(list, "00011000 16 2 24 16"), Is.equal_to, true);
            Assert.That(Regex.IsMatch(list, "00011111 21 5 31 21"), Is.equal_to, true);
            Assert.That(Regex.IsMatch(list, "00101001 49 3 41 49"), Is.equal_to, true);
        }
        private static void GrayCode_testcase2(ulong num1)
        {
            GrayCode gc = new GrayCode(num1);
            ulong code = gc.Code;
            string str = gc.ToString();
            ulong num2 = gc.Decode;
            Assert.That(num2, Is.equal_to, num1);
        }
        private static void GrayCode_testcase3()
        {
            Dictionary<double, int> count = new Dictionary<double, int>();
            string report = "";
            double pct;


            for (int j = 0; j <= 64; ++j) count.Add((double)j / 64.0, 0);
            for (int i = 0; i < 655536; ++i)
            {
                ulong num = RandomSource.New().NextULong(ulong.MaxValue);
                GrayCode gc = new GrayCode(num);
                string str = gc.ToString();


                //pct = gc.Percent;
                pct = (double)gc.Count / 64.0;
                count[pct]++;
            }
            for (int j = 0; j <= 64; ++j)
            {
                pct = (double)j / 64.0;
                report += "\r\n" + pct + " " + count[pct];
            }
        }
        private static void GrayCode_testcase4()
        {
            ulong size = 4095UL; // 268435456UL; // 2^28 // Cu
            ulong distance = RandomSource.New().NextULong(size);


            ulong num = size - distance;
            GrayCode gc = new GrayCode(num);
            string str = gc.ToString();
            double fraction = ((double)gc.Decode / (double)size);


            //double pct = gc.Percent;
            GrayCode gcSize = new GrayCode(size);
            double pct = (double)gc.Count / (double)(GrayCode.HighBit(gcSize.Code) + 1);
            Assert.That(0.0, Is.less_than_or_equal_to, pct).And(pct, Is.less_than_or_equal_to, 1.0);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Number_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Number_test()
        {
            Assert.ThingsAbout("Number", "various");


            //Number n = new Number("1.7", 10, Number.Base16StyleEncoding);
            //long num = n.Numerator();
            // Assert.That(num, __.equals, 17);
            //long denom = n.Denominator();
            // Assert.That(denom, __.equals, 10);
            //decimal dec = n.Dec();
            //ok &= Assert.That(dec, __.equals, 1.7M);
            //string output = n.ToString();
            //ok &= Assert.That(output, __.equals, "1.7");


            Number_testcase("10", 10, BigNumber.Base16StyleEncoding, "10");


            // --------------------------------------------------------------------------
            //  Construction and copy test
            // --------------------------------------------------------------------------
            Number_testcase(1234,  12, BigNumber.Base16StyleEncoding, "86A");
            Number_testcase(1,     12, BigNumber.Base16StyleEncoding, "1");
            Number_testcase(0,     12, BigNumber.Base16StyleEncoding, "0");
            Number_testcase(-1,    12, BigNumber.Base16StyleEncoding, "-1");
            Number_testcase(-1234, 12, BigNumber.Base16StyleEncoding, "-86A");
            Number_testcase(0  ,    0 );
            Number_testcase(472,  472 );
            Number_testcase(472, "472");

            // --------------------------------------------------------------------------
            //  Integer constructor and base conversion experiments
            // --------------------------------------------------------------------------
            Number_testcase(0,      22, "A");
            Number_testcase(23,     22, "BB");
            Number_testcase(44,     22, "CA");
            Number_testcase(485,    22, "BAB");
            Number_testcase(990,    22, "CBA");
            Number_testcase(9702,   22, "UBA");
            Number_testcase(10000,  22, "UOM");
            Number_testcase(100000, 22, "JINK");
            Number_testcase("CBA" , 22, "CBA");
            Number_testcase(107, 3, 99, 101, 44, 110, "107|3|99|101|44");


            // --------------------------------------------------------------------------
            //  Negative and positive number experiments
            // --------------------------------------------------------------------------
            Number_testcase(-472  , -472);
            Number_testcase(-472  , "-472");
            Number_testcase(-23   , 22, "-BB" );
            Number_testcase("-ABC", 22, "-ABC");
            Number_testcase("+ABC", 22,  "ABC");
            Number_testcase("-DEG", 77, "-DEG", -1);
            Number_testcase("-DEG", 78, "-DEG", -1);
            Number_testcase("-DEG", 79, "-DEG", -1);
            Number_testcase("-DEG", 80, "-DEG",  1);
            Number_testcase("-DEG", 81, "-DEG",  1);
            Number_testcase("-DEG", 82, "-DEG",  1);
            Number_testcase("-DEG", 83, "-DEG",  1);
            Number_testcase("-DEG", 84, "-DEG",  1);
            Number_testcase("-DEG", 85, "-DEG",  1);
            Number_testcase("-DEG", 86, "-DEG",  1);
            Number_testcase("-DEG", 87, "-DEG",  1);
            Number_testcase("-DEG", 88, "-DEG",  1);
            Number_testcase("-DEG", 89, "-DEG",  1);
            Number_testcase("+DEG", 80,  "DEG",  1);
            Number_testcase("+DEG", 81,  "DEG",  1);
            Number_testcase("+DEG", 82,  "DEG",  1);
            Number_testcase("+DEG", 83, "+DEG",  1);
            Number_testcase("+DEG", 84, "+DEG",  1);
            Number_testcase("+DEG", 85, "+DEG",  1);
            Number_testcase("+DEG", 86, "+DEG",  1);
            Number_testcase("+DEG", 87, "+DEG",  1);
            Number_testcase("+DEG", 88, "+DEG",  1);
            Number_testcase("+DEG", 89, "+DEG",  1);


            Number_testcase(-107, 3, 99, 101, 44, 110, "-|107|3|99|101|44");

            _result += Assert.Conclusion;
        }
        private static void Number_testcase(string source, int radix, string encoding, string target)
        {
            BigNumber x = new BigNumber(source, radix, encoding);
            string output = x.ToString();
            Assert.That(output, Is.equal_to, target);
        }
        private static void Number_testcase(int num, int radix, string encoding, string tgt)
        {
            BigNumber x = new BigNumber(num, radix, encoding);
            BigNumber y = new BigNumber(x);
            Assert.That(y.Equals(x) , Is.equal_to, true);
            Assert.That(y           , Is.equal_to, x);
            Assert.That(y.ToString(), Is.equal_to, tgt);
        }
        private static void Number_testcase(int input, string tgt)
        {
            BigNumber num = new BigNumber(input);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, tgt);
        }
        private static void Number_testcase(int input, int tgt)
        {
            BigNumber num = new BigNumber(input);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            int output = num.Integer();
            Assert.That(output, Is.equal_to, tgt);
        }
        private static void Number_testcase(int input, int radix, string target)
        {
            BigNumber num = new BigNumber(input, radix, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, target);
        }
        private static void Number_testcase(string input, int radix, string target)
        {
            BigNumber num = new BigNumber(input, radix, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, target);
        }
        private static void Number_testcase(string input, int radix, string target, int sign)
        {
            BigNumber num = new BigNumber(input, radix, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, target);
            Assert.That(num.Sign, Is.equal_to, sign);
        }
        private static void Number_testcase(int d5, int d4, int d3, int d2, int d1, int radix, string tgt)
        {
            BigNumber num = new BigNumber(d1, radix);
            num.Add(d2).Add(d3).Add(d4).Add(d5);
            string str = num.ToString();
            Assert.That(str, Is.equal_to, tgt);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Number_Base_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Number_Base_test()
        {
            Assert.ThingsAbout("Number", "Base");


            // --------------------------------------------------------------------------
            //  Base 16 experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "HK,",  96, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "H;4",  92, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "Ie,",  88, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "JYP",  84, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "KTP",  80, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "JYP",  84, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "P,,",  64, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "RDB",  62, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "7VP",  48, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "2bT",  44, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "bGP",  42, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "BAcP", 40, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFF", 16, BigNumber.Base16StyleEncoding, "BOUP", 36, BigNumber.ZeroAt35Encoding);


            // --------------------------------------------------------------------------
            //  Base 16-22 experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("0"    , 16, BigNumber.Base16StyleEncoding,    "A", 22, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("18"   , 16, BigNumber.Base16StyleEncoding,   "BC", 22, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("1FC"  , 16, BigNumber.Base16StyleEncoding,  "BBC", 22, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("1D13" , 16, BigNumber.Base16StyleEncoding,  "PIH", 22, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("186A0", 16, BigNumber.Base16StyleEncoding, "JINK", 22, BigNumber.ZeroAt35Encoding);


            // --------------------------------------------------------------------------
            //  Make sure the zeros in the encodings are where they are supposed to be
            // --------------------------------------------------------------------------
            Number_Base_testcase("61", 10, BigNumber.Base16StyleEncoding, "0", 90, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("35", 10, BigNumber.Base16StyleEncoding, "0", 90, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("26", 10, BigNumber.Base16StyleEncoding, "0", 90, BigNumber.ZeroAt26Encoding);
            Number_Base_testcase("52", 10, BigNumber.Base16StyleEncoding, "0", 90, BigNumber.ZeroAt52Encoding);
            Number_Base_testcase("88", 10, BigNumber.Base16StyleEncoding, "0", 90, BigNumber.ZeroAt88Encoding);


            // --------------------------------------------------------------------------
            //  Compression experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "TiKqNqcR8qA", 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "KQ6TpF3oj2o", 64, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "DLop`bWaptA", 72, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "BINCCGVw,.o", 80, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "^:@9@0pNAA" , 81, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "}w0QlC;L@c" , 82, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "5{@W+HiKqA" , 84, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "liZhJ-S@6A" , 88, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "h;#ym68}P~" , 89, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("11833012932673945320", 10, BigNumber.Base16StyleEncoding, "'*(_($pNAA", 81, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("1540-7315-7315-7315-7315-1540", 10, BigNumber.Base16StyleEncoding, "Zo-CB6-CB6-CB6-CB6-Zo", 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("231-26334-319770-26334-231"   , 10, BigNumber.Base16StyleEncoding, "Dz-HS7-Bcxe-HS7-Dz"   , 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("2600-14950-14950-14950-14950-14950-2600", 10, BigNumber.Base16StyleEncoding, "rU-EJK-EJK-EJK-EJK-EJK-rU", 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("14950-230230-230230-230230-14950"       , 10, BigNumber.Base16StyleEncoding, "EJK-BD4K-BD4K-BD4K-EJK"   , 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("14950 230230 230230 230230 14950"       , 10, BigNumber.Base16StyleEncoding, "B4/ ekc ekc ekc B4/"      , 87, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("14950-14950-5311735-14950-14950"        , 10, BigNumber.Base16StyleEncoding, "EJK-EJK-Yjc6-EJK-EJK"     , 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("14950-14950-5311735-14950-14950"        , 10, BigNumber.Base16StyleEncoding, "FxA-FxA-qYij-FxA-FxA"     , 50, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("14950 14950 5311735 14950 14950"        , 10, BigNumber.Base16StyleEncoding, "CJ6 CJ6 I8.9 CJ6 CJ6"     , 84, BigNumber.ZeroAt88Encoding);
            

            // --------------------------------------------------------------------------
            //  GUID experiments
            // --------------------------------------------------------------------------
            //  Base 41 GUIDs
            Number_Base_testcase("FFFF"                                , 16, BigNumber.Base16StyleEncoding, "ceR"                              , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF"                            , 16, BigNumber.Base16StyleEncoding, "bCcKMa"                           , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFF"                        , 16, BigNumber.Base16StyleEncoding, "0KLYG2Q4J"                        , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "bCcKMa-ceR-ceR-ceR-0KLYG2Q4J"     , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "BBb71WP-BAcP-BAcP-BAcP-BCbdTC4EGP", 40, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "bCcKMa-ceR-ceR-ceR-0KLYG2Q4J"     , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "3aLG5D-bGP-bGP-bGP-6CdWN5QBV"     , 42, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "9BdcbD-2bT-2bT-2bT-UB9PX9bJ0"     , 44, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "QfEHFP-7VP-7VP-7VP-JlWA46BbP"     , 48, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "Hie3a4-UoP-UoP-UoP-CosabjXBH"     , 56, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "EgfPMD-RDB-RDB-RDB-BRv32XKaH"     , 62, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "EUeeUD-Q3P-Q3P-Q3P-BI7vHu7IA"     , 63, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "D,,,,,-P,,-P,,-P,,-,,,,,,,,"      , 64, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "CPxA0d-MkP-MkP-MkP-7E4VZ03,"      , 72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "BquEdh-L9X-L9X-L9X-TQqa(Zf:"      , 76, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", 16, BigNumber.Base16StyleEncoding, "B4VQxn-Kdi-Kdi-Kdi-Op!4$Ypb"      , 79, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "Hbm|C-fy':hwFW/lBR(,"             , 96, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "(rFElj~X55Y?Ri`cCymA"             , 85, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "C.JbV.|6BH*23fCY3MXDP"            , 80, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "IRs@ZRqKcoDIEJZe):eVD"            , 76, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "YT~9@,8}aXTkHl5GrMgTd"            , 72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "3H{06EHl6Mb`yy}W:IaV7"            , 71, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "gjPdai9::SI{pL9udZJ2j"            , 70, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "uxtHmJH0Mk3d6mq@eYjMM"            , 69, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "BIKGaO~l8j~m,U7@Jqj,tp"           , 68, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "D,,,,,,,,,,,,,,,,,,,,,"           , 64, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "P5ga99G8Kg0BE6thdCa3EP"           , 60, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "WE2RidGRPVFslMnBvVGZuY"           , 59, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "407cthX1FRYvmhEhOBpVTr"           , 58, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "j4MlFjqX2SlE9iOTVr1GJD"           , 57, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "BKCrXDTM53VfZP1CMoGW1q4"          , 56, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "0BcLNN9JR5e4BF6kX43JXFP"          , 48, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "8QW3DXe7NYM6YNBM21WSJLeJ"         , 41, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "H4444444444444444444444444"       , 32, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "BKEZF9RIUYYLCOCQUXLXDTLWIIP"      , 30, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"    , 16, BigNumber.Base16StyleEncoding, "CDHEFOMRSRXETMSVHTOMCUNGJKBV"     , 26, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFDC80D0FFE-FF6AFFF99F-FF3E7685E46"  , 16, BigNumber.Base16StyleEncoding, "J7Z8IX87C-U5EOF2PV-J83LSQ679"     , 34, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFDC80D0-FFEFF6AF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "C9QINIM-C9RG7Q4-C9M2RBF-CRPIFOG"  , 34, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("D9DC80D0-50EA-46AF-FF99-7933E7685E46", 16, BigNumber.Base16StyleEncoding, "CMPF8CI-R4I-PWH-BWUR-CGVH7FVVJI"  , 34, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("FFDC80D0-FFEFF6AF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "FfNUQQ-FfTOgP-Fe8Jkz-E2h42i"      , 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "NkpGTa-NlJktt-NkGNCL-MVJAjY"      , 50, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "aBMZKI-aBnmlD-aAFSAT-XXkPni"      , 44, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "gjVfnW-gkLGeD-giEnIn-ddcJkK"      , 42, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "lCEURQ-lCmKMk-lAXKch-hUlhhS"      , 41, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46" , 16, BigNumber.Base16StyleEncoding, "BBkgUYQ-BBlciWP-BBjEQZL-lkWFTO"   , 40, BigNumber.ZeroAt61Encoding);

            // This one is broken:, notice the zeros
            //Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46", 16, BigNumber.Base16StyleEncoding, "BB@6UYQKBB+28WPKBB9EQZLK+@WFTO", 40, BigNumber.VoiceEncoding);
            //Number_Base_testcase("FFDC80D0-FFFFFFFF-FF99FFF3-E7685E46", 16, BigNumber.Base16StyleEncoding, "F7NUQQOF7YG4POF6+J@#OE'9]'_", 60, BigNumber.VoiceEncoding);


            // --------------------------------------------------------------------------
            //  Base 22 experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "C8?#]O+FD1j^!U~W",  88, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "C`5=X7c|=0=Il3WY",  87, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "DX1[dku[1SLCHRn;",  86, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "D.eRK*b{0t$de*B:",  85, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "EtRJ+wTiIC*UcILg",  84, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "FlpEPaI5hW!ecfZ-",  83, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "GuO8(YqodalhuBR@",  82, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "IC|)_fl!jn64RsSd",  81, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "JsihT~hAr|bL;(4W",  80, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "Lstz0DW*RKl(usDP",  79, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "OMD;2XN.7(kUt1{m",  78, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "RNniX$D(3s.4LKcA",  77, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "U;UGhqnf5Sz,pOCO",  76, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "Za@T}IT|jwR|DYz8",  75, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "4NI59HRp2,~go}m5",  74, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "cRPfhUjSkcxH5~oB",  73, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "lBtw3uBWiQgPdhr5",  72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "wAGi8Qx_VE,JhRkv",  71, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt35Encoding, "BBqpjFWc@OLR{1ewg", 70, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt76Encoding, "vB`;g~BWsQqPnr,e", 72, BigNumber.ZeroAt76Encoding);
            Number_Base_testcase("VUTSRQPONMLKJIHGFEDCBA", 22, BigNumber.ZeroAt76Encoding, ";AGsbQ{!VE|JrRu:", 71, BigNumber.ZeroAt76Encoding);
            Number_Base_testcase("VVVVVVVVVVVVVVVVVVVVVV", 22, BigNumber.ZeroAt35Encoding, "lJ2zDFGdzBfAOlhd", 72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("VVVVVVVVVVVVVVVVVVVVVV", 22, BigNumber.ZeroAt35Encoding, "wJ3Jipevg6fwCx{X", 71, BigNumber.ZeroAt35Encoding);


            // --------------------------------------------------------------------------
            //  Base 26 experiments
            // --------------------------------------------------------------------------
// blows up Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, Number.Base64StyleEncoding, "J'^# E_ Bd{ J Zw a$H Bg_ XU? Ju{Q", 96, Number.Base64StyleEncoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "Ku]M AE} Biw Kxi] c4N Blu YU7 KU5.",  94, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "M=he AFG ByG M?aw h@$ B,W 8ti Md`u",  88, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "RQ63 AFk CC} RT!9 q|t CG5 21q Qj;Q",  80, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "UFEm AF~ CT1 UI(1 wmd CXK bD3 TZGu",  76, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "XhPu AGO Ca} XlgK `YH Ce_ fTy Wqze",  72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "7BA3 AGc Ctg 7FzW BFQt Cxo kSi 9:GI", 68, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "2cam AG_ DLc 2iG9 BSiH DP_ qPU 3XfQ", 64, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 26, BigNumber.ZeroAt35Encoding, "ax1e AHM DYK bD8i B9Gp D7g tfK 0b7M", 62, BigNumber.ZeroAt35Encoding);
            //Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS", 27, " ABCDEFGHIJKLMNOPQRSTUVWXYZ", "R+[{|wkg5OYpYB1=2.J@b!ZkH}t(n", 88, Number.Base64StyleEncoding);
            //Number_Base_testcase("R+[{|wkg5OYpYB1=2.J@b!ZkH}t(n", 88, "THESE ARE THE TIMES THAT TRY MENS SOULS", 27, " ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            Number_Base_testcase("XWVUTSRQPONMLKJIHGFEDCBA"  , 24, BigNumber.ZeroAt35Encoding, "BO=h8zZ@N`G%GR20eY"   , 88, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "z3NrQ?Z?gRQpj$#w{;9"  , 88, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "D6~s)*yhwdebH3)ScS8O" , 81, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "EVO|sBoa!H){:zRTy4,K" , 80, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "LW!Elp6oe__RyVttqRD}" , 76, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "4fKTWps{rzAPZOqkcTno" , 72, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "Edxf,MLDq1zFO9fcz1jgw", 64, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "QwmmAVFFHlCpnc96Nvkey", 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "BG#pm7Tz=@Td6R3H!R'C" , 86, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "[hQ5]Y3S$+5B+/sQ.Jp"  , 87, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("ZYXWVUTSRQPONMLKJIHGFEDCBA", 26, BigNumber.ZeroAt35Encoding, "$gN,Q4Z4qRQzt=8;-^a"  , 88, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS.", 28, BigNumber.SimpleTextEncoding, "5293314784962090209386587683633167666869956547754588191939", 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("THESE ARE THE TIMES THAT TRY MENS SOULS.", 28, BigNumber.SimpleTextEncoding, "GnDmfHr5dixeBgvxUJURAVzcYTNrdi4tn", 60, BigNumber.ZeroAt61Encoding);
            Number_Base_testcase("VVV VVVV VVVVVVVV VVVV VVV", 22, BigNumber.ZeroAt26Encoding, "CD| tNn cZ6n@P tNn CD|", 72, BigNumber.ZeroAt76Encoding);
            Number_Base_testcase("VVV VVVV VVVVVVVV VVVV VVV", 22, BigNumber.ZeroAt26Encoding, "B.4 pwe XJaK|} pwe B.4", 75, BigNumber.ZeroAt76Encoding);
            Number_Base_testcase("VVVV VVVV VVVVVV VVVV VVVV", 22, BigNumber.ZeroAt26Encoding, "pwe pwe Dr~gD pwe pwe" , 75, BigNumber.ZeroAt76Encoding);
            Number_Base_testcase("VVVV VVVV VVVVVV VVVV VVVV", 22, BigNumber.ZeroAt26Encoding, "pwe pwe Dr~gD pwe pwe" , 75, BigNumber.ZeroAt88Encoding);
            Number_Base_testcase("VVVV VVVV VVVVVV VVVV VVVV", 22, BigNumber.ZeroAt26Encoding, "gj# gj# CO_$S gj# gj#" , 85, BigNumber.ZeroAt67Encoding);
            Number_Base_testcase("VVVVV VVVVV VV VVVVV VVVVV", 22, BigNumber.ZeroAt26Encoding, "IhZ= IhZ= F; IhZ= IhZ=", 85, BigNumber.ZeroAt67Encoding);
            Number_Base_testcase("VVVVVVV VVVVVVVV VVVVVVV"  , 22, BigNumber.ZeroAt26Encoding, "v.@4* MfVQ_P v.@4*"    , 85, BigNumber.ZeroAt67Encoding);


            // --------------------------------------------------------------------------
            //  Low Base experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("KJIHGFEDCBA", 11, BigNumber.ZeroAt35Encoding   , "BGP.{D`", 80, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("9999999999" , 10, BigNumber.Base16StyleEncoding, "D$uV$8" , 76, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("888888888"  , 9 , BigNumber.Base16StyleEncoding, "JasaI"  , 80, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("77777777"   , 8 , BigNumber.Base16StyleEncoding, "3z0P"   , 80, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("6666666"    , 7 , BigNumber.Base16StyleEncoding, "BmsW"   , 80, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("555555"     , 6 , BigNumber.Base16StyleEncoding, "HXP"    , 80, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("00011000"   , 2 , BigNumber.Base16StyleEncoding, "AAABC"  , 22, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("BC"         , 22, BigNumber.ZeroAt35Encoding   , "11000"  , 2 , BigNumber.Base16StyleEncoding);
            //Number_Base_testcase("E", 4, Number.Base64Encoding, "010101", 2, Number.Base64Encoding);
            //Number_Base_testcase("E", 22, Number.Base64Encoding, "AAAA", 1, Number.Base64Encoding);


            // --------------------------------------------------------------------------
            //  Base 60 experiments (date-time)
            // --------------------------------------------------------------------------
            Number_Base_testcase("xx"                 , 60, BigNumber.ZeroAt35Encoding   , "3599"               , 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("2008"               , 10, BigNumber.Base16StyleEncoding, "27"                 , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("12"                 , 10, BigNumber.Base16StyleEncoding, "M"                  , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("31"                 , 10, BigNumber.Base16StyleEncoding, "4"                  , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("23"                 , 10, BigNumber.Base16StyleEncoding, "X"                  , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("59"                 , 10, BigNumber.Base16StyleEncoding, "x"                  , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("59"                 , 10, BigNumber.Base16StyleEncoding, "x"                  , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("2008"               , 10, BigNumber.Base16StyleEncoding, "27"                 , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("2008-"              , 10, BigNumber.Base16StyleEncoding, "27-"                , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("2008-12-31-23-59-59", 10, BigNumber.Base16StyleEncoding, "27-M-4-X-x-x"       , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("27-M-4-X-x-x"       , 60, BigNumber.ZeroAt35Encoding   , "2008-12-31-23-59-59", 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("2008-12-31-23-3599" , 10, BigNumber.Base16StyleEncoding, "27-M-4-X-xx"        , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("27-M-4-X-xx"        , 60, BigNumber.ZeroAt35Encoding   , "2008-12-31-23-3599" , 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("2008-12-31-86399"   , 10, BigNumber.Base16StyleEncoding, "27-M-4-Xxx"         , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("27-M-4-Xxx"         , 60, BigNumber.ZeroAt35Encoding   , "2008-12-31-86399"   , 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("2008-12-31 23:59:59", 10, BigNumber.Base16StyleEncoding, "27-M-4 X:x:x"       , 60, BigNumber.ZeroAt35Encoding   );
            Number_Base_testcase("27-M-4 X:x:x"       , 60, BigNumber.ZeroAt35Encoding   , "2008-12-31 23:59:59", 10, BigNumber.Base16StyleEncoding);
            Number_Base_testcase("2009-12-31-23-59-59", 10, BigNumber.Base16StyleEncoding, "hd-M-f-X-2-2"       , 60, BigNumber.ZeroAt61Encoding   );


            // --------------------------------------------------------------------------
            //  High Base experiments
            // --------------------------------------------------------------------------
            Number_Base_testcase("123456", 10, BigNumber.Base16StyleEncoding, "M/("    , 98, BigNumber.ZeroAt35Encoding);
            Number_Base_testcase("M/("   , 98, BigNumber.ZeroAt35Encoding   , "123456" , 10, BigNumber.Base16StyleEncoding);
            //Number_Base_testcase("123456", 10, BigNumber.Base16StyleEncoding, "12|59|3", 99, BigNumber.ZeroAt35Encoding);
            //Number_Base_testcase("12|59|3", 99, Number.Base64Encoding, "123456", 10, Number.Base16Encoding);


            // --------------------------------------------------------------------------
            //  Special Cases
            // --------------------------------------------------------------------------
            Number_HexBase_testcase("ffff", 16, BigNumber.Base16StyleEncoding, "P,,",  64, BigNumber.ZeroAt35Encoding);
            Number_ToBase10_testcase("AABC", 22, BigNumber.ZeroAt35Encoding, "0024"  , 10);
            Number_ToBase10_testcase("ABC" , 22, BigNumber.ZeroAt35Encoding, "024"   , 10);
            Number_ToBase10_testcase("A"   , 22, BigNumber.ZeroAt35Encoding, "0"     , 10);
            Number_ToBase10_testcase("AA"  , 22, BigNumber.ZeroAt35Encoding, "00"    , 10);
            Number_ToBase10_testcase("AAA" , 22, BigNumber.ZeroAt35Encoding, "000"   , 10);
            Number_ToBase10_testcase("BC"  , 22, BigNumber.ZeroAt35Encoding, "24"    , 10);
            Number_ToBase10_testcase("CBB" , 22, BigNumber.ZeroAt35Encoding, "991"   , 10);
            Number_ToBase10_testcase("UBA" , 22, BigNumber.ZeroAt35Encoding, "9702"  , 10);
            Number_ToBase10_testcase("JINK", 22, BigNumber.ZeroAt35Encoding, "100000", 10);
            Number_ToBase10_testcase("-BC" , 22, BigNumber.ZeroAt35Encoding, "-24"   , 10);

            for (int i = 19; i < 80; ++i)
                Number_GUIDencode_testcase("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", i, BigNumber.ZeroAt35Encoding);

            _result += Assert.Conclusion;
        }
        private static void Number_Base_testcase(string src, int srcRadix, string srcEncoding
            , string tgt, int tgtRadix, string tgtEncoding)
        {
            // --------------------------------------------------------------------------
            //  Convert source to target
            // --------------------------------------------------------------------------
            BigNumber num1 = new BigNumber(src, srcRadix, srcEncoding);
            BigNumber num2 = num1.Base(tgtRadix, tgtEncoding);
            BigNumber num3 = num2.Base(srcRadix, srcEncoding);
            string    str2 = num2.ToString();
            Assert.That(str2, Is.equal_to, tgt);
            Assert.That(num3.Equals(num1), Is.equal_to, true);


            // --------------------------------------------------------------------------
            //  Convert target to source
            // --------------------------------------------------------------------------
            BigNumber num4 = new BigNumber(tgt, tgtRadix, tgtEncoding);
            BigNumber num5 = num4.Base(srcRadix, srcEncoding);
            BigNumber num6 = num5.Base(tgtRadix, tgtEncoding);
            string    str5 = num5.ToString();
            Assert.That(str5, Is.equal_to, src);
            Assert.That(num6.Equals(num4), Is.equal_to, true);

        }
        private static void Number_HexBase_testcase(string src, int srcRadix, string srcEncoding
            , string tgt, int tgtRadix, string tgtEncoding)
        {
            BigNumber num1 = new BigNumber(src, srcRadix, srcEncoding);
            BigNumber num2 = num1.Base(tgtRadix, tgtEncoding);
            BigNumber num3 = num2.Base(srcRadix, srcEncoding);
            string    str2 = num2.ToString();
            Assert.That(str2, Is.equal_to, tgt);
            Assert.That(num3.Equals(num1), Is.equal_to, true);
        }
        private static void Number_GUIDencode_testcase(string srcGuid
            , int tgtRadix, string tgtEncoding)
        {
            BigNumber guid = new BigNumber(srcGuid, 16, BigNumber.Base16StyleEncoding);
            BigNumber temp = guid.Base(tgtRadix, tgtEncoding);
            int len = temp.ToString().Length;
            BigNumber output = temp.Base(16, BigNumber.Base16StyleEncoding);
            Assert.That(output.ToString(), Is.equal_to, srcGuid);
        }
        private static void Number_ToBase10_testcase(string src, int srcRadix, string srcEncoding
            , string tgt, int tgtRadix)
        {
            BigNumber num = new BigNumber(src, srcRadix, srcEncoding);
            //Assert.That(string.IsNullOrEmpty(num.Encoding)).Or(num.Radix, Is.greater_than, 98);
            BigNumber output = num.Base(tgtRadix, "");
            string str = output.ToString();
            Assert.That(str, Is.equal_to, tgt);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Number_Encode_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Number_Encode_test()
        {
            Assert.ThingsAbout("Number", "Encode");

            string str = "";
            long num;


            bool encodeDecodeByRadixIsOk = true;
            for (int radix = 2; radix < 98; ++radix)
                for (int i = 0; i < 1000; ++i)
                    encodeDecodeByRadixIsOk &= (BigNumber.DecodeLong(radix,BigNumber.EncodeLong(radix, i)) == i);
            Assert.That(encodeDecodeByRadixIsOk, Is.equal_to, true);


            bool encodeDecodeLargeNumber = true;
            RandomSource rs = RandomSource.New();
            Random r = rs.Random;
            for (int i = 0; i < 1000; ++i)
            {
                int radix = r.Next(2, 98);
                num = rs.Skew(0, 1000000000);
                encodeDecodeByRadixIsOk &= (BigNumber.DecodeLong(radix,BigNumber.EncodeLong(radix, num)) == num);
            }
            Assert.That(encodeDecodeLargeNumber, Is.equal_to, true);


            // --------------------------------------------------------------------------
            //  Base 84 experiments
            // --------------------------------------------------------------------------
            str = BigNumber.EncodeLong(84, 41821194230000000L);
            str = BigNumber.EncodeLong(84, 4096);
            string encodedNumber = BigNumber.EncodeLong(16, 15555);
            Assert.That(BigNumber.EncodeLong(84, 1     )   , Is.equal_to, "B"  );
            Assert.That(BigNumber.EncodeLong(84, 10    )   , Is.equal_to, "K"  );
            Assert.That(BigNumber.EncodeLong(84, 84    )   , Is.equal_to, "BA" );
            Assert.That(BigNumber.EncodeLong(84, 123   )   , Is.equal_to, "Bd" );
            Assert.That(BigNumber.EncodeLong(84, 1776  )   , Is.equal_to, "VM" );
            Assert.That(BigNumber.EncodeLong(84, 4096  )   , Is.equal_to, "m@" );
            Assert.That(BigNumber.EncodeLong(84, 7056  )   , Is.equal_to, "BAA");
            Assert.That(BigNumber.EncodeLong(84, 7141  )   , Is.equal_to, "BBB");
            Assert.That(BigNumber.EncodeLong(84, 123456)   , Is.equal_to, "Rfy");
            Assert.That(BigNumber.EncodeLong(84, 4182119423), Is.equal_to, "/////");
            Assert.That(BigNumber.EncodeLong(84, 4182119424L), Is.equal_to, "BAAAAA");
            Assert.That(BigNumber.EncodeLong(84, 418211349443L), Is.equal_to, "BP/+/#/");
            str = BigNumber.EncodeLong(84, 41821194230000000L);
            num = BigNumber.DecodeLong(84, str);


            // --------------------------------------------------------------------------
            //  Base 26 experiments
            // --------------------------------------------------------------------------
            num = BigNumber.DecodeLong(26, "BANANA");
            str = BigNumber.EncodeLong(26, num);
            Assert.That(BigNumber.EncodeLong(26,BigNumber.DecodeLong(26, "BANANA")), Is.equal_to, "BANANA");

            _result += Assert.Conclusion;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Number_Int_test -->
        /// <summary>
        ///      Tests Ints and Longs
        /// </summary>
        public void Number_Int_test()
        {
            Assert.ThingsAbout("Number", "Int");


            // --------------------------------------------------------------------------
            //  Base 22 experiments
            // --------------------------------------------------------------------------
            Number_Int_testcase("BA", 22, 22);
            Number_Int_testcase("CBB", 22, 991);
            Number_Int_testcase("JINK", 22, 100000);
            Number_Int_testcase("JINKY", 22, 2147483647);
            Number_Long_testcase(0L, "0");
            Number_Long_testcase(1234567890123456789L, "1234567890123456789");
            Number_Long_testcase(-1234567890123456789L, "-1234567890123456789");

            _result += Assert.Conclusion;
        }
        private static void Number_Long_testcase(long longNum, string target)
        {
            BigNumber num = new BigNumber(longNum);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, target);
            long longOut = num.LongInt();
            Assert.That(longOut, Is.equal_to, longNum);
        }
        private static void Number_Int_testcase(string input, int radix, int target)
        {
            BigNumber num = new BigNumber(input, radix, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            int output = num.Integer();
            Assert.That(output, Is.equal_to, target);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Number_Plus_test -->
        /// <summary>
        ///      Plus and Minus tests
        /// </summary>
        public void Number_PlusMinus_test()
        {
            Assert.ThingsAbout("Number", "PlusMinus");

            // --------------------------------------------------------------------------
            //  Base 22 experiments
            // --------------------------------------------------------------------------
            Number_Plus_testcase(47, 22, 18,  "CV");
            Number_Plus_testcase(47, 22, 19,  "DA");
            Number_Plus_testcase(47, 22, 20,  "DB");
            Number_Plus_testcase(47, 22, 484, "BCD");
            Number_Plus_testcase(47, 22, 485, "BCE");

            // --------------------------------------------------------------------------
            //  Base 10 experiments
            // --------------------------------------------------------------------------
            Number_Plus_testcase ("0"   , "0"   , "0"   , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("2"   , "2"   , "4"   , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("7"   , "8"   , "15"  , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("27"  , "44"  , "71"  , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("123" , "0"   , "123" , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("0"   , "123" , "123" , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("00"  , "123" , "123" , 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("556" , "667" , "1223", 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("556" , "66701", "67257", 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("66701", "556", "67257", 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("11111111111111111111", "2222222222222222229", "13333333333333333340", 10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("44"  , "-27" , "17",  10, BigNumber.Base16StyleEncoding);
            Number_Plus_testcase ("-44" , "27"  , "-17", 10, BigNumber.Base16StyleEncoding);
            Number_Minus_testcase("542" , "378" , 10, BigNumber.Base16StyleEncoding, "164" );
            Number_Minus_testcase("378" , "542" , 10, BigNumber.Base16StyleEncoding, "-164");
            Number_Minus_testcase("-542", "-378", 10, BigNumber.Base16StyleEncoding, "-164");
            Number_Minus_testcase("-378", "-542", 10, BigNumber.Base16StyleEncoding, "164" );
            Number_Minus_testcase("999" , "-1"  , 10, BigNumber.Base16StyleEncoding, "1000");
            Number_Minus_testcase("999" , "0"   , 10, BigNumber.Base16StyleEncoding, "999" );
            Number_Minus_testcase("999" , "1"   , 10, BigNumber.Base16StyleEncoding, "998" );
            Number_Minus_testcase("1002", "15"  , 10, BigNumber.Base16StyleEncoding, "0987");
            Number_Minus_testcase("1002", "1002", 10, BigNumber.Base16StyleEncoding, "0000");
            Number_Minus_testcase("1002", "1003", 10, BigNumber.Base16StyleEncoding, "-0001");

            _result += Assert.Conclusion;
        }
        private static void Number_Minus_testcase(string num1, string num2, int radix, string encoding, string target)
        {
            BigNumber x = new BigNumber(num1, radix, encoding);
            BigNumber y = new BigNumber(num2, radix, encoding);
            BigNumber z = x.Minus(y);
            string output = z.ToString();
            Assert.That(output, Is.equal_to, target);
        }
        private static void Number_Plus_testcase(string n1, string n2, string tgt, int radix, string encoding)
        {
            BigNumber x = new BigNumber(n1, radix, encoding);   Assert.That(string.IsNullOrEmpty(x.Encoding), Is.equal_to, false).Or(x.Radix, Is.greater_than, 98);
            BigNumber y = new BigNumber(n2, radix, encoding);   Assert.That(string.IsNullOrEmpty(y.Encoding), Is.equal_to, false).Or(y.Radix, Is.greater_than, 98);
            BigNumber z = x.Plus(y);                            Assert.That(string.IsNullOrEmpty(z.Encoding), Is.equal_to, false).Or(z.Radix, Is.greater_than, 98);
            string zstr = z.ToString();
            Assert.That(zstr, Is.equal_to, tgt);
            string xstr = x.ToString();
            Assert.That(n1, Is.equal_to, "0").Or(n2, Is.equal_to, "0").Or(zstr, Is.not_equal_to, xstr);
        }
        private static void Number_Plus_testcase(int start, int radix, int plus, string target)
        {
            BigNumber num = new BigNumber(start, radix, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            num.PlusEquals(plus);
            string output = num.ToString();
            Assert.That(output, Is.equal_to, target);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Number_RelevantEncoding_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Number_RelevantEncoding_test()
        {
            Assert.ThingsAbout("Number", "RelevantEncoding");

            BigNumber num = new BigNumber("FBC", 22, BigNumber.ZeroAt35Encoding);
            Assert.That(string.IsNullOrEmpty(num.Encoding), Is.equal_to, false).Or(num.Radix, Is.greater_than, 98);
            string output = num.RelevantEncoding;
            Assert.That(output, Is.equal_to, "ABCDEFGHIJKLMNOPQRSTUV");

            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Number_Times_test -->
        /// <summary>
        /// 
        /// </summary>
        public void Number_Times_test()
        {
            Assert.ThingsAbout("Number", "Times");


            // --------------------------------------------------------------------------
            //  Number times integer cases
            // --------------------------------------------------------------------------
            Number_Times_testcase("12",  -2, 10, BigNumber.Base16StyleEncoding, "-24");
            Number_Times_testcase("12",  -1, 10, BigNumber.Base16StyleEncoding, "-12");
            Number_Times_testcase("12",  0,  10, BigNumber.Base16StyleEncoding, "00");
            Number_Times_testcase("12",  1,  10, BigNumber.Base16StyleEncoding, "12");
            Number_Times_testcase("12",  2,  10, BigNumber.Base16StyleEncoding, "24");
            Number_Times_testcase("12",  12, 10, BigNumber.Base16StyleEncoding, "144");
            Number_Times_testcase("-12", -1, 10, BigNumber.Base16StyleEncoding, "12");
            Number_Times_testcase("-12", 1,  10, BigNumber.Base16StyleEncoding, "-12");


            // --------------------------------------------------------------------------
            //  Number times Number cases
            // --------------------------------------------------------------------------
            Number_Times_testcase("0",   "0",  10, BigNumber.Base16StyleEncoding, "0"  );
            Number_Times_testcase("00",  "0",  10, BigNumber.Base16StyleEncoding, "00" );
            Number_Times_testcase("12",  "0",  10, BigNumber.Base16StyleEncoding, "00" );
            Number_Times_testcase("12",  "1",  10, BigNumber.Base16StyleEncoding, "12" );
            Number_Times_testcase("12",  "2",  10, BigNumber.Base16StyleEncoding, "24" );
            Number_Times_testcase("10",  "10", 10, BigNumber.Base16StyleEncoding, "100");
            Number_Times_testcase("12",  "-2", 10, BigNumber.Base16StyleEncoding, "-24");
            Number_Times_testcase("12",  "-1", 10, BigNumber.Base16StyleEncoding, "-12");
            Number_Times_testcase("12",  "12", 10, BigNumber.Base16StyleEncoding, "144");
            Number_Times_testcase("-12", "1",  10, BigNumber.Base16StyleEncoding, "-12");
            Number_Times_testcase("-12", "-1", 10, BigNumber.Base16StyleEncoding, "12" );
            Number_Times_testcase("1B", "12", 16, BigNumber.Base16StyleEncoding, "1E6");
            Number_Times_testcase("10000000000", "10000000000", 10, BigNumber.Base16StyleEncoding, "100000000000000000000");
            Number_Times_testcase("1234567890123", "1234567890123", 10, BigNumber.Base16StyleEncoding, "1524157875322755800955129");
            Number_Times_testcase("CHRIST", "JESUS", 26, BigNumber.ZeroAt35Encoding, "VBVVIDPPPE");
            Number_Times_testcase("CHRIST", "JESUS", 25, BigNumber.ZeroAt35Encoding, "VFFJLOAYRR");
            Number_Times_testcase("CHRIST", "JESUS", 24, BigNumber.ZeroAt35Encoding, "VIOXRCOLWG");
            Number_Times_testcase("CHRIST", "JESUS", 23, BigNumber.ZeroAt35Encoding, "VMBPBSHDFU");
            Number_Times_testcase("CHRIST", "JESUS", 22, BigNumber.ZeroAt35Encoding, "VPMIMODUPM");


            // --------------------------------------------------------------------------
            //  Does formatting make any sense to multiply?
            // --------------------------------------------------------------------------
         // Number_Times_testcase("1:1", "1", 10, Number.Base16StyleEncoding, "781");

            _result += Assert.Conclusion;
        }
        private static void Number_Times_testcase(string num1, int num2, int radix, string encoding, string tgt)
        {
            BigNumber x = new BigNumber(num1, radix, encoding);
            BigNumber y = x.TimesEquals(num2);
            string output = y.ToString();
            Assert.That(output, Is.equal_to, tgt);
        }
        private static void Number_Times_testcase(string num1, string num2, int radix, string encoding, string tgt)
        {
            BigNumber x = new BigNumber(num1, radix, encoding);
            BigNumber y = new BigNumber(num2, radix, encoding);
            BigNumber z = new BigNumber(tgt, radix, encoding);
            BigNumber v = x.Times(y);
            BigNumber w = y.Times(x);


            string output = v.ToString();
            Assert.That(output, Is.equal_to, tgt);
            output = w.ToString();
            Assert.That(output, Is.equal_to, tgt);


            Assert.That(v.Equals(z), Is.equal_to, true);
            Assert.That(z.Equals(v), Is.equal_to, true);
            Assert.That(w.Equals(z), Is.equal_to, true);
            Assert.That(v.Equals(w), Is.equal_to, true);
        }
    }
}
