using System;

namespace SanitizeFilesForOneDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            Sanitizer.Initialize();

            new DirectoryWalk(String.Empty);

            Console.WriteLine("Press any key");
            Console.ReadLine();
        }

    }

}
