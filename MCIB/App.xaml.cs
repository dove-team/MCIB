using MCIB.Lang;
using MCIB.Libs;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MCIB
{
    public partial class App : Application
    {
        public static bool IsAdministrator { get; private set; }
        public App() : base()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            IsAdministrator = !principal.IsInRole(WindowsBuiltInRole.Administrator);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var applicationName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
            using (var mutex = new Mutex(true, applicationName, out bool createNew))
            {
                if (createNew)
                {
                    base.OnStartup(e);
                    LangManager.Instance.Init();
                }
                else
                {
                    MessageBox.Show("程序已经启动了！");
                    Current.Shutdown();
                }
                mutex.ReleaseMutex();
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            HookManager.Instance.EnableTaskManager();
            base.OnExit(e);
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show("出现未知的错误！" + ex.Message);
                LogManager.Instance.LogError("UnhandledException", ex);
            }
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show("出现未知的错误！" + e.Exception.Message);
            LogManager.Instance.LogError("UnobservedTaskException", e.Exception);
        }
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("出现未知的错误！" + e.Exception.Message);
            LogManager.Instance.LogError("DispatcherUnhandledException", e.Exception);
        }
    }
}