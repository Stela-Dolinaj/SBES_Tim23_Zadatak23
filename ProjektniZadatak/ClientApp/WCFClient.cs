using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace ClientApp
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

		public bool SendMessage(string message, byte[] sign)
		{
			try
			{
				return factory.SendMessage(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
				return false;
			}
		}

        public void WriteTemp(string message, byte[] sign)
        {
			try
			{
				factory.WriteTemp(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[WriteTemp] ERROR = {0}", e.Message);
			}
		}

        public void WritePressure(string message, byte[] sign)
        {
			try
			{
				factory.WriteTemp(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[WritePressure] ERROR = {0}", e.Message);
			}
		}

        public void WriteSound(string message, byte[] sign)
        {
			try
			{
				factory.WriteSound(message, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[WriteSound] ERROR = {0}", e.Message);
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
