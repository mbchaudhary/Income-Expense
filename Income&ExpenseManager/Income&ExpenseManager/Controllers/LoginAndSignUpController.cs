using Income_ExpenseManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Income_ExpenseManager.Controllers
{
    public class LoginAndSignUpController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region configuration
        public LoginAndSignUpController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        #endregion

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string url = $"https://localhost:7291/api/Registration/LogIn/{email}/{password}";

            var loginPayload = new
            {
                Email = email,
                Password = password
            };

            var json = JsonConvert.SerializeObject(loginPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var loginResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        if (loginResponse["statusCode"] != 200)
                        {
                            TempData["Error"] = "Invalid credentials.";
                            return RedirectToAction("Login");
                        }

                        string userId = loginResponse?.userId;
                        TempData["Message"] = "Login successful.";

                        HttpContext.Session.SetString("userId", userId.ToString());

                        return RedirectToAction("Home", "Home");
                    }

                    TempData["Error"] = "Invalid credentials.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while processing your request.";
                    return RedirectToAction("Login");
                }
            }
        }



        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserModel user)
        {
            string url = "https://localhost:7291/api/Registration";

            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        TempData["Message"] = "SignUp successful.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Error"] = "Invalid credentials.";
                        return RedirectToAction("SignUp");
                    }
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while processing your request.";
                    return RedirectToAction("SignUp");
                }
            }
        }
    }
}
