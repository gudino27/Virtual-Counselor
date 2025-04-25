using System.Collections.Generic;
using System.Linq;
using static BlazorApp1.Components.Pages.Checkout;

namespace BlazorApp1.Services
{
    public class CartService
    {
        public List<CartItem> Items { get; private set; } = new List<CartItem>();
        private readonly List<CartItem> _items = new List<CartItem>(); // Fix: Use a private field to store items

        public CartService()
        {
            // Initialize the cart with some items if needed
            // Items.Add(new CartItem { Name = "Sample Item", Type = "Sample Type", Credits = 3 });
        }


        public void Add(object item)
        {
            if (item != null)
            {
                _items.Add((CartItem)item);
            }
        }
        public void AddItem(CartItem item)
        {
            // Check if item already exists
            var existingItem = Items.FirstOrDefault(i => i.Name == item.Name && i.Type == item.Type);
            if (existingItem == null)
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.Name == item.Name && i.Type == item.Type);
            if (existingItem != null)
            {
                Items.Remove(existingItem);
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int TotalCredits => Items.Sum(i => i.Credits);
    }
}
