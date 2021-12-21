using Contracts;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DatabaseHandlingService : IDatabaseHandling
    {
        /// <summary>
        /// Upisuje u bazu podataka formatiranu string poruku
        /// </summary>
        /// <param name="message">Formatirana string poruka za upis</param>
        /// <returns>true ako je uspesno upisana, false ako nije</returns>
        public bool WriteToDatabase(string message)
        {
            string path = "barometri.txt";

            if(File.Exists(path))
            {
                File.AppendAllText(path, message);
            }
            else
            {
                File.WriteAllText(path, message);
            }

            return true;
        }

        /// <summary>
        /// Upis u DB
        /// </summary>
        /// <param name="message">Formatirana poruka koju upisujem u DB</param>
        /// <param name="userGroup">Grupa kojoj korisnik pripada</param>
        /// <returns>true ako je uspela operacija, false u suprotnom slucaju</returns>
        public bool WriteToDatabase(string message, UserGroup userGroup)
        {
            if (userGroup == UserGroup.NULL)
            {
                throw new InvalidOperationException("User has no group.");
            }

            string[] dataBasePaths =
                { "barometri.txt", "senzoriTemperature.txt", "senzoriZvuka.txt" };

            string activePath;

            switch (userGroup)
            {
                case UserGroup.Barometri:
                    activePath = dataBasePaths[0];
                    break;
                case UserGroup.SenzoriTemperature:
                    activePath = dataBasePaths[1];
                    break;
                case UserGroup.SenzoriZvuka:
                    activePath = dataBasePaths[2];
                    break;
                default:
                    activePath = dataBasePaths[0];
                    break;
            }

            if (File.Exists(activePath))
            {
                File.AppendAllText(activePath, message + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(activePath, message + Environment.NewLine);
            }

            return true;
        }
    }
}
