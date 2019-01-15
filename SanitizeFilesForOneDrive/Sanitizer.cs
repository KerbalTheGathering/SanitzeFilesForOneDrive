using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SanitizeFilesForOneDrive
{
    public static class Sanitizer
    {
        static List<char> _invalidChars;

        public static void Initialize()
        {
            //Initialize with default list from Microsoft support page including 2013 Sharepoint restirctions
            _invalidChars = new List<char>() { '\"', '*', ':', '<', '>', '?', '/', '\\', '|', '~', '#', '%', '&', '{', '}' };
        }

        public static string FixFileName(string name)
        {
            bool shouldFix = false;
            foreach(var c in name)
            {
                foreach (var t in _invalidChars)
                {
                    if (c != t) continue;
                    shouldFix = true;
                    break;
                }

                if (c > 126)
                {
                    shouldFix = true;
                    break;
                }
            }
            return shouldFix ? Fix(name) : name;
        }

        public static bool IsObsolete(string name)
        {
            return name.Length >= 2 && name[0] == '.' && name[1] == '_';
        }

        public static void DeleteLists(List<DirectoryInfo> dirs, List<FileInfo> files, out int filesDeleted, out int dirsDeleted)
        {
            filesDeleted = 0;
            dirsDeleted = 0;
            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    File.Delete(files[i].FullName);
                    filesDeleted++;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            for (int i = 0; i < dirs.Count; i++)
            {
                try
                {
                    Directory.Delete(dirs[i].FullName);
                    dirsDeleted++;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static string Fix(string name)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] > 126)
                {
                    sb.Append('-');
                }
                else
                {
                    sb.Append(ShouldReplaceInvalid(name[i]) ? '-' : name[i]);
                }
            }
            Console.WriteLine("Changed: " + name + " to " + sb.ToString());
            return sb.ToString();
        }

        private static bool ShouldReplaceInvalid(char c)
        {
            for (int j = 0; j < _invalidChars.Count; j++)
            {
                if (c == _invalidChars[j])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
