using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utilities
{
    public class SaltHelper
    {

        // Method to generate salt (you can implement your own way)
        public static byte[] GenerateSalt()
        {
            // Assuming a salt size of 16 bytes
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
