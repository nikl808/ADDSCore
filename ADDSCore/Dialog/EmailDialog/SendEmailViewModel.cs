using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using ADDSCore.Service;
using ADDSCore.Command;
using ADDSCore.Model;
using System.Net;

namespace ADDSCore.Dialog.EmailDialog
{
    class SendEmailViewModel:DialogViewBaseModel<string>
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


        private UICommand sendMessage;
        public UICommand SendMessage
        {
            get
            {
                return sendMessage ??
                    (sendMessage = new UICommand(obj =>
                    {
                        MailMessage message = new MailMessage(From, To, Subject, Body);
                        SmtpClient client = new SmtpClient("server name");
                        Console.WriteLine("Changing time out from {0} to 100.", client.Timeout);
                        client.Timeout = 100;
                        // Credentials are necessary if the server requires the client 
                        // to authenticate before it will send e-mail on the client's behalf.
                        client.Credentials = CredentialCache.DefaultNetworkCredentials;

                        try
                        {
                            client.Send(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                                  ex.ToString());
                        }
                    }));
            }
        }
    }
}
