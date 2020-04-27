using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.Models;
using RedisRepo;
using StackExchange.Redis;

namespace Redis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<EmployeeRespository, EmployeeRespository>();
            services.AddTransient<RedisContext<Employee>, RedisContext<Employee>>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true"));
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConnectionMultiplexer connectionMultiplexer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            connectionMultiplexer.GetServer("127.0.0.1:6379").FlushAllDatabases();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
