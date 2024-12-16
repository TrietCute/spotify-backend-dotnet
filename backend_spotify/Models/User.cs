using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }

    public required string Password { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PersonalId { get; set; }

}
