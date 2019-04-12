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
using System.Collections.Generic;     // for List<>
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Concatenator -->
    /// <summary>
    ///      The Concatenator class concatenates many strings into one
    ///      usually including no delimiters for blank strings,
    ///      it is built on top of of StringBuilder and is used similarly to StringBuilder
    /// </summary>
    /// <remarks>
    ///      This feels like it's going to have a stack of delimiters someday
    /// </remarks>
    /// <remarks>beta code - used once in production</remarks>
    public class Concatenator
    {
        // ----------------------------------------------------------------------------------------
        //  Members (This feels like it's going to have a stack of delimiters someday)
        // ----------------------------------------------------------------------------------------
        public  string Delim { get { return _innerDelim; } } private string _innerDelim; private string _iDelim; // starts as blank, then gets set to the _delimiter above for second and subsequent concatenated values
        public  string Outer { get { return _outerDelim; } } private string _outerDelim; private string _oDelim;
        private bool _primedForOuter;
        private StringBuilder _str;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public Concatenator(                                                      ) { Init(""   ,           ","  , "\r\n"); }
        public Concatenator(                            string delim              ) { Init(""   ,           delim, "\r\n"); }
        public Concatenator(              int capacity                            ) { Init(""   , capacity, ","  , "\r\n"); }
        public Concatenator(              int capacity, string delim              ) { Init(""   , capacity, delim, "\r\n"); }
        public Concatenator(string value, int capacity                            ) { Init(value, capacity, ","  , "\r\n"); }
        public Concatenator(string value, int capacity, string delim              ) { Init(value, capacity, delim, "\r\n"); }
        public Concatenator(                            string delim, string outer) { Init(""   ,           delim, outer ); }
        public Concatenator(              int capacity, string delim, string outer) { Init(""   , capacity, delim, outer ); }
        public Concatenator(string value, int capacity, string delim, string outer) { Init(value, capacity, delim, outer ); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Concatenate -->
        /// <summary>
        ///      Concatenates the value onto the string if it is not blank
        ///      using the instantiated Concatenate object delimiter value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Concatenator Concatenate(object value)
        {
            if (value != null)
            {
                string str = value.ToString();
                if (!string.IsNullOrEmpty(str.Trim()))
                {
                    if (_primedForOuter)
                    {
                        ConcatOuter(str);
                    }
                    else
                    {
                        _str.Append(_iDelim + str);
                        _iDelim = _innerDelim;
                        _oDelim = _outerDelim;
                    }
                }
            }
            return this;
        }
        public Concatenator Concatenate(object value, bool includeBlanks)
        {
            return Concatenate(value);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Concatenate -->
        /// <summary>
        ///      Concatenates the non-blank values of a list onto a string using the delimiter
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public Concatenator Concatenate(List<object> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Concatenate(list[i]);
            }
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatenateOuter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public Concatenator ConcatenateOuter(object value)
        {
            _primedForOuter = true;
            _iDelim = "";
            if (value != null)
                if (!string.IsNullOrEmpty(value.ToString().Trim()))
                    ConcatOuter(value.ToString());
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatenateWithBlanks -->
        /// <summary>
        ///      Concatenates the value onto a (usually comma) delimited file
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public Concatenator ConcatenateWithBlanks(object value)
        {
            string str = "";
            if (value == null)  str = "";
            else str = value.ToString();


            if (Regex.IsMatch(str, _innerDelim))
                 _str.Append(_iDelim + "\"" + str + "\"");
            else _str.Append(_iDelim + str);
            _iDelim = _innerDelim;
            _oDelim = _outerDelim;
            return this;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatOuter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <remarks>beta code - used once in production</remarks>
        private void ConcatOuter(string str)
        {
            _str.Append(_oDelim + str);
            _primedForOuter = false;
            _oDelim = _outerDelim;
            _iDelim = _innerDelim;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delim"></param>
        /// <param name="outer"></param>
        /// <remarks>beta code - used once in production</remarks>
        private void Init(string delim, string outer)
        {
            _innerDelim     = delim;
            _iDelim         = ""   ;
            _outerDelim     = outer;
            _oDelim         = ""   ;
            _primedForOuter = false;
        }
        private void Init(string value              , string delim, string outer) { _str = new StringBuilder(value          ); Init(delim, outer); }
        private void Init(string value, int capacity, string delim, string outer) { _str = new StringBuilder(value, capacity); Init(delim, outer); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public override string ToString()
        {
            return _str.ToString();
        }
    }
}