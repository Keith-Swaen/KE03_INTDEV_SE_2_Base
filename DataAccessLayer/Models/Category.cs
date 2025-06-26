using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    // Model voor een categorie, wordt gebruikt om producten te groeperen.
    public class Category
    {
        // Id: Unieke sleutel voor de categorie
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Geeft aan of de categorie actief is
        public bool Active { get; set; } = true;

        // Navigatieproperty, lijst van producten die bij deze categorie horen
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 