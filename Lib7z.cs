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
    class Zipper  
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public void Compress(string zipexe, string basedir, string tmp, string source, string arc)
        {
            string zipexe1 =zipexe;
            string basedir1=basedir;
            string tmp1=tmp;
            string source1=source;
            string arcName=arc;
            
            
          //  foreach (FileInfo Tempfile in files)
           //{
                //if (!Tempfile.Exists)
               /* if (!rgx.IsMatch(Tempfile.ToString))
                {
                    Console.WriteLine("Creating zip archive...\n");
                }
                else
                {
                    Console.WriteLine("Previously archived file exists, kindly move the file to diff location\n");
                    
                    //return;
                }   */
           // }  

            DirectoryInfo di = new DirectoryInfo(tmp1);

            try
            {
                if (di.Exists)
                {
                    Console.WriteLine("That path exists already.");
                }
                else
                {
                    di.Create();
                    Console.WriteLine("\"Temp\" directory was created successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }

            

            int eCode = CreateZip(zipexe1, arcName, source1);
            Logger.Info("7-Zip exit code: {0}", eCode);
            
            Directory.Delete(tmp1, true);
           //Console.ReadKey();
      }

        private int CreateZip(string zipexe1, string arcName, string source1)
        {
            
            //Process process = new Process();
            ProcessStartInfo p = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = zipexe1,
                Arguments = "a -tzip -r -bb3 -mx3 \"" + arcName + "\" \"" + source1
            };
            Logger.Info("Creating ZipArchive -> " + arcName + ".zip\n"); //+ "Source:" + source1);
            Process x = Process.Start(p);
            string output = x.StandardOutput.ReadToEnd();
            string formattedOutput = RemoveMsg(output);
            Logger.Info(formattedOutput);
            x.WaitForExit();
            return (x.ExitCode);
            

        }

        private string RemoveMsg(string output)
        {
            int pos = output.IndexOf(":");
            if(pos >= 0)
            {
                output = output.Remove(19, 55).Insert(0, "\n").Insert(20, "\n");
                
            }
            return (output);
        }
    }
}

/*
Logger.Info("Creating zip...Source Log File:" +source1+ "Zip Archive Name:" +arcName);
//Process process = new Process();
ProcessStartInfo p = new ProcessStartInfo();
p.WindowStyle = ProcessWindowStyle.Normal;
p.FileName = zipexe1;
p.Arguments = "a -tzip -r -mx3 -mmt\"" + arcName + "\" \"" + source1;
Process x = Process.Start(p);
x.WaitForExit();
int exitCode = x.ExitCode;
return (exitCode);
*/