using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommand : IRequest
    {
        public int JobId { get; }
        public int RequesterId { get; }

        public CloseJobCommand(int jobId, int requesterId)
        {
            JobId = jobId;
            RequesterId = requesterId;
        }
    }
}
