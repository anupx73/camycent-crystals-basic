using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crystals
{
    public partial class NotifyMessage : Form
    {
        public event Action<object, EventArgs> HideParent;

        public NotifyMessage()
        {
            InitializeComponent();
            this.TopMost = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void ShowNotification(bool? type, string message, string title)
        {
            if (type == null)
            {
                picNotifyType.Image = Properties.Resources.info_note;
            }
            else if (type == true)
            {
                picNotifyType.Image = Properties.Resources.info_done;
            }
            else
            {
                picNotifyType.Image = Properties.Resources.info_warning;
            }
            labelTitle.Text = title;
            labelMessage.Text = message;
            this.ShowDialog();
        }

        private void lnkOk_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (HideParent != null)
            {
                HideParent(this, null);
            }
        }

        private void lnkCopyClipbrd_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("err: " + labelTitle.Text + "msg: " + labelMessage.Text);
            MessageBox.Show("Error message copied to clipboard. Paste it in appropiate place.", "Copied");
        }
    }
}
