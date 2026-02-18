using AutoMapper;

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
        private readonly IMapper _mapper;

        public VisitsController(IRepository<Visit> visitRepository, IMapper mapper)
        {
            _visitRepository = visitRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetVisits()
        {
            var visits = await _visitRepository.GetListAsync();

            var visitResponse = new VisitResponse();

            var actionDataset = new VisitDataset();
            var uniqueActionDataset = new VisitDataset();

            var dateGroup = visits.GroupBy(v => v?.VisitDate.Date);

            actionDataset.Label = "Действия";
            actionDataset.Data = dateGroup.Select(vg => vg.Count()).ToArray();
            actionDataset.BorderColor = "rgb(170,50,50)";

            uniqueActionDataset.Label = "Посетители";
            uniqueActionDataset.Data = dateGroup.Select(vg => vg.GroupBy(v => v?.VisitorId).Count()).ToArray();
            uniqueActionDataset.BorderColor = "rgb(20,100,20)";

            visitResponse.Labels = dateGroup.Select(vg => vg.Key.Value.ToString("dd.MM")).ToArray();
            visitResponse.Datasets = [actionDataset, uniqueActionDataset];

            return Ok(visitResponse);
        }
    }
}
