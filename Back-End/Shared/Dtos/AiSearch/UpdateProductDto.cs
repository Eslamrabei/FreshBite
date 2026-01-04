using Microsoft.AspNetCore.Http;
using Shared.Dtos.ProductDto;
namespace Shared.Dtos.AiSearch
{
    public class UpdateProductDto : CreatedProductDto
    {
        public int Id { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
