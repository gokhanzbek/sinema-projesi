using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieTicketAPI.Application
{
    public static class ServiceRegistration
    {
        // IServiceCollection'ı genişletiyoruz (Extension method)
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // MediatR'ı burada, tam da ait olduğu Application katmanında kaydediyoruz!
            // Assembly.GetExecutingAssembly() demek "Şu an içinde bulunduğum projeyi tara" demektir.
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }
    }
}
