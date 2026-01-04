namespace Shared.Dtos.ProductDto
{
    public class CreatedProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        public decimal Price { get; init; }
        public int BrandId { get; set; }
        public int TypeId { get; set; }

    }
}
