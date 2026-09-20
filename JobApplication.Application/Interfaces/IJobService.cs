using JobApplication.Application.DTOs;

namespace JobApplication.Application.Interfaces
{
    public interface IJobService
    {
        Task<int> CreateAsync(CreateJobDto createJobDto, int recruiterId);
        Task CloseAsync(int id, int requesterId);
    }
}
