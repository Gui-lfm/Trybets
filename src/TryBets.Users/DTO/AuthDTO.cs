using System.ComponentModel.DataAnnotations;

namespace TryBets.Users.DTO;

public class AuthDTORequest
{
  [Required]
  public string? Email { get; set; }
  [Required]
  public string? Password { get; set; }
}

public class AuthDTOResponse
{
  public string? Token { get; set; }
}