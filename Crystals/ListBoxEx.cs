using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Crystals
{
    public partial class ListBoxEx : ListBox
    {
        public ListBoxEx()
        {
            InitializeComponent();
            SetupFont();
            bodyFont = m_Font_Gothic;
        }

        private Font bodyFont;

        //Custom font
        #region P/Invoke Imports
        [DllImport("gdi32")]
        internal static extern IntPtr AddFontMemResourceEx(IntPtr pbFont,
                                                           uint cbFont,
                                                           IntPtr pdv,
                                                           [In] ref uint pcFonts);
        [DllImport("gdi32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RemoveFontMemResourceEx(IntPtr fh);
        #endregion

        private PrivateFontCollection m_PFC;
        private GCHandle m_pFont;
        private IntPtr m_hFont;
        private Font m_Font_Gothic;

        private void SetupFont()
        {
            m_PFC = new PrivateFontCollection();

            //Load Data
            int rsxLen = Properties.Resources.GOTHIC.Length;
            m_pFont = GCHandle.Alloc(Properties.Resources.GOTHIC, GCHandleType.Pinned);
            m_PFC.AddMemoryFont(m_pFont.AddrOfPinnedObject(), rsxLen);
            uint rsxCnt = 1;

            //Register font
            m_hFont = AddFontMemResourceEx(m_pFont.AddrOfPinnedObject(),
                                                 (uint)rsxLen, IntPtr.Zero, ref rsxCnt);

            //Create font
            FontFamily ff = m_PFC.Families[0];
            if (ff.IsStyleAvailable(FontStyle.Regular))
            {
                m_Font_Gothic = new Font(ff, 9f, FontStyle.Regular, GraphicsUnit.Point);
            }
            else
            {
                MessageBox.Show("Font error, something went terribly wrong...");
                m_Font_Gothic = DefaultFont;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // prevent from error Visual Designer
            if (this.Items.Count > 0)
            {
                Color colorSelect = Color.FromArgb(238, 238, 238);
                Color colorNormal = Color.FromArgb(255, 255, 255);

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(colorSelect), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(colorNormal), e.Bounds);
                }

                // draw some item separator
                e.Graphics.DrawLine(Pens.WhiteSmoke, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

                // calculate bounds for details text drawing
                e.Graphics.DrawString(this.Items[e.Index].ToString(), bodyFont, Brushes.Black, e.Bounds);
            }
        }

        
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void Dispose(bool disposing)
        {
            #region Release Managed
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            #endregion

            #region Release Unmanaged
            m_PFC.Dispose();
            m_pFont.Free();
            RemoveFontMemResourceEx(m_hFont);
            #endregion

            base.Dispose(disposing);
        }
    }
}
