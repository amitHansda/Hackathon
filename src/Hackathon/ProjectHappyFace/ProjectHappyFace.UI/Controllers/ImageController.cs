using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ProjectHappyFace.UI.Models;
using System.Text.RegularExpressions;
using System.Web;
using System.Net.Http.Headers;
using System.Text;

namespace ProjectHappyFace.UI.Controllers
{
    public class ImageController : ApiController
    {
        // GET: api/Image
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        public async Task<IHttpActionResult> Get()
        {
            return Ok(new { something = "HelloWorld"});
        }

        // GET: api/Image/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Image
        //public void Post([FromBody]string value)
        //{

        //}

        [HttpPost]
        public async Task<IHttpActionResult> Post(ImageViewModel model)
        {
            var something = model.encodedData;

            var base64Data = Regex.Match(something, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "012ddb775b0b4285a89e6643cf947757");

            // Request parameters
            queryString["returnFaceId"] = "true";
            queryString["returnFaceLandmarks"] = "false";
            queryString["returnFaceAttributes"] = "age,gender,smile,glasses";
            var uri = "https://api.projectoxford.ai/face/v1.0/detect?" + queryString;

            HttpResponseMessage response;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");
            var stringContent = string.Empty;
            using (var content = new ByteArrayContent(binData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                
            }


            stringContent = await response.Content.ReadAsStringAsync();
            return Ok(new {respone= stringContent });
        }
        // PUT: api/Image/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Image/5
        public void Delete(int id)
        {
        }
    }
}
