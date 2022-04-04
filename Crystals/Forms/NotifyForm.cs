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
    public partial class NotifyForm : Form
    {
        private NotifyMessage notifyMessage = null;
        public event Action<object, EventArgs> MakeActiveParent;

        public NotifyForm()
        {
            InitializeComponent();
            this.TopMost = false;
            notifyMessage = new NotifyMessage();
            notifyMessage.HideParent += notifyMessage_HideParent;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void notifyMessage_HideParent(object arg1, EventArgs arg2)
        {
            this.Hide();
            if (MakeActiveParent != null)
            {
                MakeActiveParent(this, null);
            }
        }

        public void ShowNotification(bool? type, string message, string title)
        {
            this.Show();
            notifyMessage.ShowNotification(type, message, title);
        }
    }
}
