﻿using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user.Password, password);
            if (verificationResult == PasswordVerificationResult.Success)
                return user;
        }
        return null;
    }

    public async Task AddUserAsync(User user)
    {
        if(!await IsEmailExistsAsync(user.Email))
        {
            user.Password = _passwordHasher.HashPassword(user.Password);
            await _context.Users.AddAsync(user);
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
