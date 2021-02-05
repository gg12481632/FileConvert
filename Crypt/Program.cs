using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Crypt
{

    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                //string password = "12dh1020";
                //if(false)
                //{
                //    string fileName = @"C:\githubroot\FileConvert\Crypt\bin\Debug\testdata.txt";
                //    DoIt(Action.Encoding,fileName,password);
                //}
                //else
                //{
                //    string fileName = @"C:\githubroot\FileConvert\Crypt\bin\Debug\testdata.txt.crypt.b64";
                //    DoIt(Action.Decoding,fileName,password);
                //}
                Crypt(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("e:" + e);
            }
        }

        static void Crypt(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("input error!");
                Console.WriteLine("file and password is missing");
                return;
            }

            string fileName = args[0];

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File not found {fileName}");
                return;
            }

            string pass;
            pass = args[1];
            Console.Clear();

            if (fileName.EndsWith(".crypt.b64"))
            {
                Decrypt(fileName,pass);
                Console.WriteLine("decryption sucessfull");
            }
            else
            {
                Encrypt(fileName,pass);
                Console.WriteLine("encryption sucessfull");
            }
        }


        private static byte[] AESEncryptBytes(byte[] clearBytes, byte[] passBytes, byte[] saltBytes)
        {
            byte[] encryptedBytes = null;

            // create a key from the password and salt, use 32K iterations – see note
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(),
          CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static byte[] AESDecryptBytes(byte[] cryptBytes, byte[] passBytes, byte[] saltBytes)
        {
            byte[] clearBytes = null;

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }

        static void Encrypt(string fileName, string pass)
        {
            string outFileName = fileName + ".crypt.b64";
            if (File.Exists(outFileName))
            {
                Console.WriteLine($"result file name already exists: {outFileName}");
                return;
            }
            byte[] clearBytes = File.ReadAllBytes(fileName);
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] saltBytes = Encoding.UTF8.GetBytes("43057d0a-0b7f-4cbb-bf06-b913ace65937");
            byte[] encryptedBytes = AESEncryptBytes(clearBytes, passBytes, saltBytes);
            string encryptedString = Convert.ToBase64String(encryptedBytes);//,Base64FormattingOptions.InsertLineBreaks);
            File.WriteAllText(outFileName, encryptedString, Encoding.UTF8);
        }

        static void Decrypt(string fileName, string pass)
        {
            string outFileName = fileName.Substring(0, fileName.Length - ".crypt.b64".Length);
            if (File.Exists(outFileName))
            {
                Console.WriteLine($"result file name already exists: {outFileName}");
                return;
            }
            string encryptedString = File.ReadAllText(fileName, Encoding.UTF8);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] saltBytes = Encoding.UTF8.GetBytes("43057d0a-0b7f-4cbb-bf06-b913ace65937");
            byte[] clearBytes = AESDecryptBytes(encryptedBytes, passBytes, saltBytes);
            File.WriteAllBytes(outFileName, clearBytes);
        }
    }
}
