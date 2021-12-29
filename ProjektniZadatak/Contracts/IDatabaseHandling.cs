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
        void WriteToDatabase(string message, UserGroup userGroup);

        [OperationContract]
        void TestCommunication();
    }
}
