﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public Product Product { get; set; }

        public IEnumerable<ProductTag> ProductTag { get; set; }

        public int Count { get; set; }

        public int CaseCount { get; set; }

        public double IndividualPrice { get; set; }

        public double CasePrice { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
