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
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IRepository<User> userRepository,
            IRepository<Candidate> candidateRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _candidateRepository = candidateRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = _userRepository.Get().FirstOrDefault(u => u.Email == registerDto.Email);
            if (existingUser is not null)
            {
                throw new BusinessRuleException("A user with this email already exists.");
            }

            if (registerDto.Role == UserRole.Candidate && string.IsNullOrWhiteSpace(registerDto.Name))
            {
                throw new BusinessRuleException("Name is required when registering as a candidate.");
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = _passwordHasher.Hash(registerDto.Password),
                Role = registerDto.Role
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            int? candidateId = null;
            if (registerDto.Role == UserRole.Candidate)
            {
                var candidate = new Candidate
                {
                    Name = registerDto.Name!,
                    CvUrl = registerDto.CvUrl ?? string.Empty,
                    UserId = user.Id
                };

                await _candidateRepository.AddAsync(candidate);
                await _candidateRepository.SaveChangesAsync();

                candidateId = candidate.Id;
            }

            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                CandidateId = candidateId,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = _userRepository.Get().FirstOrDefault(u => u.Email == loginDto.Email);
            if (user is null || !_passwordHasher.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            int? candidateId = null;
            if (user.Role == UserRole.Candidate)
            {
                var candidate = _candidateRepository.Get().FirstOrDefault(c => c.UserId == user.Id);
                candidateId = candidate?.Id;
            }

            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                CandidateId = candidateId,
                ExpiresAt = expiresAt
            };
        }
    }
}
