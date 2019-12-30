using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace LogArchiveTool
{
    class EmailUtil
    {
        #region Email_Constants

        private const char EMAIL_ADDRESS_SEPERATOR = ';';
        private const char EMAIL_ATTACHMENT_SEPERATOR = ';';
        private const string EMAIL_HOST = "";
        private const int EMAIL_PORT = 25;
        private const string EMAIL_FROM = "test@dxc.com";
        private const string EMAIL_TO = "chethan.m-b@dxc.com";

        #endregion Email_Constants

        #region Private_Members_Variables

        //Variable to hold SMTP Server ip address
        private static string host = string.Empty;
        //Variable to hold the Port Number
        private static int port;
        //Variable to hold
        private static string toAddresses = string.Empty;
        //Variable to hold DMS Support Email Address
        private static MailAddress fromAddress = null;
        //Variable to hold ReplyTo Email Address
       // private static MailAddress replyAddress = null;
        //Variable to hold User Credential to be used to login to SMTP Server.
        private static NetworkCredential userCredential = null;
        //Variable to hold the Logger Instance.
        //private static Infrastructure logger = new Infrastructure();

        #endregion Private_Members_Variables


        //=================Function Header===================================
        /// <summary>
        /// The function is used to Send Email to Administrator.
        /// </summary>
        /// <param name="subject">The subject of the Email.</param>
        /// <param name="body">The body of the Email.</param>
        /// <param name="attachmentfileLocations">The location of the files to 
        /// be attached with the Email. </param>
        //===================================================================
        public static void SendEmailToAdmin(string subject,
                        string body,
                        string attachmentfileLocations)
        {
            //Variable to hold Message object
            MailMessage message = null;

            try
            {
                SetFields();

                BuildMessage(out message, subject, body, attachmentfileLocations);

                SendMessage(message);
            }
            finally
            {
                //Cleanup Objects
                if (message != null)
                {
                    message.Dispose();
                }
                message = null;
            }
        }

        //=================Function Header===================================
        //Function     : SetFields
        //Purpose      : The function is used to set Usercredentials,sender
        //               address,recepient address etc.
        /// <summary>
        /// The function is used to set Usercredentials,sender address, recepient 
        /// address etc.
        /// </summary>
        //===================================================================
        private static void SetFields()
        {
            //Local Variable to hold UserId
            string userId;
            //Local Variable to hold Password
            string password;
            //Local Variable to hold Domain
            string domain;

            //Set the SMTP Host 
            host = ConfigHelper.GetValue("Host");

            //Set the Port
            port = Convert.ToInt32(ConfigHelper.GetValue("Port"));

            //Set the User Credentials
            userId = ConfigHelper.GetValue("UserId");
            password = ConfigHelper.GetValue("Password");
            domain = ConfigHelper.GetValue("Domain");

            if (userId != string.Empty)
            {
                //Set the userCredential Class Variable
                userCredential = new NetworkCredential(userId, password, domain);
            }

            //Set the From Address
            fromAddress = new MailAddress(ConfigHelper.GetValue("From"));

            //Set the ReplyTo Address
            //replyAddress = new MailAddress(ConfigHelper.GetValue("ReplyTo"));

            //Set the To Address String
            toAddresses = ConfigHelper.GetValue("To");
        }

        //=================Function Header===================================
        //Function     : BuildMessage
        //Purpose      : The function is used to build the message object.
        //IN Params    : message-message object containing the Email Message.
        //             : subject -The subject of the Email.
        //             : body-The body of the Email.
        //             : attachmentfileLocations -The location of the files to 
        //               be attached with the Email.  
        //Return Value : N/A 
        //===================================================================
        private static void BuildMessage(out MailMessage message, string subject, string body, string attachmentfileLocations)
        {
            //Variable to hold all the recepient addresses
            string[] toAddressCollection;

            //Variable to hold the names of all the attachment files
            string[] fileLocations;

            //Instantiate MailMessage object
            message = new MailMessage();

            //Set the From Address
            message.From = fromAddress;

            //if (replyAddress != null)
            //{
            //    //Set the ReplyTo Address
            //    message.ReplyTo = replyAddress;
            //}

            //Set the Subject of the email
            message.Subject = subject;

            //Set the Body of the email
            message.Body = body;

            //Create an AddressCollection containing the recepients list.
            toAddressCollection = toAddresses.Split(EMAIL_ADDRESS_SEPERATOR);

            //Create an AddressCollection containing the recepients list.
            foreach (string address in toAddressCollection)
            {
                //Add each recepient to "To" AddressCollection.
                message.To.Add(address);
            }

            //Check if the attachment Object is null or not
            if (attachmentfileLocations != null)
            {
                //Create an AddressCollection containing the recepients list.
                fileLocations = attachmentfileLocations.Split(EMAIL_ATTACHMENT_SEPERATOR);

                //Check if there are any attachments
                if (fileLocations.GetValue(0).ToString() != string.Empty)
                {
                    //Add all attachments to message object
                    foreach (string fileLocation in fileLocations)
                    {
                        if (File.Exists(fileLocation))
                        {
                            //Add each attachment to Attachments collection
                            message.Attachments.Add(new Attachment(fileLocation));
                        }
                    }
                }
            }
        }

        //=================Function Header===================================
        //Function     : SendMessage
        //Purpose      : The function is used to set Usercredentials,sender
        //               address,recepient address etc.
        //IN Params    : message-message object containing the email message.
        //Return Value : N/A 
        //===================================================================
        private static void SendMessage(MailMessage message)
        {
            //Variable to be used to send an email
            SmtpClient emailClient;
            try
            {
                //Instantiate SmtpClient object
                emailClient = new SmtpClient();

                //Set the SMTP Server's ip address
                emailClient.Host = host;

                //Set the Port
                emailClient.Port = port;

                //Check if userCredential is null or not
                if (userCredential == null)
                {
                    //Use default credentials
                    emailClient.UseDefaultCredentials = true;
                }
                else
                {
                    //Don't use default credentials
                    emailClient.UseDefaultCredentials = false;

                    //Set the Credentials
                    emailClient.Credentials = userCredential;
                }


                //Send an email
                emailClient.Send(message);
            }
           
            finally
            {
                //Cleanup objects
                emailClient = null;
            }
        }
    }
}
