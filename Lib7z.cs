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
    public class Zipper
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public int status;
        public const string NEW_LINE = "\r\n";
        public static int NoOfLogFilesToBeZipped = 0;
        public static int NoOfLogFilesNotConsidered = 0;
       // public static int NoOfLogFilesMovedToTempDir = 0;
        public static int NoOfLogFilesZipped = 0;
        public static int NoOfLogFilesFailedWhileZipping = 0;

        public static StringBuilder EmailMessage = new StringBuilder();
        public string formattedOutput = String.Empty;
        public string src = ConfigHelper.GetValue("SourceFolder");
        public string dest = ConfigHelper.GetValue("DestinationFolder");
        public string zipExe = ConfigHelper.GetValue("7ZipExe");

        public int InitSteps()
        {

            int am = Convert.ToInt32(ConfigHelper.GetValue("ArchivalMethod")); // 0 or 1
            int archDuration = Convert.ToInt32(ConfigHelper.GetValue("NoOfDays"));
            
            int curMonth = DateTime.Now.Month;
            int curYear = DateTime.Now.Year;
            string path = String.Empty;
            string ext = ".log";

            int status = checkForOldZips(src);
            if (status == 1)
            {
                return 1;
            }
            

            //DirectoryInfo di = new DirectoryInfo(dest);
            //try
            //{
            //    if (di.Exists)
            //    {
            //        Console.WriteLine("That path exists already.");
            //    }
            //    else
            //    {
            //        di.Create();
            //        Console.WriteLine("\"Temp\" directory was created successfully.");
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The process failed: {0}", e.ToString());
            //}
            //finally
            //{ }


            if (am == 0) //Filter out files based on date/month embedded in the filename
            {                
                char[] separator = { '-' };
                //Int32 count = 3;
                DirectoryInfo dir = new DirectoryInfo(src);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo Tempfile in files)
                {
                    if (Tempfile.Extension == ext)
                    {

                        DateTime dtCreationTime = Tempfile.CreationTime;
                        int crtMonth = dtCreationTime.Month;
                        int diff = curMonth - crtMonth;

                        if (diff >= archDuration)
                        {
                            {
                                string srcFileName = Tempfile.Name; //only the filename
                                string srcFileName_path = Tempfile.FullName; //filename along with the path
                                string destFileName = Path.Combine(dest, srcFileName);// gives filename with full path to which src file is moved
                                try
                                {
                                    File.Move(srcFileName_path, destFileName);
                                    NoOfLogFilesToBeZipped += 1;

                                    if (File.Exists(destFileName))
                                    {
                                        Console.WriteLine("\nFile moved to destination: " + srcFileName_path);
                                        
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nError: Failed to move following file to the destination" + srcFileName_path);
                                    }
                                }
                                catch (IOException e)
                                {
                                    Console.WriteLine($"ERROR:\t{e.Message}");
                                    Console.WriteLine(destFileName);
                                }
                                NoOfLogFilesNotConsidered += 1;

                            }

                        }
                    }

                }
            } //Filter ends here

            else if (am == 1)
            {

                Logger.Info("\n--------------------------Archiving files based on date/month embedded in the filenames------------------------\n");
               
                
                var logFiles = Directory.EnumerateFiles(src);

                foreach (string currentFile in logFiles)
                {   
                    
                    if (currentFile.Contains(ext))
                    {
                        try
                        {
                        
                            string fileDate = currentFile.Substring(23, 10).Replace( '-', ',');
                            string fileName = currentFile.Substring(src.Length);
                            DateTime date1 = DateTime.Parse(fileDate);   
                            TimeSpan diff = DateTime.Now.Subtract(date1);
                            if (diff.TotalDays > archDuration)
                                {
                                    File.Move(currentFile, Path.Combine(dest, fileName));
                                    NoOfLogFilesToBeZipped += 1;
                                }
                        }
                        catch(Exception Ex)
                        {
                            Console.WriteLine(Ex.Message);
                        }
                       
                    }  
                    
                }
            }
            else
            {

                    Logger.Info("\n------------------------Fatal Error: Check for ArchivalMethod and other keys in .config file----------------------\n");
               

            }
            return 0;
        }

        ////Console.ReadLine();
        //string zipExe = @"C:\Program Files\7-Zip\7z.exe";
        //string baseDir = @"D:\BPS\DMS\Logs\";
        //string tmp = @"D:\BPS\DMS\Logs\Temp\";
        //string sourceDir = tmp + "*.log";

        //Zipper zip = new Zipper();
        //zip.Compress(zipExe, baseDir, tmp, sourceDir, archiveName);

        //return;


        public string Zip()
        {   
            if (Directory.GetFiles(dest).Length != 0)
            {
                int nThreads = Convert.ToInt32(ConfigHelper.GetValue("nCPUThrds"));
                int CmprssnLvl = Convert.ToInt32(ConfigHelper.GetValue("CmprssnLvl"));

                string d = DateTime.Now.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_');
                string archiveName = Path.Combine(src, d);
                string zipSource = dest + "*.log";
                //Process process = new Process();
                try
                {
                    ProcessStartInfo p = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Normal,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = zipExe,
                        Arguments = "a -tzip -r -bb3 -mmt\"" + CmprssnLvl + "\" -mx\"" + nThreads + "\" \"" + archiveName + "\" \"" + zipSource
                    };
                    Logger.Info("Creating ZipArchive -> " + archiveName + ".zip\n"); //+ "Source:" + source1);
                    NoOfLogFilesZipped += 1;
                    Process x = Process.Start(p);
                    string output = x.StandardOutput.ReadToEnd();
                    formattedOutput = RemoveMsg(output);
                    Logger.Info(formattedOutput);
                
                    x.WaitForExit();
                    //Console.ReadKey();
                
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    Console.ReadLine();
                    NoOfLogFilesFailedWhileZipping += 1;
                }
                //return (x.ExitCode);
            }
            else
            {
                formattedOutput = "0 Files were added to the zip";
            }
            return (formattedOutput);
        }



        public int checkForOldZips(string src)
        {
            //string src = source;
            int status = 0;
            string pattern = @"^.*.zip";  //@"\d{2}_\d{2}_\d{4}_\d{1,2}_\d{1,2}_\d{1,2}_\w[AM].zip{1}"
            Regex rgx = new Regex(pattern);

            var Files = Directory.EnumerateFiles(src);

            foreach (string currentFile in Files)
            {
                string fileName = currentFile.Substring(src.Length);
                if (rgx.IsMatch(fileName))
                {
                    status = 1;
                    Console.WriteLine("Previously created archive exists", fileName);
                    Logger.Error("Previously created archive exists -> " + fileName);
                }
            }
            return (status);
        }


        private string RemoveMsg(string output)
        {
            
            int pos = output.IndexOf("7");
            if (pos >= 0)
            {
               output = output.Remove(0, 74);

            } 
            return (output); 
        }

    }
}
// return(0);


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


/*
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


    */
