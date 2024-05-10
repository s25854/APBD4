using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3;

public class Product
{
    [Required]
    [Key]
    public int IdProduct
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
    public string Description
    {
        get; 
        set;
    }

    [Required]
    [Column(TypeName = "decimal(25,2)")]
    public decimal Price
    {
        get; 
        set;
    }
}