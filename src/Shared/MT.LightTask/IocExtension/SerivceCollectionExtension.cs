using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.LightTask
{
    public static class SerivceCollectionExtension
    {
        public static IServiceCollection AddLightTasks(this IServiceCollection services)
        {
            services.AddSingleton<TaskFactory>();
            return services;
        }
    }
}
