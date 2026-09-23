using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using JobApplication.Application.Interfaces;
using MediatR;
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
        private readonly IMediator _mediator;

        public JobsController(IJobService jobService, ICurrentUser currentUser, IMediator mediator)
        {
            _JobService = jobService;
            _currentUser = currentUser;
            _mediator = mediator;
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

        /// <summary>
        /// Closes a job posting. Only the recruiter who owns the job may close it.
        /// </summary>
        /// <param name="id">The id of the job to close.</param>
        /// <response code="204">The job was closed successfully.</response>
        /// <response code="400">The job is already closed.</response>
        /// <response code="403">The authenticated user does not own this job.</response>
        /// <response code="404">No job exists with the given id.</response>
        [HttpPut("{id}/close")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Close(int id)
        {
            await _mediator.Send(new CloseJobCommand(id, _currentUser.UserId));
            return NoContent();
        }
    }
}
