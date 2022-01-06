using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Contracts.Enums;
using System.Diagnostics;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // pokreni visualstudio u admin modu da bi Debugger.Launch() radilo
            //Debugger.Launch();

            // Serverski sertifikat
            string serverCertNC = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // izvlacenje serverskog sertifikata iz TRUSTED PEOPLE-a
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, serverCertNC);

            // endpoint za KLIJENT to KLIJENT komunikaciju
            EndpointAddress C2DBEndpointAddress = new EndpointAddress(
                new Uri("net.tcp://localhost:8080/DbService"),
                new X509CertificateEndpointIdentity(srvCert));
            // endpoint za KLIJENT to DATABASE komunikaciju
            EndpointAddress C2CEndpointAddress = new EndpointAddress(
                new Uri("net.tcp://localhost:8090/ClientCommunication"),
                new X509CertificateEndpointIdentity(srvCert));

            // KLIJENT - DATABASE
            WCFClient2Db proxyC2DB = new WCFClient2Db(binding, C2DBEndpointAddress);

            // KLIJENT - KLIJENT
            WCFClient2Client proxyC2C = new WCFClient2Client(binding, C2CEndpointAddress);

            #region SEND MESSAGE
            bool canStart;
            UserGroup myGroup = proxyC2C.myGroup;

            while (true)
            {
                Console.WriteLine(">> Press [enter] to start sending. [q] - quit");
                if (Console.ReadLine().Equals("q"))
                    break;

                canStart = proxyC2C.SendMessage(ClientMessage.start, proxyC2C.myGroup);

                if (canStart)
                {
                    // Pauza da bi se dokazalo da drugi klijenti iste grupe u ovom momentu ne mogu da pristupe bazi podataka
                    Console.WriteLine(">> Ready to send message. Press [enter] to send.");
                    Console.ReadLine();

                    string message = GenerateMeasurement(myGroup);
                    // sign poruku

                    Console.WriteLine(">> Sent message : " + message);
                    proxyC2DB.WriteToDatabase(message, myGroup);

                    proxyC2C.SendMessage(ClientMessage.stop, myGroup);
                }
                else
                {
                    Console.WriteLine(">> Database is currently busy, try again...");
                    Thread.Sleep(1500);
                    Console.Clear();
                }
            }

            Console.WriteLine("\n\n\n\n>> Testing done.. Closing...");
            Thread.Sleep(2000);
            #endregion
        }

        // Kreiranje poruke za upis u BP
        private static string GenerateMeasurement(UserGroup myGroup)
        {
            Random random = new Random();
            string myUsername = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string valueAndUnit = string.Empty;

            switch (myGroup)
            {
                case UserGroup.Barometri:
                    valueAndUnit = $"Value: {random.Next(-100, 100)} [Pa]";
                    break;
                case UserGroup.SenzoriTemperature:
                    valueAndUnit = $"Value: {random.Next(-100, 100)} [C]";
                    break;
                case UserGroup.SenzoriZvuka:
                    valueAndUnit = $"Value: {random.Next(-100, 100)} [Db]";
                    break;
            }

            return $"Time: {DateTime.Now}, Username: {myUsername}, GroupName: {myGroup.ToString()}, {valueAndUnit}";
        }
    }
}
