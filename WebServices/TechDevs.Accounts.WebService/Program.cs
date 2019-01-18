using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TechDevs.Gibson.WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSentry("https://ecb73d14982b4cd7b84f918af3b1d0b4@sentry.io/1370468")
                .Build();
    }
}