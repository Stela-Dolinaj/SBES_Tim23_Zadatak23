using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class DataBaseRestrictionsService : IDatabaseRestrictions
    {
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
