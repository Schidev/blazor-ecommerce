using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public ICategoryService _categoryService { get; set; }
        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> GetCategories()
        { 
            var result = await _categoryService.GetCategories();
            return Ok(result);
        }
    }
}
