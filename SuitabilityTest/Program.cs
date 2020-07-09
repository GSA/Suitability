using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suitability;
using System.Configuration;

namespace SuitabilityTest
{
    /// <summary>
    /// Test class for suitability DLL
    /// </summary>
    class Program
    {
        private static string connectionString = ConfigurationManager.AppSettings["CONNECTIONSTRING"];
        private static string smtpServer = ConfigurationManager.AppSettings["SMTPSERVER"];
        private static string defaultEMail = ConfigurationManager.AppSettings["DEFAULTEMAIL"];
        private static string onboardidng = ConfigurationManager.AppSettings["ONBOARDING"];

        static void Main(string[] args)
        {
            int persId = 3244;
            persId = 30782;
            persId = 392252;

            SendNotification sendNotification = new Suitability.SendNotification(defaultEMail, persId, connectionString, smtpServer, onboardidng);

            //Calling original Suitability methods
            //sendNotification.SendAdjudicationNotificationV1();
            sendNotification.SendSponsorshipNotificationV1();            
            // Calling new Suitability methods
            //sendNotification.SendAdjudicationNotification();
            //sendNotification.SendSponsorshipNotification();
        }
    }
}
