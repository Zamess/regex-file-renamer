using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MassRegexFileRenamer
{
    public static class RegexRenamer
    {
        public static List<FileRename> Scan(string folder, string searchPattern, string replacePattern, bool searchRecursively, bool renameFiles, bool renameFolders)
        {
            var files = GetSearchSpace(folder, searchRecursively, renameFiles, renameFolders);
            var results = FilterBySearchPattern(folder, searchPattern, replacePattern, files);
            FlagConflicts(results);
            
            return results;
        }

        private static List<string> GetSearchSpace(string folder, bool searchRecursively, bool renameFiles, bool renameFolders)
        {
            var files = new List<string>(); // All the files/folders that will be tested against searchPattern.
            var searchRecursivelySO = searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (renameFolders)
            {
                files.AddRange(Directory.EnumerateDirectories(folder, "*", searchRecursivelySO));
            }
            if (renameFiles)
            {
                files.AddRange(Directory.EnumerateFiles(folder, "*", searchRecursivelySO));
            }
            return files;
        }

        private static List<FileRename> FilterBySearchPattern(string folder, string searchPattern, string replacePattern, List<string> files)
        {
            var results = new List<FileRename>();
            var searchRegex = new Regex(searchPattern);
            foreach (var fileName in files)
            {
                if (searchRegex.IsMatch(fileName))
                {
                    var newName = searchRegex.Replace(fileName, replacePattern);
                    results.Add(new FileRename(folder, fileName, newName));
                }
            }
            return results;
        }

        private static void FlagConflicts(List<FileRename> results)
        {
            int i = 1;
            foreach (var fileRename in results)
            {
                foreach (var potentialDuplicate in results.GetRange(i, results.Count - i))
                {
                    if (fileRename.NewName == potentialDuplicate.NewName)
                    {
                        fileRename.Conflict = true;
                        potentialDuplicate.Conflict = true;
                    }
                }
                i++;
            }
        }

        public class FileRename
        {
            public FileRename(string baseFolder, string oldName, string newName)
            {
                BaseFolder = baseFolder;
                OldName = oldName;
                NewName = newName;
                Conflict = false;               // Will be set later.
            }

            public readonly string BaseFolder;
            public readonly string OldName;
            public readonly string NewName;
            public bool Conflict { get; internal set; }

            public bool IsStaticName()
            {
                return OldName == NewName;
            }

            public bool Overwrites()
            {
                return File.Exists(BaseFolder + System.IO.Path.DirectorySeparatorChar + NewName);
            }

            void Execute()
            {
                if (!IsStaticName()) {
                    File.Move(BaseFolder + System.IO.Path.DirectorySeparatorChar + OldName, BaseFolder + System.IO.Path.DirectorySeparatorChar + NewName);
                }
            }
        }
    }
}
