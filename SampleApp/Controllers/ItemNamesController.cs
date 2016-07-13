using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using SampleApp.Models;

namespace SampleApp.Controllers
{
    public class ItemNamesController : ApiController
    {
        public string Get(int id)
        {
            var item = ItemsStore.Items.FirstOrDefault(i=>i.Id == id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item.Name;
        }

        public IEnumerable<string> Get()
        {
            return ItemsStore.Items.Select(i=>i.Name);
        }
    }
}
