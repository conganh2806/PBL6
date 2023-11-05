using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNovel.API.Commons
{
    public class Security
    {
        public static string Sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public static string DecodeSha256(string sha256Hash)
        {
            if (string.IsNullOrWhiteSpace(sha256Hash) || sha256Hash.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid SHA-256 hash");
            }

            int hashLength = sha256Hash.Length / 2;
            byte[] hashBytes = new byte[hashLength];

            for (int i = 0; i < hashLength; i++)
            {
                string byteValue = sha256Hash.Substring(i * 2, 2);
                hashBytes[i] = byte.Parse(byteValue, NumberStyles.HexNumber);
            }

            return Encoding.UTF8.GetString(hashBytes);
        }

    }
}