using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B64
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // b64 test
            //string testFileName = @"c:\_transits\20210205\crypt.exe.b64";
            //args = new List<string> {testFileName}.ToArray();

            B64(args);
        }

        static void B64(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("input error, file name expected as firste parameter");
                    return;
                }

                string fileName = args[0];

                if (!File.Exists(fileName))
                {
                    Console.WriteLine($"File not found {fileName}");
                    return;
                }

                if (fileName.EndsWith(".b64"))
                {
                    B64Decode(fileName);
                    Console.WriteLine("b64 decode successfull");
                }
                else
                {
                    B64Encode(fileName);
                    Console.WriteLine("b64 encode successfull");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("e:" + e);
            }
        }

        static void B64Encode(string fileName)
        {

            byte[] clearBytes = File.ReadAllBytes(fileName);
            string b64String = Convert.ToBase64String(clearBytes);
            int b64Length = b64String.Length;
            string outFileName = fileName + ".b64";
            File.WriteAllText(outFileName, b64String, Encoding.UTF8);
        }

        static void B64Decode(string fileName)
        {
            string fileSuffix = $".b64";
            int fileNameLenth = fileName.Length;
            int fileSuffixLength = fileSuffix.Length;
            string outFileName = fileName.Substring(0, fileNameLenth - fileSuffixLength );

            string b64String = File.ReadAllText(fileName, Encoding.UTF8);
            byte[] clearBytes = Convert.FromBase64String(b64String);
            File.WriteAllBytes(outFileName, clearBytes);
        }
    }
}
