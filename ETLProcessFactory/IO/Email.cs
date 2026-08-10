using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace ETLProcess
{
	/// <summary>
	/// An object that can send over the ETLP email system. 
	/// Can throw EmailException when creating the object
    /// <para>Note! It is not recommended to hard-code credentials into a program!
    /// Example members are here only to build the boilerplate InitClient method.</para>
	/// </summary>
	internal sealed class CompanyEmail
	{
        private static readonly string smtpUser = Env("ETLP_SMTP_USER");
        private static readonly string smtpPassword = Env("ETLP_SMTP_PASSWORD");
        private static readonly int smtpPort = int.Parse(Env("ETLP_SMTP_PORT"), System.Globalization.CultureInfo.InvariantCulture);
        private static readonly string smtpHost = Env("ETLP_SMTP_HOST");

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
		/// Sends an email using ETLP Inc Email Mandrill service
		/// Can throw EmailException if email fails to send
		/// </summary>
        internal void Send()
        {
            using var mailMessage = new MailMessage()
            {
                Sender = new MailAddress(Env("ETLP_SMTP_FROM")),
                From = new MailAddress(Env("ETLP_SMTP_FROM")),
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
	
        /// <summary>
        /// Reads a required setting from the environment. Fails closed: no defaults,
        /// no fallbacks, no secrets in source. See .env.example for the required keys.
        /// </summary>
        private static string Env(string key) =>
            System.Environment.GetEnvironmentVariable(key)
            ?? throw new System.InvalidOperationException(
                $"Missing required environment variable '{key}'. See .env.example.");
}
}