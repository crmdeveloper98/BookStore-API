﻿using System.ComponentModel.DataAnnotations;

namespace BookStore_UI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int? Year { get; set; }

        public string Isbn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public string Image { get; set; }

        public decimal? Price { get; set; }

        public int? AuthorId { get; set; }

        public virtual Author Author { get; set; }
    }
}