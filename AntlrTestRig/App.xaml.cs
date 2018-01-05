using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AntlrTestRig
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TestRigCore _core;
        private AppArgs _args;

        protected override void OnStartup(StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            _args = AppArgHelper.ReadOptionsFromArgs(args);
            if (_args == null)
                ShowErrorAndExit(null);

            CalculateAbsolutTargetFolder();
            _core = new TestRigCore();// _proxyCreator.CreateProxyInNewDomain<TestRigCore>("newDomain");
            var input = AppArgHelper.ReadInputFromFileOrConsole(_args.InputFile, _args.Encoding);
            _core.LoadDlls(_args.Folder);
            _core.ProcessInput(input, _args);

            if (_args.InputFile != null)
            {
                var file = new FileInfo(_args.InputFile);
                FileSystemWatcher inputWatcher = new FileSystemWatcher(file.Directory.FullName, file.Name);
                inputWatcher.Changed += CreateWatcherEventHandler(null, 0.1f, false);
                inputWatcher.EnableRaisingEvents = true;
            }

            FileSystemWatcher dllWatcher = new FileSystemWatcher(_args.Folder, "*.dll");
            dllWatcher.Changed += CreateWatcherEventHandler("Reloading dlls in 1 second", 1, true);
            dllWatcher.EnableRaisingEvents = true;
        }

        private void CalculateAbsolutTargetFolder()
        {
            if (!string.IsNullOrEmpty(_args.Folder))
            {
                if (!Path.IsPathRooted(_args.Folder))
                {
                    _args.Folder = Path.Combine(Environment.CurrentDirectory, _args.Folder);
                }
            }
            else
            {
                _args.Folder = Environment.CurrentDirectory;
            }
        }

        private FileSystemEventHandler CreateWatcherEventHandler(string message, float sleepSecond, bool loadDll)
        {
            FileSystemEventHandler handler = (object sender, FileSystemEventArgs e) =>
            {
                var watcher = (FileSystemWatcher)sender;
                watcher.EnableRaisingEvents = false;

                //TODO: 
                //Some editors (npp or notepad) saves file twice, and the first time they save, the file may not be ready yet,
                //this is a ugly solution to an unly situation. Here it sleeps a random time, not too long to freeze the UI,
                //hopefully not to short so the 2nd save finishes.
                Thread.Sleep((int)(sleepSecond*1000));
                Console.WriteLine("[{1}] File change in '{0}'.", e.Name, DateTime.Now.ToString("hh:mm:ss"));
                if (message!=null)
                    Console.WriteLine(message);
                var input = AppArgHelper.ReadInputFromFileOrConsole(_args.InputFile, _args.Encoding);
                if (loadDll)
                    _core.LoadDlls(_args.Folder);
                this.Dispatcher.Invoke(() =>
                {
                    _core.ProcessInput(input, _args);
                    watcher.EnableRaisingEvents = true;
                });
            };

            return handler;
        }

        private void ShowErrorAndExit(string msg)
        {
           Console.WriteLine(msg);
           Environment.Exit(0);
        }
    }
}
