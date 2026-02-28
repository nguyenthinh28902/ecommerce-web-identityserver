using Ecom.GoogleService.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.GoogleService.Services
{
    public class AuthGoogleService : IAuthGoogleService
    {
        private readonly ILogger<AuthGoogleService> _logger;
        public AuthGoogleService(ILogger<AuthGoogleService> logger) { 
            _logger = logger;
        }  
        
    }
}
