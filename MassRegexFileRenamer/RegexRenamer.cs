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
        // Scans the given folder, filters candidates with searchPattern, then applies replacePattern on them. Returns a list of renames to be executed.
        public static List<FileRename> Scan(string folder, string searchPattern, string replacePattern, bool searchRecursively, bool renameFiles, bool renameFolders)
        {
            var files = GetSearchSpace(folder, searchRecursively, renameFiles, renameFolders);
            var results = FilterAndApplyRename(folder, searchPattern, replacePattern, files);
            FlagConflicts(results);
            
            return results;
        }

        // Returns a list of files and/or folders in the given folder, recursively or not.
        private static List<string> GetSearchSpace(string folder, bool searchRecursively, bool renameFiles, bool renameFolders)
        {
            var files = new List<string>(); // All the files/folders that will be tested against searchPattern.
            var searchRecursivelySO = searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (renameFolders)
            {
                foreach (var fileName in Directory.EnumerateDirectories(folder, "*", searchRecursivelySO))
                {
                    files.Add(fileName.Substring(folder.Length));
                }
            }
            if (renameFiles)
            {
                foreach (var fileName in Directory.EnumerateFiles(folder, "*", searchRecursivelySO))
                {
                    files.Add(fileName.Substring(folder.Length));
                }
            }
            return files;
        }

        // Filters the list of files and/or folders with searchPattern, transforms the names according to replacePattern, and returns a list of renames that may be executed.
        private static List<FileRename> FilterAndApplyRename(string folder, string searchPattern, string replacePattern, List<string> files)
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

        // Searches through the list looking for duplicated new names, and flags them.
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

        // Represents a file rename that may be executed.
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

            // Does the name remain the same even after applying the renaming pattern?
            public bool IsStaticName()
            {
                return OldName == NewName;
            }

            // Would this rename overwrite some other file?
            public bool Overwrites()
            {
                return File.Exists(BaseFolder + System.IO.Path.DirectorySeparatorChar + NewName);
            }

            // Executes the rename.
            public void Execute()
            {
                if (!IsStaticName()) {
                    var oldPath = BaseFolder + System.IO.Path.DirectorySeparatorChar + OldName;
                    var newPath = BaseFolder + System.IO.Path.DirectorySeparatorChar + NewName;
                    if ((File.GetAttributes(oldPath) & FileAttributes.Directory) == FileAttributes.Directory) // Path is a directory.
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    else    // Path is a file.
                    {
                        File.Move(oldPath, newPath); 
                    }
                }
            }
        }
    }
}