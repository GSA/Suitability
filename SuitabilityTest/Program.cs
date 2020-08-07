﻿using System;
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
        private static string onboarding = ConfigurationManager.AppSettings["ONBOARDING"];

        static void Main(string[] args)
        {
            int persId = 3244;
            int ContractId = 34332;
            persId = 0;
            //persId = 396684;
            //persId = 396686;
            //SendNotification sendNotification = new Suitability.SendNotification(defaultEMail, persId, connectionString, smtpServer, onboarding);
            SendNotification sendNotification = new Suitability.SendNotification(defaultEMail, persId, connectionString, smtpServer, onboarding, ContractId);
            //Calling original Suitability methods
            //sendNotification.SendAdjudicationNotificationV1();
            //sendNotification.SendSponsorshipNotificationV1(); 

            // Calling new Suitability methods
            //sendNotification.SendAdjudicationNotification();
            //sendNotification.SendSponsorshipNotification();
            //sendNotification.SendSRSNotification();
            sendNotification.SendExpiringContractReminder();
        }
    }
}
