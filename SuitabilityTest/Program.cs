using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suitability;

namespace SuitabilityTest
{
    class Program
    {
        private static string connectionString = "[connection string here]";
       
        private static string smtpServer = "smtp server";
        private static string defaultEMail = "default e-mail";       
        private static string onboardidng = "folder location";

        static void Main(string[] args)
        {
            
            SendNotification sendNotification = new Suitability.SendNotification(defaultEMail, 1, connectionString, smtpServer, onboardidng);
            
            sendNotification.SendAdjudicationNotification();
            sendNotification.SendSponsorshipNotification();
        }
    }
}
