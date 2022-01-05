using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;
using System.Security.Principal;
using System.Threading;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "wcfservice";

            /// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
            /// .NET WindowsIdentity class provides information about Windows user running the given process
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			/// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, srvCertCN);
			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Receiver"),
									  new X509CertificateEndpointIdentity(srvCert));
            
            using (WCFClient proxy = new WCFClient(binding, address))
			{
				// Communication test
				proxy.TestCommunication();
				Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");
				Console.ReadLine();

                // Signing messages
                X509Certificate2 signCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertCN);
                byte[] signStart = DigitalSignature.Create("START", HashAlgorithm.SHA1, signCert);
                byte[] signStop = DigitalSignature.Create("STOP", HashAlgorithm.SHA1, signCert);
                byte[] signMessage;
                bool canStart;

                // TODO: Get groupName from cert instead of using a hardcoded value.
                string groupName = "Barometer";

                while (true)
                {
                    Console.WriteLine("Sent START");
                    canStart = proxy.SendMessage("START", signStart);

                    if (canStart)
                    {
                        string message = GenerateMeasurement(groupName);
                        signMessage = DigitalSignature.Create(message, HashAlgorithm.SHA1, signCert);

                        Console.WriteLine("Sent message: " + message);
                        proxy.SendMessage(message, signMessage);

                        proxy.SendMessage("STOP", signStop);
                    }
                    else
                    {
                        Console.WriteLine("START rejected");
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private static string GenerateMeasurement(string groupName)
        {
            Random random = new Random();
            string myUsername = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string valueAndUnit = string.Empty;

            switch (groupName)
            {
                case "TempSensor":
                   valueAndUnit = $"Value: {random.Next(-100, 100)} [C]";
                    break;
                case "Barometer":
                    valueAndUnit = $"Value: {random.Next(-100, 100)} [Pa]";
                    break;
                case "SoundSensor":
                    valueAndUnit = $"Value: {random.Next(-100, 100)} [Db]";
                    break;
            }

            return $"Time: {DateTime.Now}, Username: {myUsername}, GroupName: {groupName}, {valueAndUnit}";
        }
	}
}
