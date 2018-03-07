using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleHelper
{
    public static class HashTest
    {
        public static void run()
        {
            var hash = MD5.Create();

            var phush3 = GetMd5OfFile(hash, @"C:\Users\mazzn\Desktop\HashTest1.txt");
            var phush4 = GetMd5OfFile(hash, @"C:\Users\mazzn\Desktop\HashTest2.txt");

            var phsh = GetMd5Hash(hash, "pneumonoultramicroscopicsilicovolcanoconiosis");
            var phsh2 = SHA512("Anders1234");

            Console.WriteLine(phsh);
            Console.WriteLine(phsh2);
        }


        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public static string GetMd5OfFile(MD5 md5Hash, string Path)
        {

            string input = "";

            using (FileStream stream = new FileStream(Path, FileMode.Open))
            {
                byte[] bytes = new byte[stream.Length];

                var bytess = stream.Read(bytes, 0, bytes.Length);
                input = Encoding.ASCII.GetString(bytes);

            }
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}
