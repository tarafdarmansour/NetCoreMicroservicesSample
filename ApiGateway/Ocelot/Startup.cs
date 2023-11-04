using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace ApiGateway;

public class Startup
{
    private readonly IConfigurationRoot Configuration;

    public Startup(IWebHostEnvironment env)
    {
        var ocelotJson = new JObject();
        foreach (var jsonFilename in Directory.EnumerateFiles("Configuration", "ocelot.*.json",
                     SearchOption.AllDirectories))
            using (var fi = File.OpenText(jsonFilename))
            {
                var json = JObject.Parse(fi.ReadToEnd());
                ocelotJson.Merge(json, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

        File.WriteAllText("ocelot.json", ocelotJson.ToString());

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("ocelot.json", false, true)
            .AddJsonFile($"ocelot.{env.EnvironmentName}.json",
                true, true)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                true, true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSwaggerForOcelot(Configuration)
            .AddOcelot(Configuration)
            .AddConsul()
            .AddConfigStoredInConsul();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();

        app.UseSwaggerForOcelotUI(opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });

        app
            .UseOcelot()
            .Wait();
    }
}