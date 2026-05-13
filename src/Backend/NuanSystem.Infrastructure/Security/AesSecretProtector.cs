using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using NuanSystem.Application.Abstractions.Security;

namespace NuanSystem.Infrastructure.Security;

public sealed class AesSecretProtector(IConfiguration configuration) : ISecretProtector
{
    private const int IvSize = 16;

    public string Protect(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        using var aes = CreateAes();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var payload = new byte[IvSize + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, payload, 0, IvSize);
        Buffer.BlockCopy(encryptedBytes, 0, payload, IvSize, encryptedBytes.Length);

        return Convert.ToBase64String(payload);
    }

    public string Unprotect(string protectedText)
    {
        if (string.IsNullOrEmpty(protectedText))
        {
            return string.Empty;
        }

        var payload = Convert.FromBase64String(protectedText);
        if (payload.Length <= IvSize)
        {
            throw new InvalidOperationException("El valor cifrado no tiene un formato valido.");
        }

        var iv = payload[..IvSize];
        var encryptedBytes = payload[IvSize..];

        using var aes = CreateAes();
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private Aes CreateAes()
    {
        var secret = configuration["Security:EncryptionKey"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("Security:EncryptionKey no esta configurado.");
        }

        var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        return aes;
    }
}
