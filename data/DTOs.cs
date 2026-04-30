namespace RuleWayCase.Data;

public class CreateProductDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public int StockQuantity { get; set; }
}

public class UpdateProductDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public int StockQuantity { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int StockQuantity { get; set; }
    public bool IsLive { get; set; }
}

public class ProductFilterDto
{
    public string? Keyword { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int MinStockQuantity { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int MinStockQuantity { get; set; }
}

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinStockQuantity { get; set; }
}