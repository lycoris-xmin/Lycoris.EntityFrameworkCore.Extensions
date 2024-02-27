using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Cryptography;
using System.Text;

namespace EntityFrameworkCore.Extensions.ColumnConverter
{
    internal class SqlSensitiveConverter : ValueConverter<string, string>
    {
        private const string KEY = "2B9E8A3F";
        private const string IV = "20220101";

        public SqlSensitiveConverter() : base(x => Encrypt(x), x => Decrypt(x))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            byte[] bytes = Encoding.ASCII.GetBytes(KEY);
            byte[] bytes2 = Encoding.ASCII.GetBytes(IV);
            var dES = DES.Create();
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, dES.CreateEncryptor(bytes, bytes2), CryptoStreamMode.Write);
            var streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(value);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            streamWriter.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            byte[] bytes = Encoding.ASCII.GetBytes(KEY);
            byte[] bytes2 = Encoding.ASCII.GetBytes(IV);
            byte[] buffer;
            try
            {
                buffer = Convert.FromBase64String(value);
            }
            catch
            {
                return "";
            }

            var dES = DES.Create();
            var stream = new MemoryStream(buffer);
            var stream2 = new CryptoStream(stream, dES.CreateDecryptor(bytes, bytes2), CryptoStreamMode.Read);
            var streamReader = new StreamReader(stream2);
            return streamReader.ReadToEnd();
        }
    }
}
