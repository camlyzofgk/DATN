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
    public class HienTuongController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: HienTuong
        public ActionResult Index()
        {
            return View(db.tbl_HienTuong.ToList());
        }

        // GET: HienTuong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_HienTuong tbl_HienTuong = db.tbl_HienTuong.Find(id);
            if (tbl_HienTuong == null)
            {
                return HttpNotFound();
            }
            return View(tbl_HienTuong);
        }

        // GET: HienTuong/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HienTuong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MaLoi,PhanCap,Model,LoaiMay,TieuDeTV,TieuDeTN,ThoiDiemPhatSinh,ThoiDiemBatDauLai,PhanLoaiHT_Lon,PhanLoaiHT_Nho,NguoiXNHTLoi,DetailTV,DetailTN,LinkDinhKemFile,SoCungSuKien,TrangThai,NguoiUpdate,TimeUpdate")] tbl_HienTuong tbl_HienTuong)
        {
            if (ModelState.IsValid)
            {
                db.tbl_HienTuong.Add(tbl_HienTuong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_HienTuong);
        }

        // GET: HienTuong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_HienTuong tbl_HienTuong = db.tbl_HienTuong.Find(id);
            if (tbl_HienTuong == null)
            {
                return HttpNotFound();
            }
            return View(tbl_HienTuong);
        }

        // POST: HienTuong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MaLoi,PhanCap,Model,LoaiMay,TieuDeTV,TieuDeTN,ThoiDiemPhatSinh,ThoiDiemBatDauLai,PhanLoaiHT_Lon,PhanLoaiHT_Nho,NguoiXNHTLoi,DetailTV,DetailTN,SoCungSuKien,NguoiUpdate")] tbl_HienTuong tbl_HienTuong, List<HttpPostedFileBase> files)
        {
            tbl_HienTuong.TimeUpdate = DateTime.Now;
            tbl_HienTuong.TrangThai = "Hoàn thành";
            string basePath = Server.MapPath("~/Uploads");
            string relativeFolder = Path.Combine(tbl_HienTuong.MaLoi, "HienTuongLoi");
            string fullPath = Path.Combine(basePath, relativeFolder);
            Directory.CreateDirectory(fullPath);
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(fullPath, fileName);
                        file.SaveAs(filePath);
                    }
                }

                tbl_HienTuong.LinkDinhKemFile = $"~/Uploads/{tbl_HienTuong.MaLoi}/HienTuongLoi/";
            }

            // Cập nhật lại bản ghi với các trường bổ sung
            db.tbl_HienTuong.Add(tbl_HienTuong);
            var DetailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == tbl_HienTuong.MaLoi).FirstOrDefault();
            DetailLoi.PhanCap = tbl_HienTuong.PhanCap;
            DetailLoi.Model = tbl_HienTuong.Model;
            DetailLoi.LoaiMay = tbl_HienTuong.LoaiMay;
            DetailLoi.TieuDeTN = tbl_HienTuong.TieuDeTN;
            DetailLoi.TieuDeTV = tbl_HienTuong.TieuDeTV;
            DetailLoi.ThoiDiemBatDauLai = tbl_HienTuong.ThoiDiemBatDauLai;
            DetailLoi.ThoiDiemPhatSinh = tbl_HienTuong.ThoiDiemPhatSinh;
            DetailLoi.PhanLoaiHT_Lon = tbl_HienTuong.PhanLoaiHT_Lon;
            DetailLoi.PhanLoaiHT_Nho = tbl_HienTuong.PhanLoaiHT_Nho;
            DetailLoi.NguoiXNHTLoi = tbl_HienTuong.NguoiXNHTLoi;
            DetailLoi.DetailTN = tbl_HienTuong.DetailTN;
            DetailLoi.DetailTV = tbl_HienTuong.DetailTV;
            DetailLoi.SoCungSuKien = tbl_HienTuong.SoCungSuKien;
            DetailLoi.NguoiUpDateNew = tbl_HienTuong.NguoiUpdate;
            DetailLoi.TimeUpdateNew = DateTime.Now;

            tbl_History history = new tbl_History()
            {
                MaLoi = tbl_HienTuong.MaLoi,
                NguoiUpdate = tbl_HienTuong.NguoiUpdate,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Cập nhật hiện tượng lỗi"
            };
            db.tbl_History.Add(history);
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công!";
            return RedirectToAction("Details", "DetailLoi", new {id = DetailLoi.ID});
        }

        public ActionResult LichSuHT (string maloi)
        {
            var lisHt = db.tbl_HienTuong.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(lisHt);
        }

        // GET: HienTuong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_HienTuong tbl_HienTuong = db.tbl_HienTuong.Find(id);
            if (tbl_HienTuong == null)
            {
                return HttpNotFound();
            }
            return View(tbl_HienTuong);
        }

        // POST: HienTuong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_HienTuong tbl_HienTuong = db.tbl_HienTuong.Find(id);
            db.tbl_HienTuong.Remove(tbl_HienTuong);
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
