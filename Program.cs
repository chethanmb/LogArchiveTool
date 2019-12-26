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
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            DateTime dt = DateTime.Now;
            int curMonth = dt.Month;
            string src = @"D:\BPS\DMS\Logs\";
            string dest = @"D:\BPS\DMS\Logs\Temp\";
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

                    if (diff >= 1)
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
            Console.ReadLine();
        }
    }
}



/* string srcFilesrc = @"D:\BPS\DMS\Logs";
         string destZipFilesrc = @"D:\BPS\DMS\Logs\Temp";


         ZipFile.CreateFromDirectory(srcFilesrc, destZipFilesrc);


         using (FileStream myzipfile = new FileStream("D:\\BPS\\DMS\\Logs\\output.zip", FileMode.Open))
         {
                 using (ZipArchive myzipfilearchive = new ZipArchive(myzipfile, ZipArchiveMode.Update))
             {
                 ZipArchiveEntry helpfile = myzipfilearchive.CreateEntry("");

                 using (StreamWriter writer = new
                    StreamWriter(helpfile.Open()))
                 {

                     writer.WriteLine("");
                 }
             }
         } */
