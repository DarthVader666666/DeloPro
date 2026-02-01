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
        private readonly string[] AdminIpAddresses = ["37.214.25.23", "46.216.112.76"];

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var visitRepository = httpContext.RequestServices.GetRequiredService<IRepository<Visit>>();

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://ipinfo.io/{ipAddress}/json");

            Location? location = new();

            if (response.IsSuccessStatusCode)
            {
                location = JsonSerializer.Deserialize<Location>(await response.Content.ReadAsStringAsync());
            }

            var visit = new Visit
            {
                UserId = AdminIpAddresses.Contains(ipAddress) ? 2 : null,
                IpAddress = ipAddress,
                Url = httpContext.Request.Path,
                Country = location?.Country,
                City = location?.City,
                VisitDate = DateTime.Now
            }; 
            
            await visitRepository.CreateAsync(visit); 
            
            await next(); 
        }
    }
}
