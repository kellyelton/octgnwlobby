using System;
using System.Reflection;
using System.Windows;
using Octgn.Lobby;

namespace Octgn
{
    public partial class OctgnApp : Application
    {
        internal const string ClientName = "OCTGN.NET";
        internal static readonly Version OctgnVersion = GetClientVersion();
        internal static readonly Version BackwardCompatibility = new Version(0, 2, 0, 0);

        private static Version GetClientVersion()
        {
            Assembly asm = typeof(OctgnApp).Assembly;
            AssemblyProductAttribute at = (AssemblyProductAttribute)asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
            return asm.GetName().Version;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args)
                {
                    Exception ex = args.ExceptionObject as Exception;
                    var wnd = new ErrorWindow(ex);
                    wnd.ShowDialog();
                    ErrorLog.WriteError(ex, "Unhandled Exception main", false);
                    ErrorLog.CheckandUpload();
                    MessageBox.Show("Uploaded error log.");
                };
            AppDomain.CurrentDomain.ProcessExit += delegate(object sender, EventArgs ea)
            {
                ErrorLog.CheckandUpload();
                MessageBox.Show("Uploaded error log.");
            };
            Updates.PerformHouskeeping();

            Program.GamesRepository = new Octgn.Data.GamesRepository();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Fix: this can happen when the user uses the system close button.
            // If a game is running (e.g. in StartGame.xaml) some threads don't
            // stop (i.e. the database thread and/or the networking threads)
            if (Program.IsGameRunning) Program.StopGame();
            ErrorLog.CheckandUpload();
            MessageBox.Show("Uploaded error log.");
            base.OnExit(e);
        }
    }
}