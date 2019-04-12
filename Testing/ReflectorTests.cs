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
using InfoLib.Endemes;                // for Endeme
using InfoLib.Data   ;                // for Throws
using System         ;                // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Testing  // add to this namespace very sparingly and use only System references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- ReflectorTests -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code
    ///      look at delegate and reflection in the BackgroundWorkerClass in Alan
    /// </remarks>
    public class ReflectorTests
    {
        public static void Equality(string className, string methodName, int parameterValue, int output, int target)
        {
            if (output != target)
            {
                Throws.A(new Exception(className + "." + methodName + "(" + parameterValue + ") = " + output + " != " + target), Throws.Actions, "PS");
            }
        }

        public static void Equality(string className, string methodName, string parameterValue, int output, int target)
        {
            if (output != target)
            {
                Throws.A(new Exception(className + "." + methodName + "(" + parameterValue + ") = " + output + " != " + target), Throws.Actions, "PS");
            }
        }

        public static void Equality(string className, string methodName, Endeme parameterValue, int output, int target)
        {
            if (output != target)
            {
                Throws.A(new Exception(className + "." + methodName + "(" + parameterValue + ") = " + output + " != " + target), Throws.Actions, "PS");
            }
        }

        public static void Equality(string className, string methodName, string parameterValue, object output, string target)
        {
            if (output == null || output.ToString() != target)
            {
                Throws.A(new Exception(className + "." + methodName + "(" + parameterValue + ") = " + output + " != " + target), Throws.Actions, "PS");
            }
        }
    }
}
