using B4B.Application.DTOs.Product;
using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        private readonly IProductSearchService _productSearchService;
        private readonly ITenantService _tenantService;

        public ProductController(
            IProductService productService,
            IProductSearchService productSearchService,
            ITenantService tenantService)
        {
            _productService = productService;
            _productSearchService = productSearchService;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto request)
        {
            var result = await _productService.CreateAsync(request);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Ürün başarıyla oluşturuldu."
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateProductDto request)
        {
            var result = await _productService.UpdateAsync(id, request);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Ürün başarıyla güncellendi."
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string search)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return BadRequest(new
                {
                    message = "Firma bilgisi doğrulanamadı."
                });

            var products = await _productSearchService.SearchAsync(
                companyId.Value,
                search);

            return Ok(products);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Ürün başarıyla silindi."
            });
        }
    }
}