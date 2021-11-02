﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class PostOrderDto
    {
        [Required]
        public int HardwareStoreId { get; set; }
        [Required(ErrorMessage = "Firstname of customer is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname of customer is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Address of customer is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Contact number of customer is required")]
        public string ContactNo { get; set; }
        [Required(ErrorMessage = "Email of customer is required")]
        public string Email { get; set; }
        public bool Deliver { get; set; }

        public List<GetProductToCartDto> Products { get; set; }
    }
}
