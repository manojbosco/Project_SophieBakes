using Project_SophieBakes.Models;

public class Cart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
}