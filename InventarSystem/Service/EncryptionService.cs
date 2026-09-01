using System.Security.Cryptography;
using System.Text;

namespace InventarSystem.Services;

public class EncryptionService
{
    // 32-bytes nøgle (Gem denne sikkert i appsettings.json eller Environment Variables!)
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();

        // Skriv IV først i streamen, så den kan bruges til dekryptering
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText)) return cipherText;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Key;

            int ivLength = aes.BlockSize / 8; // 16 bytes

            // Hvis teksten i databasen er kortere end 16 bytes, er den ikke krypteret endnu
            if (fullCipher.Length < ivLength)
            {
                return cipherText;
            }

            var iv = new byte[ivLength];
            var cipher = new byte[fullCipher.Length - ivLength];

            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            Array.Copy(fullCipher, ivLength, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
        catch
        {
            // Hvis dekrypteringen fejler (f.eks. fordi data var klartekst), returneres den oprindelige tekst
            return cipherText;
        }
    }
}