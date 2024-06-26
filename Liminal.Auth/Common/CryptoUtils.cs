using System.Security.Cryptography;

namespace Liminal.Auth.Common;

public static class CryptoUtils
{
    public static string GenerateRandomString(int length)
    {
        const string chars = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890";

        return new string(
            Enumerable.Range(1, length)
                .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
                .ToArray());
    }
}