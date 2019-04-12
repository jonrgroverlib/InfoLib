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
using System;                         // for FormatException
using System.Collections.Generic;     // for List
using System.Data;                    // for NoNullAllowedException
using System.IO;                      // for File
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for RegexOptions, MatchEvaluator

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RegexTable -->
    /// <summary>
    ///      The RegexTable class contains and applies a list of Regex.Replace's to a string
    /// </summary>
    /// <remarks>production ready</remarks>
    public class RegexTable
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Steps, Comment, Log -->
        /// <summary>The steps in the list or processes to put the focus string through</summary>
        public List<RegexStep>    Step     { get { return _step;    } set { _step    = value; } } private List<RegexStep>    _step;        /// <summary>Comments on what each of the steps does</summary>
        public List<string>       Comments { get { return _comment; } set { _comment = value; } } private List<string>       _comment;      /// <summary>The log that holds the transformations</summary>
        public List<RegexLogStep> Log      { get { return _log;     }                           } private List<RegexLogStep> _log;
        public bool               KeepLog  { get { return _keepLog; } set { _keepLog = value; } } private bool               _keepLog;
        public string             LogPath  { get { return _logPath; } set { _logPath = value; } } private string             _logPath;


        // ----------------------------------------------------------------------------------------
        /// <!-- Initial, Str -->                                                                        /// <summary>The initial string</summary>
        public string Initial { get { return _initial; }                       } private string _initial; /// <summary>The target string upon which the work is done</summary>
        public string Str     { get { return _str;     } set { _str = value; } } private string _str;


        // ----------------------------------------------------------------------------------------
        ///  Constructors
        // ----------------------------------------------------------------------------------------
        public RegexTable()                                         { Set("" , false  , ""     ); Init(); } /// <param name="initialStr">The starting string to be modified</param>
        public RegexTable(string str)                               { Set(str, false  , ""     ); Init(); } /// <param name="initialStr">The starting string to be modified</param><param name="keepLog">whether to keep a (potentially huge) log of the steps and their effects</param><param name="logFilePath">The file path to put the log in</param>
        public RegexTable(string str, bool keepLog, string logPath) { Set(str, keepLog, logPath); Init(); }

        private void Set(string initialStr, bool keepLog, string logPath)
        {
            _keepLog = keepLog;
            _str = initialStr;
            _initial = initialStr;
            _logPath = logPath;
        }

        private void Init()
        {
            _step = new List<RegexStep>();
            _comment = new List<string>();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds another step
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <param name="options"></param>
        /// <remarks>production ready</remarks>
        public   void Add(string pattern, string replacement, RegexOptions options, string comment) { _step.Add(new RegexStep(pattern     , replacement , options     )); _comment.Add(comment); }
        public   void Add(string pattern, string replacement                      , string comment) { _step.Add(new RegexStep(pattern     , replacement               )); _comment.Add(comment); }
        public   void Add(RegexStep step                                          , string comment) { _step.Add(new RegexStep(step.Pattern, step.Replace, step.Options)); _comment.Add(comment); }
        internal void Add(string pattern, MatchEvaluator method                   , string comment) { _step.Add(new RegexStep(pattern, method                         )); _comment.Add(comment); }
        public   void Add(string command                                          , string comment)
        {
            if (Regex.IsMatch(command, @"\[\[\w+\]\]$"))
                _step.Add(new RegexStep("", command));
            else throw new FormatException("This isn't a special operation: " + command);
            _comment.Add(comment);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Convert -->
        /// <summary>
        ///      Runs the string through the list of Regex.Replace's
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string Convert()
        {
            string input;
            _initial = _str;
            if (_keepLog) _log = new List<RegexLogStep>();
            int count = _step.Count;
            for (int i = 0; i < count; ++i)
            {
                input = _str;
                _str = _step[i].Process(input);
                if (_keepLog) _log.Add(new RegexLogStep(input, _step[i], _str));
            }
            if (_keepLog)
                WriteStringToFile(_logPath, LogListing(), false);
            return _str;
        }
        public string Convert(string str) { Set(str, _keepLog, _logPath); return Convert(); }
        public string Convert(string str, bool keepLog, string logPath)
                                          { Set(str,  keepLog,  logPath); return Convert(); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns a listing of the object
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public override string ToString()
        {
            StringBuilder listing = new StringBuilder();
            string delim = "";
            int count = _step.Count;
            for (int i = 0; i < count; ++i)
            {
                listing.Append(delim + _step[i].ToString() + "    -- " + _comment[i]);
                delim = "\r\n";
            }
            return listing.ToString();
        }
        public string Listing { get { return this.ToString(); } }

        // ----------------------------------------------------------------------------------------
        /// <!-- LogListing -->
        /// <summary>
        ///      Returns a listing of the log
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string LogListing()
        {
            if (_keepLog)
            {
                StringBuilder output = new StringBuilder();
                int count = _log.Count;
                string delim1 = "";
                string delim = delim1;
                string delim2 = "\r\n"
                    + "=================================================================================================================="
                    + "\r\n";


                if (count > 0)                  { output.Append(delim + ShowOriginal()); delim = delim2; }
                for (int i = 0; i < count; ++i) { output.Append(delim + ShowStep(i));    delim = delim2; }
                return output.ToString();
            }
            else return "no log was kept";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ShowOriginal, ShowStep -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        private string ShowOriginal()
        {
            string step = "original" + ":" + " " + _log[0].ShowStep + " :" + " " + _comment[0] + " :";
            return step + "\r\n" + _log[0].Input;
        }
        private string ShowStep(int i)
        {
            string step = "step " + i + ":" + " " + _log[i].ShowStep + " :" + " " + _comment[i] + " :";
            return step + "\r\n" + _log[i].Output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WriteStringToFile -->
        /// <summary>
        ///      Writes a non-humongous string to a file
        /// </summary>
        /// <remarks>Shouldn't this be done in another class?</remarks>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <remarks>production ready</remarks>
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
                catch { throw new IOException("Unable to write string to file " + filePath); }
                finally { if (fileWriter != null)  fileWriter.Close(); }
            }
        }
    }
}
