using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace ETLProcess
{
	/// <summary>
	/// An object that can send over the metropresort email system. 
	/// Can throw EmailException when creating the object
    /// <para>Note! It is not recommended to hard-code credentials into a program!
    /// Example members are here only to build the boilerplate InitClient method.</para>
	/// </summary>
	internal sealed class CompanyEmail
	{
        private const string smtpUser = @"Username";
        private const string smtpPassword = @"SuperSecretPassword_!";
        private const int smtpPort = 12345;
        private const string smtpHost = @"smtp.hostname.com";

        private static SmtpClient client;

        internal static void InitClient()
		{
			client = new SmtpClient(smtpHost)
			{
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(smtpUser, smtpPassword),
				Port = smtpPort
			};
		}

        //private MailMessage mailMessage;
        internal string Subject { get; set; }
        internal string Body { get; set; }
        internal bool IsBodyHtml { get; set; }
        internal List<string> To { get; set; }
        internal List<string> Cc { get; set; }
        internal List<string> Bcc { get; set; }

        /// <summary>
		/// Creates a new email object
		/// </summary>
        internal CompanyEmail()
		{
			Subject = "";
			Body = "";
			IsBodyHtml = false;
			To = new List<string>();
			Cc = new List<string>();
			Bcc = new List<string>();
		}

        /// <summary>
		/// Sends an email using Metropresort Inc Email Mandrill service
		/// Can throw EmailException if email fails to send
		/// </summary>
        internal void Send()
        {
            using var mailMessage = new MailMessage()
            {
                Sender = new MailAddress(@"WebSupport@Metropresort.com"),
                From = new MailAddress(@"WebSupport@Metropresort.com"),
                Subject = Subject,
                Body = Body,
                IsBodyHtml = IsBodyHtml
            };
            foreach (string s in To)
            {
                mailMessage.To.Add(s);
            }
            foreach (string s in Cc)
            {
                mailMessage.CC.Add(s);
            }
            foreach (string s in Bcc)
            {
                mailMessage.Bcc.Add(s);
            }
            client.Send(mailMessage);
        }
	}
}
