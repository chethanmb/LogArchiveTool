﻿using System;
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
        static void Main(string[] args)
        {
            try
            {
                Logger.Info("XXXX");
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            int am = Convert.ToInt32(ConfigHelper.GetValue("ArchivalMethod")); // 0 or 1

            string path;
            DateTime dt = DateTime.Now;       
            string d = dt.ToString().Replace('/', '_').Replace(' ', '_').Replace(':', '_');         
            int curMonth = dt.Month;
            int curYear = dt.Year;
            string src = @"D:\BPS\DMS\Logs";
            string archiveName = Path.Combine(src,d);
            string dest = @"D:\BPS\DMS\Logs\Temp\";
            int archDuration = Convert.ToInt32(ConfigHelper.GetValue("MonthCount"));

            string pattern = @"\d{2}_\d{2}_\d{4}_\d{1,2}_\d{1,2}_\d{1,2}_\w[AM].zip{1}";
                Regex rgx = new Regex(pattern);

                var Files = Directory.EnumerateFiles(src);

                    foreach (string currentFile in Files)
                    {
                        string fileName = currentFile.Substring(src.Length);
                        if(rgx.IsMatch(fileName))
                        {
                        Console.WriteLine("Previously created archive exists", fileName);
                        Logger.Error("Previously created archive exists -> " + fileName);
                        Console.ReadKey();
                        return;
                        }
                    }

            if( am == 0) //Filter out files based on date/month embedded in the filename
            {
                
                if(!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }

                
                string ext = ".log";
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
                            }

                        }
                    }

                } 
            } //Filter ends here

            else if (am == 1)
            {
                Logger.Info("\n--------------------------Archiving files based on date/month embedded in the filenames------------------------\n");
                
                 if(!Directory.Exists(dest))
                    {
                    Directory.CreateDirectory(dest);
                    }

                var logFiles = Directory.EnumerateFiles(src);
            
                foreach (string currentFile in logFiles)
                {
                    int fileMonth = Int32.Parse(currentFile.Substring(28, 2));
                    int fileYear = Int32.Parse(currentFile.Substring(23, 4));               
                    string fileName = currentFile.Substring(src.Length + 1);
                    int monthDiff = curMonth - fileMonth;
                    int yearDiff = curYear - fileYear;
                    if ( yearDiff >= 0 && monthDiff >= archDuration )   
                    {
                        File.Move(currentFile, Path.Combine(dest, fileName));             
				    }
                    else if ( fileYear < curYear )
                    {
                        File.Move(currentFile, Path.Combine(dest, fileName));  
                    }
                    else
                    {
                        Logger.Error("Invalid Year/Month found in the logfile name -> " + currentFile);
                    }
			    }
            }
            else
            {
                Console.WriteLine("\nFatal Error: Check for ArchivalMethod and other keys in .config file\n");
                Logger.Info("\n------------------------Fatal Error: Check for ArchivalMethod and other keys in .config file----------------------\n");
                return;
            }
            //Console.ReadLine();
            string zipExe = @"C:\Program Files\7-Zip\7z.exe";
            string baseDir = @"D:\BPS\DMS\Logs\";
            string tmp = @"D:\BPS\DMS\Logs\Temp\";
            string sourceDir = tmp + "*.log";
            Zipper zip = new Zipper();
            zip.Compress(zipExe,baseDir,tmp,sourceDir,archiveName);
            return;
        }
    }
}
