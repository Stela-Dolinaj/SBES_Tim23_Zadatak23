using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class MyAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            WindowsIdentity identity = operationContext.ServiceSecurityContext.WindowsIdentity;
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole("Read");
        }
    }
}
