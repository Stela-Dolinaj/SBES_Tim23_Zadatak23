﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract]
	public interface IWCFContract
	{
		[OperationContract]
		void TestCommunication();

		[OperationContract]
		void SendMessage(string message, byte[] sign);

		[OperationContract]
		void SendPressure(string message, byte[] sign);

		[OperationContract]
		void SendTemp(string message, byte[] sign);

		[OperationContract]
		void SendSound(string message, byte[] sign);
	}
}
