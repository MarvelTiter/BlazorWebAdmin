using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Utils;

public static class HashHelper
{
    public static string ToHash(this string? value, string algorithm = "SHA256")
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        byte[] inputBytes = Encoding.UTF8.GetBytes(value);
        var bytes = algorithm switch
        {
            "MD5" => MD5.HashData(inputBytes),
            "SHA1" => SHA1.HashData(inputBytes),
            "SHA256" => SHA256.HashData(inputBytes),
            "SHA384" => SHA384.HashData(inputBytes),
            "SHA512" => SHA512.HashData(inputBytes),
            _ => throw new ArgumentException("Unsupported hash algorithm", nameof(algorithm))
        };
        return Convert.ToHexString(bytes);
    }
}
