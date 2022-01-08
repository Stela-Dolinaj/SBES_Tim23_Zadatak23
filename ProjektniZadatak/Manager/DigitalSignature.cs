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
            // Nadje privatni kljuc sertifikata da bi potpisao poruku
            RSACryptoServiceProvider csp = certificate.PrivateKey as RSACryptoServiceProvider;

            if (csp == null)
            {
                throw new Exception("Valid certificate not found");
            }

            // Prevodjenje poruke u niz bajtova
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] messageByte = encoding.GetBytes(message);

            // Pravljenje hash-ovane poruke uz pomoc SHA1Managed klase
            SHA1Managed sha1 = new SHA1Managed();
            byte[] hash = sha1.ComputeHash(messageByte);

            // RSACryptoServiceProvider klasa pravi digitalno potpisanu poruku uz pomoc hash-ovane poruke
            byte[] signature = csp.SignHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm.ToString()));           

            return signature;
        }


		public static bool Verify(string message, HashAlgorithm hashAlgorithm, byte[] signature, X509Certificate2 certificate)
		{
            // Nadje javni kljuc sertifikata da bi potpisao poruku
            RSACryptoServiceProvider csp = certificate.PublicKey.Key as RSACryptoServiceProvider;

            if (csp == null)
            {
                throw new Exception("Valid certificate not found");
            }

            // Prevodjenje poruke u niz bajtova
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] messageByte = encoding.GetBytes(message);

            // Pravljenje hash-ovane poruke uz pomoc SHA1Managed klase
            SHA1Managed sha1 = new SHA1Managed();
            byte[] hash = sha1.ComputeHash(messageByte);

            // RSACryptoServiceProvider klasa poredi digitalno potpisanu poruku i novokreiranu hash-ovanu poruku (nakon sto je potpise)
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm.ToString()), signature);
        }
	}
}
