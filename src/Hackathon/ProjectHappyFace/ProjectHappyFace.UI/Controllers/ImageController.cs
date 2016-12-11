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
using Microsoft.ProjectOxford.Face;
using System.IO;

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
            MemoryStream fileStream = new MemoryStream(binData);
            var faceServiceClient = CognitiveServiceFramework.CreateFaceServiceClient();

            Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(fileStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair });
            return Ok(faces);
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
