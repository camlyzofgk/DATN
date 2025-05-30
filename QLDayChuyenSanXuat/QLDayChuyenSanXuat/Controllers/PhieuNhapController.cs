using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using QLDayChuyenSanXuat.Models;

namespace QLDayChuyenSanXuat.Controllers
{
    public class PhieuNhapController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: PhieuNhap
        public ActionResult Index(string timerange, string sophieu, string TrangThai, string PhongBan, string NguoiTP, string NguoiNhan, string nguoiUpdate)
        {
            var KQ = db.tbl_PhieuNhap.AsQueryable();
            var datetime = DateTime.Now;
            if (timerange != null)
            {
                switch (timerange)
                {
                    case "All":
                        break;
                    case "7":
                        datetime = DateTime.Now.AddDays(-1);
                        KQ = KQ.Where(x => x.NgayLap >= datetime);
                        break;
                    case "30":
                        datetime = DateTime.Now.AddDays(-30);
                        KQ = KQ.Where(x => x.NgayLap >= datetime);
                        break;
                }
            }
            if (sophieu != null)
            {
                KQ = KQ.Where(x => x.SoPhieu == sophieu);
            }
            if (TrangThai != null && TrangThai != "All")
            {
                KQ = KQ.Where(x => x.TrangThai == TrangThai);
            }
            if (PhongBan != null && PhongBan != "All")
            {
                var listADID = db.tbl_User.Where(x => x.MaPB == PhongBan).Select(x => x.ADID).ToList();
                KQ = KQ.Where(x => listADID.Contains(x.NguoiNhan));
            }
            if (NguoiTP != null && NguoiTP != "All")
            {
                KQ = KQ.Where(x => x.NguoiTaoPhieu == NguoiTP);
            }
            if (NguoiNhan != null && NguoiNhan != "All")
            {
                KQ = KQ.Where(x => x.NguoiNhan == NguoiNhan);
            }
            if (nguoiUpdate != null && nguoiUpdate != "All")
            {
                KQ = KQ.Where(x => x.NguoiCapNhat == nguoiUpdate);
            }
            return View(KQ.ToList());
        }

        // GET: PhieuNhap/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuNhap tbl_PhieuNhap = db.tbl_PhieuNhap.Find(id);
            if (tbl_PhieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(tbl_PhieuNhap);
        }

        // GET: PhieuNhap/Create
        public ActionResult Create()
        {
            var linhKienList = db.LinhKiens.Select(x => new {
                MaLK = x.MaLinhKien,
                TenLK = x.TênLinhKien,
                ViTri = x.ViTriLuuTru,
                NhaCC = x.NhaCungCap,
            }).ToList();
            ViewBag.LinhKienList = linhKienList;
            return View();
        }

        // POST: PhieuNhap/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            var soPhieu = form["SoPhieu"];
            var ngayLap = Convert.ToDateTime(form["NgayLap"]);
            var nguoiTao = form["NguoiTaoPhieu"];
            var emailNN = form["EmailNN"];
            var nguoiNhan = db.tbl_User.Where(x => x.Mail == emailNN).FirstOrDefault();
            var nguoiTaoPhieu = db.tbl_User.Where(x => x.ADID == nguoiTao).FirstOrDefault();
            var phieu = new tbl_PhieuNhap
            {
                SoPhieu = soPhieu,
                NgayLap = ngayLap,
                NguoiTaoPhieu = nguoiTao,
                NguoiNhan = nguoiNhan.ADID,
                NguoiCapNhat = nguoiTao,
                TimeUpdate = DateTime.Now,
                TrangThai = "Đã tạo"
            };
            db.tbl_PhieuNhap.Add(phieu);

            // 2. Lưu danh sách linh kiện
            var maLKs = form.GetValues("MaLK[]");
            var soLuongs = form.GetValues("SoLuong[]");

            StringBuilder tableHtml = new StringBuilder();
            tableHtml.Append("<table border='1' cellpadding='5' cellspacing='0'>");
            tableHtml.Append("<thead><tr>");
            tableHtml.Append("<th>Mã linh kiện</th>");
            tableHtml.Append("<th>Tên linh kiện</th>");
            tableHtml.Append("<th>Số lượng</th>");
            tableHtml.Append("<th>Vị trí lưu trữ</th>");
            tableHtml.Append("<th>Nhà cung cấp</th>");
            tableHtml.Append("</tr></thead><tbody>");

            for (int i = 0; i < maLKs.Length; i++)
            {
                var maLK = maLKs[i];
                var LK = db.LinhKiens.FirstOrDefault(x => x.MaLinhKien == maLK);
                var soLuong = Convert.ToInt32(soLuongs[i]);

                var ct = new tbl_ChiTietPhieuNhap
                {
                    SoPhieu = soPhieu,
                    MaLinhKien = LK.MaLinhKien,
                    TenLinhKien = LK.TênLinhKien,
                    ViTriLuuTru = LK.ViTriLuuTru,
                    NhaCungCap = LK.NhaCungCap,
                    SoLuong = soLuong
                };
                db.tbl_ChiTietPhieuNhap.Add(ct);

                // Add row to table
                tableHtml.Append("<tr>");
                tableHtml.Append($"<td>{LK.MaLinhKien}</td>");
                tableHtml.Append($"<td>{LK.TênLinhKien}</td>");
                tableHtml.Append($"<td>{soLuong}</td>");
                tableHtml.Append($"<td>{LK.ViTriLuuTru}</td>");
                tableHtml.Append($"<td>{LK.NhaCungCap}</td>");
                tableHtml.Append("</tr>");
            }

            tableHtml.Append("</tbody></table>");

            db.SaveChanges();
            TempData["ThongBao"] = "Tạo phiếu mới thành công!";
            // Tạo email
            MailMessage mail = new MailMessage();
            mail.To.Add(nguoiNhan.Mail);
            mail.To.Add(nguoiTaoPhieu.Mail);
            mail.Subject = "Thông báo phiếu nhận kho mới";
            mail.Body = $@"
            <p><strong>Thông báo có phiếu nhận kho mới</strong></p>
            <p><strong>Số phiếu:</strong> {soPhieu}</p>
            <p><strong>Ngày lập:</strong> {ngayLap:dd/MM/yyyy}</p>
            <p><strong>Người tạo phiếu:</strong> {nguoiTaoPhieu.Name}</p>
            <p><strong>Người nhận:</strong> {nguoiNhan.Name}</p>
            <p><strong>Danh sách linh kiện:</strong></p>
            {tableHtml.ToString()}
            <p>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
            <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
            ";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);

            return RedirectToAction("Index"); // hoặc trang xác nhận
        }

        // GET: PhieuNhap/Edit/5
        public ActionResult Edit(string sophieu)
        {
            if (sophieu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuNhap tbl_PhieuNhap = db.tbl_PhieuNhap.Find(sophieu);
            if (tbl_PhieuNhap == null)
            {
                return HttpNotFound();
            }
            var linhKienList = db.LinhKiens.ToList();
            ViewBag.LinhKienList = linhKienList;

            var chitietNhap = db.tbl_ChiTietPhieuNhap.Where(x => x.SoPhieu == sophieu).ToList();
            ViewBag.ChiTiet = chitietNhap;
            return View(tbl_PhieuNhap);
        }

        // POST: PhieuNhap/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SoPhieu,NgayLap,NguoiTaoPhieu,NguoiCapNhat")] tbl_PhieuNhap tbl_PhieuNhap, string EmailNN, string[] MaLK, int[] SoLuong)
        {
            var phieuNhap = db.tbl_PhieuNhap.Where(x => x.SoPhieu == tbl_PhieuNhap.SoPhieu).FirstOrDefault();
            if (phieuNhap.TrangThai == "Đã tạo")
            {
                phieuNhap.NguoiCapNhat = tbl_PhieuNhap.NguoiCapNhat;
                var nguoiN = db.tbl_User.Where(x => x.Mail == EmailNN).FirstOrDefault();
                var nguoiTao = db.tbl_User.Where(x => x.ADID == phieuNhap.NguoiTaoPhieu).FirstOrDefault();
                phieuNhap.NguoiNhan = nguoiN.ADID;
                phieuNhap.TimeUpdate = DateTime.Now;
                var chitietNhap = db.tbl_ChiTietPhieuNhap.Where(x => x.SoPhieu == tbl_PhieuNhap.SoPhieu).ToList();
                db.tbl_ChiTietPhieuNhap.RemoveRange(chitietNhap);

                StringBuilder tableHtml = new StringBuilder();
                tableHtml.Append("<table border='1' cellpadding='5' cellspacing='0'>");
                tableHtml.Append("<thead><tr>");
                tableHtml.Append("<th>Mã linh kiện</th>");
                tableHtml.Append("<th>Tên linh kiện</th>");
                tableHtml.Append("<th>Số lượng</th>");
                tableHtml.Append("<th>Vị trí lưu trữ</th>");
                tableHtml.Append("<th>Nhà cung cấp</th>");
                tableHtml.Append("</tr></thead><tbody>");

                for (int i = 0; i < MaLK.Length; i++)
                {
                    var maLK = MaLK[i];
                    var LK = db.LinhKiens.FirstOrDefault(x => x.MaLinhKien == maLK);
                    var soLuong = Convert.ToInt32(SoLuong[i]);

                    var ct = new tbl_ChiTietPhieuNhap
                    {
                        SoPhieu = phieuNhap.SoPhieu,
                        MaLinhKien = LK.MaLinhKien,
                        TenLinhKien = LK.TênLinhKien,
                        ViTriLuuTru = LK.ViTriLuuTru,
                        NhaCungCap = LK.NhaCungCap,
                        SoLuong = soLuong
                    };
                    db.tbl_ChiTietPhieuNhap.Add(ct);

                    // Add row to table
                    tableHtml.Append("<tr>");
                    tableHtml.Append($"<td>{LK.MaLinhKien}</td>");
                    tableHtml.Append($"<td>{LK.TênLinhKien}</td>");
                    tableHtml.Append($"<td>{soLuong}</td>");
                    tableHtml.Append($"<td>{LK.ViTriLuuTru}</td>");
                    tableHtml.Append($"<td>{LK.NhaCungCap}</td>");
                    tableHtml.Append("</tr>");
                }

                tableHtml.Append("</tbody></table>");

                db.SaveChanges();
                // Tạo email
                MailMessage mail = new MailMessage();
                mail.To.Add(nguoiN.Mail);
                mail.To.Add(nguoiTao.Mail);
                mail.Subject = "Thông báo cập nhật phiếu nhập kho";
                mail.Body = $@"
            <p><strong>Thông báo cập nhật phiếu nhập kho</strong></p>
            <p><strong>Số phiếu:</strong> {phieuNhap.SoPhieu}</p>
            <p><strong>Ngày lập:</strong> {phieuNhap.NgayLap:dd/MM/yyyy}</p>
            <p><strong>Người tạo phiếu:</strong> {nguoiTao.Name}</p>
            <p><strong>Người nhận:</strong> {nguoiN.Name}</p>
            <p><strong>Danh sách linh kiện:</strong></p>
            {tableHtml.ToString()}
            <p>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
            <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
            ";

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);


            }
            else if (phieuNhap.TrangThai == "Đã nhận")
            {
                phieuNhap.NguoiCapNhat = tbl_PhieuNhap.NguoiCapNhat;
                phieuNhap.TimeUpdate = DateTime.Now;
                phieuNhap.TrangThai = "Đã nhập kho";
                for (int i = 0; i < MaLK.Length; i++)
                {
                    var maLK = MaLK[i];
                    var LK = db.LinhKiens.FirstOrDefault(x => x.MaLinhKien == maLK);
                    var soLuong = Convert.ToInt32(SoLuong[i]);
                    LK.SoLuong = LK.SoLuong + soLuong;
                }
            }
            TempData["ThongBao"] = "Cập nhật phiếu thành công!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UpdateTrangThai(string sophieu, string nguoiUpdate, string noidung)
        {
            var PhieuNhap = db.tbl_PhieuNhap.Where(x => x.SoPhieu == sophieu).FirstOrDefault();
            if (noidung == "Hủy phiếu")
            {
                PhieuNhap.TrangThai = "Đã hủy";
                PhieuNhap.NguoiCapNhat = nguoiUpdate;
                PhieuNhap.TimeUpdate = DateTime.Now;
                TempData["ThongBao"] = "Hủy phiếu thành công!";
            }
            else if(noidung == "Đã nhận")
            {
                PhieuNhap.TrangThai = "Đã nhận";
                PhieuNhap.NguoiCapNhat = nguoiUpdate;
                PhieuNhap.TimeUpdate = DateTime.Now;
                TempData["ThongBao"] = "Cập nhật phiếu thành công!";
            }
            db.SaveChanges();
            return RedirectToAction("Index"); // hoặc trang xác nhận
        }

        // GET: PhieuNhap/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuNhap tbl_PhieuNhap = db.tbl_PhieuNhap.Find(id);
            if (tbl_PhieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(tbl_PhieuNhap);
        }

        // POST: PhieuNhap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tbl_PhieuNhap tbl_PhieuNhap = db.tbl_PhieuNhap.Find(id);
            db.tbl_PhieuNhap.Remove(tbl_PhieuNhap);
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
