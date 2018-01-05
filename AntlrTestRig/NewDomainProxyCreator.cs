using System;
using System.Reflection;

namespace AntlrTestRig
{
    class NewDomainProxyCreator
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