using System;
using System.Collections.Specialized;
using System.IO;

namespace SanitizeFilesForOneDrive
{
    class DirectoryWalk
    {
        static StringCollection log = new StringCollection();
        private DirectoryInfo _rootDir = null;

        public DirectoryWalk(string startDirectory)
        {
            if (startDirectory == string.Empty) _rootDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            WalkDirectoryTree(_rootDir);

            Console.WriteLine("Files with restricted access:");
            foreach(string s in log)
            {
                Console.WriteLine(s);
            }
        }

        void WalkDirectoryTree(DirectoryInfo root)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

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
                foreach (FileInfo fi in files)
                {
                    //Add call to modify the file name
                    File.Move(Path.Combine(root.FullName, fi.Name), Path.Combine(root.FullName, Sanitizer.FixFileName(fi.Name)));
                }

                subDirs = root.GetDirectories();
                foreach(DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }
    }
}
