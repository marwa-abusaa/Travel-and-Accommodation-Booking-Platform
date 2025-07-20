using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class UserCommandService : IUserCommandService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserCommandService> _logger;

    public UserCommandService(IUserRepository userRepository, IMapper mapper,
        IRoleRepository roleRepository, IUnitOfWork unitOfWork, 
        ILogger<UserCommandService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task SignUpAsync(SignUpDto signUpDto)
    {
        _logger.LogInformation("Signup process started for email: {Email}", signUpDto.Email);

        if (await _userRepository.IsEmailExistsAsync(signUpDto.Email))
        {
            _logger.LogWarning("Signup failed: Email already exists - {Email}", signUpDto.Email);
            throw new EmailAlreadyExistsException("Email is already registered.");
        }

        var role = await _roleRepository.GetRoleByNameAsync(signUpDto.RoleName);
        if (role is null)
        {
            _logger.LogWarning("Signup failed: Role not found - {Role}", signUpDto.RoleName);
            throw new NotFoundException($"Role '{signUpDto.RoleName}' not found.");
        }

        var newUser = _mapper.Map<User>(signUpDto);
        newUser.Role = role;

        await _userRepository.AddUserAsync(newUser);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Signup successful for email: {Email}", signUpDto.Email);
    }
}
