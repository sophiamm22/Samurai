﻿using System.ComponentModel.DataAnnotations;

namespace Samurai.Web.Client.ViewModels
{
  public class UsersLogInModel
  {
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}