using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalStore.Models
{
	public class TvShow
	{
		public int Id { get; set; }
		[StringLength(60, MinimumLength = 3)]
		[Required]
		public string? Title { get; set; }
		public string? Description { get; set; }
		public byte[]? TvShowImage { get; set; }
		[Range(1, 100)]
		[DataType(DataType.Currency)]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal Price { get; set; }
		[Display(Name = "Release Date")]
		[DataType(DataType.Date)]
		public DateTime ReleaseDate { get; set; }
		[Display(Name = "Duration")]
		public int DurationMinutes { get; set; }
		[RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
		[Required]
		[StringLength(30)]
		public string? Genre { get; set; }
		public int InStock { get; set; }
		[RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
		[StringLength(5)]
		[Required]
		public string? Rating { get; set; }
		[Range(1, 5)]
		[Column(TypeName = "decimal(3, 1)")]
		public decimal Reviews { get; set; }
	}
	public class TvShowViewModel
	{
		public List<TvShow>? tvShows { get; set; }
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public byte[]? TvShowImage { get; set; }
		public decimal Price { get; set; }
		public DateTime ReleaseDate { get; set; } // Date when the movie was released
		[Display(Name = "Duration")]
		public int DurationMinutes { get; set; } // Duration of the movie in minutes
		public string? Genre { get; set; } // Genre of the movie (e.g., Action, Drama, Comedy)
		public int InStock { get; set; }
		public string? Rating { get; set; }
		public decimal Reviews { get; set; }
	}
}
