using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Exceptions;
using MediatR;
using System.Threading;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommandHandler : IRequestHandler<CloseJobCommand>
    {
        private readonly IRepository<Job> _jobRepository;

        public CloseJobCommandHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == request.JobId);
            if (job is null)
            {
                throw new NotFoundException($"Job '{request.JobId}' was not found.");
            }

            if (job.RecruiterId != request.RequesterId)
            {
                throw new ForbiddenException("You do not own this job.");
            }

            job.Close(request.RequesterId);
            await _jobRepository.SaveChangesAsync();
        }
    }
}
