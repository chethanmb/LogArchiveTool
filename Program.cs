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
        public const string NEW_LINE = "\r\n";
        private static string source = ConfigHelper.GetValue("SourceFolder");
        public static string zipDestination = ConfigHelper.GetValue("ZipDestinationFolder");

        static void Main(string[] args)
        {

           string StartTime = DateTime.Now.ToString();


            try
            {
 
                string message = string.Empty;
                string zipOutMsg = null;
                string ZipFound = null;
                #region Directory_check
                if (Directory.Exists(ConfigHelper.GetValue("DestinationFolder")))
                {
                    Directory.Delete(ConfigHelper.GetValue("DestinationFolder"), true);
                }

                Directory.CreateDirectory(ConfigHelper.GetValue("DestinationFolder"));

                if (!Directory.Exists(zipDestination))
                {
                    Directory.CreateDirectory(zipDestination);
                }
                  
                #endregion Directory_check
                
                Zipper compress = new Zipper();

                int status = compress.checkForOldZips();
                if (status == 0)
                {   
                    int rtn = compress.InitSteps();
                    if (rtn == 1)
                    {
                        return;
                    }
                                        
                    zipOutMsg = compress.Zip();
                                   
                }
                else
                {
                    ZipFound = "ABORT: Old archive found in " + zipDestination + NEW_LINE;
                }

                
                 
                

                #region Email
               // message = string.Empty;

                message = "SUMMARY OF LOGFILEARCHIVAL TOOL" + NEW_LINE;

                message = message + "----------------------------------------------------" + NEW_LINE;

                message = message + "Starting Time of the Application: " + StartTime + NEW_LINE + NEW_LINE;

                message = message + "Number of Log Files To Be Zipped: " + Zipper.NoOfLogFilesToBeZipped.ToString() + NEW_LINE;

                message = message + "Number of Log Files Not Considered: " + Zipper.NoOfLogFilesNotConsidered.ToString() + NEW_LINE + NEW_LINE;

                message = message + "Completion Time of the Application: " + DateTime.Now.ToString() + NEW_LINE + NEW_LINE;

                message = message +  NEW_LINE + "Zip Archival Process Status" + NEW_LINE;

                message = message + "-----------------------------------" + NEW_LINE;

                message = message + ZipFound + NEW_LINE;

                message = message + zipOutMsg;

                message = message +  NEW_LINE + NEW_LINE + "Thanks & Regards," + NEW_LINE + "Chethan";

                Zipper.EmailMessage = Zipper.EmailMessage.Append(message);

                message = ConfigHelper.GetValue("HostName") + " - Log File Archival Tool Execution Summary - Date: " + DateTime.Now.ToString();

                
                if (Directory.Exists(ConfigHelper.GetValue("DestinationFolder")))
                {
                    Directory.Delete(ConfigHelper.GetValue("DestinationFolder"), true);
                }
                
                EmailUtil.SendEmailToAdmin(message, Zipper.EmailMessage.ToString(), null);
                
                #endregion Email

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
