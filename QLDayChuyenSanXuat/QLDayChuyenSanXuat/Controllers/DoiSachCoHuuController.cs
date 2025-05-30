using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class DoiSachCoHuuController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: DoiSachCoHuu
        public ActionResult Index()
        {
            return View(db.tbl_DoiSachCoHuu.ToList());
        }

        // GET: DoiSachCoHuu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachCoHuu tbl_DoiSachCoHuu = db.tbl_DoiSachCoHuu.Find(id);
            if (tbl_DoiSachCoHuu == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DoiSachCoHuu);
        }

        // GET: DoiSachCoHuu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoiSachCoHuu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MaLoi,TimeStart,PhanLoaiDSCH_Lon,PhanLoaiDSCH_Nho,NguoiXN,NguoiTH,NguoiUpDate,TimeUpDate,LinkFileDinhKem,SoCungSuKien,ChiTietTV,ChiTietTN,TrangThai")] tbl_DoiSachCoHuu tbl_DoiSachCoHuu)
        {
            if (ModelState.IsValid)
            {
                db.tbl_DoiSachCoHuu.Add(tbl_DoiSachCoHuu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_DoiSachCoHuu);
        }

        // GET: DoiSachCoHuu/Edit/5
        public ActionResult Edit(string maloi)
        {
            if (maloi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachCoHuu tbl_DoiSachCoHuu = db.tbl_DoiSachCoHuu.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_DoiSachCoHuu DSCH = new tbl_DoiSachCoHuu()
            {
                MaLoi = maloi
            };
            if (tbl_DoiSachCoHuu != null)
            {
                DSCH.TimeStart = tbl_DoiSachCoHuu.TimeStart;
                DSCH.PhanLoaiDSCH_Lon = tbl_DoiSachCoHuu.PhanLoaiDSCH_Lon;
                DSCH.PhanLoaiDSCH_Nho = tbl_DoiSachCoHuu.PhanLoaiDSCH_Nho;
                DSCH.NguoiTH = tbl_DoiSachCoHuu.NguoiTH;
                DSCH.NguoiXN = tbl_DoiSachCoHuu .NguoiXN;
                DSCH.SoCungSuKien = tbl_DoiSachCoHuu.SoCungSuKien;
                DSCH.ChiTietTN = tbl_DoiSachCoHuu.ChiTietTN;
                DSCH.ChiTietTV = tbl_DoiSachCoHuu.ChiTietTV;
            }
            return View(DSCH);
        }

        // POST: DoiSachCoHuu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoi,TimeStart,PhanLoaiDSCH_Lon,PhanLoaiDSCH_Nho,NguoiXN,NguoiTH,NguoiUpDate,SoCungSuKien,ChiTietTV,ChiTietTN")] tbl_DoiSachCoHuu tbl_DoiSachCoHuu,
            List<HttpPostedFileBase>files)
        {
            tbl_DoiSachCoHuu.TimeUpDate = DateTime.Now;
            tbl_DoiSachCoHuu.TrangThai = "Chờ phê duyệt";
            string basePath = Server.MapPath("~/Uploads");
            string Folder = Path.Combine(tbl_DoiSachCoHuu.MaLoi, "DoiSachCoHuu");
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
                tbl_DoiSachCoHuu.LinkFileDinhKem = $"~/Uploads/{tbl_DoiSachCoHuu.MaLoi}/DoiSachCoHuu";
            }
            db.tbl_DoiSachCoHuu.Add(tbl_DoiSachCoHuu);
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == tbl_DoiSachCoHuu.MaLoi).FirstOrDefault();
            detailLoi.NguoiUpDateNew = tbl_DoiSachCoHuu.NguoiUpDate;
            detailLoi.TimeUpdateNew = DateTime.Now;
            detailLoi.TienDo = "Chờ phê duyệt đối sách cố hữu";
            tbl_History Lsu = new tbl_History()
            {
                MaLoi = tbl_DoiSachCoHuu.MaLoi,
                NguoiUpdate = tbl_DoiSachCoHuu.NguoiUpDate,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Ghi nhập đối sách cố hữu"
            };
            db.tbl_History.Add(Lsu);
            TempData["ThongBao"] = "Chỉnh sửa thành công!";
            db.SaveChanges();
            return RedirectToAction("Details", "DetailLoi", new {id = detailLoi.ID});
        }
        public ActionResult PheDuyetOk(string maloi)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            var DSCH = db.tbl_DoiSachCoHuu.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_DoiSachCoHuu LSu_DSCH = new tbl_DoiSachCoHuu()
            {
                MaLoi = DSCH.MaLoi,
                TrangThai = "Hoàn thành",
                TimeStart = DSCH.TimeStart,
                PhanLoaiDSCH_Lon = DSCH.PhanLoaiDSCH_Lon,
                PhanLoaiDSCH_Nho = DSCH.PhanLoaiDSCH_Nho,
                NguoiTH = DSCH.NguoiTH,
                NguoiUpDate = detailLoi.NguoiDNPheDuyetDS,
                NguoiXN = DSCH.NguoiXN,
                LinkFileDinhKem = DSCH.LinkFileDinhKem,
                SoCungSuKien = DSCH.SoCungSuKien,
                ChiTietTN = DSCH.ChiTietTN,
                ChiTietTV = DSCH.ChiTietTV,
                TimeUpDate = DateTime.Now
            };
            db.tbl_DoiSachCoHuu.Add(LSu_DSCH);
            detailLoi.NguoiUpDateNew = detailLoi.NguoiDNPheDuyetDS;
            detailLoi.TimeUpdateNew = DateTime.Now;
            detailLoi.TienDo = "Hoàn thành đối sách cố hữu";
            tbl_History LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = detailLoi.NguoiDNPheDuyetDS,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Phê duyệt đối sách cố hữu : OK"
            };
            db.tbl_History.Add(LSu);
            db.SaveChanges();
            TempData["ThongBao"] = "Phê duyệt thành công!";
            return RedirectToAction("Details", "DetailLoi", new { id = detailLoi.ID});
        }
        public ActionResult PheDuyetNG(string maloi, string LyDo)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            detailLoi.NguoiUpDateNew = detailLoi.NguoiDNPheDuyetDS;
            detailLoi.TimeUpdateNew = DateTime.Now;
            tbl_History LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = detailLoi.NguoiDNPheDuyetDS,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Phê duyệt đối sách cố hữu : NG"
            };
            db.tbl_History.Add(LSu);
            db.SaveChanges();
            var nguoinhan = db.tbl_User.Where(x => x.ADID == detailLoi.NguoiDNGhiNhapDSCH).FirstOrDefault();
            MailMessage mail = new MailMessage();
            mail.To.Add(nguoinhan.Mail);
            mail.Subject = "Thông báo phê duyệt NG";
            mail.Body = $@"
                <div style='margin-top:16px;'>
                <h3>Thông báo phê duyệt NG đối sách cố hữu lỗi {detailLoi.Maloi}</h3>
                <br />
                <span style='font-size:16px;'>Người phê duyệt: {detailLoi.NguoiDNPheDuyetDS}</span>
                <br />
                <span style='font-size:16px;'>Lý do:</span>
                <p style='font-size:16px;'>{LyDo}</p>
                <p>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
                <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
                </div>            
            ";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
            TempData["ThongBao"] = "Phê duyệt thành công!";
            return RedirectToAction("Details", "DetailLoi", new { id = detailLoi.ID });

        }

        public ActionResult LichSuDSCH(string maloi)
        {
            var lisLS = db.tbl_DoiSachCoHuu.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(lisLS);
        }

        // GET: DoiSachCoHuu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DoiSachCoHuu tbl_DoiSachCoHuu = db.tbl_DoiSachCoHuu.Find(id);
            if (tbl_DoiSachCoHuu == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DoiSachCoHuu);
        }

        // POST: DoiSachCoHuu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_DoiSachCoHuu tbl_DoiSachCoHuu = db.tbl_DoiSachCoHuu.Find(id);
            db.tbl_DoiSachCoHuu.Remove(tbl_DoiSachCoHuu);
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
