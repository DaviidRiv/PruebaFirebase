using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaFireBase.Models
{
    public class PhoneModel
    {
        public string? PhoneId { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Price { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public string? UrlImage { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }
}
