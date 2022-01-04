using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Contracts.Enums;
using System.Diagnostics;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            #region COMMENTED
            /*
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "wcfservice";

            /// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
            /// .NET WindowsIdentity class provides information about Windows user running the given process
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

            /// Define subjectName for certificate used for signing which is not as expected by the service
            string wrongCertCN = "wrong_sign";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Receiver"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (WCFClient proxy = new WCFClient(binding, address))
            {
                /// 1. Communication test
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");
                Console.ReadLine();

                /// 2. Digital Signing test	
				string message = "Message";

                /// Create a signature based on the "signCertCN" using SHA1 hash algorithm
                Console.WriteLine("Created sign");
                X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
                byte[] sign = DigitalSignature.Create(message, HashAlgorithm.SHA1, signCert);

                /// For the same message, create a signature based on the "wrongCertCN" using SHA1 hash algorithm
                Console.WriteLine("Created wrong sign");
                X509Certificate2 wrongSignCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, wrongCertCN);
                byte[] wrongSign = DigitalSignature.Create(message, HashAlgorithm.SHA1, wrongSignCert);

                Console.WriteLine("Sent message with sign");
                proxy.SendMessage(message, sign);

                Console.WriteLine("Sent message with wrong sign");
                proxy.SendMessage(message, wrongSign);

                Console.ReadLine();
            }
        */
            #endregion

            // pokreni visualstudio u admin modu da bi Debugger.Launch() radilo
            //Debugger.Launch();

            // Serverski sertifikat
            string serverCertNC = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // izvlacenje serverskog sertifikata iz TRUSTED PEOPLE-a
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, serverCertNC);

            // endpoint za KLIJENT to KLIJENT komunikaciju
            EndpointAddress C2DBEndpointAddress = new EndpointAddress(
                new Uri("net.tcp://localhost:8080/DbService"),
                new X509CertificateEndpointIdentity(srvCert));
            // endpoint za KLIJENT to DATABASE komunikaciju
            EndpointAddress C2CEndpointAddress = new EndpointAddress(
                new Uri("net.tcp://localhost:8090/ClientCommunication"),
                new X509CertificateEndpointIdentity(srvCert));

            //using (WCFClient2Db proxyC2DB = new WCFClient2Db(binding, C2DBEndpointAddress))
            //{
            //    proxyC2DB.TestCommunication();

            //    Console.WriteLine("Test C2DB done!");
            //}

            //using (WCFClient2Client proxyC2C = new WCFClient2Client(binding, C2CEndpointAddress))
            //{
            //    proxyC2C.TestCommunication();

            //    Console.WriteLine("Test C2C done!");
            //}

            // KLIJENT - DATABASE
            WCFClient2Db proxyC2DB = new WCFClient2Db(binding, C2DBEndpointAddress);

            // KLIJENT - KLIJENT
            WCFClient2Client proxyC2C = new WCFClient2Client(binding, C2CEndpointAddress);

            proxyC2C.TestCommunication();

            Console.WriteLine("Test C2C done!");

            proxyC2DB.TestCommunication();

            Console.WriteLine("Test C2DB done!");

            Console.WriteLine("Tests done!");

            Console.ReadLine();
        }
    }
}
