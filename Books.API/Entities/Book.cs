﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.API.Entities
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Title { get; set; }

        [MaxLength(2500)]
        public string Description { get; set; }

        public Guid AuthorId { get; set; }

        public Author Author { get; set; }




    }
}
