using Microsoft.AspNetCore.Mvc;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;

namespace PruebaFireBase.Controllers
{
    public class PhoneController : Controller
    {
        private readonly IPhoneRepository _phoneService;

        public PhoneController(IPhoneRepository phoneService)
        {
            _phoneService = phoneService;
        }

        public async Task<IActionResult> IndexPhone()
        {
            var phones = await _phoneService.GetAllPhonesAsync();
            return View(phones); 
        }

        public IActionResult CreatePhone()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhone(PhoneModel phone)
        {
            if (ModelState.IsValid)
            {
                await _phoneService.AddPhoneAsync(phone);
                return RedirectToAction(nameof(IndexPhone));
            }

            return View(phone);
        }

        public async Task<IActionResult> UpdatePhone(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Obtén el teléfono actual por su ID de Firebase
            var phone = await _phoneService.GetPhoneByIdAsync(id);

            if (phone == null)
            {
                return NotFound();
            }

            phone.PhoneId = id;

            return View(phone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhoneUpdate(string id, PhoneModel phone)
        {
            if (id != phone.PhoneId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var success = await _phoneService.UpdatePhoneAsync(id, phone);

                if (success)
                {
                    return RedirectToAction(nameof(IndexPhone));
                }
                else
                {
                    return RedirectToAction(nameof(UpdatePhone), new { id = phone.PhoneId });
                }
            }

            return RedirectToAction(nameof(UpdatePhone), new { id = phone.PhoneId });
        }

        public async Task<IActionResult> DetailsPhone(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Obtén el teléfono actual por su ID de Firebase
            var phone = await _phoneService.GetPhoneByIdAsync(id);

            if (phone == null)
            {
                return NotFound();
            }

            phone.PhoneId = id;

            return View(phone);
        }
    }
}
