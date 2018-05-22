using System.Collections.Generic;
using System.Text;

namespace SanitizeFilesForOneDrive
{
    public static class Sanitizer
    {
        static List<char> _invalidChars;

        public static void Initialize()
        {
            //Initialize with default list from Microsoft support page
            _invalidChars = new List<char>() { '\"', '*', ':', '<', '>', '?', '/', '\\', '|' };
        }

        public static string FixFileName(string name)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i< name.Length; i++)
            {
                if(name[i] > 126)
                {
                    sb.Append('-');
                }
                else
                {
                    for (int j = 0; j < _invalidChars.Count; j++)
                    {
                        if(name[i] == _invalidChars[j])
                        {
                            sb.Append('-');
                            break;
                        }
                    }
                    sb.Append(name[i]);
                }
            }
            return sb.ToString();
        }
    }
}
