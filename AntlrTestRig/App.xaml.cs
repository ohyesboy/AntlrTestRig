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
        private TestRigCore core  = new TestRigCore();
        private AppArgs appArg;


        private string dllDir;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            appArg = AppArgHelper.ReadOptionsFromArgs(args);
            if (appArg == null)
                ShowErrorAndExit(null);

            dllDir = Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(appArg.TargetFolder))
            {
                if (!Path.IsPathRooted(appArg.TargetFolder))
                {
                    dllDir = Path.Combine(dllDir, appArg.TargetFolder);
                }
                else
                {
                    dllDir = appArg.TargetFolder;
                }
            }
            core.Input = AppArgHelper.ReadInputFromFileOrConsole(appArg.InputFile, appArg.Encoding);
            core.LoadDll(dllDir);
            core.ParseAndShowResult(appArg);

            if (appArg.InputFile != null)
            {
                var file = new FileInfo(appArg.InputFile);
                FileSystemWatcher inputWatcher = new FileSystemWatcher(file.Directory.FullName, file.Name);
                inputWatcher.Changed += CreateWatcherEventHandler(null, 0.1f, true, false);
                inputWatcher.EnableRaisingEvents = true;
            }

            FileSystemWatcher dllWatcher = new FileSystemWatcher(dllDir, "*.dll");
            dllWatcher.Changed += CreateWatcherEventHandler("Reloading dlls in 1 second", 1, false, true);
            dllWatcher.EnableRaisingEvents = true;
        }

        private FileSystemEventHandler CreateWatcherEventHandler(string message, float sleepSecond, bool loadInput, bool loadDll)
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
                if(loadInput)
                    core.Input = AppArgHelper.ReadInputFromFileOrConsole(appArg.InputFile, appArg.Encoding);
                if (loadDll)
                    core.LoadDll(dllDir);
                this.Dispatcher.Invoke(() =>
                {
                    core.ParseAndShowResult(appArg);
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
