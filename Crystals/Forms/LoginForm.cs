using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using Crystals.Controllers;
using Crystals.Models;
using System.Runtime.InteropServices;

namespace Crystals
{
    public partial class LoginForm : Form
    {
        //public static MainForm MainFrm;
        private const int HTCAPTION = 0x2;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Crystals - Login";
            this.Icon = Properties.Resources.app_icon;

            txtUsername.Text = "admin";
            txtPassword.Text = "admin";
            comboUserLevel.Text = "Store Owner";

            if ((txtUsername.Text == "") || (txtPassword.Text == ""))
            {
                UIUtility.DisableButton(btnLogin, true);
                btnLogin.Enabled = false;
            }
        }

        #region FormHandlers
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                this.Close();
                Application.Exit();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Textboxes_TextChanged(object sender, EventArgs e)
        {
            if ((txtUsername.Text == "") || (txtPassword.Text == "") || (comboUserLevel.Text == ""))
                UIUtility.DisableButton(btnLogin, true);
            else
                UIUtility.DisableButton(btnLogin, false);
        }

       

 

        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void lnkSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.camycent.com");
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void Cmbuserlevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboUserLevel.Text == null)
            {
                UIUtility.DisableButton(btnLogin, true);
            }
            else
            {
                if ((txtUsername.Text != "") && (txtPassword.Text != ""))
                {
                    UIUtility.DisableButton(btnLogin, false);
                }
            }
        }

        private void LoginForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

       
        #endregion


        private void LoginForm_Load(object sender, EventArgs e)
        {
            //Database connect
            try
            {
                Program.db = new CrudDB();
                Boolean success = Program.db.connectDB();
                if (success == false)
                {
                    MessageBox.Show("Err_001: Database corrupted. Please contact Camycent at support@camycent.com.", "Database connectivity issue");
                    Application.Exit();
                }
                else
                {
                    this.Activate();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                Log.SQLError("ERR:201 Failed to Connect DB " + ex.Message);

                MessageBox.Show("Err_002: Database corrupted. Please contact Camycent at support@camycent.com.", "Database connectivity issue");
                this.Hide();
                Application.Exit();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.lblLoginError.Visible = false;

            string UserLevel = comboUserLevel.SelectedItem.ToString();
            string Username = txtUsername.Text;
            string Password = txtPassword.Text;

            users login = new users();
            Boolean success = login.AuthUser(Username, Password, UserLevel);
            if (success)
            {
                this.Hide();
                MainForm _instance = new MainForm(UserLevel, Username);
                _instance.Show();
            }
            else
            {
                this.lblLoginError.Visible = true;
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.db.disconnectedDB();
        }


    }
}
