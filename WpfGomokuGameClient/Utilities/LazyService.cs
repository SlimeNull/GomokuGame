using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WpfGomokuGameClient.Utilities
{
    public class LazyService<TService> where TService : class
    {
        private Lazy<TService> laziedService;

        public TService Service => laziedService.Value;

        public LazyService(IServiceProvider services)
        {
            laziedService = new Lazy<TService>(() => services.GetRequiredService<TService>());
        }
    }
}
