using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using Manager;

namespace SecurityManager
{
    public class RolesConfig
    {
        //Treba promeniti u zavisnosti od lokacije na lokalnoj masini
        static string path = @"D:\RepoV1\SBES_Tim23_Zadatak23\ProjektniZadatak\Manager\RolesConfigFile1.resx";
        public static bool GetPermissions(string rolename, out string[] permissions)
        {  
            permissions = new string[10];
            string permissionString = string.Empty;
            permissionString = RolesConfigFile1.ResourceManager.GetObject(rolename).ToString();
            if (permissionString.Contains(","))
            {
                permissions = permissionString.Split(',');
                return true;
            }
            return false;

        }

        public static bool IsInRole(string groupName, string permission)
        {
            string[] permissions;
           
            GetPermissions(groupName, out permissions);
            foreach (string perm in permissions)
            {
                Console.WriteLine(perm);
                if (perm.Equals(permission))
                {
                    return true;
                }
            }

            return false;
        }

        
    }
}
