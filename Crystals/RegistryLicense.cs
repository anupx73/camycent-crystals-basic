using System;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Crystals
{
    class RegistryLicense
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(String sClassname, String sAppName);

        private const string REG_LOCATION_MASTER        = "Software\\Classes\\CLSID\\{E5BA2C22-F96E-4AA0-B1AC-8498053D0475}";
        private const string REG_LOCATION_APP           = "HKEY_CURRENT_USER\\SOFTWARE\\Camycent\\Crystals";

        private const string REG_VALUE_UNI_KEY          = "LicKey";
        private const string REG_VALUE_LIC_DAYS         = "ValidityDays";
        private const string REG_VALUE_INSTALL_DATE     = "InstallDate";
        private const string REG_VALUE_LAST_RUN         = "LastRun";
        private const string REG_VALUE_INSTALLER_PATH   = "InstallerPath";
        private const string REG_VALUE_DATA_PATH        = "DataPath";
        private const string REG_VALUE_FIRST_RUN        = "FirstRun";

        private static string[] LicenseKeys             = {"18C6802D-BA5A-43E4-8797-EB8E688A7BAF",
                                                           "0B165C8A-2B3B-4DD6-A527-91B20A4BAC54",
                                                           "3DB9F10D-95EC-4700-99CD-3F447C5A16D5",
                                                           "EFBA76A8-3B2C-4672-8DC7-706449FEB44B",
                                                           "77B98BBD-B874-42E0-9CEA-253FA6655A09",
                                                           "20FD275E-14B3-403A-A54A-5E379487471A",
                                                           "8707755F-68CE-40FE-AAD4-483CDAB6F654",
                                                           "1EAB4496-E74E-4812-8E65-BF3CE3EEB2A2",
                                                           "C351A73B-59B6-47C6-9374-14965EE13DD3",
                                                           "B11448A3-3428-42E3-97C1-3FA5859D678A"};

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static bool IsLicenseKeyPresent()
        {
            bool found = false;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);  
                if (regKey != null)
                {
                    string installedKey = (string)regKey.GetValue(REG_VALUE_UNI_KEY);
                    if (!String.IsNullOrEmpty(installedKey))
                    {
                        installedKey = Base64Decode(installedKey);
                        foreach (string key in LicenseKeys)
                        {
                            if (key.CompareTo(installedKey) == 0)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                regKey.Close();
            }

            return found;
        }

        public static bool IsLicenseExpired()
        {
            bool expired = true;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);
                if (regKey != null)
                {
                    string installDate = (string)regKey.GetValue(REG_VALUE_INSTALL_DATE);
                    if (!String.IsNullOrEmpty(installDate))
                    {
                        installDate = Base64Decode(installDate);
                        DateTime dtInstalled = Convert.ToDateTime(installDate);

                        string todayDate = DateTime.Today.ToShortDateString();
                        DateTime dtToday = Convert.ToDateTime(todayDate);

                        int daysLeft = (dtToday - dtInstalled).Days;

                        string validity = (string)regKey.GetValue(REG_VALUE_LIC_DAYS);
                        validity = Base64Decode(validity);
                        if (Convert.ToInt32(validity) > daysLeft)
                        {
                            expired = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey!= null)
                    regKey.Close();
            }

            return expired;
        }

        public static bool IsSystemDateValid()
        {
            bool valid = false;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);
                if (regKey != null)
                {
                    string lastRun = (string)regKey.GetValue(REG_VALUE_INSTALL_DATE);
                    if (!String.IsNullOrEmpty(lastRun))
                    {
                        lastRun = Base64Decode(lastRun);
                        DateTime dtlastRun = Convert.ToDateTime(lastRun);
                        DateTime currentDay = DateTime.Today;
                        if (currentDay >= dtlastRun)
                        {
                            valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }

            return valid;
        }

        public static void WriteLastRun()
        {
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER, true);
                if (regKey != null)
                {
                    string lastRun = DateTime.Today.ToShortDateString();
                    lastRun = Base64Encode(lastRun);
                    regKey.SetValue(REG_VALUE_LAST_RUN, lastRun);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static bool IsInstallerRunning()
        {
            IntPtr hwnd = FindWindow("#32770", "Crystals Setup");
            if (hwnd != IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteInstaller()
        {
            RegistryKey regKey = null;
            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);
                if (regKey != null)
                {
                    string installerPath = (string)regKey.GetValue(REG_VALUE_INSTALLER_PATH);
                    if (!String.IsNullOrEmpty(installerPath))
                    {
                        installerPath = Base64Decode(installerPath);
                        if (System.IO.File.Exists(installerPath))
                        {
                            System.IO.File.Delete(installerPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
        }

        public static string GetDataBaseLocation()
        {
            string databasePath = "";
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);
                if (regKey != null)
                {
                    string dbPath = (string)regKey.GetValue(REG_VALUE_DATA_PATH);
                    if (!String.IsNullOrEmpty(dbPath))
                    {
                        dbPath = Base64Decode(dbPath);
                        dbPath += "\\Database\\";
                        if (System.IO.Directory.Exists(dbPath) && System.IO.File.Exists(dbPath + "Crystals.mdf") && System.IO.File.Exists(dbPath + "Crystals_log.ldf"))
                        {
                            databasePath = dbPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }

            return databasePath;
        }

        public static string GetInvoiceFolderLocation()
        {
            string destFolder = "";
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER);
                if (regKey != null)
                {
                    string savedPath = (string)regKey.GetValue(REG_VALUE_DATA_PATH);
                    if (!String.IsNullOrEmpty(savedPath))
                    {
                        savedPath = Base64Decode(savedPath);
                        savedPath += "\\Invoices\\";
                        if (System.IO.Directory.Exists(savedPath))
                        {
                            destFolder = savedPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }

            //set default path to bin location if no reg data found
            if (string.IsNullOrEmpty(destFolder))
            {
                destFolder = System.IO.Directory.GetCurrentDirectory() + "\\Invoices\\";
            }

            //create month wise folder
            destFolder += DateTime.Today.ToString("MM-yyyy") + "\\";

            if (!System.IO.Directory.Exists(destFolder))
            {
                System.IO.Directory.CreateDirectory(destFolder);
            }
            //test code
            //string timestamp = DateTime.Now.ToString("MMddyyyyHHmmss");
            //string dstFilename = destFolder + timestamp + "_" + "Tia Kaur" + ".pdf";

            return destFolder;
        }

        public static bool IsFirstRun()
        {
            bool yes = false;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(REG_LOCATION_MASTER, true);
                if (regKey != null)
                {
                    string firstRun = (string)regKey.GetValue(REG_VALUE_FIRST_RUN);
                    if (firstRun == "Yes")
                    {
                        regKey.SetValue(REG_VALUE_FIRST_RUN, "No");
                        yes = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }

            return yes;
        }
    }
}
