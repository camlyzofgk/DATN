using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class NguyenNhanController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: NguyenNhan
        public ActionResult Index()
        {
            return View(db.tbl_NguyenNhan.ToList());
        }

        // GET: NguyenNhan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NguyenNhan tbl_NguyenNhan = db.tbl_NguyenNhan.Find(id);
            if (tbl_NguyenNhan == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NguyenNhan);
        }

        // GET: NguyenNhan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NguyenNhan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MaLoi,PhanLoaiNN_Lon,PhanLoaiNN_Nho,SoNgayClose,DealineCloseNN,DealineCloseDSTT,DealineGhiNhapDSCH,DealinePheDuyetDSCH,DealineGhiNhapHQ,DealinePheDuyetHQ,NguoiUpdate,LinkFileDinhKem,SoCungSuKien,ChiTietTV,ChiTietTN,TimeUpdate,TrangThai")] tbl_NguyenNhan tbl_NguyenNhan)
        {
            if (ModelState.IsValid)
            {
                db.tbl_NguyenNhan.Add(tbl_NguyenNhan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_NguyenNhan);
        }

        // GET: NguyenNhan/Edit/5
        public ActionResult Edit(string maloi)
        {
            if (maloi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NguyenNhan tbl_NguyenNhan = db.tbl_NguyenNhan.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_NguyenNhan NguyenNhan = new tbl_NguyenNhan()
            {
                MaLoi = maloi,
                SoNgayClose = null
            };
            if (tbl_NguyenNhan != null)
            {
                NguyenNhan.PhanLoaiNN_Lon = tbl_NguyenNhan.PhanLoaiNN_Lon;
                NguyenNhan.PhanLoaiNN_Nho = tbl_NguyenNhan.PhanLoaiNN_Nho;
                NguyenNhan.SoNgayClose = tbl_NguyenNhan.SoNgayClose;
                NguyenNhan.SoCungSuKien = tbl_NguyenNhan.SoCungSuKien;
                NguyenNhan.ChiTietTN = tbl_NguyenNhan.ChiTietTN;
                NguyenNhan.ChiTietTV = tbl_NguyenNhan.ChiTietTV;
            }
            return View(NguyenNhan);
        }

        // POST: NguyenNhan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoi,PhanLoaiNN_Lon,PhanLoaiNN_Nho,SoNgayClose,DealineCloseNN,DealineCloseDSTT,DealineGhiNhapDSCH,DealinePheDuyetDSCH,DealineGhiNhapHQ,DealinePheDuyetHQ,NguoiUpdate,SoCungSuKien,ChiTietTV,ChiTietTN")] tbl_NguyenNhan tbl_NguyenNhan,
            List<HttpPostedFileBase> files)
        {
            tbl_NguyenNhan.TimeUpdate = DateTime.Now;
            tbl_NguyenNhan.TrangThai = "Hoàn thành";
            string basePath = Server.MapPath("~/UpLoads");
            string relativeFolder = Path.Combine(tbl_NguyenNhan.MaLoi, "NguyenNhanLoi");
            string fullPath = Path.Combine(basePath, relativeFolder);
            Directory.CreateDirectory(fullPath);
            if (files != null && files.Count > 0)
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
            }
            tbl_NguyenNhan.LinkFileDinhKem = $"~/Update/{tbl_NguyenNhan.MaLoi}/NguyenNhanLoi";
            db.tbl_NguyenNhan.Add(tbl_NguyenNhan);
            tbl_History LSu = new tbl_History()
            {
                MaLoi = tbl_NguyenNhan.MaLoi,
                TimeUpDate = DateTime.Now,
                NguoiUpdate = tbl_NguyenNhan.NguoiUpdate,
                DetailUpdate = "Ghi nhập nguyên nhân"
            };
            db.tbl_History.Add(LSu);
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == tbl_NguyenNhan.MaLoi).FirstOrDefault();
            detailLoi.TienDo = "Hoàn thành điều tra nguyên nhân";
            detailLoi.NguoiUpDateNew = tbl_NguyenNhan.NguoiUpdate;
            detailLoi.TimeUpdateNew = DateTime.Now;
            detailLoi.SoNgayClose = tbl_NguyenNhan.SoNgayClose;
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công!";
            return RedirectToAction("Details","DetailLoi",new {id = detailLoi.ID});
        }

        public ActionResult LichSuNN(string maloi)
        {
            var lisLS = db.tbl_NguyenNhan.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(lisLS);
        }

        // GET: NguyenNhan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NguyenNhan tbl_NguyenNhan = db.tbl_NguyenNhan.Find(id);
            if (tbl_NguyenNhan == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NguyenNhan);
        }

        // POST: NguyenNhan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_NguyenNhan tbl_NguyenNhan = db.tbl_NguyenNhan.Find(id);
            db.tbl_NguyenNhan.Remove(tbl_NguyenNhan);
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
