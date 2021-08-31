using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Votacao.View;
using SplashScreen = Votacao.View.SplashScreen;

namespace Votacao
{
    public partial class App : Application, ISingleInstanceApp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string Unique = "Votacao";

        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen ss = new SplashScreen();
            ss.Show();
            ss.VerifyDatabase();

            SetupExceptionHandling();

            base.OnStartup(e);

            if (ss.DatabaseExist)
            {
                Login login = new Login(ss);

                if (login.IsOK)
                    login.Show();
            }
            else 
            {
                ss.Close();
            }
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                log.Error(message, exception);
            }
        }

        [STAThread]
        public static void Main()
        {
            try
            {
                if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
                {
                    var application = new App();

                    application.InitializeComponent();
                    application.Run();

                    // Allow single instance code to perform cleanup operations
                    SingleInstance<App>.Cleanup();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }
    }
}
