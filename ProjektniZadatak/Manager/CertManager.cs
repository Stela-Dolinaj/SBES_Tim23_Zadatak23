using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using Contracts.Enums;
using System.Security.Principal;

namespace Manager
{
	public class CertManager
	{
		/// <summary>
		/// Get a certificate with the specified subject name from the predefined certificate storage
		/// Only valid certificates should be considered
		/// </summary>
		/// <param name="storeName"></param>
		/// <param name="storeLocation"></param>
		/// <param name="subjectName"></param>
		/// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

			/// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"

			foreach (X509Certificate2 c in certCollection)
			{
                //if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
                //{
                //    return c;
                //}
                if (c.SubjectName.Name.Contains(string.Format("CN={0}", subjectName)))
				{
					return c;
				}
			}

			return null;
		}

        // TO DO: IZVUCI GRUPU IZ SERTIFIKATA
        public static UserGroup GetMyGroupFromCert(X509Certificate2 cert)
        {
            if (cert.SubjectName.Name.Contains(string.Format("OU=Barometar")))
            {
                return UserGroup.Barometri;
            }
            else if (cert.SubjectName.Name.Contains(string.Format("OU=SenzorTemp")))
            {
                return UserGroup.SenzoriTemperature;
            }
            else if (cert.SubjectName.Name.Contains(string.Format("OU=SenzorZvuka")))
            {
                return UserGroup.SenzoriZvuka;
            }
            else
            {
                return UserGroup.NULL;
            }
        }

        public static UserGroup GetMyGroup(WindowsIdentity myIdentity)
        {
            string myGroup = string.Empty;

            foreach (IdentityReference group in myIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                string groupName = Formatter.ParseName(sid.Translate(typeof(NTAccount)).ToString()).ToLower();
                if (groupName.Equals("barometar") || groupName.Equals("senzorzvuka") || groupName.Equals("senzortemp"))
                {
                    myGroup = groupName;
                    break;
                }
            }

            switch (myGroup.ToLower())
            {
                case "barometar":
                    return UserGroup.Barometri;
                case "senzortemp":
                    return UserGroup.SenzoriTemperature;
                case "senzorzvuka":
                    return UserGroup.SenzoriZvuka;
                default:
                    return UserGroup.NULL;
            }

        }


        /// <summary>
        /// Get a certificate from file.		
        /// </summary>
        /// <param name="fileName"> .cer file name </param>
        /// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
        public static X509Certificate2 GetCertificateFromFile(string fileName)
		{
			X509Certificate2 certificate = null;
			

			return certificate;
		}

        /// <summary>
        /// Get a certificate from file.
        /// </summary>
        /// <param name="fileName">.pfx file name</param>
        /// <param name="pwd"> password for .pfx file</param>
        /// <returns>The requested certificate. If no valid certificate is found, returns null.</returns>
		public static X509Certificate2 GetCertificateFromFile(string fileName, SecureString pwd)
        {
            X509Certificate2 certificate = null;


            return certificate;
        }
    }
}
