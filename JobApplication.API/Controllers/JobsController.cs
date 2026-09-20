using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _JobService;
        private readonly ICurrentUser _currentUser;

        public JobsController(IJobService jobService, ICurrentUser currentUser)
        {
            _JobService = jobService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var id = await _JobService.CreateAsync(createJobDto, _currentUser.UserId);
            return Ok(new
            {
                id = id
            });
        }

        [HttpPut("{id}/close")]
        [Authorize]
        public async Task<IActionResult> Close(int id)
        {
            await _JobService.CloseAsync(id, _currentUser.UserId);
            return NoContent();
        }
    }
}
