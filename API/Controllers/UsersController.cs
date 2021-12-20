using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController:ControllerBase
    {
        private readonly DataContext _dataContext;
        public UsersController(DataContext dataContext)
        {
            _dataContext = dataContext;
            
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _dataContext.AppUsers.ToListAsync();
        } 
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int Id)
        {
            return await _dataContext.AppUsers.FindAsync(Id);
        }
    }
}