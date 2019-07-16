using System;
using System.Collections.Generic;
using System.IO;
using Cleaner.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cleaner.Core
{

    public static class RunContainer
    {
        public static IServiceProvider Build<T>(Action<IServiceCollection> p) where T : class, IRunner
        {
            IServiceCollection ServiceCollection = new ServiceCollection();

            ServiceCollection.AddSingleton<IRunner, T>();
            //ServiceCollection.AddSingleton<IServiceCollection>(ServiceCollection);

            p(ServiceCollection);

            return ServiceCollection.BuildServiceProvider();
        }
    }
}
