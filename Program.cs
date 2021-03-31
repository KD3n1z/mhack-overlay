using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MHackOverlay
{
    public static class Program
    {
        public static List<Extension> exts = new List<Extension>();
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }


        public static void LoadExts()
        {
            foreach(Extension e in exts)
            {
                e.End();
            }
            exts = new List<Extension>();
            string extsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Shu", "Overlay", "exts");
            if (Directory.Exists(extsFolder))
            {
                foreach (string f in Directory.GetFiles(extsFolder))
                {
                    if (f.EndsWith(".exe"))
                    {
                        exts.Add(new Extension(f));
                    }
                }
            }
        }
    }

    public class Extension
    {
        public class Arg
        {
            public string Name;
            public string Value;

            public Arg(string n, string v)
            {
                Name = n;
                Value = v;
            }
        }
        string title;
        public string Title
        {
            get
            {
                return title;
            }
        }
        public readonly string Path;
        Process process;
        List<Arg> output = new List<Arg>();
        public Arg[] Output
        {
            get
            {
                return output.ToArray();
            }
        }

        public Extension(string path)
        {
            Path = path;
            process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.Unicode;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
        }

        public void End()
        {
            process.CancelOutputRead();
            process.Kill();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data.StartsWith("title:"))
                {
                    title = e.Data.Remove(0, "title:".Count());
                }
                else
                {
                    string[] args = e.Data.Split(new char[] { ':' }, 2, StringSplitOptions.None);
                    foreach (Arg arg in output)
                    {
                        if (arg.Name == args[0])
                        {
                            arg.Value = args[1];
                            return;
                        }
                    }
                    output.Add(new Arg(args[0], args[1]));
                }
            }
            catch
            {

            }
        }
    }
}
