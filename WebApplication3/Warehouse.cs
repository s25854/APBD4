using System.ComponentModel.DataAnnotations;

namespace WebApplication3;

public class Warehouse
{
    [Required]
    [Key]
    public int IdWarehouse
    {
        get; 
        init;
    }

    [Required]
    [MaxLength(200)]
    public string Name
    {
        get; 
        set;
    }

    [Required]
    [MaxLength(200)]
    public string Address
    {
        get; 
        set;
    }
}