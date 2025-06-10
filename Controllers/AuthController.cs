using Microsoft.AspNetCore.Mvc;
using UsersService.Models;
using UsersService.Repositories;
using UsersService.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UsersService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthController(UserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    var user = new User
    {
        IdUser = Guid.NewGuid(),
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        PhoneNumber = request.PhoneNumber,
        DateOfBirth = request.DateOfBirth,
        Gender = request.Gender,
        Country = request.Country,
        City = request.City,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    await _userRepository.CreateAsync(user);

    return Ok(new { message = "Usuario registrado con éxito", user.IdUser });
}

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest credentials)
    {
        var user = await _userRepository.GetByEmailAsync(credentials.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        var token = _jwtService.GenerateToken(user);
        return Ok(new
        {
            token,
            user = new
            {
                user.IdUser,
                user.FirstName,
                user.LastName,
                user.Email
            }
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

