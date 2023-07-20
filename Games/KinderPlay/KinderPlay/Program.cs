using KinderPlay.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace KinderPlay
{
    public class Program
    {
        private static IConfiguration configuration;

        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json".ToAppPath(), false);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                configBuilder.AddJsonFile("appsettings.Development.json".ToAppPath(), true);
            }

            configuration = configBuilder.Build();

            var host = CreateWebHostBuilder(args);
            if (configuration.GetValue<bool>("LetsEncrypt:Enabled"))
            {
                host.UseLetsEncrypt(o =>
                {
                    o.AcceptTermsOfService = true;
                    o.EmailAddress = configuration["LetsEncrypt:EmailAddress"];
                    o.HostNames = new string[] { configuration["LetsEncrypt:HostName"] };
                    o.UseStagingServer = configuration.GetValue<bool>("LetsEncrypt:UseStagingServer");
                });
            }

            host.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(configuration["Urls"]);
    }
}