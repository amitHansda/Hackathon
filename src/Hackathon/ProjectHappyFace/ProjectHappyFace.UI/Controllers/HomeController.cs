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
        public ActionResult Index()
        {
            var a = new UserFaceDetection();
           // await a.AddImageToUser();
            //Task task = new Task(a.DetectUserImage);
            //task.Start();
            //task.Wait();

            a.VerifyUserImageForRegister();
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}