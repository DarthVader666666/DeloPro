using System.Text.Json;
using System.Text.Json.Serialization;

using Delopro.Data.Interfaces;
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
            using var scope = rootServiceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var visitRepository = serviceProvider.GetRequiredService<IRepository<Visit>>();
            var visitorRepository = serviceProvider.GetRequiredService<IRepository<Visitor>>();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");

            Location? location = new();

            if (response.IsSuccessStatusCode)
            {
                var jsonPayload = await response.Content.ReadAsStringAsync();
                location = JsonSerializer.Deserialize<Location>(jsonPayload);
            }

            var visitor = await visitorRepository.FindByAsync(ipAddress);

            visitor ??= await visitorRepository.CreateAsync(
                new Visitor
                {
                    IpAddress = ipAddress,
                    Country = location?.Country,
                    City = location?.City
                }
            );

            var visit = new Visit
            {
                Url = url,
                VisitorId = visitor?.VisitorId,
                VisitDate = DateTime.Now
            };

            await visitRepository.CreateAsync(visit);
        }
    }
}
