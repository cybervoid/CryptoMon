using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace CryptoService
{
    class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<Service.CryptoCompareHandler>(s =>
                {
                    s.ConstructUsing(name => new Service.CryptoCompareHandler());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("CryptoMon Service Host");
                x.SetDisplayName("CryptoMon");
                x.SetDisplayName("CryptoMon Service");
            });
        }
    }
}
