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
	public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
	{
		IWCFContract factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
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
        public bool Order(string state)
        {
            if(state == "Start")
            {
                return true;
            }
            else
            {
                return false;
            }
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
	}
}
