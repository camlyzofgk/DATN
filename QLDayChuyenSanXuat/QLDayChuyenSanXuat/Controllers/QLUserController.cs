using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class QLUserController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: QLUser
        public ActionResult Index()
        {
            return View(db.tbl_User.ToList());
        }

        // GET: QLUser/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_User tbl_User = db.tbl_User.Find(id);
            if (tbl_User == null)
            {
                return HttpNotFound();
            }
            return View(tbl_User);
        }

        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string ADID, string Password)
        {
            var user = db.tbl_User.Where(x => x.ADID == ADID && x.Password == Password).FirstOrDefault();
            if(user == null)
            {
                ViewBag.loi = "Sai ADID hoặc mật khẩu";
                return View("Login");
            }
            else
            {
                Session["ADID"] = ADID;
                TempData["LoginSuccess"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "DetailLoi");
            }
        }

        public ActionResult Logout()
        {
            Session["ADID"] = null;
            return RedirectToAction("Login", "QLUser");
        }

        // GET: QLUser/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QLUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ADID,Name,Mail,MaPB,Role_PhatHanhLoi,Role_PheDuyet,Role_QuanLyUser,Role_XuatKho,Role_NhapKho,Role_KiemKe,Password")] tbl_User tbl_User)
        {
            if (ModelState.IsValid)
            {
                db.tbl_User.Add(tbl_User);
                db.SaveChanges();
                TempData["ThongBao"] = "Tạo mới thành công!";
                return RedirectToAction("Index","QLUser");
            }

            return View(tbl_User);
        }

        // GET: QLUser/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_User tbl_User = db.tbl_User.Find(id);
            if (tbl_User == null)
            {
                return HttpNotFound();
            }
            return View(tbl_User);
        }

        // POST: QLUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ADID,Name,Mail,MaPB,Role_PhatHanhLoi,Role_PheDuyet,Role_QuanLyUser,Role_XuatKho,Role_NhapKho,Role_KiemKe,Password")] tbl_User tbl_User)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_User).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Cập nhật thành công!";
                return RedirectToAction("Index", "QLUser");
            }
            return View(tbl_User);
        }

        // GET: QLUser/Delete/5
       
        public ActionResult Delete(string id)
        {
            tbl_User tbl_User = db.tbl_User.Find(id);
            db.tbl_User.Remove(tbl_User);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa thành công!";
            return RedirectToAction("Index");
        }

        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tbl_User tbl_User = db.tbl_User.Find(id);
        //    if (tbl_User == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tbl_User);
        //}
        // POST: QLUser/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    tbl_User tbl_User = db.tbl_User.Find(id);
        //    db.tbl_User.Remove(tbl_User);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
