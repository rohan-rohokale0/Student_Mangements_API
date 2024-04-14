using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMangement.Data;
using StudentMangement.Model;

namespace StudentMangement.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]
    public class CategoryController : Controller
    {
        private readonly StudentMangementDBContext _studentMangementDBContext;
        public CategoryController(StudentMangementDBContext studentMangementDbContext)
        {
            _studentMangementDBContext = studentMangementDbContext;
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Category_Model category)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
                bool categoryExists = await _studentMangementDBContext.Category.AnyAsync(s =>
                s.categoryName == category.categoryName);
                if (categoryExists)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Category already exists...";

                    return Conflict(apiResponse);
                }
                category.ID = Guid.NewGuid();
                await _studentMangementDBContext.Category.AddAsync(category);
                await _studentMangementDBContext.SaveChangesAsync();
                apiResponse.Success = true;
                apiResponse.Message = "Category Added Sucessfully";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the Category. Please try again later."
                };
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var categorytList = await _studentMangementDBContext.Category.ToListAsync();
                return Ok(categorytList);
            }
            catch (Exception ex)
            {
                // For demonstration purposes, let's return a generic error response
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                // You can also return a specific status code like 500 Internal Server Error
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("getCategoryById/{id:Guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var result = await _studentMangementDBContext.Category.FirstOrDefaultAsync(x => x.ID == id);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPut("updateCategoryById/{id:Guid}")]
        public async Task<ActionResult<Student_Model>> UpdateCategory([FromRoute] Guid id, Category_Model updateCategoeyModel)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
                var Category = await _studentMangementDBContext.Category.FindAsync(id);
                if (id != Category.ID)
                {
                    return BadRequest("Category Not Match");
                }
                if (Category == null)
                {
                    return NotFound($"Student with Id = {id} not found");
                }
                Category.ID = id;
                Category.categoryName=updateCategoeyModel.categoryName;
                await _studentMangementDBContext.SaveChangesAsync();
                apiResponse.Success = true;
                apiResponse.Message = "Category Updated Successfully";
                return Ok(apiResponse);
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the Category. Please try again later."
                };
                // You can also return a specific status code like 500 Internal Server Error
                return StatusCode(500, apiResponse);
            }
        }
        [HttpDelete("deleteCategoryById/{id:Guid}")]
        public async Task<ActionResult<Student_Model>> DeleteCategory(Guid id)
        {
            try
            {
                var categoeyToDelete = await _studentMangementDBContext.Category.FindAsync(id);
                if (categoeyToDelete == null)
                {
                    return NotFound($"Category with Id = {id} not found");
                }
                _studentMangementDBContext.Category.Remove(categoeyToDelete);
                await _studentMangementDBContext.SaveChangesAsync();
                ApiResponse apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.Message = "Categoey Deleted Sucessfully..";
                return Ok(apiResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }
    }
}
