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
    public class DatabaseHandlingService : IDatabaseHandling, IClientCommunications
    {
        private bool barometriDatabaseOpen;
        private bool senzoriTemperatureDatabaseOpen;
        private bool senzoriZvukaDatabaseOpen;

        public DatabaseHandlingService()
        {
            barometriDatabaseOpen = true;
            senzoriTemperatureDatabaseOpen = true;
            senzoriZvukaDatabaseOpen = true;
        }
        /// <summary>
        /// Stize poruka na servis.
        /// Analiziraj poruku:
        /// -> start = klijent zeli da pise u DB
        ///     Ako kanal nije zauzet, zauzmi ga i vrati true.
        ///     Ako kanal jeste zauzet, vrati false.
        /// -> stop = klijent je zavrsio pisanje u DB
        ///     Zavrsi zauzimanje kanala i vrati true.
        /// </summary>
        /// <param name="messageForClients"></param>
        public bool SendMessage(string messageForClients, UserGroup clientGroup)
        {
            switch (clientGroup)
            {
                case UserGroup.NULL:
                    throw new InvalidOperationException("Client has no group.");
                case UserGroup.Barometri:
                    if (messageForClients.ToLower().Equals("stop"))
                    {
                        barometriDatabaseOpen = true;
                        Console.WriteLine("[Barometri]: OPEN");
                        return true;
                    }
                    else if (messageForClients.ToLower().Equals("start"))
                    {
                        if (barometriDatabaseOpen)
                        {
                            barometriDatabaseOpen = false;
                            Console.WriteLine("[Barometri]: CLOSED");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("[Barometri]: ACCESS DENIED, DB ALREADY IN USE!");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case UserGroup.SenzoriTemperature:
                    if (messageForClients.ToLower().Equals("stop"))
                    {
                        senzoriTemperatureDatabaseOpen = true;
                        Console.WriteLine("[Temperatura]: OPEN");
                        return true;
                    }
                    else if (messageForClients.ToLower().Equals("start"))
                    {
                        if (senzoriTemperatureDatabaseOpen)
                        {
                            senzoriTemperatureDatabaseOpen = false;
                            Console.WriteLine("[Temperatura]: CLOSED");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("[Temperatura]: ACCESS DENIED, DB ALREADY IN USE!");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case UserGroup.SenzoriZvuka:
                    if (messageForClients.ToLower().Equals("stop"))
                    {
                        senzoriZvukaDatabaseOpen = true;
                        Console.WriteLine("[Zvuk]: OPEN");
                        return true;
                    }
                    else if (messageForClients.ToLower().Equals("start"))
                    {
                        if (senzoriZvukaDatabaseOpen)
                        {
                            senzoriZvukaDatabaseOpen = false;
                            Console.WriteLine("[Zvuk]: CLOSED");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("[Zvuk]: ACCESS DENIED, DB ALREADY IN USE!");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("[Client:TestCommunication]>> Test success!");
        }


        /// <summary>
        /// Upis u DB
        /// </summary>
        /// <param name="message">Formatirana poruka koju upisujem u DB</param>
        /// <param name="userGroup">Grupa kojoj korisnik pripada</param>
        /// <returns>true ako je uspela operacija, false u suprotnom slucaju</returns>
        public void WriteToDatabase(string message, UserGroup userGroup)
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
        }
    }
}
