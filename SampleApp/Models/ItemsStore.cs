using System.Collections.Generic;

namespace SampleApp.Models
{
    public class ItemsStore
    {
        private static List<Item> _items = new List<Item>
        {
            new Item {Id = 0, Name = "Chainsaw", Price = 999},
            new Item {Id = 1, Name = "Barbie Doll", Price = 20},
            new Item {Id = 2, Name = "Yacht", Price = 50000}
        };

        public static List<Item> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}