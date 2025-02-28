using System.ComponentModel.DataAnnotations;

namespace BackOlSoftware.Models.DTOs
{
    public class MerchantDTO
    {
        [Required(ErrorMessage = "Business name is required")]
        [MaxLength(255, ErrorMessage = "Business name cannot exceed 255 characters")]
        public string BusinessName { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(255, ErrorMessage = "City cannot exceed 255 characters")]
        public string City { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be 'Activo' or 'Inactivo'")]
        public string Status { get; set; } = null!;
    }
}
