using System.Text.Json;
using System.Text.Json.Serialization;

using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Delopro.Server.Attributes
{
    public class Location
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TrackIpAddressAttribute : ActionFilterAttribute
    {        
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            var httpContext = context.HttpContext;
            var rootServiceProvider = httpContext.RequestServices;
            var url = httpContext.Request.Path;
            var ipAddress = httpContext.Connection?.RemoteIpAddress?.ToString();

            await next();
            _ = TrackIpAddressAsync(rootServiceProvider, url, ipAddress);
        }

        private static async Task TrackIpAddressAsync(IServiceProvider rootServiceProvider, string url, string? ipAddress)
        {
            string[] AdminIpAddresses = ["37.214.25.23", "46.216.112.76"];

            using var scope = rootServiceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var visitRepository = serviceProvider.GetRequiredService<IRepository<Visit>>();
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");

            Location? location = new();

            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync();
                location = JsonSerializer.Deserialize<Location>(r);
            }

            var visit = new Visit
            {
                UserId = AdminIpAddresses.Contains(ipAddress) ? 2 : null,
                IpAddress = ipAddress,
                Url = url,
                Country = location?.Country,
                City = location?.City,
                VisitDate = DateTime.Now
            };

            await visitRepository.CreateAsync(visit);
        }
    }
}
