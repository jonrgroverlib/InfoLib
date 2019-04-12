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
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RegexLogStep -->
    /// <summary>
    ///      The RegexLogStep class logs the steps taken to generate a target
    /// </summary>
    /// <remarks>beta code</remarks>
    public class RegexLogStep
    {
        public string Input      { get { return _input;           } } private string _input;
        public string Output     { get { return _output;          } } private string _output;
        public string ShowStep   { get { return _step.ToString(); } } private RegexStep _step;
        public int    Iterations { get { return _step.Changes;    } }
        public string Report
        {
            get
            {
                return (_step.ToString()
                    + "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n" + _input
                    + "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n" + _output);
            }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public RegexLogStep(string input, string from, string to, string output)
        {
            _input      = input;
            _step       = new RegexStep(from, to);
            _output     = output;
        }
        public RegexLogStep(string input, RegexStep step, string output)
        {
            _input      = input;
            _step       = step;
            _output     = output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Str -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + _step.Changes + ")"
                + " " + _step.ToString() + " :"
                + "\r\n" + _output;
            //return _step.ToString() + " :" + "\r\n" + _input + "\r\n" + " ------->" + "\r\n" + _output;
        }


    }
}
