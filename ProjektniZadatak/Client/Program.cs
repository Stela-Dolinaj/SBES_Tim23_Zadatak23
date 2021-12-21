using Contracts;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static public bool CanSend { get; set; } = true;
        static private string myName;
        static private string myGroup;
        static private bool sending = false;
        static private bool threadsRunning = true;
        private static readonly object consoleWriterLock = new object();

        static void Main(string[] args)
        {
            myName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            myGroup = GetMyGroup();
            Console.WriteLine($"Name: {myName}, Group: {myGroup}");

            Task taskClientToServer = Task.Factory.StartNew(() => ClientToServer());
            Task taskClientAsRecieverToClient = Task.Factory.StartNew(() => ClientAsRecieverToClient());
            Task taskClientAsSenderToClient = Task.Factory.StartNew(() => ClientAsSenderToClient());
            Task.WaitAll(taskClientToServer, taskClientAsRecieverToClient, taskClientAsSenderToClient);

            Console.WriteLine("All threads complete. Client shutdown.");
            Console.ReadLine();

        }

        #region ClientToServer

        private static void ClientToServer()
        {
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "wcfservice";

            /// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
            /// .NET WindowsIdentity class provides information about Windows user running the given process
            string signCertCN = myName + "_sign";

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

                while (threadsRunning)
                {
                    // 1. Klijent ceka da drugi thread (ClientAsSenderToClient) zablokira sve ostale klijente iste grupe.
                    // 2. Klijent ceka dok god nismo pritisli nesto na konzoli, inace se ceo kljient gasi.
                    while (!sending && threadsRunning);

                    // Ako je klijent bio ugasen dok je cekao, ovaj thread se gasi.
                    if (!threadsRunning) break;
                    //Console.WriteLine("ClientToServer - Sending. Thread running.");

                    /// 2. Digital Signing test	
                    Measurement measurement = GetRandomMeasurement();
                    string message = measurement.ToString();

                    /// Create a signature based on the "signCertCN" using SHA1 hash algorithm
                    X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
                    byte[] sign = DigitalSignature.Create(message, HashAlgorithm.SHA1, signCert);

                    switch (myGroup.ToLower())
                    {
                        case "senzortoplote":
                            proxy.SendTemp(message, sign);
                            break;

                        case "senzorzvuka":
                            proxy.SendSound(message, sign);
                            break;

                        case "barometar":
                            proxy.SendPressure(message, sign);
                            break;

                        default:
                            throw new Exception("ClientToServer - No such group.");
                    }
                    Console.WriteLine($"ClientToServer - Sent message: {message}");

                    // Kaze thread-u ClientAsSenderToClient da odblokira druge klijente iste grupe.
                    sending = false;
                }

                Console.WriteLine("ClientToServer thread closed.");
            }
        }

        static private Measurement GetRandomMeasurement()
        {
            Random random = new Random();
            switch (myGroup.ToLower())
            {
                case "senzortoplote":
                    return new Measurement(Unit.C, random.Next(-100, 100), DateTime.Now);

                case "senzorzvuka":
                    return new Measurement(Unit.Db, random.Next(0, 50), DateTime.Now);

                case "barometar":
                    return new Measurement(Unit.Pa, random.Next(0, 100000), DateTime.Now);

                default:
                    throw new Exception("ClientToServer - No such group.");
            }
        }

        #endregion

        #region ClientAsRecieverToClient

        private static void ClientAsRecieverToClient()
        {
            // Klijent je ovde server, tako da se klijentovo ime koristi kao ime servera.
            string srvCertCN = myName;

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // Vadimo nas port iz konfig fajla.
            string myPort = ClientPortConfigFile.ResourceManager.GetString(srvCertCN);
            string address = "net.tcp://localhost:" + myPort + "/Receiver";
            Console.WriteLine($"Host Address: {address}");
            ServiceHost host = new ServiceHost(typeof(WCFClientAsService));
            host.AddServiceEndpoint(typeof(IWCFContractClientToClient), binding, address);

            ///ChainTrust validation mode enables validation if both clients have the same Cert Authority.
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = 
                System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            try
            {
                host.Open();
                Console.WriteLine("WCFClientAsService is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
                Console.WriteLine("Client host closed.");
                threadsRunning = false;
            }
        }

        #endregion

        #region ClientAsSenderToClient

        private static void ClientAsSenderToClient()
        {
            // Vadjenje imena i portova klijenata iste grupe iz konfig fajla.
            Dictionary<string, string> otherClients = new Dictionary<string, string>();
            var clients = ClientPortConfigFile.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, true);
            foreach (DictionaryEntry keyValuePair in clients)
            {
                string clientName = keyValuePair.Key.ToString().ToLower();
                if (clientName.Contains(myGroup.ToLower()) && !clientName.Equals(myName))
                {
                    otherClients.Add(keyValuePair.Key.ToString(), keyValuePair.Value.ToString());
                }
            }

            // Kod se vrti u beskonacnoj petlji koja se moze zaustaviti uz pomoc ReadKey na konzoli.
            while (threadsRunning)
            {
                // Da li je klijent blokiran?
                Console.WriteLine("-----------------------------------------\n");
                while (!CanSend)
                {
                    // Jeste blokiran. Nista ne radimo dok nas klijent koji nas je blokirao ne odblokira.
                    Thread.Sleep(1000);
                }

                // Nije blokiran. Krecemo da blokiramo sve druge klijente iste grupe da bi mi slali poruke serveru.
                SendStopMessages(otherClients);

                // Kaze metodi ClientToServer da posalje poruku serveru.
                sending = true;

                // Cekamo dok metoda ClientToServer salje poruku serveru.
                while (sending) ;

                // Odblokiraj ostale klijente.
                SendContinueMessage(otherClients);

                Thread.Sleep(3000);
            }
            Console.WriteLine("ClientAsSenderToClient thread closed.");
        }

        static private void SendStopMessages(Dictionary<string, string> otherClients)
        {
            foreach (KeyValuePair<string, string> client in otherClients)
            {
                string clientName = client.Key;
                string clientPort = client.Value;

                string srvCertCN = clientName;

                NetTcpBinding binding = new NetTcpBinding();
                // Timeout posle 3000 milisekundi. (1 milisekunda = 10.000 tick-ova)
                //binding.SendTimeout = new TimeSpan(3000 * 10000);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
                X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                    StoreLocation.LocalMachine, srvCertCN);
                EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:" + clientPort + "/Receiver"),
                                          new X509CertificateEndpointIdentity(srvCert));

                try
                {
                    using (WCFClientToClient proxy = new WCFClientToClient(binding, address))
                    {
                        proxy.TestCommunication();
                        proxy.StopSending();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Timeout for client {clientName}");
                }
            }
        }

        static private void SendContinueMessage(Dictionary<string, string> otherClients)
        {
            foreach (KeyValuePair<string, string> client in otherClients)
            {
                string clientName = client.Key;
                string clientPort = client.Value;

                string srvCertCN = clientName;

                NetTcpBinding binding = new NetTcpBinding();
                // Timeout posle 3000 milisekundi. (1 milisekunda = 10.000 tick-ova)
                //binding.SendTimeout = new TimeSpan(3000 * 10000);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
                X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                    StoreLocation.LocalMachine, srvCertCN);
                EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:" + clientPort + "/Receiver"),
                                          new X509CertificateEndpointIdentity(srvCert));

                try
                {
                    using (WCFClientToClient proxy = new WCFClientToClient(binding, address))
                    {
                        proxy.TestCommunication();
                        proxy.ContinueSending();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Timeout for client {clientName}");
                }
            }
        }

        #endregion

        #region GetMyGroup

        static private string GetMyGroup()
        {
            string myGroup = string.Empty;

            foreach (IdentityReference group in WindowsIdentity.GetCurrent().Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                string groupName = Formatter.ParseName(sid.Translate(typeof(NTAccount)).ToString()).ToLower();
                if (groupName.Equals("barometar") || groupName.Equals("senzorzvuka") || groupName.Equals("senzortemp"))
                {
                    myGroup = groupName;
                    break;
                }
            }

            return myGroup;
        }

        #endregion
    }
}
