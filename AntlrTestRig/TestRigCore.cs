using System.Linq;
using System.Security.Policy;

namespace AntlrTestRig
{
    class TestRigCore
    {
        private MainWindow _window;
        private ProxyCreator _proxyCreator = new ProxyCreator();
        private Proxy _proxy;

        public void LoadDlls(string dllDir)
        {
            _proxy = _proxyCreator.CreateProxyInNewDomain<Proxy>("newDomain");
            _proxy.LoadDlls(dllDir);
        }


        public void ProcessInput(string inputText, AppArgs args)
        {
            var output = _proxy.ProcessInput(inputText, args);
            if (args.ShowGui)
            {
                if (_window == null)
                {
                    _window = new MainWindow();
                    _window.Show();
                }
                _window.UpdateModel(output.Model);
                _window.lbLastContext.Content = string.Join(" < ", output.LastContextNameStack);
            }
        }


    }
}