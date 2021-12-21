using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class WCFClientToClient : ChannelFactory<IWCFContractClientToClient>, IWCFContractClientToClient, IDisposable
	{
		IWCFContractClientToClient factory;

		public WCFClientToClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate =
				CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

		public void TestCommunication()
		{
			try
			{
				factory.TestCommunication();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public void SendMessage(string message, byte[] sign)
		{
			try
			{
				factory.SendMessage(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
			}
		}

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void StopSending()
        {
			try
			{
				factory.StopSending();
			}
			catch (EndpointNotFoundException)
            {
				Console.WriteLine("Client not yet up.");
            }
			catch (Exception e)
			{
                Console.WriteLine("[StopSending] ERROR = {0}", e.Message);
            }
		}

        public void ContinueSending()
        {
			try
			{
				factory.ContinueSending();
			}
			catch (EndpointNotFoundException)
			{
				Console.WriteLine("Client not yet up.");
			}
			catch (Exception e)
			{
                Console.WriteLine("[ContinueSending] ERROR = {0}", e.Message);
            }
		}
    }
}
