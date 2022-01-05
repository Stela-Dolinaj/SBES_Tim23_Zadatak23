using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
	[ServiceContract]
	public interface IWCFContract
	{
		[OperationContract]
		void TestCommunication();

		[OperationContract]
		bool SendMessage(string message, byte[] sign);

		[OperationContract]
		void WriteTemp(string message, byte[] sign);

		[OperationContract]
		void WritePressure(string message, byte[] sign);

		[OperationContract]
		void WriteSound(string message, byte[] sign);
	}
}
