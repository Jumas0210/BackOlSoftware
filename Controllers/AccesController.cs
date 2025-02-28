using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackOlSoftware.Custom;
using BackOlSoftware.Models;
using BackOlSoftware.Models.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace BackOlSoftware.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesController : ControllerBase
    {
        private readonly OlSoftwareContext _olSoftwareContex;
        private readonly Util _util;
        public AccesController(OlSoftwareContext olSoftwareContex, Util util)
        {
            _olSoftwareContex = olSoftwareContex;
            _util = util;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            var userModel = new User
            {
                Name = user.Name,
                Password = _util.encryptSHA256(user.Password),
                Email = user.Email,
                RoleId = user.RoleID
            };

            await _olSoftwareContex.Users.AddAsync(userModel);
            await _olSoftwareContex.SaveChangesAsync();

            if (userModel.UserId != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });

        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO user)
        {
            var userFind = await _olSoftwareContex.Users
                            .Include(u => u.Role)
                            .Where(u =>
                            u.Email == user.Email &&
                            u.Password == _util.encryptSHA256(user.Password)
                            ).FirstOrDefaultAsync();
            if(userFind == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _util.generateJWT(userFind) });
        }
    }
}
