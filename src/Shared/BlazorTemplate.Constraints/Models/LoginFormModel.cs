using System.ComponentModel.DataAnnotations;

namespace BlazorTemplate.Constraints.Models;

public class LoginFormModel
{
    [Required]
    public string UserName { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}