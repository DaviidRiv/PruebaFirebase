using Microsoft.AspNetCore.Mvc;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;

namespace PruebaFireBase.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authService;

        public AuthController(IAuthRepository authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginRequest(AuthLoginModel request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Email, request.Password);

                // Almacenar el token en la sesión o usarlo según tu lógica
                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("UserEmail", request.Email);
                // Redirigir a la acción IndexPhone del controlador Phone
                return RedirectToAction("IndexPhone", "Phone");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRequest(AuthLoginModel request)
        {
            try
            {
                // Registrar al usuario y obtener el token
                var token = await _authService.RegisterAsync(request.Email, request.Password);

                // Almacenar el token y el correo en la sesión (opcional)
                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("UserEmail", request.Email);

                // Redirigir al usuario después del registro exitoso
                return RedirectToAction("IndexPhone", "Phone");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _authService.SendPasswordResetEmailAsync(email);
                ViewData["Error"] = "Send email for recovery password";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Login");
            }
        }
    }
}
