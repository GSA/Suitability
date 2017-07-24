using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suitability;
using System.Configuration;

namespace SuitabilityTest
{
    class Program
    {
        private static string connectionString = ConfigurationManager.AppSettings["CONNECTIONSTRING"];       
        private static string smtpServer = ConfigurationManager.AppSettings["SMTPSERVER"];
        private static string defaultEMail = ConfigurationManager.AppSettings["DEFAULTEMAIL"];
        private static string onboardidng = ConfigurationManager.AppSettings["ONBOARDING"];

        static void Main(string[] args)
        {
            
            SendNotification sendNotification = new Suitability.SendNotification(defaultEMail, 1, connectionString, smtpServer, onboardidng);
            
            //sendNotification.SendAdjudicationNotification();
            sendNotification.SendSponsorshipNotification();
        }
    }
}
