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

            using (ServiceHost serviceHost = 
                new ServiceHost(typeof(DatabaseRestrictionsService)))
            {
                serviceHost.Open();
                Console.WriteLine("Client Service Opened @"
                    + DateTime.Now.ToLongTimeString());

                ChannelFactory<IDatabaseHandling> channelServiceCommunication =
                    new ChannelFactory<IDatabaseHandling>("ServiceEndpoint");

                IDatabaseHandling proxyService =
                    channelServiceCommunication.CreateChannel();

                SetClientGroup();

                try
                {
                    proxyService.WriteToDatabase("Test1", DatabaseRestrictionsService.ClientGroup);

                    proxyService.WriteToDatabase("Test2", DatabaseRestrictionsService.ClientGroup);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Error >> " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void SetClientGroup()
        {
            PrincipalSearchResult<Principal> groups =
                UserPrincipal.Current.GetGroups();
            List<string> groupNames = groups.Select(x => x.SamAccountName).ToList();

            foreach (var name in groupNames)
            {
                switch (name)
                {
                    case "Barometri":
                        DatabaseRestrictionsService.ClientGroup = 
                            UserGroup.Barometri;
                        break;
                    case "SenzoriTemperature":
                        DatabaseRestrictionsService.ClientGroup = 
                            UserGroup.SenzoriTemperature;
                        break;
                    case "SenzoriZvuka":
                        DatabaseRestrictionsService.ClientGroup = 
                            UserGroup.SenzoriZvuka;
                        break;
                    default:
                        DatabaseRestrictionsService.ClientGroup = 
                            UserGroup.NULL;
                        break;
                }
            }
        }
    }
}
