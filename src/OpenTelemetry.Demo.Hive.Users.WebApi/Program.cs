using Microsoft.Extensions.DependencyInjection;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
using SB.Community.Hive;
using System;

namespace OpenTelemetry.Demo.Hive.Users.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hiveHost = ServiceManager.Run<CommunityServiceProvider>();

            while (true) ;
            
            hiveHost?.Dispose();
        }
    }
}
