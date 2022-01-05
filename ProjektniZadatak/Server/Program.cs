using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            #region COMMENT
            /*
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			string address = "net.tcp://localhost:9999/Receiver";
			ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

			///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
			host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

			///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
			host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
			host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

			try
			{
				host.Open();
				Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");
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
			}
            */
            #endregion

            // pokreni visual studio u admin modu
            Debugger.Launch();

            // Ime servisnog korisnika
            string serviceCertCN =
                Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType =
                TcpClientCredentialType.Certificate;

            string hostAddressC2DB = "net.tcp://localhost:8080/DbService";
            string hostAddressC2C = "net.tcp://localhost:8090/ClientCommunication";

            ServiceHost host = new ServiceHost(typeof(DatabaseHandlingService));

            // endpoint za komunikaciju sa bazom podataka
            host.AddServiceEndpoint(typeof(IDatabaseHandling), binding, hostAddressC2DB);
            // endpoint za medjuklijentsku komunikaciju
            host.AddServiceEndpoint(typeof(IClientCommunications), binding, hostAddressC2C);

            // Trust Chain validacija
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode =
                X509RevocationMode.NoCheck;

            // Postavljanje servisnog sertifikata
            host.Credentials.ServiceCertificate.Certificate =
                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serviceCertCN);

            try
            {
                host.Open();

                Console.WriteLine(">> DbHandling Service is running...\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] " + e.Message);
                Console.WriteLine("[StackTrace] " + e.StackTrace);
                Console.ReadLine();
            }
            finally
            {
                host.Close();
            }
        }
    }
}
