using Income_ExpenseApiManager.Data;
using Income_ExpenseApiManager.Model;
using Microsoft.AspNetCore.Mvc;

namespace Income_ExpenseApiManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        [HttpPost]

        public IActionResult RegistrationUser([FromBody] UserModel user, RegistrationRepositery registrationRepositery)
        {
            if(user == null)
            {
                return BadRequest();
            }

            bool isInserted = registrationRepositery.registration(user);

            if(isInserted)
            {
                return Ok("Insert Successfully");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("LogIn/{email}/{password}")]

        public IActionResult Login(string email , string password, RegistrationRepositery registrationRepositery)
        {
            if(email == null || password == null)
            {
                return BadRequest();
            }

            Dictionary<string,dynamic> res = registrationRepositery.login(email , password);

            return StatusCode(res["statusCode"],res);
        }
    }
}
