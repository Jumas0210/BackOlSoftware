using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackOlSoftware.Models;
using BackOlSoftware.Models.DTOs;
using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace BackOlSoftware.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MerchantController : ControllerBase
{
    private readonly OlSoftwareContext _dbContext;

    public MerchantController(OlSoftwareContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllMerchants(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? name = null,
        [FromQuery] string? status = null)
    {
        var query = _dbContext.Merchants.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(m => m.BusinessName.Contains(name));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(m => m.Status == status);

        var totalRecords = await query.CountAsync();
        var merchants = await query
            .OrderBy(m => m.MerchantId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { status = 200, msg = "Comerciantes encontrados", totalRecords, data = merchants });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetMerchantById(int id)
    {
        var merchant = await _dbContext.Merchants.FindAsync(id);
        if (merchant == null)
            return NotFound(new { status = 404, msg = "Comerciante no encontrado" });

        return Ok(new { status = 200, msg = "Comerciante encontrado", data = merchant });
    }


    [HttpPost]
    public async Task<IActionResult> CreateMerchant([FromBody] MerchantDTO merchantDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserIdFromToken();

        var newMerchant = new Merchant
        {
            BusinessName = merchantDto.BusinessName,
            City = merchantDto.City,
            Phone = merchantDto.Phone,
            Email = merchantDto.Email,
            Status = merchantDto.Status,
            UpdatedByUser = userId
        };

        _dbContext.Merchants.Add(newMerchant);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMerchantById), new { id = newMerchant.MerchantId },
            new { status = 201, msg = "Comerciante creado con éxito", data = newMerchant });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMerchant(int id, [FromBody] MerchantDTO merchantDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserIdFromToken();

        var merchant = await _dbContext.Merchants.FindAsync(id);
        if (merchant == null)
            return NotFound(new { status = 404, msg = "Comerciante no encontrado" });

        merchant.BusinessName = merchantDto.BusinessName;
        merchant.City = merchantDto.City;
        merchant.Phone = merchantDto.Phone;
        merchant.Email = merchantDto.Email;
        merchant.Status = merchantDto.Status;
        merchant.UpdatedByUser = userId;

        await _dbContext.SaveChangesAsync();
        return Ok(new { status = 200, msg = "Comerciante actualizado con éxito", data = merchant });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteMerchant(int id)
    {
        var merchant = await _dbContext.Merchants.FindAsync(id);
        if (merchant == null)
            return NotFound(new { status = 404, msg = "Comerciante no encontrado" });

        _dbContext.Merchants.Remove(merchant);
        await _dbContext.SaveChangesAsync();
        return Ok(new { status = 200, msg = "Comerciante eliminado con éxito" });
    }


    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateMerchantStatus(int id, [FromBody] string status)
    {
        var merchant = await _dbContext.Merchants.FindAsync(id);
        if (merchant == null)
            return NotFound(new { status = 404, msg = "Comerciante no encontrado" });

        if (status != "Activo" && status != "Inactivo")
            return BadRequest(new { status = 400, msg = "Estado inválido" });

        merchant.Status = status;
        await _dbContext.SaveChangesAsync();

        return Ok(new { status = 200, msg = "Estado actualizado con éxito", data = merchant });
    }

    [HttpGet("report")]
    [Authorize(Roles = "Administrador")] 
    public async Task<IActionResult> GenerateMerchantReport()
    {
        try
        {
            
            var reportData = await _dbContext.MerchantReports
                .FromSqlRaw("EXEC GetActiveMerchantsReport")
                .ToListAsync();

            if (!reportData.Any())
                return NotFound(new { status = 404, msg = "No hay comerciantes activos para el reporte" });

           
            var csv = new StringBuilder();
            csv.AppendLine("Business Name|City|Phone|Email|Registration Date|Status|Establishment Count|Total Revenue|Total Employees");

            foreach (var merchant in reportData)
            {
                csv.AppendLine($"{merchant.BusinessName}|{merchant.City}|{merchant.Phone}|{merchant.Email}|{merchant.RegistrationDate:yyyy-MM-dd}|{merchant.Status}|{merchant.EstablishmentCount}|{merchant.TotalRevenue.ToString("F2", CultureInfo.InvariantCulture)}|{merchant.TotalEmployees}");
            }

            
            var fileName = $"MerchantReport_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(fileBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, msg = "Error interno al generar el reporte", error = ex.Message });
        }
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }
}
