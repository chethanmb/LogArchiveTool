using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;

namespace LogArchiveTool
{
    class Zipper  //class Lib7z
    {
        
        
      public Zipper()
          {
            string zipExe = @"C:\Program Files\7-Zip\7z.exe";
            string baseDir = @"D:\BPS\DMS\Logs\";
            string tmp = @"D:\BPS\DMS\Logs\Temp\";
            string sourceDir = tmp + "*.log";
            DirectoryInfo dir = new DirectoryInfo(baseDir);
            FileInfo[] files = dir.GetFiles("Archived.zip");
            foreach (FileInfo Tempfile in files)
            {
                if (!Tempfile.Exists)
                {
                    Console.WriteLine("Creating zip archive...\n");
                }
                else
                {
                    Console.WriteLine("Previously archived file exists, kindly move the file to diff location\n");
                    
                    //return;
                }
            }

            DirectoryInfo di = new DirectoryInfo(tmp);

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
            p.FileName = zipExe;
            p.Arguments = "a -r -mx6 \"" + targetArchive + "\" \"" + sourceDir;
            Process x = Process.Start(p);
            x.WaitForExit();

            Directory.Delete(tmp, true);
            Console.ReadKey();
       }
    }
}
    

