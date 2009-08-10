// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;

namespace Altinoren.ActiveWriter
{
    using Microsoft.VisualStudio.Modeling;
    
    public static class Helper
    {
        public static Model GetModel(Store store)
        {
            foreach (ModelElement element in store.ElementDirectory.AllElements)
            {
                if (element is Model)
                    return (Model) element;
            }

            return null;
        }

        /// <summary>
        /// The relative path may contain ... which indicates that we want to search all parent folders for the same file.
        /// The files in deeper folders will match before files in more shallow folders.  If there are multiple ... entries,
        /// the system will match using the path following the first ... before matching using the path following the second.
        /// </summary>
        /// <param name="startingDirectory"></param>
        /// <param name="relativePath"></param>
        /// <returns>null if no file can be found.</returns>
        public static string FindFile(string startingDirectory, string relativePath)
        {
            // If there are no dots in the path, we just check for the file and return it.
            if (!relativePath.Contains(Dots))
            {
                string file = Path.Combine(startingDirectory, relativePath);
                return File.Exists(file) ? file : null;
            }

            // If there are dots at the start, search up the parent tree.
            if (relativePath.StartsWith(Dots))
            {
                // If we have the file here, return it.
                string file = FindFile(startingDirectory, ChopDots(relativePath));
                if (file != null)
                    return file;

                // Otherwise, search in the parent folder if we have one.
                if (Path.GetDirectoryName(startingDirectory) == null)
                    return null;
                return FindFile(Path.GetDirectoryName(startingDirectory), relativePath);
            }

            // If there are dots in the middle of the path but not at the start,
            // take the fixed portion and make it a part of the starting directory.
            string[] pieces = relativePath.Split(new[] { Dots }, 2, StringSplitOptions.None);
            return FindFile(Path.Combine(startingDirectory, pieces[0]), Path.Combine("...", pieces[1]));
        }

        private static readonly string Dots = "..." + Path.DirectorySeparatorChar;

        private static string ChopDots(string path)
        {
            return path.Substring(Dots.Length);
        }
    }
}