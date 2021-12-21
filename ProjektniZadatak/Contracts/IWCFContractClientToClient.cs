using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract]
	public interface IWCFContractClientToClient
	{
		[OperationContract]
		void TestCommunication();

		[OperationContract]
		void SendMessage(string message, byte[] sign);

		[OperationContract]
		void StopSending();

		[OperationContract]
		void ContinueSending();
	}
}
