using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleWayCase.Data;
using RuleWayCase.Models;

namespace RuleWayCase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Categories.Select(c => ToDto(c)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _context.Categories.FindAsync(id);
        return c is null ? NotFound($"Kategori bulunamadı: {id}") : Ok(ToDto(c));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { Errors = new[] { "Kategori adı boş olamaz." } });

        var category = new Category { Name = dto.Name, MinStockQuantity = dto.MinStockQuantity };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, ToDto(category));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { Errors = new[] { "Kategori adı boş olamaz." } });

        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound($"Kategori bulunamadı: {id}");

        category.Name = dto.Name;
        category.MinStockQuantity = dto.MinStockQuantity;

        var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();
        foreach (var p in products)
            p.IsLive = p.StockQuantity >= dto.MinStockQuantity;

        await _context.SaveChangesAsync();
        return Ok(ToDto(category));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound($"Kategori bulunamadı: {id}");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static CategoryResponseDto ToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        MinStockQuantity = c.MinStockQuantity
    };
}