using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Api.Models
{
    public class FileUploadModel
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
