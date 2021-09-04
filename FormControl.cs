using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyExtension
{
    /// <summary>
    /// 重置ProgressBar，透過Text成員，可在ProgressBar上顯示文字
    /// </summary>
    public class MyProgressBar : ProgressBar
    {
        /// <summary>
        /// 重置ProgressBar，透過Text成員，可在ProgressBar上顯示文字
        /// </summary>
        public MyProgressBar()
        {
        }

        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private Color _TextColor = Color.Black;
        private Font _TextFont = new Font("微軟正黑體", 12);

        /// <summary>
        /// 文字顏色
        /// </summary>
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; this.Invalidate(); }
        }

        /// <summary>
        /// 文字字型
        /// </summary>
        public Font TextFont
        {
            get { return _TextFont; }
            set { _TextFont = value; this.Invalidate(); }
        }

        /// <summary>
        /// 物件重繪時觸發
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xf || m.Msg == 0x133)
            {
                // 0xf:要求一個窗口重畫自己
                // 0x133:當一個編輯型控件將要被繪制時發送此消息給它的父窗口；通過響應這條消息，
                //       所有者窗口可以通過使用給定的相關顯示設備的句柄來設置編輯框的文本和背景顏色
                IntPtr hDC = GetWindowDC(m.HWnd);
                if (hDC.ToInt32() == 0)
                {
                    return;
                }

                //base.OnPaint(e);
                Graphics g = Graphics.FromHdc(hDC);
                SolidBrush brush = new SolidBrush(_TextColor);
                //輸入文字
                string s = Text;
                SizeF size = g.MeasureString(s, _TextFont);
                float x = (this.Width - size.Width) / 2;
                float y = (this.Height - size.Height) / 2;
                g.DrawString(s, _TextFont, brush, x, y);
                //返回結果
                m.Result = IntPtr.Zero;
                //釋放
                ReleaseDC(m.HWnd, hDC);
            }
        }

    }
}
