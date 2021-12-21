using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class RolesConfig
    {
        public static bool GetPermissions(string rolename, out string[] permissions)
        {
            // Izvuci sve permisije na osnovu prosledjene grupe
            permissions = new string[10];
            string permissionString = RolesConfigFile.ResourceManager.GetObject(rolename) as string;
            if (permissionString != null)
            {
                permissions = permissionString.Split(',');
                return true;
            }

            return false;
        }
    }
}
