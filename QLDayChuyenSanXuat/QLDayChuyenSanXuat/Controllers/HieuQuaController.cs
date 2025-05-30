using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class HieuQuaController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: HieuQua
        public ActionResult Index()
        {
            return View(db.tbl_HieuQua.ToList());
        }

        // GET: HieuQua/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_HieuQua tbl_HieuQua = db.tbl_HieuQua.Find(id);
            if (tbl_HieuQua == null)
            {
                return HttpNotFound();
            }
            return View(tbl_HieuQua);
        }

        // GET: HieuQua/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HieuQua/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MaLoi,TimeDoneHQ,NguoiXN,NguoiUpdate,TimeUpdate,LinkFileDinhKem,SoCungSuKien,ChiTietTV,ChiTietTN,TrangThai")] tbl_HieuQua tbl_HieuQua)
        {
            if (ModelState.IsValid)
            {
                db.tbl_HieuQua.Add(tbl_HieuQua);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_HieuQua);
        }

        // GET: HieuQua/Edit/5
        public ActionResult Edit(string maloi)
        {
            if (maloi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbl_HieuQua = db.tbl_HieuQua.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_HieuQua HQ = new tbl_HieuQua()
            {
                MaLoi = maloi
            };
            if (tbl_HieuQua != null)
            {
                HQ.TimeDoneHQ = tbl_HieuQua.TimeDoneHQ;
                HQ.NguoiXN = tbl_HieuQua.NguoiXN;
                HQ.SoCungSuKien = tbl_HieuQua.SoCungSuKien;
                HQ.ChiTietTN = HQ.ChiTietTN;
                HQ.ChiTietTV = tbl_HieuQua.ChiTietTV;
            }
            return View(HQ);
        }

        // POST: HieuQua/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoi,TimeDoneHQ,NguoiXN,NguoiUpdate,SoCungSuKien,ChiTietTV,ChiTietTN")] tbl_HieuQua tbl_HieuQua,
            List<HttpPostedFileBase> files)
        {
            tbl_HieuQua.TrangThai = "Chờ phê duyệt";
            tbl_HieuQua.TimeUpdate = DateTime.Now;
            string basePath = Server.MapPath("~/Uploads");
            string Folder = Path.Combine(tbl_HieuQua.MaLoi, "HieuQua");
            string fullPath = Path.Combine(basePath,Folder);
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
                tbl_HieuQua.LinkFileDinhKem = $"~/Uploads/{tbl_HieuQua.MaLoi}/HieuQua";
            }
            db.tbl_HieuQua.Add(tbl_HieuQua);
            var detaiLoi = db.tbl_DetailLoi.Where(x => x.Maloi == tbl_HieuQua.MaLoi).FirstOrDefault();
            detaiLoi.TienDo = "Chờ phê duyệt hiệu quả";
            detaiLoi.NguoiUpDateNew = tbl_HieuQua.NguoiUpdate;
            detaiLoi.TimeUpdateNew = DateTime.Now;
            tbl_History LSu = new tbl_History()
            {
                MaLoi = tbl_HieuQua.MaLoi,
                NguoiUpdate = tbl_HieuQua.NguoiUpdate,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Ghi nhập hiệu quả đối sách"
            };
            db.tbl_History.Add(LSu);
            TempData["ThongBao"] = "Chỉnh sửa thành công!";
            db.SaveChanges();
            return RedirectToAction("Details", "DetailLoi", new {id = detaiLoi.ID});
        }
        public ActionResult PheDuyetOK(string maloi)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            var HQ = db.tbl_HieuQua.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).FirstOrDefault();
            tbl_HieuQua Lsu_HQ = new tbl_HieuQua()
            {
                MaLoi = HQ.MaLoi,
                TimeDoneHQ = HQ.TimeDoneHQ,
                NguoiXN = HQ.NguoiXN,
                LinkFileDinhKem = HQ.LinkFileDinhKem,
                SoCungSuKien = HQ.SoCungSuKien,
                ChiTietTN = HQ.ChiTietTN,
                ChiTietTV = HQ.ChiTietTV,
                TrangThai = "Hoàn thành",
                TimeUpdate = DateTime.Now,
                NguoiUpdate = detailLoi.NguoiDNPheDuyetHQ
            };
            db.tbl_HieuQua.Add(Lsu_HQ);
            detailLoi.TienDo = "Close";
            detailLoi.NguoiUpDateNew = detailLoi.NguoiDNPheDuyetHQ;
            detailLoi.TimeUpdateNew = DateTime.Now;
            var LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = detailLoi.NguoiDNPheDuyetHQ,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Phê duyệt hiệu quả đối sách:OK"
            };
            db.tbl_History.Add(LSu);
            db.SaveChanges();
            TempData["ThongBao"] = "Phê duyệt thành công!";
            return RedirectToAction("Details","DetailLoi",new {id = detailLoi.ID});
        }

        public ActionResult PheDuyetNG(string maloi, string LyDo)
        {
            var detailLoi = db.tbl_DetailLoi.Where(x => x.Maloi == maloi).FirstOrDefault();
            detailLoi.NguoiUpDateNew = detailLoi.NguoiDNPheDuyetHQ;
            detailLoi.TimeUpdateNew = DateTime.Now;
            var LSu = new tbl_History()
            {
                MaLoi = maloi,
                NguoiUpdate = detailLoi.NguoiDNPheDuyetHQ,
                TimeUpDate = DateTime.Now,
                DetailUpdate = "Phê duyệt hiệu quả đối sách:NG"
            };
            db.tbl_History.Add(LSu);
            db.SaveChanges();
            var nguoinhan = db.tbl_User.Where(x => x.ADID == detailLoi.NguoiDNGhiNhapHQ).FirstOrDefault().Mail;
            MailMessage mail = new MailMessage();
            mail.To.Add(nguoinhan);
            mail.Subject = "Thông báo phê duyệt NG";
            mail.Body = $@"
                <div style='margin-top:16px;'>
                <h3>Thông báo phê duyệt NG hiệu quả đối sách lỗi {detailLoi.Maloi}</h3>
                <br />
                <span style='font-size:16px;'>Người phê duyệt: {detailLoi.NguoiDNPheDuyetHQ}</span>
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

        public ActionResult LichSuHQ(string maloi) 
        { 
            var LSu = db.tbl_HieuQua.Where(x => x.MaLoi == maloi).OrderByDescending(x => x.ID).ToList();
            return View(LSu);
        }

        // GET: HieuQua/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_HieuQua tbl_HieuQua = db.tbl_HieuQua.Find(id);
            if (tbl_HieuQua == null)
            {
                return HttpNotFound();
            }
            return View(tbl_HieuQua);
        }

        // POST: HieuQua/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_HieuQua tbl_HieuQua = db.tbl_HieuQua.Find(id);
            db.tbl_HieuQua.Remove(tbl_HieuQua);
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
