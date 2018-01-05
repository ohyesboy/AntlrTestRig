using System;
using System.Reflection;

namespace AntlrTestRig
{    
    /// <summary>
    /// A proxy live in a new domain can access loaded dlls and be unloaded along the new domain
    /// 
    /// This class creates a new domain and a proxy that lives in that domain.
    /// </summary>
    class ProxyCreator
    {
        private AppDomain dom;
        public TProxy CreateProxyInNewDomain<TProxy>(string domainName)
        {
            if (dom != null)
            {
                AppDomain.Unload(dom);
            }
            dom = AppDomain.CreateDomain(domainName);

            var proxy = (TProxy)dom.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(TProxy).FullName);
            return proxy;
        }

 
    }
}