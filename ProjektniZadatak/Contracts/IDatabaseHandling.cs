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
    public interface IDatabaseHandling
    {
        [OperationContract]
        void WriteToTempDb(string message, byte[] sign);

        [OperationContract]
        void WriteToPressureDb(string message, byte[] sign);

        [OperationContract]
        void WriteToSoundDb(string message, byte[] sign);

        [OperationContract]
        void TestCommunication();

    }
}
