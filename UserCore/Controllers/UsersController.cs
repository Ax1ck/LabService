using Microsoft.AspNetCore.Mvc;
using UserCore.DTOs;
using UserCore.Entities;
using UserCore.Interfaces.Services;
using Prometheus;

namespace UserCore.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService service) : ControllerBase
{
    // всего успешных регистраций
    private static readonly Counter RegistrationTotal =
        Metrics.CreateCounter(
            "usercore_registration_total",
            "Total number of successfully registered users");

    // всего неудачных регистраций
    private static readonly Counter RegistrationFailedTotal =
        Metrics.CreateCounter(
            "usercore_registration_failed_total",
            "Total number of failed user registration attempts");

    // длительность операции регистрации
    private static readonly Histogram RegistrationDuration =
        Metrics.CreateHistogram(
            "usercore_registration_duration_seconds",
            "User registration duration in seconds");
    
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        using (RegistrationDuration.NewTimer())
        {
            try
            {
                await service.Register(registerDto);
                RegistrationTotal.Inc();
                return Ok();
            }
            catch (Exception)
            {
                RegistrationFailedTotal.Inc();
                throw;
            }
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