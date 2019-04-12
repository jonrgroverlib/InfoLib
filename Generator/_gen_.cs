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
using InfoLib.Testing;                            // for Is.IsNull
using System;                                     // for DateTime, Guid
using System.Collections.Generic;                 // for Dictionary<,>
using System.Data;                                // for NoNullAllowedException
using System.Data.SqlTypes;                       // for SqlDateTime
using System.IO;                                  // for File, StreamWriter, TextReader
using System.Reflection;                          // for BindingFlags, MethodInfo
using System.Text;                                // for UTF8Encoding
using System.Text.RegularExpressions;             // for Regex
using System.Xml;                                 // for XmlNode
using System.Xml.Schema;                          // for XmlSchemaSet etc
using System.Windows.Forms;     // add reference  // for CheckBox, ComboBox, KeyPressEventArgs

// ------------------------------------------------------------------------------------------------
/// <!-- namespace CHNw.CommonLib.Core -->
/// <summary>
///      The CHNw.CommonLib.Core directory is a place to put various core classes, enums, and
///      interfaces that work together with the static _ methods.
/// Core namespace is an atom
/// </summary>
namespace InfoLib.Generator
{
    // --------------------------------------------------------------------------------------------
    /// <!-- _ -->
    /// <summary>
    ///      A copy of the functions needed from __  , refactor until removed
    /// </summary>
    /// <remarks>beta code - used once in production, move this stuff to __</remarks>
    public static class _gen_
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Integer -->
        /// <summary>
        ///      Takes a shot at converting what is sent to it to an integer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Integer(object obj, int defaultValue)
        {
            if (Is.IsNull(obj))
                return defaultValue;
            else
            {
                Type type = obj.GetType();
                string str;
                int value;


                // ----------------------------------------------------------------------
                //  Standard approaches
                // ----------------------------------------------------------------------
                if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int32)
                    || type == typeof(Int64))
                    return (int)obj;
                str = obj.ToString();
                if (int.TryParse(str, out value))  return value;


                return defaultValue;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Str -->
        /// <summary>
        ///      Converts an object to a string defaulting as specified if it is a null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public static string Str(object obj, string defaultValue)
        {
            if (Is.IsNull(obj)) return defaultValue;
            else
            {
                if (obj.GetType() == typeof(Control))
                    return ((Control)obj).Text;
                return obj.ToString();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteStringToFile -->
        /// <summary>
        ///      Writes a non-humongous string to a file
        /// </summary>
        /// <remarks>Shouldn't this be done in another class?</remarks>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteStringToFile(string filePath, string content, bool append)
        {
            if (content == null)
            {
                throw new NoNullAllowedException("WriteStringToFile error - content is null");
            }
            else
            {
                string[] lines = new string[1];
                StreamWriter fileWriter = null;
                lines[0] = content;


                if (filePath == null || filePath.Length == 0) return;
                if (lines == null || lines.Length == 0) return;


                try
                {
                    if (append)
                        fileWriter = File.AppendText(filePath);
                    else
                        fileWriter = File.CreateText(filePath);
                    foreach (string line in lines) fileWriter.Write(line);
                }
                catch { throw new IOException("Unable to write string to file "+filePath); }
                finally { if (fileWriter != null)  fileWriter.Close(); }
            }
        }
    }
}
