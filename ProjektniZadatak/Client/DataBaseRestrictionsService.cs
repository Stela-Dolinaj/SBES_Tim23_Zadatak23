using Contracts;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class DatabaseRestrictionsService : IDatabaseRestrictions
    {
        public static UserGroup ClientGroup = UserGroup.NULL;
        public static bool IAmCurrentlyWriting = false;

        public void NotifyConnectionIsClosed()
        {
            throw new NotImplementedException();
        }

        public void NotifyConnectionIsOpen()
        {
            throw new NotImplementedException();
        }
    }
}
