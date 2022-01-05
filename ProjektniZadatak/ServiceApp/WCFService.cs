using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.ServiceModel;
using System.IO;

namespace ServiceApp
{
	public class WCFService : IWCFContract
	{
        object tempLock = new object();
        object pressureLock = new object();
        object soundLock = new object();

        bool usingTempDatabase = false;
        bool usingPressureDatabase = false;
        bool usingSoundDatabase = false;

        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }

        #region Send Message

        public bool SendMessage(string message, byte[] sign)
        {
            // TODO: Authorization

            // Sanity check.
            if (message != "START" && message != "STOP")
            {
                Console.WriteLine("[Requests] Invalid request");
            }

            // Get client cert.
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);

            // Is sign valid?
            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                string groupName = "Barometer"; // Get groupName from cert instead of using a hardcoded value

                // Sanity check.
                if (groupName != "TempSensor" && 
                    groupName != "Barometer" && 
                    groupName != "SoundSensor")
                {
                    Console.WriteLine("[Requests] Invalid group");
                    return false;
                }

                // All "STOP" messages are accepted.
                if (message == "STOP")
                {
                    Console.WriteLine("[Requests] STOP");
                    SetDatabaseAccess(groupName, true);
                    return true;
                }

                // Reject "START" if database is in use, otherwise accept it.
                if (message == "START" && usingPressureDatabase)
                {
                    Console.WriteLine("[Requests] START rejected");
                    return false;
                }
                else
                {
                    // Accept "START"
                    Console.WriteLine("[Requests] START");
                    SetDatabaseAccess(groupName, false);
                    return true;
                }
            }
            else
            {
                Console.WriteLine("[Requests] Sign is invalid.");
                return false;
            }
        }

        #endregion

        #region Set Database Access

        private void SetDatabaseAccess(string groupName, bool value)
        {
            switch (groupName)
            {
                case "TempSensor":
                    lock (tempLock)
                    {
                        usingTempDatabase = value;
                    }
                    break;
                case "Barometer":
                    lock (pressureLock)
                    {
                        usingPressureDatabase = value;
                    }
                    break;
                case "SoundSensor":
                    lock (soundLock)
                    {
                        usingSoundDatabase = value;
                    }
                    break;
            }
        }

        #endregion

        #region Write To Database

        public void WriteTemp(string message, byte[] sign)
        {
            // TODO: Authorization

            WriteToDatabase(message, sign, "temp.txt");
        }

        public void WritePressure(string message, byte[] sign)
        {
            // TODO: Authorization

            WriteToDatabase(message, sign, "pressure.txt");
        }

        public void WriteSound(string message, byte[] sign)
        {
            // TODO: Authorization

            WriteToDatabase(message, sign, "sound.txt");
        }

        private void WriteToDatabase(string message, byte[] sign, string path)
        {
            // Get client cert.
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName);

            // Is sign valid?
            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine($"[Database] Writen message : {message}");
                File.AppendAllLines(path, new string[] { message });
            }
            else
            {
                Console.WriteLine("[Database] Sign is invalid.");
            }
        }

        #endregion
    }
}
