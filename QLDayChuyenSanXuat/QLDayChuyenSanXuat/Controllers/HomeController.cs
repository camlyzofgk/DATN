using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLDayChuyenSanXuat.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult TrangChu() 
        {
            return View();
        }

        public ActionResult PhatHanhLoi()
        {
            return View();
        }

        public ActionResult DetailLoi() 
        { 
            return View();
        }

        public ActionResult EditHienTuong()
        {
            return View();
        }

        public ActionResult EditNguyenNhan()
        {
            return View();
        }

        public ActionResult EditDoiSachTamThoi()
        {
            return View();
        }

        public ActionResult EditDoiSachCoHuu()
        {
            return View();
        }

        public ActionResult EditHieuQuaDoiSach()
        {
            return View();
        }

        public ActionResult NhapKho()
        {
            return View();
        }

        public ActionResult XuatKho()
        {
            return View();
        }

        public ActionResult Index()
        {
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