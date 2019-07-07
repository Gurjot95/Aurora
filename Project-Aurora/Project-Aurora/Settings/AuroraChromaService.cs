using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Windows.Input;
using System.Reflection;
using Aurora.Utils;

namespace Aurora.Settings {
    public class AuroraChromaService : INotifyPropertyChanged {

        /// <summary>The name of the AuroraChroma service.</summary>
        private const string SERVICE_NAME = "AuroraChroma";
        //private const string EXEPATH = "\\"+ SERVICE_NAME;

        // Service manager singleton
        private static AuroraChromaService instance;
        public static AuroraChromaService Instance { get => instance ?? (instance = new AuroraChromaService()); }

        /// <summary>Controller bound to the AuroraChroma service</summary>
        private ServiceController controller = new ServiceController(SERVICE_NAME);

        // Property to indicate whether the service is currently running or not.
        private bool running;
        public bool Running { get => running; private set { running = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("")); } }
        public string RunningText => Running ? "Running" : "Stopped";

        // Commands to start/stop the service from the UI
        public ICommand StartStopCommand { get; }
        public ICommand InstallUninstallCommand { get; }

        /// <summary>Check if service exists</summary>
        public bool ServiceInstalled => ServiceController.GetServices().Any(s => s.ServiceName == SERVICE_NAME);
        public string ServiceInstalledText => ServiceInstalled ? "Installed" : "Not installed";

        /// <summary>Event that fires when the "Running" property changes state.</summary>
        public event PropertyChangedEventHandler PropertyChanged;


        // Ctor
        private AuroraChromaService() {
            StartStopCommand = new StartStopCommandImpl(this);
            InstallUninstallCommand = new InstallUninstallCommandImpl(this);
        }

        /// <summary>Method that should be called when Aurora initialises.</summary>
        public void Initialise() {
            // Get initial state of service
            try { Running = controller.Status == ServiceControllerStatus.Running; }
            catch { Running = false; }

            // If the user wishes to start the service on launch, do so
            if (Global.Configuration.AuroraChromaOnLaunch)
                Start();
        }

        /// <summary>
        /// Attempts to start the AuroraChroma service. Returns true if the service was able to start (or was already running).
        /// </summary>
        public bool Start() {
            try {
                if (controller.Status == ServiceControllerStatus.Running) return true;
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 15));
                return true;
            } catch (System.ServiceProcess.TimeoutException) {
                Global.logger.Error("Timeout while waiting for AuroraChroma service to start.");
            } catch (Exception ex) {
                Global.logger.Error("Error trying to start AuroraChroma service: " + ex.Message);
            } finally {
                Running = controller.Status == ServiceControllerStatus.Running;
            }
            return false;
        }

        /// <summary>
        /// Attempts to stop the AuroraChroma service. Returns true if the service was able to stop (or was already stopped).
        /// </summary>
        public bool Stop() {
            try {
                if (controller.Status == ServiceControllerStatus.Stopped) return true;
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 15));
                return true;
            } catch (System.ServiceProcess.TimeoutException) {
                Global.logger.Error("Timeout while waiting for AuroraChroma service to stop.");
            } catch (Exception ex) {
                Global.logger.Error("Error trying to stop AuroraChroma service: " + ex.Message);
            } finally {
                Running = controller.Status == ServiceControllerStatus.Running;
            }
            return false;
        }

        /// <summary>Install or uninstalls the AuroraChroma service.</summary>
        private void InstallUninstallService(bool install)
        {
            ManagedInstallerClass.InstallHelper((install ? new string[0] : new[] { "/u" }).Concat(new[] { "/LogFile=", "/LogToConsole=true", AppDomain.CurrentDomain.BaseDirectory + SERVICE_NAME + ".exe" }).ToArray());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }


        /// <summary>
        /// Class that can be used to create commands to start/stop the service.
        /// </summary>
        class StartStopCommandImpl : ICommand {

            public StartStopCommandImpl(AuroraChromaService inst) {
                inst.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter) => Instance.Running ^ bool.Parse(parameter.ToString());

            public void Execute(object parameter) {
                if (bool.Parse(parameter.ToString())) Instance.Start();
                else Instance.Stop();
            }
        }

        /// <summary>
        /// Class that can be used to create commands to install/uninstall the service
        /// </summary>
        class InstallUninstallCommandImpl : ICommand {

            public event EventHandler CanExecuteChanged;

            public InstallUninstallCommandImpl(AuroraChromaService inst) {
                inst.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            public bool CanExecute(object parameter) => Instance.ServiceInstalled ^ bool.Parse(parameter.ToString());
            public void Execute(object parameter) => Instance.InstallUninstallService(bool.Parse(parameter.ToString()));
        }
    }
}
