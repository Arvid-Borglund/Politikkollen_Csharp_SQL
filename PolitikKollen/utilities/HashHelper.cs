using System;
using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static byte[] GenerateHash(string input, byte[] salt)
    {
        // Convert the input string to a byte array
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Combine the input bytes and salt bytes into a single array
        byte[] combinedBytes = new byte[inputBytes.Length + salt.Length];
        Array.Copy(inputBytes, 0, combinedBytes, 0, inputBytes.Length);
        Array.Copy(salt, 0, combinedBytes, inputBytes.Length, salt.Length);

        // Create the hash object
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Compute the hash
            return sha256Hash.ComputeHash(combinedBytes);
        }
    }
}
