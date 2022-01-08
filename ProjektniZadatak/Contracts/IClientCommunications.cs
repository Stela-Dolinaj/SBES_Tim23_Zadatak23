using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IClientCommunications
    {
        [OperationContract]
        bool SendMessage(ClientMessage messageForClients, byte[] sign);

        [OperationContract]
        void TestCommunication();
    }
}
