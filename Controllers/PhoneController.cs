using Microsoft.AspNetCore.Mvc;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;

namespace PruebaFireBase.Controllers
{
    public class PhoneController : Controller
    {
        private readonly IPhoneRepository _phoneService;
        private readonly IPhoneStorageRepository _imageService;

        public PhoneController(IPhoneRepository phoneService, IPhoneStorageRepository imageService)
        {
            _phoneService = phoneService;
            _imageService = imageService;
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
        public async Task<IActionResult> CreatePhone(PhoneModel phone, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                // Subir la imagen a Dropbox
                if (Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var imagePath = $"/Phones/{phone.Brand}/{Image.FileName}"; // Definir ruta en Dropbox

                        // Guardar la URL o el path de la imagen en el modelo
                        var (path, url) = await _imageService.UploadImageAsync(imagePath, memoryStream);

                        phone.UrlImage = url;
                        phone.ImagePath = path;
                    }
                }

                phone.Image = null;
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
                var success = await _imageService.UpdateImageAsync(id, phone, phone.Image);

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

        public async Task<IActionResult> DeletePhone(string id)
        {
            var phone = await _phoneService.GetPhoneByIdAsync(id);
            await DeleteImage(phone!.ImagePath!);

            var result = await _phoneService.DeletePhoneAsync(id);

            return RedirectToAction(nameof(IndexPhone));
        }
        
        public async Task<IActionResult> DeleteImagePhone(string path, string id)
        {
            var result = await _imageService.DeleteImageAsync(path);
            var phone = await _phoneService.GetPhoneByIdAsync(id);
            phone!.ImagePath = null;
            phone!.UrlImage = null;
            phone!.PhoneId = null;

            await _phoneService.UpdatePhoneAsync(id, phone);
            return RedirectToAction(nameof(IndexPhone));
        }

        public async Task<IActionResult> DeleteImage(string path)
        {
            var result = await _imageService.DeleteImageAsync(path);

            return RedirectToAction(nameof(IndexPhone));
        }
    }
}
