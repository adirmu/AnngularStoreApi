using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using AnngularStoreApi.Data;
using JWT.Algorithms;
using JWT.Builder;

namespace AnngularStoreApi.Controllers {
    public class HomeController : Controller {
        static MockDB db = MockDB.getInstance();

        public ActionResult Index() {
            return View(db.Users);
        }

    }
}
