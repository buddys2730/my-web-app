using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models
{
    public class FileUpload
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}
