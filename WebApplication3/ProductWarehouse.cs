using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3;

public class ProductWarehouse
{
    [Required] 
    [Key]
    public int IdProductWarehouse { 
        get; 
        init; 
    }

    [Required]
    [ForeignKey("Warehouse")]
    public int IdWarehouse
    {
        get; 
        set;
    }

    [Required]
    [ForeignKey("Product")]
    public int IdProduct
    {
        get; 
        set;
    }

    [Required]
    [ForeignKey("Order")]
    public int IdOrder
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
    [Column(TypeName = "decimal(25,2)")]
    public decimal Price
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
}