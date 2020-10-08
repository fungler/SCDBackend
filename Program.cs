using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using System;


namespace SCDBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            HttpClient c = new HttpClient();
            var a = await c.GetAsync("https://localhost:7001/api/home");
            Console.Write(a.Content);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
