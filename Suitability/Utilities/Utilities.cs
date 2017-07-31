﻿using System.Security.Cryptography;
using System.Text;

namespace Suitability.Utilities
{
    class Utilities
    {
        /// <summary>
        /// Returns hashed ssn
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public byte[] HashSSN(string ssn)
        {
            byte[] hashedFullSSN = null;

            SHA256 shaM = new SHA256Managed();

            ssn = ssn.Replace("-", string.Empty);

            //Using UTF8 because this only contains ASCII text
            if (ssn.Length == 9)
                hashedFullSSN = shaM.ComputeHash(Encoding.UTF8.GetBytes(ssn));

            shaM.Dispose();

            return hashedFullSSN;
        }

    }
}
