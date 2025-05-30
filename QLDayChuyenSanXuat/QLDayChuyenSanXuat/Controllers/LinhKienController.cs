using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class LinhKienController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: LinhKien
        public ActionResult Index(string MaLinhKien)
        {
            if(MaLinhKien != null && MaLinhKien != "")
            {
                var lk = db.LinhKiens.Where(x => x.MaLinhKien == MaLinhKien).ToList();
                return View(lk);
            }
            return View(db.LinhKiens.ToList());
        }

        // GET: LinhKien/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LinhKien linhKien = db.LinhKiens.Find(id);
            if (linhKien == null)
            {
                return HttpNotFound();
            }
            return View(linhKien);
        }

        // GET: LinhKien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LinhKien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLinhKien,TênLinhKien,SoLuong,ViTriLuuTru,NhaCungCap")] LinhKien linhKien)
        {
            if (ModelState.IsValid)
            {
                db.LinhKiens.Add(linhKien);
                db.SaveChanges();
                TempData["ThongBao"] = "Tạo linh kiện mới thành công!";
                return RedirectToAction("Index");
            }

            return View(linhKien);
        }

        // GET: LinhKien/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LinhKien linhKien = db.LinhKiens.Find(id);
            if (linhKien == null)
            {
                return HttpNotFound();
            }
            return View(linhKien);
        }

        public ActionResult DSPhieuKK()
        {
            var dsPhieu = db.tbl_KiemKe.Select(x => x.SoPhieu).Distinct().ToList();
            return View(dsPhieu);
        }

        [HttpGet] 
        public ActionResult KiemKe(string sophieu)
        {
            ViewBag.SoPhieu = sophieu;
            if(!string.IsNullOrEmpty(sophieu))
            {
                var kiemke = db.tbl_KiemKe.Where(x => x.SoPhieu == sophieu);
                return View(kiemke.ToList());
            }
            return View(new List<tbl_KiemKe>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LuuKiemKe(string sophieu, FormCollection form)
        {
            string action = form["action"]; // VD: OK_LK001
            if (!string.IsNullOrEmpty(action))
            {
                var part = action.Split('_');
                string trangThai = part[0];
                string maLK = part[1];
                string ghiChu = form["GhiChu_"+maLK];
                string nguoiKK = form["NguoiKiemKe_"+maLK];
                var sLg = form["SoLuongThucTe_"+maLK];
                int soLg = int.Parse(sLg);
                db.tbl_KiemKe.Add(new tbl_KiemKe
                {
                    SoPhieu = sophieu,
                    MaLinhKien = maLK,
                    SoLuongThucTe = soLg,
                    NguoiKiemKe = nguoiKK,
                    GhiChu = ghiChu,
                    TrangThai = trangThai,
                    NgayKiemKe = DateTime.Now
                });
                var linhKien = db.LinhKiens.Where(x => x.MaLinhKien == maLK).FirstOrDefault();
                linhKien.SoLuong = soLg;
            }
            db.SaveChanges();
            TempData["ThongBao"] = "Kiểm kê thành công!";
            return RedirectToAction("KiemKe", new { sophieu = sophieu });
        }
        // POST: LinhKien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLinhKien,TênLinhKien,SoLuong,ViTriLuuTru,NhaCungCap")] LinhKien linhKien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(linhKien).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            return View(linhKien);
        }

        // GET: LinhKien/Delete/5
        public ActionResult Delete(string id)
        {
            LinhKien linhKien = db.LinhKiens.Find(id);
            db.LinhKiens.Remove(linhKien);
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa linh kiện thành công!";
            return RedirectToAction("Index");
        }

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
