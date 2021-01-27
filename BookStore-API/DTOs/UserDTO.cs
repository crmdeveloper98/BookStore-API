﻿using System.ComponentModel.DataAnnotations;

namespace BookStore_API.DTOs
{
    public class UserDTO
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Your Password is limited to {2} to {1}", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
