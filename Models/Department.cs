using System.Text.Json.Serialization;

namespace BackOlSoftware.Models
{
    public class Department
    {

        [JsonPropertyName("municipio")]
        public string Municipio { get; set; } = null!;

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; } = null!;
    }
}
