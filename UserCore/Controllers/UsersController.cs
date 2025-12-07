using Microsoft.AspNetCore.Mvc;
using UserCore.DTOs;
using UserCore.Entities;
using UserCore.Interfaces.Services;

namespace UserCore.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService service) : ControllerBase
{
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        try
        {
            await service.Register(registerDto);
            return Ok();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            var user = await service.Login(loginDto);
            HttpContext.Response.Cookies.Append("hot-cookies", user.Token);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await service.GetById(id);
            return Ok(user);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(User updateUser)
    {
        try
        {
            await service.Update(updateUser);
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await service.Delete(id);
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }
}