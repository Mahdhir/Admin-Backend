using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.Dtos;
using Project.Entities;
using Project.Services;
using WebApi.Helpers;
using WebApi.Services;
using WebApi.Dtos;

namespace Project.Controllers
{
        [Authorize]
        [ApiController]
        [Route("[controller]")]
        public class AdminController  : ControllerBase

    {
                private iAdminServices _adminService;
                private IUserService _userService;
                private IMapper _mapper;
                private readonly AppSettings _appSettings;

                public AdminController(
                    iAdminServices adminService,
                    IUserService userService,
                    IMapper mapper,
                    IOptions<AppSettings> appSettings)
                {
                    _adminService = adminService;
                    _userService = userService;
                    _mapper = mapper;
                    _appSettings = appSettings.Value;
                }
                
             [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AdminDto userDto)
        {
            var user = _adminService.AuthenticateUser(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                // token dont expire 
                Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.Id,
                Username = user.Username, //follow the vedio if it failed change Task to normal process
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        } 

        
        [HttpPost("register")]
        public  IActionResult Register([FromBody]AdminDto adminDto)
        {
            // map dto to entity
            var admin = _mapper.Map<Admin>(adminDto);

            try 
            {
                // save 
                _adminService.AddAdmin(admin, adminDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

         [HttpGet]
        public IActionResult GetAll()
        {
            var admin =  _adminService.GetAllAdmins();
            var adminDtos = _mapper.Map<IList<AdminDto>>(admin);
            return Ok(adminDtos);
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users =  _userService.GetAllUser();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user =  _adminService.GetById(id);
            var userDto = _mapper.Map<AdminDto>(user);
            return Ok(userDto);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]AdminDto adminDto)
        {
            // map dto to entity and set id
            var admin = _mapper.Map<Admin>(adminDto);
            admin.Id = id;

            try 
            {
                // save 
                _adminService.UpdateAdmin(admin, adminDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _adminService.DeleteAdmin(id);
            return Ok();
        }
    }

    

}