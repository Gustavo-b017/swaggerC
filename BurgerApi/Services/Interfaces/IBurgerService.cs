using BurgerApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerApi.Services.Interfaces;

public interface IBurgerService
{
    Task<List<BurgerDto>> GetAllAsync();
    Task<BurgerDto?> GetByIdAsync(int id);
    Task<BurgerDto> CreateAsync(CreateBurgerDto dto);
    Task<bool> UpdateAsync(int id, UpdateBurgerDto dto);
    Task<bool> DeleteAsync(int id);

    Task<bool> AddToppingAsync(int burgerId, int toppingId);
    Task<bool> RemoveToppingAsync(int burgerId, int toppingId);
}
