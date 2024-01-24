

using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController:ControllerBase
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
        _context= context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

        if( await UserExist(registerDto.UserName)) return BadRequest("username already exist");

        var user =  _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();
        
        user.UserName = registerDto.UserName.ToLower();

        _context.Users.Add(user);

        await _context.SaveChangesAsync();
        return new UserDto{
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
        var user = await _context.Users.SingleOrDefaultAsync(x=> x.UserName.ToLower() == loginDto.UserName.ToLower());
        if(user == null) return Unauthorized("invalid user");
      
          return new UserDto{
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };;
    }

    private async Task<bool> UserExist(string username){
        return await _context.Users.AnyAsync(x=>x.UserName == username.ToLower());
    }
}