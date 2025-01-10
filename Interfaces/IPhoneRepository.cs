using PruebaFireBase.Models;

namespace PruebaFireBase.Interfaces
{
    public interface IPhoneRepository
    {
        Task<List<PhoneModel>> GetAllPhonesAsync(); // Leer todos
        Task<PhoneModel?> GetPhoneByIdAsync(string phoneId); // Leer uno por ID
        Task<string> AddPhoneAsync(PhoneModel phone); // Crear
        Task<bool> UpdatePhoneAsync(string phoneId, PhoneModel phone); // Actualizar
        Task<bool> DeletePhoneAsync(string phoneId); // Eliminar
    }
}
