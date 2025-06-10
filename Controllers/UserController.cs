using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.Dtos;
using UsersService.Repositories;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            email,
            name,
            role
        });
    }

    [HttpGet("{id}/email")]
    public async Task<IActionResult> GetEmailById(Guid id)
    {
        var email = await _userRepository.GetEmailByIdAsync(id);
        if (email == null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(new { email });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // sub en el token
        if (userIdClaim == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        return Ok(new
        {
            user.IdUser,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.DateOfBirth,
            user.Gender,
            user.Country,
            user.City
        });
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        // Actualizamos los campos permitidos
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
        user.Gender = request.Gender ?? user.Gender;
        user.Country = request.Country ?? user.Country;
        user.City = request.City ?? user.City;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return Ok(new { message = "Perfil actualizado con éxito" });
    }

    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
