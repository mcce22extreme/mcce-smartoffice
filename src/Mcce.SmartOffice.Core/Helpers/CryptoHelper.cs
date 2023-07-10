using System.Security.Cryptography;

namespace Mcce.SmartOffice.Core.Helpers
{
    public static class CryptoHelper
    {
        public static string Encrypt(string key, string content)
        {
            using var aesAlg = Aes.Create();

            var keyDerivation = new Rfc2898DeriveBytes(key, aesAlg.KeySize / 8, 1000, HashAlgorithmName.SHA256);
            aesAlg.Key = keyDerivation.GetBytes(aesAlg.KeySize / 8);
            aesAlg.GenerateIV();

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(content);
            }

            var encryptedBytes = msEncrypt.ToArray();
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string key, string encryptedText)
        {
            var cipherText = Convert.FromBase64String(encryptedText);

            using var aesAlg = Aes.Create();

            var keyDerivation = new Rfc2898DeriveBytes(key, aesAlg.KeySize / 8, 1000, HashAlgorithmName.SHA256);
            aesAlg.Key = keyDerivation.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = cipherText.Take(aesAlg.BlockSize / 8).ToArray();

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(cipherText.Skip(aesAlg.BlockSize / 8).ToArray());
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
