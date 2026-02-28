using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.GoogleService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicatiGoogleServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
