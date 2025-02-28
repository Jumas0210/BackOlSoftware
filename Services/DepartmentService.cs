using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using BackOlSoftware.Models;

namespace BackOlSoftware.Services
{
    public class DepartmentService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://www.datos.gov.co/resource/xdk5-pm3f.json";


        public DepartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> getDepartments()
        {
            try
            {
                var response = await _httpClient.GetStreamAsync(ApiUrl);

                var locations = JsonSerializer.Deserialize<List<Department>>(response) ?? new List<Department>();

                var departments = locations
                    .Where(l => !string.IsNullOrEmpty(l.Departamento))
                    .Select(l => l.Departamento)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                return departments;
            }
            catch(Exception e)
            {
                throw new Exception("Error al obtener los municipios", e);
            }
        }

        public async Task<List<string>> GetMunicipalitiesByDepartmentAsync(string department)
        {
            var cacheKey = $"municipalities_{department}";

            
            try
            {
                var url = $"{ApiUrl}?departamento={Uri.EscapeDataString(department)}";
                var response = await _httpClient.GetStringAsync(url);
                var locations = JsonSerializer.Deserialize<List<Department>>(response) ?? new List<Department>();

                var municipalities = locations
                    .Where(l => !string.IsNullOrEmpty(l.Municipio))
                    .Select(l => l.Municipio)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                return municipalities;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener municipios de {department}", ex);
            }
        }

    }
}
