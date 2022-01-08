using Contracts;
using Contracts.Enums;
using Manager;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Server
{
    public class DatabaseHandlingService : IDatabaseHandling, IClientCommunications
    {
        private static bool barometriDatabaseOpen = true;
        private static bool senzoriTemperatureDatabaseOpen = true;
        private static bool senzoriZvukaDatabaseOpen = true;
        EventLog eventLog = new EventLog("Application");
        
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
        ///

        public bool SendMessage(ClientMessage messageForClients, byte[] sign)
        {
            // Provera digitalnog potpisa.
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name).Split(',')[0];
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);
            if (!DigitalSignature.Verify(messageForClients.ToString(), HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("[Client:SendMessage]>> Sign is invalid!");
                throw new InvalidOperationException("Sign is invalid!");
            }

            // Autorizacija
            UserGroup clientGroup = CertManager.GetMyGroupFromCert(certificate);
            if (!RolesConfig.IsInRole(clientGroup.ToString(), "SendMessage"))
            {
                Console.WriteLine("User doesn't have perrmision to use function SendMessage");
                //Dodat log
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been denied access to SendMessage function", EventLogEntryType.Information, 101, 1);
                throw new InvalidOperationException("Access denied!");
            }
            else
            {
                
               
                Console.WriteLine("Client {0} from group {1} has been authorised to use function SendMessage",clientName,clientGroup);
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been granted access to SendMessage function", EventLogEntryType.Information, 101, 1);
            }

            switch (clientGroup)
            {
                case UserGroup.NULL:
                    throw new InvalidOperationException("Client has no group.");
                case UserGroup.Barometri:
                    /// Ako je klijent poslao STOP
                    /// -> zavrsio je upis u BP i zeli da otvori kanal
                    if (messageForClients.ToString().ToLower().Equals("stop"))
                    {
                        barometriDatabaseOpen = true;
                        Console.WriteLine("[Barometri]: OPEN");
                        return true;
                    }
                    /// Ako je klijent poslao START
                    /// -> zeli da upisuje u BP
                    /// -> ako je kanal OTVOREN, zatvorice ga i dobice povratnu vrednost TRUE, kako bi znao da moze da upisuje
                    /// -> ako je kanal ZATVOREN, dobice povratnu vrednost FALSE, kako bi znao da ne moze da upisuje
                    else if (messageForClients.ToString().ToLower().Equals("start"))
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
                    if (messageForClients.ToString().ToLower().Equals("stop"))
                    {
                        senzoriTemperatureDatabaseOpen = true;
                        Console.WriteLine("[Temperatura]: OPEN");
                        return true;
                    }
                    else if (messageForClients.ToString().ToLower().Equals("start"))
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
                    if (messageForClients.ToString().ToLower().Equals("stop"))
                    {
                        senzoriZvukaDatabaseOpen = true;
                        Console.WriteLine("[Zvuk]: OPEN");
                        return true;
                    }
                    else if (messageForClients.ToString().ToLower().Equals("start"))
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


        /*////////////////////////////////////////////////////*/

        /// <summary>
        /// Upis u DB
        /// </summary>
        /// <param name="message">Formatirana poruka koju upisujem u DB</param>
        /// <param name="userGroup">Grupa kojoj korisnik pripada</param>
        /// <returns>true ako je uspela operacija, false u suprotnom slucaju</returns>
 
        public void WriteToPressureDb(string message, byte[] sign)
        {
            // Provera digitalnog potpisa
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name).Split(',')[0];
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);
            if (!DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("[Client:WriteToDatabase]>> Sign is invalid!");
                throw new InvalidOperationException("Sign is invalid!");
            }

            // Autorizacija
            UserGroup clientGroup = CertManager.GetMyGroupFromCert(certificate);
            if (!RolesConfig.IsInRole(clientGroup.ToString(), "WriteToPressureDb"))
            {
                Console.WriteLine("User doesn't have perrmision to write to the DataBase");
                //Dodat log
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been denied access", EventLogEntryType.Information, 101, 1);
                throw new InvalidOperationException("Access denied!");
            }
            else
            {
                Console.WriteLine("Client {0} from group {1} has been authorised to use function WriteToPressureDb",clientName ,clientGroup);
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been granted access", EventLogEntryType.Information, 101, 1);
            }

            if (clientGroup == UserGroup.NULL)
            {
                throw new InvalidOperationException("User has no group.");
            }

            string[] dataBasePaths =
                { "barometri.txt", "senzoriTemperature.txt", "senzoriZvuka.txt" };

            string activePath;
            if (clientGroup.ToString() == "Barometri")
            {
                activePath = dataBasePaths[1];
            }
            else
            {
                activePath = dataBasePaths[0];
            }


            if (File.Exists(activePath))
            {
                File.AppendAllText(activePath, message + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(activePath, message + Environment.NewLine);
            }

            Console.WriteLine($"[{clientGroup.ToString()}]: {message}");
        }

        public void WriteToSoundDb(string message, byte[] sign)
        {
            // Provera digitalnog potpisa
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name).Split(',')[0];
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);
            if (!DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("[Client:WriteToDatabase]>> Sign is invalid!");
                throw new InvalidOperationException("Sign is invalid!");
            }

            // Autorizacija
            UserGroup clientGroup = CertManager.GetMyGroupFromCert(certificate);
            if (!RolesConfig.IsInRole(clientGroup.ToString(), "WriteToSoundDb"))
            {
                Console.WriteLine("User doesn't have perrmision to write to the DataBase");
                //Dodat log
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been denied access", EventLogEntryType.Information, 101, 1);
                throw new InvalidOperationException("Access denied!");
            }
            else
            {
                Console.WriteLine("Client {0} from group {1} has been authorised to use function WriteToSoundDb", clientName, clientGroup);
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been granted access", EventLogEntryType.Information, 101, 1);
            }

            if (clientGroup == UserGroup.NULL)
            {
                throw new InvalidOperationException("User has no group.");
            }

            string[] dataBasePaths =
                { "barometri.txt", "senzoriTemperature.txt", "senzoriZvuka.txt" };

            string activePath;
            if (clientGroup.ToString() == "SenzoriZvuka")
            {
                activePath = dataBasePaths[2];
            }
            else
            {
                activePath = dataBasePaths[0];
            }


            if (File.Exists(activePath))
            {
                File.AppendAllText(activePath, message + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(activePath, message + Environment.NewLine);
            }

            Console.WriteLine($"[{clientGroup.ToString()}]: {message}");
        }

        public void WriteToTempDb(string message, byte[] sign)
        {
            // Provera digitalnog potpisa
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name).Split(',')[0];
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);
            if (!DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("[Client:WriteToDatabase]>> Sign is invalid!");
                throw new InvalidOperationException("Sign is invalid!");
            }

            // Autorizacija
            UserGroup clientGroup = CertManager.GetMyGroupFromCert(certificate);
            if (!RolesConfig.IsInRole(clientGroup.ToString(), "WriteToTempDb"))
            {
                Console.WriteLine("User doesn't have perrmision to write to the DataBase");
                //Dodat log
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been denied access", EventLogEntryType.Information, 101, 1);
                throw new InvalidOperationException("Access denied!");
            }
            else
            {
                Console.WriteLine("Client {0} from group {1} has been authorised to use function WriteToTempDb", clientName, clientGroup);
                eventLog.Source = "Application";
                eventLog.WriteEntry("Client has been granted access", EventLogEntryType.Information, 101, 1);
            }

            if (clientGroup == UserGroup.NULL)
            {
                throw new InvalidOperationException("User has no group.");
            }

            string[] dataBasePaths =
                { "barometri.txt", "senzoriTemperature.txt", "senzoriZvuka.txt" };

            string activePath;
            if(clientGroup.ToString() == "SenzoriTemperature")
            {
                activePath = dataBasePaths[1];
            }
            else
            {
                activePath = dataBasePaths[0];
            }
            

            if (File.Exists(activePath))
            {
                File.AppendAllText(activePath, message + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(activePath, message + Environment.NewLine);
            }

            Console.WriteLine($"[{clientGroup.ToString()}]: {message}");
        }
    }
}
