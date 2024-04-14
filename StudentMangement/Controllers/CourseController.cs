using Microsoft.AspNetCore.Mvc;
using StudentMangement.Data;
using StudentMangement.Model;
using Microsoft.EntityFrameworkCore;
namespace StudentMangement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CourseController : Controller
    {
        private readonly StudentMangementDBContext _studentMangementDBContext;
        public CourseController(StudentMangementDBContext studentMangementDbContext)
        {
            _studentMangementDBContext = studentMangementDbContext;
        }

        [HttpPost("AddCourse")]
        public async Task<IActionResult> AddCourse([FromBody] Course_Model course)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
                bool courseExists = await _studentMangementDBContext.Course.AnyAsync(s =>
                s.Title == course.Title);
                if (courseExists)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Course already exists...";
                    return Conflict(apiResponse);
                }
                course.Id = Guid.NewGuid();
                await _studentMangementDBContext.Course.AddAsync(course);
                await _studentMangementDBContext.SaveChangesAsync();
                apiResponse.Success = true;
                apiResponse.Message = "Course Added Sucessfully";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the Course. Please try again later."
                };
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("GetAllCourse")]
        public async Task<ActionResult<IEnumerable<CourseResponseModel>>> GetAllCourse()
        {
            try
            {
                var coursesList = await _studentMangementDBContext.Course
                    .Join(_studentMangementDBContext.Category,
                          course => course.categoryId,
                          category => category.ID,
                          (course, category) => new  
                          {
                              course.Id,
                              course.CourseName,
                              course.TeacherName,
                              course.Title,
                              course.Description,
                              course.CourseImage,
                              course.categoryId,
                              course.price,
                              categoryName = category.categoryName
                          }).ToListAsync();
                return Ok(coursesList);
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("getCourseById/{id:Guid}")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            try
            {
                var course = await _studentMangementDBContext.Course
                       .Where(c => c.Id == id)
                       .Join(
                           _studentMangementDBContext.Category, // Join with the Category table
                           course => course.categoryId, // Join condition: Course.CategoryId = Category.ID
                           category => category.ID,
                           (course, category) => new // Project the result into a new object
                           {
                               course.Id,
                               course.CourseName,
                               course.TeacherName,
                               course.Title,
                               course.Description,
                               course.CourseImage,
                               course.categoryId,
                               course.price,
                               categoryName = category.categoryName // Access category information
                           }
                       )
                       .FirstOrDefaultAsync();

                if (course == null)
                {
                    return NotFound(); // Return 404 Not Found if the course with the specified ID is not found
                }
                return Ok(course);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPut("updateCourseById/{id:Guid}")]
        public async Task<ActionResult<Student_Model>> UpdateCourse([FromRoute] Guid id, Course_Model updateCourseModel)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
                var courseList = await _studentMangementDBContext.Course.FindAsync(id);
                if (id != courseList.Id)
                {
                    return BadRequest("Course Not Match");
                }
                if (courseList == null)
                {
                    return NotFound($"Course with Id = {id} not found");
                }
                courseList.Id = id;
                courseList.CourseName = updateCourseModel.CourseName;
                courseList.TeacherName = updateCourseModel.TeacherName;
                courseList.Title = updateCourseModel.Title;
                courseList.Description = updateCourseModel.Description;
                courseList.price= updateCourseModel.price;
                courseList.CourseImage = updateCourseModel.CourseImage;
                courseList.categoryId = updateCourseModel.categoryId;
                await _studentMangementDBContext.SaveChangesAsync();
                apiResponse.Success = true;
                apiResponse.Message = "Course Updated Successfully";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the Course. Please try again later."
                };
                // You can also return a specific status code like 500 Internal Server Error
                return StatusCode(500, apiResponse);
            }
        }


        [HttpDelete("deleteCourseById/{id:Guid}")]
        public async Task<ActionResult<Course_Model>> DeleteCourse(Guid id)
        {
            try
            {
                var courseToDelete = await _studentMangementDBContext.Course.FindAsync(id);
                if (courseToDelete == null)
                {
                    return NotFound($"Course with Id = {id} not found");
                }
                _studentMangementDBContext.Course.Remove(courseToDelete);
                await _studentMangementDBContext.SaveChangesAsync();
                ApiResponse apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.Message = "Course Deleted Sucessfully..";
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
