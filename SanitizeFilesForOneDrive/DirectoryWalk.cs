using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace SanitizeFilesForOneDrive
{
    class DirectoryWalk
    {
        static StringCollection log = new StringCollection();
        private DirectoryInfo _rootDir = null;
        int _filesDeleted = 0, _filesRenamed = 0;
        int _dirsDeleted = 0, _dirsRenamed = 0;

        public DirectoryWalk(string startDirectory)
        {
            if (startDirectory == string.Empty) _rootDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            WalkDirectoryTree(_rootDir);

            if (log.Count > 0)
            {
                Console.WriteLine("Files with restricted access:");
                foreach (var s in log)
                {
                    Console.WriteLine(s);
                }
            }

            Console.WriteLine("\nFiles Renamed: " + _filesRenamed + "\tDirectories Renamed: " + _dirsRenamed);
            Console.WriteLine("Files Deleted: " + _filesDeleted + "\tDirectories Deleted: " + _dirsDeleted + "\n");
        }

        void WalkDirectoryTree(DirectoryInfo root)
        {
            FileInfo[] files = null;
            List<FileInfo> filesToDelete = new List<FileInfo>();
            List<DirectoryInfo> dirsToDelete = new List<DirectoryInfo>();

            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
                //Just write out the message and continue if privileges are insufficient
                log.Add(e.Message);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                var subDirs = root.GetDirectories("*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < subDirs.Length; i++)
                {
                    WalkDirectoryTree(subDirs[i]);
                    if (Sanitizer.IsObsolete(subDirs[i].Name))
                    {
                        dirsToDelete.Add(subDirs[i]);
                    }
                    else
                    {
                        var newName = Sanitizer.FixFileName(subDirs[i].Name);
                        if (newName != subDirs[i].Name)
                        {
                            try
                            {
                                Directory.Move(Path.Combine(root.FullName, subDirs[i].Name), Path.Combine(root.FullName, newName));
                                _dirsRenamed++;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
                for(int i = 0; i < files.Length; i++)
                {
                    if (Sanitizer.IsObsolete(files[i].Name))
                    {
                        filesToDelete.Add(files[i]);
                    }
                    else
                    {
                        var newName = Sanitizer.FixFileName(files[i].Name);
                        if (newName != files[i].Name)
                        {
                            try
                            {
                                File.Move(Path.Combine(root.FullName, files[i].Name), Path.Combine(root.FullName, newName));
                                _filesRenamed++;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }

                var delFiles = 0; var delDirs = 0;
                Sanitizer.DeleteLists(dirsToDelete, filesToDelete, out delFiles, out delDirs);
                _filesDeleted += delFiles;
                _dirsDeleted += delDirs;
            }
        }
    }
}
