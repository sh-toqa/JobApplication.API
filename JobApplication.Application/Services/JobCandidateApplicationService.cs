using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using JobApplication.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Services
{
    public class JobCandidateApplicationService : IJobCandidateApplicationService
    {
        private readonly IRepository<JobCandidateApplication> _jobApplicationRepository;
        private readonly IRepository<Candidate> _candidateRepository;

        public JobCandidateApplicationService(
            IRepository<JobCandidateApplication> jobApplicationRepository,
            IRepository<Candidate> candidateRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateRepository = candidateRepository;
        }
        public IEnumerable<JobCandidateApplication> GetAll()
        {
            var applications = _jobApplicationRepository.Get().ToList();
            return applications;
        }
        public async Task<int> CreateAsync(CreateJobCandidateApplicationDto createJobApplicationDto, int requesterId)
        {
            var candidate = _candidateRepository.Get().FirstOrDefault(c => c.UserId == requesterId);
            if (candidate is null)
            {
                throw new ForbiddenException("No candidate profile is linked to this account.");
            }

            var jobApplication = new JobCandidateApplication()
            {
                JobId = createJobApplicationDto.JobId,
                CandidateId = candidate.Id,
            };
            await _jobApplicationRepository.AddAsync(jobApplication);
            await _jobApplicationRepository.SaveChangesAsync();
            return jobApplication.Id;
        }
        public async Task<JobCandidateApplication?> UpdateStatus(int id, JobApplicationStatus status)
        {
            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == id);
            if (jobApplication == null)
            {
                return null;
            }
            jobApplication.UpdateStatus(status);
            await _jobApplicationRepository.SaveChangesAsync();
            return jobApplication;
        }

        public async Task CancelAsync(int id, int requesterId)
        {
            var jobApplication = _jobApplicationRepository.Get().FirstOrDefault(a => a.Id == id);
            if (jobApplication is null)
            {
                throw new NotFoundException($"Application '{id}' was not found.");
            }

            var candidate = _candidateRepository.Get().FirstOrDefault(c => c.Id == jobApplication.CandidateId);
            if (candidate is null || candidate.UserId != requesterId)
            {
                throw new ForbiddenException("You do not own this application.");
            }

            jobApplication.Cancel();
            await _jobApplicationRepository.SaveChangesAsync();
        }
    }
}
