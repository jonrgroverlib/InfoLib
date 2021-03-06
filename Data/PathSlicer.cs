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
using System;                         // for ArgumentException, ArgumentNullException
using System.Collections.Generic;     // for List
using System.IO;                      // for FileInfo
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- PathSlicer -->
    /// <summary>
    ///      deprecated
    ///      
    ///      The PathSlicer class implements a very simple path storage and navigation mechanism
    ///      allowing easy movement, analysis and construction up and down the length of a path
    ///
    ///      The PathBuilder class stores a path in four formats
    ///      convenient for xpath or FileInfo:
    ///       1. The path as a string
    ///       2. The path as a series of paths each one closer to the objective than the last
    ///       3. The remainders of the way to go for each path in the series above to the objective
    ///       4. The path as a list of segments
    /// </summary>
    /// <remarks>
    /// TODO:
    ///      There are three kinds of paths: XPaths, File paths (and directory paths), and URI/URL's
    ///      Therefore this probably ought to be four classes: a base class and a class for each of
    ///      these three (or four) types of paths
    /// 
    ///      This namespace is a primitive so it should use nothing but System references
    ///      
    /// 
    ///      deprecated
    /// </remarks>
    public class PathSlicer
    {


        // ----------------------------------------------------------------------------------------
        /// <!-- Delimiter -->
        private string  _delimiter;
        /// <summary>
        ///      Saves the path delimiter
        /// </summary>
        public string  Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Path -->
        private string _path;
        /// <summary>
        ///      The path itself
        /// </summary>
        public string Path { get { return _path; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Remainders -->
        private List<string> _remainders;
        /// <summary>
        ///     The remainders so that:  (the subPath + delimiter + remainder) = path
        /// </summary>
        public List<string> Remainders
        {
            get { return _remainders; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Segments -->
        private List<string> _segments;
        /// <summary>
        ///      The path separated into a node chain
        /// </summary>
        public List<string> Segments { get { return _segments; } set { _segments = value; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- SubPaths -->
        private List<string> _subPaths;
        /// <summary>
        ///     Pieces of the path listed from smallest to largest
        /// </summary>
        public List<string> SubPaths { get { return _subPaths; } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public PathSlicer() { Init(); }
        public PathSlicer(string path)
        {
            Init();
            if (Regex.IsMatch(path, "/") && Regex.IsMatch(path, @"\\"))
                throw new Exception("bad path delimiters: " + path);
            if (Regex.IsMatch(path, "/"))  Init(path, "/");
            else Init(path, "\\");
        }
        public PathSlicer(string path, string delim) { Init(); Init(path, delim); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Append -->
		/// <summary>
        ///      From Wankuma.IO
		///      連結するパス文字列です。
        ///      I have no idea if this works but it looks cool because it's in Chinese or Japanese
        /// 
        ///      Here's what was at the top of the file:
        ///      PathBuilderはPathを連結してフルパスを構築するためのBuilderです。
        ///      I think this means:
        ///      "PathBuilder Path is linked to the full path to build a Builder. "
		/// </summary>
		/// <param name="path">連結するパスの名前です</param>
		public void Append(string path)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path", "pathにnullは指定できません。"); // "path can not be null."
			if (string.IsNullOrEmpty(path)) throw new ArgumentException("path", @"pathに""は指定できません。");        // "path can not be ""."
			if (_path == null)
                _path = path;
			else _path = System.IO.Path.Combine(_path, path);
		}

        // ----------------------------------------------------------------------------------------
        /// <!-- Count -->
        /// <summary>
        ///      Returns number of segments
        /// </summary>
        public int Count
        {
            get { return _segments.Count; }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Name -->
        /// <summary>
        ///      Returns the last segment or the file name
        /// </summary>
        public string Name
        {
            get
            {
                string lastSegment = _segments[_segments.Count-1];
                if (_delimiter == "." && lastSegment.Length <= 4 && _segments.Count > 1)
                    return _segments[_segments.Count-2] + _delimiter + lastSegment;
                return lastSegment;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FindPath -->
        /// <summary>
        ///      Looks around the file system to try to find the file using the supplied hints
        /// </summary>
        /// <remarks>
        ///      Example usage:
        ///          _.DigFor("PatientProfile.xsd", "xml", "Jon.CWTestHarness"));
        ///        could return:
        ///          "..\\..\\..\\Jon.CWTestHarness\\xml\\PatientProfile.xsd"
        ///        or
        ///          "..\\..\\xml\\PatientProfile.xsd"
        /// </remarks>
        /// <param name="fileName"></param>
        /// <param name="hint1"></param>
        /// <param name="hint2"></param>
        /// <returns></returns>
        public static string FindPath(string fileName, string hint1, string hint2)
        {
            string path;
            string p;
            TryPaths(fileName, out path);
            if (!string.IsNullOrEmpty(path)) return path;
            if (TryPath("..\\..\\..\\..\\..\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\"     + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\"         + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\"             + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\"                 + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath(                         hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\..\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\"     + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\"         + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\"             + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\"                 + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath(                         hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\..\\" + hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\"     + hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\"         + hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\"             + hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\"                 + hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath(                         hint1 + "\\" + hint2 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\..\\" + hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\..\\"     + hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\..\\"         + hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\..\\"             + hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath("..\\"                 + hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;
            if (TryPath(                         hint2 + "\\" + hint1 + "\\" + fileName, out p))  return p;


            //"..\\..\\..\\schemas"
            //"..\\..\\schemas"


            return fileName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes a blank path telescope
        /// </summary>
        private void Init()
        {
            _path = "";
            _subPaths  = new List<string>();
            _remainders = new List<string>();
            _segments  = new List<string>();
            _delimiter = "/";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        ///      Initializes a path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="delim"></param>
        private void Init(string path, string delim)
        {
            _path = path;
            _delimiter = delim;
            if (!DelimiterOK())
                throw new FormatException("wrong delimiter specified");
            InitSubPaths(path);
            InitRemainders();
            InitSegments();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DelimiterOK -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool DelimiterOK()
        {
            if (Regex.IsMatch(_path, "[\\/]") && !Regex.IsMatch(_path, Delim))  return false;
            else return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FileExists -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static bool FileExists(string path)
        {
            bool exists = false;
            try { exists = (new FileInfo(path)).Exists; }
            catch (Exception ex) { if (ex.ToString() != "hi") exists = false; }
            return exists;
        }
        public bool FileExists() { return FileExists(_path); }

        // ----------------------------------------------------------------------------------------
        /// <!-- InitRemainders -->
        /// <summary>
        ///      Must be done after InitSubPaths
        /// </summary>
        /// <param name="path"></param>
        private void InitRemainders()
        {
            try
            {
                foreach (string xp in _subPaths)
                {
                    string str = Regex.Replace(xp, Delim, ".");
                    string remainder = Regex.Replace(_path, "^" + str, "");
                    remainder = Regex.Replace(remainder, "^" + Delim, "");
                    _remainders.Add(remainder);
                }
            }
            catch { }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InitSegments -->
        /// <summary>
        ///      Must be done after InitSubPaths
        /// </summary>
        private void InitSegments()
        {
            foreach (string xp in _subPaths)
                _segments.Add(Regex.Replace(xp, @"^.*" + Delim, ""));
        }

        private string NotDelim
        {
            get
            {
                string pattern;
                if (_delimiter == "\\") pattern = "[^\\\\]";
                else pattern = "[^" + _delimiter + "]";
                return pattern;
            }
        }

        private string Delim
        {
            get
            {
                string pattern;
                if (_delimiter == "\\") pattern = "[\\\\]";
                else pattern = "[" + _delimiter + "]";
                return pattern;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InitSubPaths -->
        /// <summary>
        ///      Must be done before InitRemainders and InitSegments
        /// </summary>
        /// <param name="path"></param>
        private void InitSubPaths(string path)
        {
            for (int i = 0; i < 20 && path.Length > 1; ++i)
            {
                _subPaths.Add(path);
                path = Regex.Replace(path, Delim + NotDelim + "*$", "");
                if (path == _subPaths[_subPaths.Count-1])
                    path = "";
            }
            _subPaths.Reverse();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Root -->
        /// <summary>
        ///      Returns the root of the path or ".."
        /// </summary>
        public string Root
        { get { if (_segments.Count > 0)  return _segments[0]; else return ".."; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _path;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryPath -->
        /// <summary>
        ///      This is built sort of like TryParse. Checks to see if the file exists.
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool TryPath(string inPath, out string path)
        {
            bool found = FileExists(inPath);
            if (found)  path = inPath;
            else path = null;
            return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TryPaths -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool TryPaths(string fileName, out string path)
        {
            path = "";
            string p = "";
            if (string.IsNullOrEmpty(p) && TryPath("..\\..\\..\\..\\..\\" + fileName, out p)) path = p;
            if (string.IsNullOrEmpty(p) && TryPath("..\\..\\..\\..\\"     + fileName, out p)) path = p;
            if (string.IsNullOrEmpty(p) && TryPath("..\\..\\..\\"         + fileName, out p)) path = p;
            if (string.IsNullOrEmpty(p) && TryPath("..\\..\\"             + fileName, out p)) path = p;
            if (string.IsNullOrEmpty(p) && TryPath("..\\"                 + fileName, out p)) path = p;
            if (string.IsNullOrEmpty(p) && TryPath(fileName, out p)) path = p;
            return (!string.IsNullOrEmpty(path));
        }
    }
}
