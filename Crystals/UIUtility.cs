using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using System.Net.Mail;

namespace Crystals
{
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public static class UIUtility
    {
        public static int UserPwdMinLength = 4;
        public static int PhoneMinLength = 10;

        public static void DisableButton(Button btnName, bool bDisabled)
        {
            if (bDisabled)
            {
                if (btnName.Enabled)
                {
                    btnName.Enabled = false;
                    btnName.ForeColor = Color.FromArgb(209, 17, 65);
                    btnName.BackColor = Color.FromArgb(171, 171, 171);
                }
            }
            else
            {
                if (!btnName.Enabled)
                {
                    btnName.Enabled = true;
                    btnName.ForeColor = Color.FromArgb(255, 255, 255);
                    btnName.BackColor = Color.FromArgb(209, 17, 65);
                }
            }
        }

        public static bool ValidateTextBoxUserPwd(MetroTextBox control)
        {
            if (control.Text.Length >= UserPwdMinLength)
            {
                return true;
            }

            return false;
        }

        public static bool ValidateTextBoxPhone(MetroTextBox control)
        {
            if (control.Text.Length >= PhoneMinLength)
            {
                return true;
            }

            return false;
        }

        public static bool ValidateTextBoxDiscount(MetroTextBox control)
        {
            string amountVal = control.Text;
            if (amountVal.Length >= 3 && Convert.ToInt32(amountVal) <= 100)
            {
                return true;
            }

            return false;
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                MailAddress ma = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string DateFormatPattern = "dd/MM/yyyy";
        public static string TimeFormatPattern = "dd/MM/yyyy hh:mm tt";

        public static bool IsValidDecimalNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                return false;
            }

            if (e.KeyChar != '\b')
            {
                //check for multiple dot
                var textBoxControl = (MetroTextBox)sender;
                string controlText = textBoxControl.Text;
                if (e.KeyChar == '.' && controlText.IndexOf('.') > -1)
                {
                    return false;
                }

                //allow only two decimal 
                if (controlText.IndexOf('.') > -1)
                {
                    string[] wholeTxt = controlText.Split('.');
                    if (wholeTxt[1].Length >= 2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
