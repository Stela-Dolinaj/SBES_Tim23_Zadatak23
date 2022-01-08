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
        void WriteToDatabase(string message, byte[] sign, UserGroup userGroup);

        [OperationContract]
        void TestCommunication();

        [OperationContract]
        void ManagePermission(bool isAdd, string rolename, params string[] permissions);

        [OperationContract]
        void ManageRoles(bool isAdd, string rolename);
    }
}
