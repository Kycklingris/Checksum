using System;
using System.Security.Cryptography;
using System.IO;

namespace Checksum
{
    class Program
    {
        static void Main(string[] args)
        {
            string checksum;
            string path;
            bool interfa = false;
            if (args.Length != 1)
            {
                interfa = true;
                while (true)
                {
                    Console.WriteLine("Input the directory path");
                    var input = Console.ReadLine();
                    var exists = Directory.Exists(input);

                    if (exists)
                    {
                        path = input;
                        break;
                    }
                    Console.WriteLine("Directory does not exist");
                    Console.ReadLine();
                    Console.Clear();
                }
            } else
            {
                path = args[0];
            }
            Console.Clear();
            if(path[path.Length-1] == '/' || path[path.Length-1] == '\\') {
                checksum = path + "checksum.sha256";
            } else {
                checksum = path + "/checksum.sha256";
            }
            var fs = System.IO.File.Create(checksum);
            fs.Close();

            var FilePaths = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
            foreach(string FilePath in FilePaths) {
                var substring = FilePath.Substring(path.Length);
                if(substring[0] == '/' || substring[0] == '\\') {
                    substring = substring.Substring(1);
                }
                var RelativePath = substring;

                var File = new FileInfo(FilePath);
                string ToWrite = "";

                using (SHA256 mySHA256 = SHA256.Create())
                {
                    try
                    {
                        // Create a fileStream for the file.
                        FileStream fileStream = File.Open(FileMode.Open);
                        // Be sure it's positioned to the beginning of the stream.
                        fileStream.Position = 0;
                        // Compute the hash of the fileStream.
                        byte[] hashValue = mySHA256.ComputeHash(fileStream);

                        string HashValue = "";
                        for (int i = 0; i < hashValue.Length; i++)
                        {
                            string asString = FormattableString.Invariant($"{hashValue[i]:X2}");
                            HashValue += asString;
                            //if ((i % 4) == 3) Console.Write(" ");
                        }

                        ToWrite = "#" + RelativePath + "\n" + HashValue + "\n";
                        // Close the file.
                        fileStream.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
                System.IO.File.AppendAllText(checksum, ToWrite);
            }




            Console.Write("le Done");
            if(interfa) {
                Console.ReadLine();
            }
            
        }
    }
}
