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
    class Zipper  //class Lib7z
    {  
        
      public void Compress(string zipexe, string basedir, string tmp, string source, string arc)
          {
            string zipexe1 =zipexe;
            string basedir1=basedir;
            string tmp1=tmp;
            string source1=source;
            string arcName=arc;
            
            
          //  foreach (FileInfo Tempfile in files)
           {
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
            }  

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

            string targetArchive = @"D:\BPS\DMS\Logs\Archived.zip";

            Process process = new Process();
            ProcessStartInfo p = new ProcessStartInfo();
            p.WindowStyle = ProcessWindowStyle.Normal;
            p.FileName = zipexe1;
            p.Arguments = "a -tzip -r -mx3 \"" + arcName + "\" \"" + source1;
            Process x = Process.Start(p);
            x.WaitForExit();
            int exitcode = x.ExitCode;

            Console.WriteLine(exitcode);
           Directory.Delete(tmp1, true);
           Console.ReadKey();
       }
    }
}
    

