using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.WebUtilities;

namespace Utils
{
    public static class Compress
    {
        public static string Base64UrlEncode(byte[] s)
        {
            return Base64UrlTextEncoder.Encode(s);
        }
        public static byte[] Base64UrlDecode(string s)
        {
            return Base64UrlTextEncoder.Decode(s);
        }
        public static string Zip (string s)
        {
            //s = Tools.UrlEncode(s);
            //Algorithm Enc = new Algorithm();
            //s = Enc.Encrypt(s);
            //Enc = null;
            MemoryStream m = new MemoryStream();
            GZipStream gz = new GZipStream(m, CompressionMode.Compress);
            StreamWriter sw = new StreamWriter(gz);
            sw.Write(s); sw.Close();
            byte[] b = m.ToArray();

            //string zip = Convert.ToBase64String(b, 0, b.Length);
            string zip = Base64UrlEncode(b);
            //Algorithm Enc = new Algorithm();
            //zip = Enc.Encrypt(zip);
            //Enc = null;
            //zip = Tools.UrlEncode(zip);
            return zip;
        }
        public static string UnZip(string s)
        {
            string r = ""; 
            try
            {
                byte[] r1;
                //s = Tools.UrlDecode(s);
                //Algorithm Enc = new Algorithm();
                //s = Enc.Decrypt(s);
                //Enc = null;
                //r1 = Convert.FromBase64String(s);
                r1 = Base64UrlDecode(s);
                MemoryStream m = new MemoryStream(r1);
                GZipStream gz = new GZipStream(m, CompressionMode.Decompress);
                StreamReader sr = new StreamReader(gz);
                r = sr.ReadToEnd();
                //Algorithm Enc = new Algorithm();
                //r = Enc.Decrypt(r);
                //Enc = null;
                //r = Tools.UrlDecode(r);
                sr.Close();
            }
            catch
            {
                r = "";
            }
            return r;
        }
        public static byte[] ZipAddStream(string[] fFullName) //ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        {
            byte[] fileBytes = null;
            using (MemoryStream zipMS = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMS, ZipArchiveMode.Create, true))
                {
                    foreach (string fileToZip in fFullName)
                    {
                        var f = new FileInfo(fileToZip);
                        //read the file bytes
                        byte[] fileToZipBytes = File.ReadAllBytes(fileToZip);
                        //create the entry - this is the zipped filename
                        //change slashes - now it's VALID
                        ZipArchiveEntry zipFileEntry = zipArchive.CreateEntry(f.Name);
                        //add the file contents
                        using (Stream zipEntryStream = zipFileEntry.Open())
                        using (BinaryWriter zipFileBinary = new BinaryWriter(zipEntryStream))
                        {
                            zipFileBinary.Write(fileToZipBytes);
                        }
                    }
                }
                fileBytes = zipMS.ToArray();
            }
            return fileBytes;
        }
    }
}
