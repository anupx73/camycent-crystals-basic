using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crystals.Models;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Crystals
{
    static class Program
    {
        //Static Object for Database Handling.
        public static CrudDB db;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool mutexCreated = true;
            using (Mutex mutex = new Mutex(true, "CRYSTAL-4BFCE54B-7836-4D11-B0CD-91AAED489293", out mutexCreated))
            {
                if (mutexCreated)
                {
                    //Installer run check and delete
                    if (RegistryLicense.IsInstallerRunning())
                    {
                        MessageBox.Show("Please stop Crystals Setup before launching application.", "Crystals", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    RegistryLicense.DeleteInstaller();

                    //License check
                    if (!RegistryLicense.IsLicenseKeyPresent())
                    {
                        MessageBox.Show("No license key found. Please contact Camycent support.", "Crystals", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (RegistryLicense.IsLicenseExpired())
                    {
                        MessageBox.Show("Your license has expired. Please contact Camycent support.", "Crystals", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!RegistryLicense.IsSystemDateValid())
                    {
                        MessageBox.Show("Failed to initialize application due to system date modification. Please contact Camycent support.", "Crystals", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RegistryLicense.WriteLastRun();

                    //Load application dialog
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new LoginForm());
                    //Application.Run(new MainForm("admin", "admin"));
                }
                else
                {
                    MessageBox.Show("Another instance of application is already running.", "Already running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
