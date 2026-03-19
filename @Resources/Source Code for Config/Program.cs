using System;
using System.Windows.Forms;

namespace RainConfigApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                string CURRENTCONFIG = args.Length > 0 ? args[0] : "";
                string CURRENTPATH = args.Length > 1 ? args[1] : "";

                if (!string.IsNullOrEmpty(CURRENTPATH))
                {
                    CURRENTPATH = CURRENTPATH.Trim().Trim('"');
                    CURRENTPATH = CURRENTPATH.TrimEnd('\\');
                }
                string CURRENTFILE = args.Length > 2 ? args[2] : "";
                string PROGRAMPATH = args.Length > 3 ? args[3] : "";
                int? CURRENTCONFIGX = args.Length > 4 ? int.Parse(args[4]) : (int?)null;
                int? CURRENTCONFIGY = args.Length > 5 ? int.Parse(args[5]) : (int?)null;

                Application.Run(new MainForm(
                    CURRENTCONFIG,
                    CURRENTPATH,
                    CURRENTFILE,
                    PROGRAMPATH,
                    CURRENTCONFIGX,
                    CURRENTCONFIGY));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Startup Error");
            }
        }
    }
}