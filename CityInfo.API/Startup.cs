using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
	public class Startup
	{
		public static IConfigurationRoot Configuration;

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc()
				.AddMvcOptions(o => o.OutputFormatters.Add(
					new XmlDataContractSerializerOutputFormatter()));
			//.AddJsonOptions(o => {
			//	if (o.SerializerSettings.ContractResolver != null)
			//	{
			//		var castedResolver = o.SerializerSettings.ContractResolver
			//			as DefaultContractResolver;
			//		castedResolver.NamingStrategy = null;
			//	}
			//});
#if DEBUG
			services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
			var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
			services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

			services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoContext cityInfoContext)
		{
			loggerFactory.AddConsole();

			loggerFactory.AddDebug();

			//loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());

			loggerFactory.AddNLog();
			
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler();
			}

			cityInfoContext.EnsureSeeDataForContext();

			app.UseStatusCodePages();

			app.UseMvc();

			//app.UseMvc(config =>
			//{
			//	config.MapRoute(
			//		name: "Default",
			//		template: "{controller}/{action}/{id?}",
			//		defaults: new { controller = "Home", action = "Index" }
			//		);
			//});

			//app.Run((context) =>
			//{
			//	throw new Exception("Example exception");
			//});

			//app.Run(async (context) =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});
		}
	}
}


