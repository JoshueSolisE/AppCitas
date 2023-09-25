using System.Security.Cryptography;
using System.Text;
using AppCitas.Data;
using AppCitas.DTOs;
using AppCitas.Entities;
using AppCitas.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCitas.Controllers;

public class AccountController : BaseAPIController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private const string USSER_PASS_ERR_MESS = "Username or Password are wrong";

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }
    [HttpPost("Register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) 
            return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDTO.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => 
            x.UserName.ToLower() == loginDto.Username.ToLower());

        if (user == null) return Unauthorized(USSER_PASS_ERR_MESS);

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized(USSER_PASS_ERR_MESS);
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
