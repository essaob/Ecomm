using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecomm.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        [Required]
		[Display(Name = "Old Price")]
		public double OldPrice { get; set; }
        [Required]
        public double Quantity { get; set; }
        [Required]
        public double Price { get; set; }
		[ValidateNever]
        [Display(Name = "Image")]
		public string? ImageUrl { get; set; }
		[ValidateNever]
        [Display(Name="Category")]
		public int CategoryId { get; set; }
        [ValidateNever]
        public Category? Category { get; set; }

        public string? Slug {  get; set; }
        public int? Views { get; set; }
        public string? Color { get; set; }
    }
}
