using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMangement.Data;
using StudentMangement.Model;

namespace StudentMangement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentController : Controller
    {
        private readonly StudentMangementDBContext _studentMangementDBContext;
        public StudentController(StudentMangementDBContext studentMangementDbContext)
        {
            _studentMangementDBContext = studentMangementDbContext;
        }

        [HttpGet("GetAllStudent")]
        public async Task<IActionResult> GetAllStudnet()
        {
            try
            {
                var studentList = await _studentMangementDBContext.Student.ToListAsync();
                return Ok(studentList);
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
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] Student_Model student)
        {

            try
            {
                ApiResponse apiResponse = new ApiResponse();
                bool studentExists = await _studentMangementDBContext.Student.AnyAsync(s => 
                s.FirstName == student.FirstName && s.LastName == student.LastName && s.Email == student.Email);
                if(studentExists)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Student already exists...";

                    return Conflict(apiResponse);
                }

                student.ID = Guid.NewGuid();
                await _studentMangementDBContext.Student.AddAsync(student);
                await _studentMangementDBContext.SaveChangesAsync();
                apiResponse.Success = true;
                apiResponse.Message = "Student Added Sucessfully";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                // For demonstration purposes, let's return a generic error response
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the student. Please try again later."
                };
                // You can also return a specific status code like 500 Internal Server Error
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet("getStudentById/{id:Guid}")]
        public async Task<IActionResult> GetStudentById(Guid id)
        {
            try
            {
                var result = await _studentMangementDBContext.Student.FirstOrDefaultAsync(x => x.ID == id);

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

        [HttpPut("updateStudentById/{id:Guid}")]
        public async Task<ActionResult<Student_Model>> UpdateStudent([FromRoute] Guid id, Student_Model updateStudentModel)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
/*                bool studentExists = await _studentMangementDBContext.Student.AnyAsync(s =>
                s.FirstName == updateStudentModel.FirstName && s.LastName == updateStudentModel.LastName && s.Email == updateStudentModel.Email);
                if (studentExists)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Student data already Exists";
                    return Conflict(apiResponse);
                }
*/
                var student = await _studentMangementDBContext.Student.FindAsync(id);
                if (id != student.ID)
                {
                    return BadRequest("Student Not Match");
                }
                if (student == null)
                {
                    return NotFound($"Student with Id = {id} not found");
                }
                student.ID = id;
                student.FirstName = updateStudentModel.FirstName;
                student.LastName = updateStudentModel.LastName;
                student.Email = updateStudentModel.Email;
                student.Address = updateStudentModel.Address;
                student.phoneNumber = updateStudentModel.phoneNumber;
                student.Education=updateStudentModel.Education;
                student.SchoolName = updateStudentModel.SchoolName;
                student.City = updateStudentModel.City;
                await _studentMangementDBContext.SaveChangesAsync();

                apiResponse.Success = true;
                apiResponse.Message = "Student Updated Successfully";
                return Ok(apiResponse);
                return Ok(apiResponse);  
            }
            catch (Exception ex)
            {
                var apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while adding the student. Please try again later."
                };
                // You can also return a specific status code like 500 Internal Server Error
                return StatusCode(500, apiResponse);
            }
        }

        [HttpDelete("deleteStudentById/{id:Guid}")]
        public async Task<ActionResult<Student_Model>> DeleteStudent(Guid id)
        {
            try
            {
                var studentToDelete = await _studentMangementDBContext.Student.FindAsync(id);
                if (studentToDelete == null)
                {
                    return NotFound($"Student with Id = {id} not found");
                }
                _studentMangementDBContext.Student.Remove(studentToDelete);
                await _studentMangementDBContext.SaveChangesAsync();
                ApiResponse apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.Message = "Student Deleted Sucessfully..";
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
