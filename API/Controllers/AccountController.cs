using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entity;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext dataContext,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _dataContext = dataContext;
        }
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Regiter(RegisterDto registerDto)
        {
            if(await IsUserExists(registerDto.UserName)) return new BadRequestObjectResult("Invalid Uasername");
            using var hmac= new HMACSHA512();
            var appUser=new AppUser{
                UserName=registerDto.UserName,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };
            _dataContext.AppUsers.Add(appUser);
            await _dataContext.SaveChangesAsync();
            return new UserDto{ UserName=appUser.UserName, Token=_tokenService.CreateToken(appUser)};
        }
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {   
            var user=await _dataContext.AppUsers.SingleOrDefaultAsync(user=>user.UserName==loginDto.UserName);
            if(user==null)return new UnauthorizedObjectResult("Invalid Username");
            using var hmac= new HMACSHA512(user.PasswordSalt);
            var cumputedHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i=0;i<cumputedHash.Length;i++)
            {
                if(cumputedHash[i]!=user.PasswordHash[i]) return new UnauthorizedObjectResult("Invalid password");                
            }
         return new UserDto{ UserName=user.UserName, Token=_tokenService.CreateToken(user)};
            
        }
        private async Task<bool>IsUserExists(string userName)
        {   
            return await _dataContext.AppUsers.AnyAsync(user=>user.UserName.ToLower()==userName.ToLower());
        }
        
    }
}