using Company.WebApi;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                            .UseIIS()
                            .UseIISIntegration();
            })
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);

                if (hostContext.HostingEnvironment.IsDevelopment())
                {
                    config.AddUserSecrets<Program>();
                }
                config.AddEnvironmentVariables();
            })
            .UseSerilog((hostContext, configuration) => configuration.ReadFrom.Configuration(hostContext.Configuration));
    }
}