using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Exceptions;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public class AuthService(
    AppDbContext context,
    ITokenService tokenService) : IAuthService
{
    private readonly PasswordHasher<User> hasher = new();

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await context.Users.AnyAsync(x => x.Email == email))
            throw new ConflictException("Email already exists.");

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = UserRoles.Customer,
            PasswordHash = hasher.HashPassword(null!, dto.Password)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return CreateResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null ||
            hasher.VerifyHashedPassword(null!, user.PasswordHash, dto.Password)
            == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        return CreateResponse(user);
    }

    private AuthResponseDto CreateResponse(User user)
    {
        var (token, expiresAt) = tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            ExpiresAtUtc = expiresAt
        };
    }
}