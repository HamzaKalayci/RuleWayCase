using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleWayCase.Data;
using RuleWayCase.Models;

namespace RuleWayCase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var urunler = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        var sonuc = new List<ProductResponseDto>();

        foreach (var p in urunler)
        {
            sonuc.Add(new ProductResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                StockQuantity = p.StockQuantity,
                IsLive = p.IsLive
            });
        }

        return Ok(sonuc);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var urun = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (urun == null)
        {
            return NotFound("Ürün bulunamadı.");
        }

        var sonuc = new ProductResponseDto
        {
            Id = urun.Id,
            Title = urun.Title,
            Description = urun.Description,
            CategoryId = urun.CategoryId,
            CategoryName = urun.Category != null ? urun.Category.Name : null,
            StockQuantity = urun.StockQuantity,
            IsLive = urun.IsLive
        };

        return Ok(sonuc);
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        // Title boş olamaz
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Ürün adı boş olamaz.");
        }

     
        if (dto.Title.Length > 200)
        {
            return BadRequest("Ürün adı 200 karakterden fazla olamaz.");
        }

        
        if (dto.StockQuantity < 0)
        {
            return BadRequest("Stok miktarı negatif olamaz.");
        }

      
        Category? kategori = null;
        if (dto.CategoryId != null)
        {
            kategori = await _context.Categories.FindAsync(dto.CategoryId);
            if (kategori == null)
            {
                return BadRequest("Böyle bir kategori bulunamadı.");
            }
        }

        var yeniUrun = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            StockQuantity = dto.StockQuantity
        };

        if (kategori != null && yeniUrun.StockQuantity >= kategori.MinStockQuantity)
        {
            yeniUrun.IsLive = true;
        }
        else
        {
            yeniUrun.IsLive = false;
        }

        _context.Products.Add(yeniUrun);
        await _context.SaveChangesAsync();

        return Ok("Ürün başarıyla eklendi.");
    }

    // Ürün güncelle
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var urun = await _context.Products.FindAsync(id);

        if (urun == null)
        {
            return NotFound("Ürün bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Ürün adı boş olamaz.");
        }

        if (dto.Title.Length > 200)
        {
            return BadRequest("Ürün adı 200 karakterden fazla olamaz.");
        }

        if (dto.StockQuantity < 0)
        {
            return BadRequest("Stok miktarı negatif olamaz.");
        }

        Category? kategori = null;
        if (dto.CategoryId != null)
        {
            kategori = await _context.Categories.FindAsync(dto.CategoryId);
            if (kategori == null)
            {
                return BadRequest("Böyle bir kategori bulunamadı.");
            }
        }

        urun.Title = dto.Title;
        urun.Description = dto.Description;
        urun.CategoryId = dto.CategoryId;
        urun.StockQuantity = dto.StockQuantity;

        if (kategori != null && urun.StockQuantity >= kategori.MinStockQuantity)
        {
            urun.IsLive = true;
        }
        else
        {
            urun.IsLive = false;
        }

        await _context.SaveChangesAsync();

        return Ok("Ürün başarıyla güncellendi.");
    }

    // Ürün sil
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var urun = await _context.Products.FindAsync(id);

        if (urun == null)
        {
            return NotFound("Ürün bulunamadı.");
        }

        _context.Products.Remove(urun);
        await _context.SaveChangesAsync();

        return Ok("Ürün silindi.");
    }

    // Ürün filtrele
    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterDto filter)
    {
        var urunler = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

    
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            urunler = urunler.Where(p =>
                p.Title.Contains(filter.Keyword) ||
                (p.Description != null && p.Description.Contains(filter.Keyword)) ||
                (p.Category != null && p.Category.Name.Contains(filter.Keyword))
            ).ToList();
        }
        if (filter.MinStock != null)
        {
            urunler = urunler.Where(p => p.StockQuantity >= filter.MinStock).ToList();
        }

        if (filter.MaxStock != null)
        {
            urunler = urunler.Where(p => p.StockQuantity <= filter.MaxStock).ToList();
        }

        return Ok(urunler);
    }
}