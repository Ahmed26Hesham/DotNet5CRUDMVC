using System.ComponentModel.DataAnnotations;

namespace DotNet5CRUDMVC.Models
{
    public class Movie
    {
        // int => by default auto increament
        public int Id { get; set; }

        [Required , MaxLength(250)]
        public string Title { get; set; }

        // int => by default required
        public int Year { get; set; }

        // double => by default required
        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string StoryLine { get; set; }

        [Required]
        // byte[] for images
        public byte[] Poster { get; set; }

        public byte GenreId { get; set; }

        // Navigation property
        public Genre Genre { get; set; }

    }
}
