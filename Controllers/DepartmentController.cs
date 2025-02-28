using BackOlSoftware.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackOlSoftware.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _departmentService;

        public DepartmentController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("Departments")]
        public async Task<IActionResult> getDepartments()
        {
            try
            {
                var departments = await _departmentService.getDepartments();

                if (departments == null || departments.Count == 0)
                    return NotFound(new { status = 404, msg = "No se encontraron municipios" });

                return Ok(new { status = 200, msg = "Lista de municipios obtenida con éxito", data = departments });
            }
            catch(Exception e)
            {
                return StatusCode(500, new { status = 500, msg = "Error interno", error = e.Message });
            }
        }

        [HttpGet("municipalities/{department}")]
        public async Task<IActionResult> GetMunicipalitiesByDepartment(string department)
        {
            try
            {
                var municipalities = await _departmentService.GetMunicipalitiesByDepartmentAsync(department);
                if (municipalities == null || !municipalities.Any())
                    return NotFound(new { status = 404, msg = $"No se encontraron municipios en {department}" });

                return Ok(new { status = 200, msg = $"Lista de municipios de {department} obtenida con éxito", data = municipalities });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, msg = "Error interno", error = ex.Message });
            }
        }

    }
}
