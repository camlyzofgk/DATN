using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class DoiSachTamThoiController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: DoiSachTamThoi
        public ActionResult Index()
        {
            return View(db.tbl_DoiSachTamThoi.ToList());
        }

        // GET: DoiSachTamThoi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachTamThoi tbl_DoiSachTamThoi = db.tbl_DoiSachTamThoi.Find(id);
            if (tbl_DoiSachTamThoi == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DoiSachTamThoi);
        }

        // GET: DoiSachTamThoi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoiSachTamThoi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MaLoi,TimeStart,PhanLoaiDSTT_Lon,PhanLoaiDSTT_Nho,NguoiTH,NguoiUpdate,TimeUpdate,LinkFileDinhKem,SoCungSuKien,ChiTietTV,ChiTietTN,TrangThai")] tbl_DoiSachTamThoi tbl_DoiSachTamThoi)
        {
            if (ModelState.IsValid)
            {
                db.tbl_DoiSachTamThoi.Add(tbl_DoiSachTamThoi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_DoiSachTamThoi);
        }

        // GET: DoiSachTamThoi/Edit/5
        public ActionResult Edit(string maLoi)
        {
            if (maLoi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachTamThoi tbl_DoiSachTamThoi = db.tbl_DoiSachTamThoi.Where(x => x.MaLoi == maLoi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_DoiSachTamThoi DSTT = new tbl_DoiSachTamThoi()
            {
                MaLoi = maLoi,
                TimeStart = null
            };
            if (tbl_DoiSachTamThoi != null)
            {
                DSTT.TimeStart = tbl_DoiSachTamThoi.TimeStart;
                DSTT.PhanLoaiDSTT_Lon = tbl_DoiSachTamThoi.PhanLoaiDSTT_Lon;
                DSTT.PhanLoaiDSTT_Nho = tbl_DoiSachTamThoi.PhanLoaiDSTT_Nho;
                DSTT.NguoiTH = tbl_DoiSachTamThoi.NguoiTH;
                DSTT.SoCungSuKien = tbl_DoiSachTamThoi.SoCungSuKien;
                DSTT.ChiTietTN = tbl_DoiSachTamThoi.ChiTietTN;
                DSTT.ChiTietTV = tbl_DoiSachTamThoi.ChiTietTV;
            }
            return View(DSTT);
        }

        // POST: DoiSachTamThoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoi,TimeStart,PhanLoaiDSTT_Lon,PhanLoaiDSTT_Nho,NguoiTH,NguoiUpdate,SoCungSuKien,ChiTietTV,ChiTietTN")] tbl_DoiSachTamThoi tbl_DoiSachTamThoi,
            List<HttpPostedFileBase> files)
        {
            tbl_DoiSachTamThoi.TimeUpdate = DateTime.Now;
            tbl_DoiSachTamThoi.TrangThai = "Hoàn thành";
            string basePath = Server.MapPath("~/Uploads");
            string Folder = Path.Combine(tbl_DoiSachTamThoi.MaLoi, "DoiSachTamThoi");
            string fullPath = Path.Combine(basePath, Folder);
            Directory.CreateDirectory(fullPath);
            if(files != null && files.Count > 0)
            {
                foreach(var file in files)
                {
                    if(file != null)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(fullPath, fileName);
                        file.SaveAs(filePath);
                    }
                }
                tbl_DoiSachTamThoi.LinkFileDinhKem = $"~/Uploads/{tbl_DoiSachTamThoi.MaLoi}/DoiSachTamThoi";
            }
            db.tbl_DoiSachTamThoi.Add(tbl_DoiSachTamThoi);
            tbl_History LSu = new tbl_History()
            {
                MaLoi = tbl_DoiSachTamThoi.MaLoi,
                NguoiUpdate = tbl_DoiSachTamThoi.NguoiUpdate,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Ghi nhập đối sách tạm thời"
            };
            db.tbl_History.Add(LSu);
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == tbl_DoiSachTamThoi.MaLoi).FirstOrDefault();
            detailLoi.TienDo = "Hoàn thành đối sách tạm thời";
            detailLoi.NguoiUpDateNew = tbl_DoiSachTamThoi.NguoiUpdate;
            detailLoi.TimeUpdateNew = DateTime.Now;
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công!";
            return RedirectToAction("Details","DetailLoi",new {id = detailLoi.ID});
        }

        public ActionResult LichSuDSTT (string maloi)
        {
            var lisLS = db.tbl_DoiSachTamThoi.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(lisLS);
        }

        // GET: DoiSachTamThoi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachTamThoi tbl_DoiSachTamThoi = db.tbl_DoiSachTamThoi.Find(id);
            if (tbl_DoiSachTamThoi == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DoiSachTamThoi);
        }

        // POST: DoiSachTamThoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_DoiSachTamThoi tbl_DoiSachTamThoi = db.tbl_DoiSachTamThoi.Find(id);
            db.tbl_DoiSachTamThoi.Remove(tbl_DoiSachTamThoi);
            db.SaveChanges();
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
