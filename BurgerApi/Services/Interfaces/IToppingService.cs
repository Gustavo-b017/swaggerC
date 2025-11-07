using BurgerApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerApi.Services.Interfaces;

public interface IToppingService
{
    Task<List<ToppingDto>> GetAllAsync();
    Task<ToppingDto?> GetByIdAsync(int id);
    Task<ToppingDto> CreateAsync(CreateToppingDto dto);
    Task<bool> UpdateAsync(int id, UpdateToppingDto dto);
    Task<bool> DeleteAsync(int id);
}
