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

        public static void AddPermissions(string rolename, string[] permissions)
        {
            string permissionString = string.Empty;
            permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename);

            if (permissionString != null) // dodaju se nove permisije
            {
                var reader = new ResXResourceReader(path);
                var node = reader.GetEnumerator();
                var writer = new ResXResourceWriter(path);
                while (node.MoveNext())
                {
                    if (node.Key.ToString().Equals(rolename))
                    {
                        string value = node.Value.ToString();
                        foreach (string prms in permissions)
                        {
                            value += "," + prms;
                        }
                        writer.AddResource(node.Key.ToString(), value);
                    }
                    else
                    {
                        writer.AddResource(node.Key.ToString(), node.Value.ToString());
                    }
                }
                writer.Generate();
                writer.Close();
            }          
            
        }

        public static void RemovePermissions(string rolename, string[] permissions)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);
            while (node.MoveNext())
            {
                if (!node.Key.ToString().Equals(rolename))
                {
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
                }
                else
                {
                    List<string> currentPermisions = (node.Value.ToString().Split(',')).ToList();
                                    
                    foreach (string permForDelete in permissions)
                    {
                        for (int i = 0; i < currentPermisions.Count(); i++)
                        {
                            if (currentPermisions[i].Equals(permForDelete))
                            {
                                currentPermisions.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    string value = currentPermisions[0];
                    for (int i = 1; i < currentPermisions.Count(); i++)
                    {
                        value += "," + currentPermisions[i];
                    }
                    writer.AddResource(node.Key.ToString(), value);

                }
            }

            writer.Generate();
            writer.Close();
        }

        public static void RemoveRole(string rolename)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);
            while (node.MoveNext())
            {
                if(!node.Key.ToString().Equals(rolename))
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
            }
            
            writer.Generate();
            writer.Close();
        }

        public static void AddRole(string rolename)
        {
            var reader = new ResXResourceReader(path);
            var node = reader.GetEnumerator();
            var writer = new ResXResourceWriter(path);
            while (node.MoveNext())
            {
                writer.AddResource(node.Key.ToString(), node.Value.ToString());
            }
            var newNode = new ResXDataNode(rolename, "");
            writer.AddResource(newNode);
            writer.Generate();
            writer.Close();
        }
        
    }
}
