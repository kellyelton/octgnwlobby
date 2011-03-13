using System;
using System.IO;
using System.Management;
using System.Net;
using System.Windows;

namespace Octgn.Lobby
{
    public class ErrorLog
    {
        private static object locker = 1;

        public static void WriteError(Exception ex, String error, bool KillApp)
        {
            lock (locker)
            {
                //lol
                if (!ex.Message.Trim().Equals("Thread was being aborted."))
                {
                    FileStream f = File.Open("errors.log", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(f);
                    sw.WriteLine("------------------------------------------------------------------------");
                    sw.WriteLine(DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString());
                    sw.WriteLine("handleError message: " + error);
                    if (ex.Message != null)
                        sw.WriteLine("Exception Message: " + ex.Message);
                    if (ex.Source != null)
                        sw.WriteLine("Source: " + ex.Source);
                    if (ex.TargetSite != null)
                        sw.WriteLine("Target Site: " + ex.TargetSite.ToString());
                    if (ex.Data != null)
                        sw.WriteLine("Data: " + ex.Data.ToString());
                    if (ex.InnerException != null)
                    {
                        sw.WriteLine("---------Inner Exception---------------------------------");
                        sw.WriteLine(ex.ToString());
                        sw.WriteLine("---------------------------------------------------------");
                    }
                    if (ex.StackTrace != null)
                    {
                        sw.WriteLine("StackTrace: ");
                        sw.Write(ex.StackTrace);
                    }
                    sw.WriteLine("------------------------------------------------------------------------\n");
                    sw.Close();
                    if (KillApp)
                    {
                        if (Program.LClient != null)
                        {
                            Program.LClient.Close("Error.", false);
                        }
                        if (Program.lwLobbyWindow != null)
                        {
                            Program.lwLobbyWindow.Close();
                        }
                        Application.Current.MainWindow.Close();
                        Application.Current.Shutdown(0);
                    }
                }
            }
        }

        public static void CheckandUpload()
        {
            lock (locker)
            {
                WqlObjectQuery objectQuery = new WqlObjectQuery("select * FROM Win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(objectQuery);
                string temp = "";
                foreach (ManagementObject MO in searcher.Get())
                {
                    foreach (PropertyData bo in MO.Properties)
                    {
                        temp += bo.Name + ": " + Convert.ToString(bo.Value) + "\n";
                    }
                }
                try
                {
                    StreamReader sr = File.OpenText("errors.log");
                    try
                    {
                        String errors = sr.ReadToEnd();
                        sr.Close();
                        if (!String.IsNullOrEmpty(errors) && !String.IsNullOrWhiteSpace(errors))
                        {
                            errors += "Exit time: " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
                            errors += "------------------------------------------------------------------------\n";
                            errors += "System info\n";
                            errors += "------------------------------------------------------------------------\n";
                            errors += temp;
                            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                            byte[] ubytes = encoding.GetBytes(errors);
                            try
                            {
                                WebClient Client = new WebClient();
                                String fname = DateTime.Now.ToFileTimeUtc().ToString() + ".log";
                                Client.UploadData("http://www.skylabsonline.com/oerrors/" + fname, "PUT", ubytes);
                                try
                                {
                                    File.Delete("errors.log");
                                }
                                catch (Exception e) { }
                            }
                            catch { }
                        }
                    }
                    catch
                    {
                        sr.Close();
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}