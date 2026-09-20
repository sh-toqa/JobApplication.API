using JobApplication.Application.DTOs;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;

namespace JobApplication.Application.Interfaces
{
    public interface IJobCandidateApplicationService
    {
        IEnumerable<JobCandidateApplication> GetAll();
        Task<int> CreateAsync(CreateJobCandidateApplicationDto createJobApplicationDto, int requesterId);
        Task<JobCandidateApplication?> UpdateStatus(int id, JobApplicationStatus status);
        Task CancelAsync(int id, int requesterId);
    }
}
