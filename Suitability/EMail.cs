using System;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using Polly;

namespace Suitability
{
    class EMail
    {
        //Class variables
        protected string _strSmtpServer = string.Empty;
        protected string _strEmailFrom = string.Empty;
        protected string _strEmailTo = string.Empty;
        protected string _strEmailCc = string.Empty;
        protected string _strEmailBcc = string.Empty;
        protected string _strEmailSubject = string.Empty;
        protected string _strEmailMessageBody = string.Empty;
        protected string _strEmailAttachments = string.Empty;
        protected bool _IsBodyHtml = false;

        /// <summary>
        /// A function that calls private function after assignment of variables
        /// </summary>
        /// <param name="strEmailFrom"></param>
        /// <param name="strEmailTo"></param>
        /// <param name="strEmailCc"></param>
        /// <param name="strEmailBcc"></param>
        /// <param name="strEmailSubject"></param>
        /// <param name="strEmailMessageBody"></param>
        /// <param name="strEmailAttachments"></param>
        /// <param name="strSmtpServer"></param>
        /// <param name="IsBodyHtml"></param>
        /// <returns><see cref="String.Empty"/> if send succeeds; Error message otherwise.</returns>
        public string Send(
            string strEmailFrom, 
            string strEmailTo, 
            string strEmailCc, 
            string strEmailBcc, 
            string strEmailSubject,
            string strEmailMessageBody, 
            string strEmailAttachments,
            string strSmtpServer,
            bool IsBodyHtml = false)
        {
            _strEmailFrom = strEmailFrom;
            _strEmailTo = strEmailTo;
            _strEmailCc = strEmailCc;
            _strEmailBcc = strEmailBcc;
            _strEmailSubject = strEmailSubject;
            _strEmailMessageBody = strEmailMessageBody;
            _strEmailAttachments = strEmailAttachments;
            _strSmtpServer = strSmtpServer;
            _IsBodyHtml = IsBodyHtml;

            var returnMessage = string.Empty;

            // Define retry policy...
            // Includes all 'To' emails (To, Cc, Bcc)
            var allEmails = $"To: {_strEmailTo}";

            allEmails += !string.IsNullOrEmpty(_strEmailCc) ? $", Cc: {_strEmailCc}" : string.Empty;
            allEmails += !string.IsNullOrEmpty(_strEmailBcc) ? $", Bcc: {_strEmailBcc}" : string.Empty;

            // Setup retry policy to handle SmtpExceptions. If first attempt fails, waits 1 second followed by retry intervals (in seconds): 2, 4, 8
            var result = Policy.Handle<SmtpException>()
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAndCapture(SendEmail);

            if (result.Outcome != OutcomeType.Successful)
            {
                returnMessage = $"Failed to send email with subject '{_strEmailSubject}' to: {allEmails}. Exception: {result.FinalException}";
            }

            return returnMessage;
        }

        /// <summary>
        /// Sends an email using smtp server
        /// </summary>
        private void SendEmail()
        {
            // Don't attempt an email if there is no smtp server
            if (string.IsNullOrEmpty(_strSmtpServer))
            {
                throw new SmtpException(SmtpStatusCode.ServiceNotAvailable, "SMTP server value is empty. Check configuration.");
            }

            if (IsSmtpConnectionAvailable(_strSmtpServer))
            {
                throw new SmtpException(SmtpStatusCode.ServiceNotAvailable, $"The SMTP server {_strSmtpServer} is unavailable.");
            }

            try
            {
                var mailFrom = new MailAddress(_strEmailFrom);
                var message = new MailMessage
                {
                    From = mailFrom,
                    Subject = _strEmailSubject,
                    Body = _strEmailMessageBody,
                    IsBodyHtml = _IsBodyHtml
                };

                if (_strEmailTo.Trim().Length > 0)
                    message.To.Add(_strEmailTo);

                if (_strEmailCc.Trim().Length > 0)
                    message.CC.Add(_strEmailCc);

                if (_strEmailBcc.Trim().Length > 0)
                    message.Bcc.Add(_strEmailBcc);

                // TO-DO: Include additional validation for all parameter fields (RegEx)

                if (_strEmailAttachments.IndexOf(";", StringComparison.Ordinal) > 0)
                {
                    // Split multiple attachments into a string array
                    Array a = _strEmailAttachments.Split(';');

                    // Loop through attachments list and add to objMail.Attachments one at a time
                    for (var i = 0; i < a.Length; i++)
                    {
                        message.Attachments.Add(new Attachment(a.GetValue(i).ToString().Trim()));
                    }
                }
                else if (_strEmailAttachments.Length > 0) // Single attachment without trailing separator
                {
                    message.Attachments.Add(new Attachment(_strEmailAttachments.Trim()));
                }

                var smtpMail = new SmtpClient(_strSmtpServer);
                smtpMail.Send(message);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                var failedRecipientsMsg = string.Empty;

                foreach (var innerException in ex.InnerExceptions)
                {
                    var status = innerException.StatusCode;

                    if (status != SmtpStatusCode.MailboxBusy ||
                        status != SmtpStatusCode.MailboxUnavailable)
                    {     
                        failedRecipientsMsg += $"Failed to deliver message to {innerException.FailedRecipient}\n";
                    }
                }

                throw new SmtpException(failedRecipientsMsg, ex);
            }
        }
        /// <summary>
        ///     Determine if the SMTP is available
        /// </summary>
        /// <param name="smtpServer">The SMTP server</param>
        /// <returns>True if the SMTP server is available; false otherwise.</returns>
        private static bool IsSmtpConnectionAvailable(string smtpServer)
        {
            var isConnectionAvailable = false;

            try
            {
                using (var smtpTest = new TcpClient())
                {
                    smtpTest.Connect(smtpServer, 25);

                    if (smtpTest.Connected)
                    {
                        string returnedValue;

                        using (var ns = smtpTest.GetStream())
                        {
                            using (var sr = new StreamReader(ns))
                            {
                                returnedValue = sr.ReadLine() ?? string.Empty;
                            }
                        }

                        // If returned value contains 220, connection established
                        isConnectionAvailable = returnedValue.Contains("220");
                    }
                }
            }
            catch
            {
                throw new SmtpException(SmtpStatusCode.GeneralFailure, $"The SMTP Server {smtpServer} is NOT available");
            }

            return isConnectionAvailable;
        }
    }
}