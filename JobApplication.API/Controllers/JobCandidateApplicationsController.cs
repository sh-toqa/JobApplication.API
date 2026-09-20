using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobCandidateApplicationsController : ControllerBase
    {
        private readonly IJobCandidateApplicationService _JobCandidateApplicationService;
        private readonly ICurrentUser _currentUser;

        public JobCandidateApplicationsController(IJobCandidateApplicationService jobApplicationService, ICurrentUser currentUser)
        {
            _JobCandidateApplicationService = jobApplicationService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applications = _JobCandidateApplicationService.GetAll();
            return Ok(new { applications });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = _JobCandidateApplicationService.GetAll().FirstOrDefault(j=>j.Id == id);
            if (application is null) return NotFound(new
            {
                message = "invalid Id"
            });
            return Ok(new { application });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateJobCandidateApplicationDto createApplicationDto)
        {
            var id = await _JobCandidateApplicationService.CreateAsync(createApplicationDto, _currentUser.UserId);
            return Ok(new { id = id });
        }
        [HttpPatch("{id}/{status}")]
        public async Task<IActionResult> Update(int id, JobApplicationStatus status)
        {
            var job = await _JobCandidateApplicationService.UpdateStatus(id, status);
            if (job == null) return NotFound();
            return Ok(new { id = job.Id });
        }

        [HttpDelete("/api/applications/{id}")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            await _JobCandidateApplicationService.CancelAsync(id, _currentUser.UserId);
            return NoContent();
        }
    }
}
