using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.Repositories;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly UserRepository _userRepository;
    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
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
}