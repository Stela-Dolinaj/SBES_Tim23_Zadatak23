using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Manager
{
    public enum HashAlgorithm { SHA1, SHA256}
	public class DigitalSignature
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"> a message/text to be digitally signed </param>
		/// <param name="hashAlgorithm"> an arbitrary hash algorithm </param>
		/// <param name="certificate"> certificate of a user who creates a signature </param>
		/// <returns> byte array representing a digital signature for the given message </returns>
		public static byte[] Create(string message, HashAlgorithm hashAlgorithm, X509Certificate2 certificate)
		{
            //TO DO

            /// Looks for the certificate's private key to sign a message
            RSACryptoServiceProvider csp = certificate.PrivateKey as RSACryptoServiceProvider;

            if (csp == null)
            {
                throw new Exception("Valid certificate not found");
            }

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] messageByte = encoding.GetBytes(message);

            /// Use RSACryptoServiceProvider support to create a signature using a previously created hash value
            byte[] hash = null;
            if (HashAlgorithm.SHA1 == hashAlgorithm)
            {
                SHA1Managed sha1 = new SHA1Managed();
                hash = sha1.ComputeHash(messageByte);
            }
            else if (hashAlgorithm == HashAlgorithm.SHA256)
            {
                SHA256Managed sha256 = new SHA256Managed();
                hash = sha256.ComputeHash(messageByte);
            }


            byte[] signature = csp.SignHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm.ToString()));           

            return signature;
        }


		public static bool Verify(string message, HashAlgorithm hashAlgorithm, byte[] signature, X509Certificate2 certificate)
		{
            //TO DO

            /// Looks for the certificate's public key to sign a message
            RSACryptoServiceProvider csp = certificate.PublicKey.Key as RSACryptoServiceProvider;

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] messageByte = encoding.GetBytes(message);

            byte[] hash = null;
            if (HashAlgorithm.SHA1 == hashAlgorithm)
            {
                SHA1Managed sha1 = new SHA1Managed();
                hash = sha1.ComputeHash(messageByte);
            }
            else if (hashAlgorithm == HashAlgorithm.SHA256)
            {
                SHA256Managed sha256 = new SHA256Managed();
                hash = sha256.ComputeHash(messageByte);
            }

            /// Use RSACryptoServiceProvider support to compare two - hash value from signature and newly created hash value
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm.ToString()), signature);
        }
	}
}
