using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [Authorize(Roles = "Owner,Admin")]
    public class VisitsController: ControllerBase
    {
        private readonly IRepository<Visit> _visitRepository;

        public VisitsController(IRepository<Visit> visitRepository)
        {
            _visitRepository = visitRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetVisits([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var nowDate = DateTime.Now;

            if (fromDate is null)
            {
                var date = nowDate.AddDays(-30);
                fromDate = new DateTime(date.Year, date.Month, date.Day);
            }
            else
            {
                fromDate = (DateTime)fromDate;
            }

            if (toDate is null)
            {
                toDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day);
            }
            else
            {
                toDate = (DateTime)toDate;
            }

            var allDates = Enumerable.Range(0, (toDate - fromDate).Value.Days + 1).Select(i => fromDate.Value.AddDays(i)).ToList();

            var visits = await _visitRepository.GetRangeAsync(fromDate, toDate);

            var groupedByDate = visits.GroupBy(v => v!.VisitDate.Date).ToDictionary(g => g.Key, g => g.ToList());

            var actionCounts = new List<int>(); 
            var uniqueVisitorCounts = new List<int>(); 
            var labels = new List<string>(); 
            
            foreach (var date in allDates) 
            { 
                if (groupedByDate.TryGetValue(date, out var dayVisits)) 
                { 
                    actionCounts.Add(dayVisits.Count);
                    uniqueVisitorCounts.Add(dayVisits.GroupBy(v => v!.VisitorId).Count()); 
                } 
                else 
                { 
                    actionCounts.Add(0);
                    uniqueVisitorCounts.Add(0); 
                } 
                
                labels.Add(date.ToString("dd.MM")); 
            }

            var visitResponse = new VisitResponse 
            { 
                Labels = labels.ToArray(), 
                Datasets = 
                [
                    new VisitDataset 
                    {
                        Label = "Действия", 
                        Data = actionCounts.ToArray(),
                        BorderColor = "rgb(170,50,50)"
                    },
                    new VisitDataset 
                    {
                        Label = "Посетители", 
                        Data = uniqueVisitorCounts.ToArray(), 
                        BorderColor = "rgb(20,100,20)" 
                    }
                ] 
            };

            return Ok(visitResponse);
        }
    }
}
