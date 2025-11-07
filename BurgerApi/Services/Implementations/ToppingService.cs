using AutoMapper;
using BurgerApi.Data;
using BurgerApi.Domain;
using BurgerApi.DTOs;
using BurgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerApi.Services.Implementations;

/// <summary>
/// Service de Topping com CRUD básico.
/// </summary>
public class ToppingService : IToppingService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ToppingService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ToppingDto>> GetAllAsync()
    {
        var list = await _db.Toppings.AsNoTracking().ToListAsync();
        return _mapper.Map<List<ToppingDto>>(list);
    }

    public async Task<ToppingDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Toppings.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return entity == null ? null : _mapper.Map<ToppingDto>(entity);
    }

    public async Task<ToppingDto> CreateAsync(CreateToppingDto dto)
    {
        var entity = _mapper.Map<Topping>(dto);
        _db.Toppings.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.Map<ToppingDto>(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateToppingDto dto)
    {
        var entity = await _db.Toppings.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return false;

        entity.Name = dto.Name;
        entity.Price = dto.Price;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Toppings.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return false;

        _db.Toppings.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
