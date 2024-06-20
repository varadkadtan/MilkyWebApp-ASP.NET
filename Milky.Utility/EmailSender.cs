using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Utility
{
    public class EmailSender : IEmailSender
    {
        public string PostMarkServerKey { get; set; } //property to aquire Postmark key

        public EmailSender(IConfiguration _config)
        {
            PostMarkServerKey = _config.GetValue<string>("Postmark:ServerApiToken"); //key set
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //logic to send email

            //create new postmark client

            var client = new PostmarkClient(PostMarkServerKey);

            var message = new PostmarkMessage
            {
                From = "support@kollamdairyfarm.shop",
                To = email,
                Subject = subject,
                HtmlBody = htmlMessage
            };

            // Send the email using the PostmarkClient's SendMessageAsync method
            return client.SendMessageAsync(message);

        }
    }
}
