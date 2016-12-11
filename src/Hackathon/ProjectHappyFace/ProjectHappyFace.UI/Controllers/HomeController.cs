using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectHappyFace.UI;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectHappyFace.UI.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public async Task<ActionResult> Index()
        {
            //string path = @"C:\Users\nirotpalm626\Desktop\Hackathon\Cognitive-Face-Windows-master\Cognitive-Face-Windows-master\Data\PersonGroup\Family1-Dad\Family1-Dad1.jpg";
            //var userFaceLayer = new UserFaceDetection();
            //await userFaceLayer.VerifyUserImageForRegister(path);

            //var userDetected = userFaceLayer.GetUserData();
            
            return View();
        }

        public async Task<ActionResult> About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}