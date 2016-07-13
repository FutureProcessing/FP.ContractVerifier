using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using SampleApp.Models;

namespace SampleApp.Controllers
{
    public class ItemsController : ApiController
    {
        public Item Get(int id)
        {
            var item = ItemsStore.Items.FirstOrDefault(i=>i.Id == id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
        }

        public IEnumerable<Item> Get()
        {
            return ItemsStore.Items;
        }

        public void Post(Item item)
        {
            if (!item.IsValid())
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            ItemsStore.Items.Add(item);
        }

        public void Put(int id, Item item)
        {
            if (!item.IsValid())
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var existingItem = ItemsStore.Items.FirstOrDefault(i => i.Id == id);
            if (existingItem == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            existingItem.Name = item.Name;
            existingItem.Price = item.Price;
        }

        public void Delete(int id)
        {
            var item = ItemsStore.Items.FirstOrDefault(i=>i.Id == id);
            if (item != null)
            {
                ItemsStore.Items.Remove(item);
            }
        }
    }
}
