using System.ComponentModel.DataAnnotations;

namespace TryBets.Matches.DTO;
public class TeamDTOResponse
{
  [Required]
  public int TeamId { get; set; }
  [Required]
  public string? TeamName { get; set; }
}