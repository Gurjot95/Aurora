using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Windows.Input;
using System.Reflection;

namespace Aurora.Settings {
    public class AuroraChromaService : INotifyPropertyChanged {

        /// <summary>The name of the AuroraChroma service.</summary>
        private const string SERVICE_NAME = "AuroraChroma";
        //private const string EXEPATH = "\\"+ SERVICE_NAME;

        // Service manager singleton
        private static AuroraChromaService instance;
        public static AuroraChromaService Instance { get => instance ?? (instance = new AuroraChromaService()); }

        // Check if service exists
        private static bool isServiceExist()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == SERVICE_NAME);
        }

        //install service
        private static void installService()
        {
            ManagedInstallerClass.InstallHelper(new[] { "/LogFile=", "/LogToConsole=true", AppDomain.CurrentDomain.BaseDirectory + SERVICE_NAME+".exe" });
        }

        private static void uninstallService()
        {
            ManagedInstallerClass.InstallHelper(new[] { "/u", "/LogFile=", "/LogToConsole=true", AppDomain.CurrentDomain.BaseDirectory + SERVICE_NAME+".exe" });
        }


        /// <summary>Controller bound to the AuroraChroma service</summary>
        private ServiceController controller = new ServiceController(SERVICE_NAME);

        // Property to indicate whether the service is currently running or not.
        private bool running;
        public bool Running { get => running; private set { running = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("")); } }
        public string RunningText => Running ? "Running" : "Stopped";

        // Commands to start/stop the service from the UI
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }


        private AuroraChromaService() {
            StartCommand = new StartStopCommand(this, true);
            StopCommand = new StartStopCommand(this, false);
        }

        /// <summary>Method that should be called when Aurora initialises.</summary>
        public void Initialise() {
            if (!isServiceExist())
            {
                installService();
            }
            // Get initial state of service
            Running = controller.Status == ServiceControllerStatus.Running;

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

        /// <summary>Event that fires when the "Running" property changes state.</summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Class that can be used to create commands to start/stop the service.
        /// </summary>
        class StartStopCommand : ICommand {

            private bool isStartCommand;

            public StartStopCommand(AuroraChromaService inst, bool isStartCommand) {
                this.isStartCommand = isStartCommand;
                inst.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter) => Instance.Running ^ isStartCommand;

            public void Execute(object parameter) {
                if (isStartCommand) Instance.Start();
                else Instance.Stop();
            }
        }
    }
}
