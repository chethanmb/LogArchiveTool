using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogArchiveTool
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static string source = ConfigHelper.GetValue("SourceFolder");
        static void Main(string[] args)
        {

           


            try
            {
                #region Email_Start_Message
                string message = string.Empty;
                //message = Zipper.NEW_LINE + "LOGS FOR LOGFILEARCHIVE TOOL" + Zipper.NEW_LINE;
                //Zipper.EmailMessage = Zipper.EmailMessage.Append(message);
                //message = "--------------------------------" + Zipper.NEW_LINE;
                //Zipper.EmailMessage = Zipper.EmailMessage.Append(message);
                #endregion Email_Start_Message

                #region Packaging
                if (Directory.Exists(ConfigHelper.GetValue("DestinationFolder")))
                {
                    Directory.Delete(ConfigHelper.GetValue("DestinationFolder"), true);
                }

                Directory.CreateDirectory(ConfigHelper.GetValue("DestinationFolder"));
                
                //Zipper loZipper = new Zipper();
                //loZipper.CompressFile();              
                #endregion Packaging
                
                Zipper compress = new Zipper();

                int rtn = compress.InitSteps();
                if (rtn == 1)
                {
                    return;
                }
                
                
                string zipOutMsg = compress.Zip();

                #region App_Summary_For_Email
                message = string.Empty;

                message = "SUMMARY OF LOGFILEARCHIVAL TOOL" + Zipper.NEW_LINE;

                message = message + "----------------------------------------------------" + Zipper.NEW_LINE;

                message = message + "Number of Log Files To Be Zipped: " + Zipper.NoOfLogFilesToBeZipped.ToString() + Zipper.NEW_LINE;

                message = message + "Number of Log Files Not Considered: " + Zipper.NoOfLogFilesNotConsidered.ToString() + Zipper.NEW_LINE + Zipper.NEW_LINE;

                message = message + "Completion Time of the Application: " + DateTime.Now.ToString() + Zipper.NEW_LINE + Zipper.NEW_LINE;

                message = message +  Zipper.NEW_LINE + "Zip Archival Process Status" + Zipper.NEW_LINE;

                message = message + "-----------------------------------" + Zipper.NEW_LINE;

                message = message + zipOutMsg;

                message = message +  Zipper.NEW_LINE + Zipper.NEW_LINE + "Thanks & Regards," + Zipper.NEW_LINE + "Chethan";

                Zipper.EmailMessage = Zipper.EmailMessage.Append(message);

                message = ConfigHelper.GetValue("HostName") + " - Log File Archival Tool Execution Summary - Date: " + DateTime.Now.ToString();

                
                if (Directory.Exists(ConfigHelper.GetValue("DestinationFolder")))
                {
                    Directory.Delete(ConfigHelper.GetValue("DestinationFolder"), true);
                }
                
                EmailUtil.SendEmailToAdmin(message, Zipper.EmailMessage.ToString(), null);
                
                #endregion App_Summary_For_Email

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.ReadKey();
                Logger.Error(Ex);

            }
            
        }
    }
}
