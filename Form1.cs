using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MHackOverlay
{
    public partial class Form1 : Form
    {
        DateTime timerStop = new DateTime();
        DateTime timerStart = new DateTime();
        int timerState = 0;
        bool isNlPressed = IsKeyLocked(Keys.NumLock);
        string sPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu");
        string fsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "font.txt");
        string osPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "outlineSize.txt");
        string dsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "delay.txt");
        string mCPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "mColor.txt");
        string tCPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "tColor.txt");
        string olCPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "otColor.txt");
        string lsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "L.txt");
        string rsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "R.txt");
        string ssPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "space.txt");
        Color tColor = Color.FromArgb(1,1,1);
        Color olColor = Color.FromArgb(0,0,0);
        Color mColor = Color.FromArgb(255,255,255);
        Point mouseOffset;
        bool mouseDown = false;
        bool leftMouseDown = false;
        bool rightMouseDown = false;
        int delay = 250;
        int Lcps = 0;
        int Rcps = 0;
        string fontString = "";

        public async void UpdateText()
        {
            while (true)
            {
                text.Text = formattedText(toolStripTextBox1.Text);
                Width = text.Width;
                Height = text.Height;
                await Task.Delay(delay);
                
            }
        }

        public void UpdateColors()
        {
            text.ForeColor = mColor;
            text.OutlineForeColor = olColor;
            TransparencyKey = tColor;
            BackColor = tColor;
        }

        private string formattedText(string text)
        {
            ComputerInfo myCompInfo = new ComputerInfo();
            string eTxt = text;

            foreach(Extension ext in Program.exts)
            {
                foreach(Extension.Arg arg in ext.Output)
                {
                    eTxt = eTxt.Replace("%" + arg.Name, arg.Value);
                }
            }

            return eTxt
                .Replace("%freerammb", ((int)myCompInfo.AvailablePhysicalMemory / 1000000).ToString())
                .Replace("%freeramkb", ((int)myCompInfo.AvailablePhysicalMemory / 1000).ToString())
                .Replace("%line", "\n")
                .Replace("%rmb", rightMouseDown.ToString()
                .Replace("True", toolStripTextBox5.Text)
                .Replace("False", toolStripTextBox6.Text))
                .Replace("%lmb", leftMouseDown.ToString()
                .Replace("True", toolStripTextBox4.Text)
                .Replace("False", toolStripTextBox6.Text))
                .Replace("%lcps", Lcps.ToString())
                .Replace("%rcps", Rcps.ToString())
                .Replace("%fixedrcps", fixedCps(Rcps.ToString(), 2))
                .Replace("%fixedlcps", fixedCps(Lcps.ToString(), 2))
                .Replace("%space", " ")
                .Replace("%percent", "%")
                .Replace("%timer", getTimer());
        }

        private string getTimer()
        {
            switch (timerState)
            {
                default:
                    return "";
                case 1:
                    return dts(DateTime.Now - timerStart);
                case 2:
                    return dts(timerStop - timerStart);

            }
        }

        private string dts(TimeSpan ts)
        {
            string hours = ts.Hours.ToString();
            string mins = ts.Minutes.ToString();
            string secs = ts.Seconds.ToString();
            string millis = ts.Milliseconds.ToString();

            hours = fixedCps(hours, 2, '0', true);
            mins = fixedCps(mins, 2, '0', true);
            secs = fixedCps(secs, 2, '0', true);
            millis = fixedCps(millis, 3, '0', true);

            return hours + ":" + mins + ":" + secs + ":" + millis;
        }

        public Form1()
        {
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
                sPath = Path.Combine(sPath, "Overlay");
                Directory.CreateDirectory(sPath);
            }
            else
            {
                sPath = Path.Combine(sPath, "Overlay");
            }
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            Program.LoadExts();
            sPath = Path.Combine(sPath, "text.txt");
            InitializeComponent();
            Width = text.Width;
            Height = text.Height;
            TopMost = true;
            ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            try
            {
                if (File.Exists(sPath))
                {
                    StreamReader sr = new StreamReader(sPath);
                    toolStripTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(tCPath))
                {
                    StreamReader sr = new StreamReader(tCPath);
                    ColorConverter cc = new ColorConverter();
                    tColor = (Color)cc.ConvertFromString(sr.ReadToEnd());
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(olCPath))
                {
                    StreamReader sr = new StreamReader(olCPath);
                    ColorConverter cc = new ColorConverter();
                    olColor = (Color)cc.ConvertFromString(sr.ReadToEnd());
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(mCPath))
                {
                    StreamReader sr = new StreamReader(mCPath);
                    ColorConverter cc = new ColorConverter();
                    mColor = (Color)cc.ConvertFromString(sr.ReadToEnd());
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(fsPath))
                {
                    StreamReader sr = new StreamReader(fsPath);
                    fontString = sr.ReadToEnd();
                    sr.Close();

                    FontConverter cvt = new FontConverter();
                    text.Font = cvt.ConvertFromString(fontString) as Font;
                }
            } catch { }
            try
            {
                if (File.Exists(dsPath))
                {
                    StreamReader sr = new StreamReader(dsPath);
                    toolStripTextBox3.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(lsPath))
                {
                    StreamReader sr = new StreamReader(lsPath);
                    toolStripTextBox4.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(rsPath))
                {
                    StreamReader sr = new StreamReader(rsPath);
                    toolStripTextBox5.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(ssPath))
                {
                    StreamReader sr = new StreamReader(ssPath);
                    toolStripTextBox6.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }
            try
            {
                if (File.Exists(osPath))
                {
                    StreamReader sr = new StreamReader(osPath);
                    toolStripTextBox7.Text = sr.ReadToEnd();
                    sr.Close();
                }
            } catch { }

            int.TryParse(toolStripTextBox3.Text, out delay);
            UpdateText();
            UpdateColors();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timerStart = DateTime.Now;
            BackColor = tColor;
            TransparencyKey = tColor;
            timer1.Start();
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void text_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
            }
        }

        private void text_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point mPos = Control.MousePosition;
                mPos.Offset(mouseOffset);
                Location = mPos;
            }
        }

        private void text_MouseDown(object sender, MouseEventArgs e)
        {
            int xOff;
            int yOff;

            if (e.Button == MouseButtons.Left)
            {
                xOff = -e.X - SystemInformation.FrameBorderSize.Width;
                yOff = -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height;

                mouseOffset = new Point(xOff, yOff);

                mouseDown = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(Extension ext in Program.exts)
            {
                ext.End();
            }
            Application.Exit();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(sPath);
            sw.Write(toolStripTextBox1.Text);
            sw.Close();
            StreamWriter sw2 = new StreamWriter(lsPath);
            sw2.Write(toolStripTextBox4.Text);
            sw2.Close();
            StreamWriter sw3 = new StreamWriter(rsPath);
            sw3.Write(toolStripTextBox5.Text);
            sw3.Close();
            StreamWriter sw4 = new StreamWriter(ssPath);
            sw4.Write(toolStripTextBox6.Text);
            sw4.Close();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontConverter cvt = new FontConverter();
            fontString = cvt.ConvertToString(text.Font);

            StreamWriter sw = new StreamWriter(fsPath);
            sw.Write(fontString);
            sw.Close();
            StreamWriter sw2 = new StreamWriter(osPath);
            sw2.Write(toolStripTextBox7.Text);
            sw2.Close();
        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(dsPath);
            sw.Write(toolStripTextBox3.Text);
            sw.Close();
        }

        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(toolStripTextBox3.Text, out delay);
            if(delay < 1)
            {
                delay = 1;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isNlPressed != IsKeyLocked(Keys.NumLock))
            {
                isNlPressed = IsKeyLocked(Keys.NumLock);
                ChangeTimerState();
            }

            switch (MouseButtons)
            {
                case MouseButtons.Left:
                    rightMouseDown = false;
                    if (!leftMouseDown)
                    {
                        Lclick();
                    }
                    leftMouseDown = true;
                    return;
                case MouseButtons.Right:
                    leftMouseDown = false;
                    if (!rightMouseDown)
                    {
                        Rclick();
                    }
                    rightMouseDown = true;
                    return;
                case MouseButtons.None:
                    leftMouseDown = false;
                    rightMouseDown = false;
                    return;
                default:
                    return;
            }
        }

        async void Lclick()
        {
            Lcps++;
            await (Task.Delay(1000));
            Lcps--;
        }
        async void Rclick()
        {
            Rcps++;
            await (Task.Delay(1000));
            Rcps--;
        }

        string fixedCps(string val, int symbols, char symbol = ' ', bool left = false)
        {
            if (val.Length >= symbols)
            {
                return val;
            }
            else
            {
                if (!left)
                {
                    return val + new string(symbol, symbols - val.Length);
                }
                else
                {
                    return new string(symbol, symbols - val.Length) + val;
                }
            }
        }

        private void toolStripTextBox7_TextChanged(object sender, EventArgs e)
        {
            int w = 2;
            int.TryParse(toolStripTextBox7.Text, out w);
            text.OutlineWidth = w;
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = text.Font;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                text.Font = fontDialog1.Font;
            }
        }

        private void text_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void timerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeTimerState();
        }

        public void ChangeTimerState()
        {
            switch (timerState)
            {
                case 0:
                    timerState = 1;
                    timerStart = DateTime.Now;
                    break;
                case 1:
                    timerState = 2;
                    timerStop = DateTime.Now;
                    break;
                case 2:
                    timerState = 0;
                    break;
            }
        }

        private void extensionsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ex x = new ex();
            x.ShowDialog();
        }

        private void outlineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = olColor;
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                olColor = colorDialog1.Color;
            }
            UpdateColors();
        }

        private void mainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = mColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                mColor = colorDialog1.Color;
            }
            UpdateColors();
        }

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = tColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                tColor = colorDialog1.Color;
            }
            UpdateColors();
        }

        private void saveToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ColorConverter cc = new ColorConverter();
            StreamWriter sw = new StreamWriter(mCPath);
            sw.Write(cc.ConvertToString(mColor));
            sw.Close();
            StreamWriter sw1 = new StreamWriter(tCPath);
            sw1.Write(cc.ConvertToString(tColor));
            sw1.Close();
            StreamWriter sw2 = new StreamWriter(olCPath);
            sw2.Write(cc.ConvertToString(olColor));
            sw2.Close();
        }
    }
}


public class MyLabel : Label
{
    private TextRenderingHint _textRenderingHint = TextRenderingHint.SystemDefault;
    private float _outlineWidth = 2;
    private Color _outlineForeColor = Color.FromArgb(0, 0, 0);

    public TextRenderingHint TextRenderingHint
    {
        get { return _textRenderingHint; }
        set { _textRenderingHint = value; }
    }
    public float OutlineWidth
    {
        get { return _outlineWidth; }
        set { _outlineWidth = value; }
    }
    public Color OutlineForeColor
    {
        get { return _outlineForeColor; }
        set { _outlineForeColor = value; }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.TextRenderingHint = _textRenderingHint;
        e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
        using (GraphicsPath gp = new GraphicsPath())
        using (Pen outline = new Pen(_outlineForeColor, _outlineWidth)
        { LineJoin = LineJoin.Round })
        using (StringFormat sf = new StringFormat())
        using (Brush foreBrush = new SolidBrush(ForeColor))
        {
            gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                Font.Size, ClientRectangle, sf);
            e.Graphics.ScaleTransform(1.3f, 1.35f);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawPath(outline, gp);
            e.Graphics.FillPath(foreBrush, gp);
        }
    }
}