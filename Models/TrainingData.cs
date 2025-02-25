using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class TrainingData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Url { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
