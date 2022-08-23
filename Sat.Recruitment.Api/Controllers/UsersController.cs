using Microsoft.AspNetCore.Mvc;
using Sat.Recruitment.Api.Model;
using Sat.Recruitment.Service.Abstract;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var isSuccess = false;
            var errors = _userService.ValidateErrors(name, email, address, phone);

            if (errors != null && errors != "")
            {
                return new Result()
                {
                    IsSuccess = isSuccess,
                    Errors = errors
                };
            }

            (isSuccess, errors) = await _userService.RegisterUser(name, email, address, phone, userType, money);

            return new Result()
            {
                IsSuccess = isSuccess,
                Errors = errors
            };
        }
    }
}
