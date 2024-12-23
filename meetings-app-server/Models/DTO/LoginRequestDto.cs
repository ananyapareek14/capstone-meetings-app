﻿using System.ComponentModel.DataAnnotations;

namespace meetings_server.Models.DTO;

public class LoginRequestDto
{
    [Required] public string email{  get; set; }
    [Required] public string password { get; set; }
}
