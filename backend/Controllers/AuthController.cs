using backend.Data;
using backend.Services;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using backend.Contracts;

namespace backend.Controllers;

[Route("api/register")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AccountServices accountServices;
    private readonly TodoDb db;
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IConfiguration configuration;

    public AuthController(
        AccountServices accountServices, 
        TodoDb db, 
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        this.accountServices = accountServices;
        this.db = db;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var account = new Account
        {
            Email = model.Email,
            FirstName  = model.FirstName,
        };
        await accountServices.CreateAccountAsync(account);

        return Ok("User and account created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login ([FromBody] Login model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = GenerateJwtToken(user);
            var account = await db.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (account == null)
            {
                return StatusCode(500, "Account data missing");
            }

            var response = new LoginResponse
            {
                Token = token,
                FirstName = account.FirstName,
                AccountId = account.Id
            };
            
            return Ok(response);
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(IdentityUser user){

        var secretKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured.");
        }
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}