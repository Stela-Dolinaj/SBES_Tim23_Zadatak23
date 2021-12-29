﻿using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class WCFClientAsService : IWCFContractClientToClient
    {
        public void StopSending()
        {
            Console.WriteLine("Recieved StopSending message!");
            Program.CanSend = false;
        }

        public void ContinueSending()
        {
            Console.WriteLine("Recieved ContinueSending message!");
            Program.CanSend = true;
        }

        public void SendMessage(string message, byte[] sign)
        {
            //TO DO
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string signCertCN = $"{clientName}_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, signCertCN);

            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("Sign is valid.");
            }
            else
            {
                Console.WriteLine("Sign is invalid");
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }
    }
}