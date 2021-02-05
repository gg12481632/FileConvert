using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            // Split test
            string testFileName = @"c:\_transits\20210205\NDP471-DevPack-ENU.exe.crypt.b64";
            args = new List<string> {testFileName, "3" }.ToArray();

            // Merge test
            //string testFileName = @"c:\_transits\20210205\NDP471-DevPack-ENU.exe.crypt.b64.1.3.split";
            //args = new List<string> {testFileName }.ToArray();
            */

            // Split test
            //string testFileName = @"c:\_transits\20210205\NDP462-DevPack-KB3151934-ENU.exe.crypt.b64";
            //args = new List<string> { testFileName, "3" }.ToArray();

            // Merge test
            //string testFileName = @"c:\_transits\20210205\NDP462-DevPack-KB3151934-ENU.exe.crypt.b64.1.3.split";
            //args = new List<string> {testFileName }.ToArray();

            Split(args);
        }

        static void Split(string[] args) { 
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

                if (fileName.EndsWith(".split"))
                {
                    Merge(fileName);
                    Console.WriteLine("Merge successfull");
                }
                else
                {
                    if (args.Length != 2)
                    {
                        Console.WriteLine("input error!, fileName and number of output files expected");
                        return;
                    }

                    string str_fileCount = args[1];
                    int fileCount = 0;
                    if (!int.TryParse(str_fileCount, out fileCount))
                    {
                        Console.WriteLine($"invalid file count:{str_fileCount}");
                        return;
                    }

                    if (fileCount > 10 || fileCount <= 1)
                    {
                        Console.WriteLine("error, file count must be between 2 and 10");
                        return;
                    }

                    Split(fileName, fileCount);
                    Console.WriteLine("Split successfull");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("e:" + e);
            }
        }

        static void Split(string fileName, int fileCount)
        {
            byte[] byteArray = File.ReadAllBytes(fileName);
            int totalBytes = byteArray.Length;
            int bytesPerFile = totalBytes / fileCount;
            if (bytesPerFile * fileCount < totalBytes)
            {
                bytesPerFile += 1;
            }

            byte[] temp = new byte[bytesPerFile];
            using (FileStream fs = File.OpenRead(fileName))
            {
                for (int i = 0; i < fileCount; i++)
                {
                    string outFileName = $"{fileName}.{i}.{fileCount}.split";
                    int actualRead = fs.Read(temp, 0, bytesPerFile);
                    using (FileStream fsOut = File.OpenWrite(outFileName))
                    {
                        fsOut.Write(temp, 0, actualRead);
                    }
                }
            }
        }

        static void Merge(string fileName)
        {
            string[] fileNameArr = fileName.Split('.');
            int fileNameArrSize = fileNameArr.Length;
            if (fileNameArrSize < 3)
            {
                Console.WriteLine($"invalid fileName:{fileName}");
                return;
            }
            string str_fileCount = fileNameArr[fileNameArrSize - 2];
            int fileCount = 0;
            if (!int.TryParse(str_fileCount, out fileCount))
            {
                Console.WriteLine($"invalid file count:{fileCount}");
                return;
            }
            string fileSuffix = $".{fileCount}.split";
            int fileNameLenth = fileName.Length;
            int fileSuffixLength = fileSuffix.Length;
            int fileCountLen = fileCount.ToString().Length;
            string orgFileName = fileName.Substring(0, fileNameLenth - fileSuffixLength - 1 - fileCountLen);

            using (FileStream fs = File.Create(orgFileName))
            {
                for (int fileNo = 0; fileNo < fileCount; fileNo++)
                {
                    string filePartName = $"{orgFileName}.{fileNo}.{fileCount}.split";
                    byte[] byteArray = File.ReadAllBytes(filePartName);
                    fs.Write(byteArray, 0, byteArray.Length);
                }
            }

        }
    }
}
