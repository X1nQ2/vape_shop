using System.ComponentModel;
using System.Linq;

namespace vape_shop
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal Sum { get { return Price * Qty; } }
    }

    static class Cart
    {
        public static BindingList<CartItem> Items = new BindingList<CartItem>();

        public static decimal Total { get { return Items.Sum(x => x.Sum); } }
        public static int Count { get { return Items.Sum(x => x.Qty); } }

        public static int QtyOf(int id)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            return item == null ? 0 : item.Qty;
        }

        public static void Add(int id, string name, decimal price, int qty)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
                Items.Add(new CartItem { Id = id, Name = name, Price = price, Qty = qty });
            else
            {
                item.Qty += qty;
                Items.ResetBindings();
            }
        }

        public static void ChangeQty(CartItem item, int delta)
        {
            if (item == null) return;
            int newQty = item.Qty + delta;
            if (newQty <= 0)
                Items.Remove(item);
            else
                item.Qty = newQty;
            Items.ResetBindings();
        }
    }
}
