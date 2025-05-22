using System.ComponentModel.DataAnnotations;

namespace FocusBot.Data.Models;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}