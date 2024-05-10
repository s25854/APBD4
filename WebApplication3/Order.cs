using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3;

public class Order
{
    [Required]
    [Key]
    public int IdOrder
    {
        get; 
        init;
    }

    [Required]
    [ForeignKey("Product")]
    public int IdProduct
    {
        get; 
        set;
    }

    [Required]
    public int Amount
    {
        get; 
        set;
    }

    [Required]
    public DateTime CreatedAt
    {
        get; 
        set;
    }

    public DateTime? FulfilledAt
    {
        get; 
        set;
    }
}