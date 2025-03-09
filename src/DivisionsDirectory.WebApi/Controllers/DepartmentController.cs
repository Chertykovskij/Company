using Company.Core.DTO;
using Company.Database.Entities;
using Company.WebApi.Abstractions;
using Company.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Company.WebApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1")]
    public class DepartmentController : Controller
    {
        private readonly IRepository _repository;

        public DepartmentController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("get-departments")]
        [ProducesResponseType(typeof(IEnumerable<Department>), 200)]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _repository.GetDepartments();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        [Route("get-department")]
        [ProducesResponseType(typeof(Department), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _repository.GetDepartment(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpPost]
        [Route("get-department")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(503)]
        public async Task<ActionResult<Department>> CreateDepartment(DepartmentModel department)
        {
            try
            {
                var departmentEntity = department.GetDepartment();
                var idDepartment = await _repository.AddDepartment(departmentEntity);
                return Ok(idDepartment);
            }
            catch (Exception)
            {
                return StatusCode(503, "Department not added");
            }
        }

        // Прочие контроллеры для работы с департаментами
    }
}
