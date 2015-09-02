using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using PaperTrail;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class DocumentsController : Controller
    {
        private readonly StorageProvider _storageProvider;

        public DocumentsController(StorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        //[Produces("application/json", Type = typeof())]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file == null)
            {
                return new HttpNotFoundResult();
            }

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            var fileName = parsedContentDisposition.FileName.Replace("\"", "");
            var uri = await _storageProvider.UploadAsync("test", fileName, file.OpenReadStream());

            return Created(Request.Path, uri.AbsoluteUri);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
