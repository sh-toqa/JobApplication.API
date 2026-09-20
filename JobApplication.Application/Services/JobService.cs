using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<Job> _jobRepository;

        public JobService(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> CreateAsync(CreateJobDto createJobDto, int recruiterId)
        {
            var job = new Job()
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = true,
                RecruiterId = recruiterId
            };
            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            return job.Id;
        }

        public async Task CloseAsync(int id, int requesterId)
        {
            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == id);
            if (job is null)
            {
                throw new NotFoundException($"Job '{id}' was not found.");
            }

            if (job.RecruiterId != requesterId)
            {
                throw new ForbiddenException("You do not own this job.");
            }

            job.Close(requesterId);
            await _jobRepository.SaveChangesAsync();
        }
    }
}
