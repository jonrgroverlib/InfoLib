using InfoLib.Strings;                // for 
using System;                         // for 
using System.Collections.Generic;     // for 
using System.IO;                      // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.HardData
{
    // --------------------------------------------------------------------------------------------
    /// <!-- FileGroup -->
    /// <summary>
    /// 
    /// </summary>
    public class FileGroup
    {
        // ------------------------------------------------------------------------------
        //  Members
        // ------------------------------------------------------------------------------
        public string       RootPath         { get; set; }
        public List<string> RelativeFilePath { get; set; }
        public List<string> Extension        { get; set; }
        public int          Depth            { get; set; }


        // ------------------------------------------------------------------------------
        //  Constructors
        // ------------------------------------------------------------------------------
        public FileGroup()
        {
        }
        public FileGroup(string rootPath, string extension1)
        {
            RootPath  = rootPath;
            Extension = new List<string>(); Extension.Add(extension1);
            RelativeFilePath = AddFiles(rootPath);
        }
        public FileGroup(string rootPath, string path1, string extension1)
        {
            RootPath  = rootPath;
            Extension = new List<string>(); Extension.Add(extension1);
            RelativeFilePath = new List<string>(); RelativeFilePath.Add(path1);
        }
        public FileGroup(string rootPath, List<string> extensionList)
        {
            RootPath  = rootPath;
            Extension = new List<string>();
            foreach (string extension in extensionList)
                Extension.Add(extension);
            RelativeFilePath = AddFiles(rootPath);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AddFiles -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="path"></param>
        /// <param name="list"></param>
        /// <param name="depth"></param>
        private void AddFiles(string rootPath, string path, ref List<string> list, int depth)
        {
            Depth = Math.Max(Depth, depth);


            // --------------------------------------------------------------------------
            //  Check local files in path
            // --------------------------------------------------------------------------
            string[] filePath = Directory.GetFiles(path);
            for (int i = 0; i < filePath.Length; ++i)
            {
                if (Regex.IsMatch(filePath[i], Extension[0] + "$", RegexOptions.IgnoreCase))
                {
                    string relativePath = __.RelativePath(rootPath, filePath[i]);
                    list.Add(relativePath);
                }
            }


            // --------------------------------------------------------------------------
            //  Recursion
            // --------------------------------------------------------------------------
            string[] dirPath = Directory.GetDirectories(path);
            for (int i = 0; i < dirPath.Length; ++i)
            {
                AddFiles(rootPath, dirPath[i], ref list, depth+1);
            }
        }
        private List<string> AddFiles(string rootPath) { List<string> list = new List<string>(); AddFiles(rootPath, rootPath, ref list, 0);  return list; }

    }
}
