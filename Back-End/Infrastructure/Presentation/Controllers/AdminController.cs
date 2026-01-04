using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Dtos.AiSearch;

namespace Presentation.Controllers
{
    public class AdminController(IVectorService _vectorService,
        IEmbeddingService _embeddingService, IServiceManager _service, IValidator<CreatedProductDto> _validator,
        ILogger<AdminController> _logger) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> AddProduct(CreatedProductDto dto)
        {
            var result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest();
            }
            int id = 0;
            try
            {
                var TextToEmbed = $"{dto.Name} - {dto.Description} - {dto.Price}";
                var vectors = await _embeddingService.GetEmbeddingAsync(TextToEmbed);

                id = await _service.ProductService.AddProduct(dto);
                await _vectorService.UpsertProductAsync(id, vectors, dto);
                return Ok(new { Message = "The product cearted Successfully in db and Vectore db" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                if (id > 0)
                    await _service.ProductService.DeleteProduct(id);
                return StatusCode(500, "Internal Server Error");
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0) return BadRequest();
            try
            {
                bool result = await _service.ProductService.DeleteProduct(id);
                if (!result) return NotFound();

                _ = _vectorService.DeleteProductAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product");
                return BadRequest();
            }

        }


        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProduct(int Id, [FromForm] UpdateProductDto dto)
        {
            if (Id <= 0 && Id != dto.Id) return BadRequest("Invalid Product ID");
            try
            {

                await _service.ProductService.UpdateProduct(dto);

                await _vectorService.UpdateProductAsync(dto);

                return Ok(new { Message = "Product updated successfully synced with AI" });

            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: {ex.Message}");
                return BadRequest($"Failed to update product: {ex.Message}");
            }
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UpoaldImage(IFormFile file)
        {
            if (file == null) return BadRequest("No File Sent");

            var fileName = await _service.FileService.UploadFileAsync(file, "products");
            return Ok(new { filePath = fileName });
        }



    }
}
