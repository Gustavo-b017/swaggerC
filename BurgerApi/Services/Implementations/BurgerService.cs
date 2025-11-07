using AutoMapper;
using AutoMapper.QueryableExtensions;
using BurgerApi.Data;
using BurgerApi.Domain;
using BurgerApi.DTOs;
using BurgerApi.Services.Interfaces;
using BurgerApi.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerApi.Services.Implementations;

/// <summary>
/// Service de Burger com operações CRUD e gerenciamento de Toppings.
/// </summary>
public class BurgerService : IBurgerService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public BurgerService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<BurgerDto>> GetAllAsync()
    {
        var query = _db.Burgers
            .Include(b => b.BurgerToppings).ThenInclude(bt => bt.Topping)
            .AsNoTracking();

        var list = (await query.ToListAsync())
            .Select(b =>
            {
                var dto = _mapper.Map<BurgerDto>(b);
                dto.CalculatedPrice = b.CalculatePrice();
                return dto;
            })
            .ToList();

        return list;
    }

    public async Task<BurgerDto?> GetByIdAsync(int id)
    {
        var b = await _db.Burgers
            .Include(x => x.BurgerToppings).ThenInclude(bt => bt.Topping)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b == null) return null;

        var dto = _mapper.Map<BurgerDto>(b);
        dto.CalculatedPrice = b.CalculatePrice();
        return dto;
    }

    public async Task<BurgerDto> CreateAsync(CreateBurgerDto dto)
    {
        var entity = _mapper.Map<Burger>(dto);

        if (dto.ToppingIds is { Count: > 0 })
        {
            var toppings = await _db.Toppings
                .Where(t => dto.ToppingIds.Contains(t.Id))
                .ToListAsync();

            foreach (var t in toppings)
                entity.BurgerToppings.Add(new BurgerTopping { Burger = entity, Topping = t });
        }

        _db.Burgers.Add(entity);
        await _db.SaveChangesAsync();

        // Recarrega com includes
        entity = await _db.Burgers
            .Include(b => b.BurgerToppings).ThenInclude(bt => bt.Topping)
            .FirstAsync(b => b.Id == entity.Id);

        var outDto = _mapper.Map<BurgerDto>(entity);
        outDto.CalculatedPrice = entity.CalculatePrice();
        return outDto;
    }

    public async Task<bool> UpdateAsync(int id, UpdateBurgerDto dto)
    {
        var entity = await _db.Burgers.FirstOrDefaultAsync(b => b.Id == id);
        if (entity == null) return false;

        entity.Name = dto.Name;
        entity.BasePrice = dto.BasePrice;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Burgers.FirstOrDefaultAsync(b => b.Id == id);
        if (entity == null) return false;

        _db.Burgers.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddToppingAsync(int burgerId, int toppingId)
    {
        var burger = await _db.Burgers
            .Include(b => b.BurgerToppings)
            .FirstOrDefaultAsync(b => b.Id == burgerId);
        var topping = await _db.Toppings.FirstOrDefaultAsync(t => t.Id == toppingId);

        if (burger == null || topping == null) return false;

        if (!burger.BurgerToppings.Any(bt => bt.ToppingId == toppingId))
        {
            burger.BurgerToppings.Add(new BurgerTopping { BurgerId = burgerId, ToppingId = toppingId });
            await _db.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> RemoveToppingAsync(int burgerId, int toppingId)
    {
        var bt = await _db.BurgerToppings
            .FirstOrDefaultAsync(x => x.BurgerId == burgerId && x.ToppingId == toppingId);
        if (bt == null) return false;

        _db.BurgerToppings.Remove(bt);
        await _db.SaveChangesAsync();
        return true;
    }
}
