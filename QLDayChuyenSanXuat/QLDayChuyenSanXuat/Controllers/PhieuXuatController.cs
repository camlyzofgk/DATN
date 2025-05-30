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
    public class PhieuXuatController : Controller
    {
        private QLDayChuyenSX db = new QLDayChuyenSX();

        // GET: PhieuXuat
        public ActionResult Index(string timerange,string sophieu, string TrangThai, string PhongBan, string NguoiTP, string NguoiYC, string NguoiPD, string NguoiUpdate)
        {
            var KQ = db.tbl_PhieuXuat.AsQueryable();
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
                KQ = KQ.Where(x => listADID.Contains(x.NguoiYeuCau));
            }
            if (NguoiTP != null && NguoiTP != "All")
            {
                KQ = KQ.Where(x => x.NguoiTaoPhieu == NguoiTP);
            }
            if (NguoiYC != null && NguoiYC != "All")
            {
                KQ = KQ.Where(x => x.NguoiYeuCau == NguoiYC);
            }
            if (NguoiPD != null && NguoiPD != "All")
            {
                KQ = KQ.Where(x => x.NguoiPheDuyet == NguoiPD);
            }
            if (NguoiUpdate != null && NguoiUpdate != "All")
            {
                KQ = KQ.Where(x => x.NguoiCapNhat == NguoiUpdate);
            }
            return View(KQ.ToList());
        }

        // GET: PhieuXuat/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuXuat tbl_PhieuXuat = db.tbl_PhieuXuat.Find(id);
            if (tbl_PhieuXuat == null)
            {
                return HttpNotFound();
            }
            return View(tbl_PhieuXuat);
        }
        // GET: PhieuXuat/Create
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

        // POST: PhieuXuat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            // 1. Lưu thông tin Phiếu Xuất
            var soPhieu = form["SoPhieu"];
            var ngayLap = Convert.ToDateTime(form["NgayLap"]);
            var nguoiTao = form["NguoiTaoPhieu"];
            var emailYC = form["EmailYC"];
            var emailPD = form["EmailPD"];
            var nguoiYC = db.tbl_User.Where(x => x.Mail == emailYC).FirstOrDefault();
            var nguoiPD = db.tbl_User.Where(x => x.Mail == emailPD).FirstOrDefault();
            var nguoiTaoPhieu = db.tbl_User.Where(x => x.ADID == nguoiTao).FirstOrDefault();
            var phieu = new tbl_PhieuXuat
            {
                SoPhieu = soPhieu,
                NgayLap = ngayLap,
                NguoiTaoPhieu = nguoiTao,
                NguoiYeuCau = nguoiYC.ADID,
                NguoiPheDuyet = nguoiPD.ADID,
                NguoiCapNhat = nguoiTao,
                TimeUpDate = DateTime.Now,
                TrangThai = "Chờ phê duyệt"
            };
            db.tbl_PhieuXuat.Add(phieu);

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

                var ct = new tbl_ChiTietPhieuXuat
                {
                    SoPhieu = soPhieu,
                    MaLinhKien = LK.MaLinhKien,
                    TenLinhKien = LK.TênLinhKien,
                    ViTriLuuTru = LK.ViTriLuuTru,
                    NhaCungCap = LK.NhaCungCap,
                    SoLuong = soLuong
                };
                db.tbl_ChiTietPhieuXuat.Add(ct);

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
            mail.To.Add(nguoiYC.Mail);
            mail.To.Add(nguoiPD.Mail);
            mail.To.Add(nguoiTaoPhieu.Mail);
            mail.Subject = "Thông báo phiếu xuất kho mới";
            mail.Body = $@"
            <p><strong>Thông báo có phiếu xuất kho mới</strong></p>
            <p><strong>Số phiếu:</strong> {soPhieu}</p>
            <p><strong>Ngày lập:</strong> {ngayLap:dd/MM/yyyy}</p>
            <p><strong>Người tạo phiếu:</strong> {nguoiTaoPhieu.Name}</p>
            <p><strong>Người yêu cầu:</strong> {nguoiYC.Name}</p>
            <p><strong>Người phê duyệt:</strong> {nguoiPD.Name}</p>
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

        // GET: PhieuXuat/Edit/5
        public ActionResult Edit(string sophieu)
        {
            if (sophieu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuXuat tbl_PhieuXuat = db.tbl_PhieuXuat.Find(sophieu);
            if (tbl_PhieuXuat == null)
            {
                return HttpNotFound();
            }
            var linhKienList = db.LinhKiens.ToList();
            ViewBag.LinhKienList = linhKienList;

            var chitietPX = db.tbl_ChiTietPhieuXuat.Where(x => x.SoPhieu == sophieu).ToList();
            ViewBag.ChiTiet = chitietPX;
            return View(tbl_PhieuXuat);
        }

        // POST: PhieuXuat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SoPhieu,NguoiCapNhat")] tbl_PhieuXuat tbl_PhieuXuat, string EmailPD, string EmailYC, string[] MaLK, int[] SoLuong )
        {
            var phieuXuat = db.tbl_PhieuXuat.Where(x => x.SoPhieu == tbl_PhieuXuat.SoPhieu).FirstOrDefault();
            if(phieuXuat.TrangThai == "Chờ phê duyệt")
            {
                phieuXuat.NguoiCapNhat = tbl_PhieuXuat.NguoiCapNhat;
                var nguoiYC = db.tbl_User.Where(x => x.Mail == EmailYC).FirstOrDefault();
                var nguoiPD = db.tbl_User.Where(x => x.Mail == EmailPD).FirstOrDefault();
                var nguoiTao = db.tbl_User.Where(x => x.ADID == phieuXuat.NguoiTaoPhieu).FirstOrDefault();
                phieuXuat.NguoiYeuCau = nguoiYC.ADID;
                phieuXuat.NguoiPheDuyet = nguoiPD.ADID;
                phieuXuat.TimeUpDate = DateTime.Now;
                var chitietPhieu = db.tbl_ChiTietPhieuXuat.Where(x => x.SoPhieu == tbl_PhieuXuat.SoPhieu).ToList();
                db.tbl_ChiTietPhieuXuat.RemoveRange(chitietPhieu);

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

                    var ct = new tbl_ChiTietPhieuXuat
                    {
                        SoPhieu = phieuXuat.SoPhieu,
                        MaLinhKien = LK.MaLinhKien,
                        TenLinhKien = LK.TênLinhKien,
                        ViTriLuuTru = LK.ViTriLuuTru,
                        NhaCungCap = LK.NhaCungCap,
                        SoLuong = soLuong
                    };
                    db.tbl_ChiTietPhieuXuat.Add(ct);

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
                mail.To.Add(nguoiYC.Mail);
                mail.To.Add(nguoiPD.Mail);
                mail.To.Add(nguoiTao.Mail);
                mail.Subject = "Thông báo cập nhật phiếu xuất kho";
                mail.Body = $@"
            <p><strong>Thông báo cập nhật phiếu xuất kho</strong></p>
            <p><strong>Số phiếu:</strong> {phieuXuat.SoPhieu}</p>
            <p><strong>Ngày lập:</strong> {phieuXuat.NgayLap:dd/MM/yyyy}</p>
            <p><strong>Người tạo phiếu:</strong> {nguoiTao.Name}</p>
            <p><strong>Người yêu cầu:</strong> {nguoiYC.Name}</p>
            <p><strong>Người phê duyệt:</strong> {nguoiPD.Name}</p>
            <p><strong>Danh sách linh kiện:</strong></p>
            {tableHtml.ToString()}
            <p>🔗 Vui lòng truy cập hệ thống Quản lý dây chuyền sản xuất để xử lý.</p>
            <p style='color:#999; font-size:12px;'>Email được gửi tự động từ hệ thống. Vui lòng không trả lời thư này.</p>
            ";

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);


            }
            else if(phieuXuat.TrangThai == "Đang giao hàng")
            {
                phieuXuat.NguoiCapNhat = tbl_PhieuXuat.NguoiCapNhat;
                phieuXuat.TimeUpDate = DateTime.Now;
                phieuXuat.TrangThai = "Đã giao hàng";
                for (int i = 0; i < MaLK.Length; i++)
                {
                    var maLK = MaLK[i];
                    var LK = db.LinhKiens.FirstOrDefault(x => x.MaLinhKien == maLK);
                    var soLuong = Convert.ToInt32(SoLuong[i]);
                    LK.SoLuong = LK.SoLuong - soLuong;
                }
            }
            TempData["ThongBao"] = "Cập nhật phiếu thành công!";
            db.SaveChanges();
            return RedirectToAction("Index"); // hoặc trang xác nhận
        }
        public ActionResult PheDuyetOK(string sophieu)
        {
            var PhieuXuat = db.tbl_PhieuXuat.Where(x => x.SoPhieu == sophieu).FirstOrDefault();
            PhieuXuat.TrangThai = "Đã phê duyệt";
            PhieuXuat.NguoiCapNhat = PhieuXuat.NguoiPheDuyet;
            PhieuXuat.TimeUpDate = DateTime.Now;
            db.SaveChanges();
            TempData["ThongBao"] = "Phê duyệt phiếu thành công!";
            return RedirectToAction("Index"); // hoặc trang xác nhận
        }
        public ActionResult PheDuyetNG(string sophieu, string LyDo)
        {
            var PhieuXuat = db.tbl_PhieuXuat.Where(x => x.SoPhieu == sophieu).FirstOrDefault();
            var nguoiTao = db.tbl_User.Where(x => x.ADID == PhieuXuat.NguoiTaoPhieu).FirstOrDefault();
            var nguoiPD = db.tbl_User.Where(x => x.ADID == PhieuXuat.NguoiPheDuyet).FirstOrDefault();
            MailMessage mail = new MailMessage();
            mail.To.Add(nguoiTao.Mail);
            mail.Subject = "Thông báo phê duyệt NG";
            mail.Body = $@"
                <div style='margin-top:16px;'>
                <h3>Thông báo phê duyệt NG phiếu xuất {PhieuXuat.SoPhieu}</h3>
                <br />
                <span style='font-size:16px;'>Người phê duyệt: {nguoiPD.Name}</span>
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
            TempData["ThongBao"] = "Phê duyệt phiếu thành công!";
            return RedirectToAction("Index"); // hoặc trang xác nhận
        }

        public ActionResult UpdateTrangThai(string sophieu, string nguoiUpdate)
        {
            var PhieuXuat = db.tbl_PhieuXuat.Where(x => x.SoPhieu == sophieu).FirstOrDefault();
            if (PhieuXuat.TrangThai == "Chờ phê duyệt")
            {
                PhieuXuat.TrangThai = "Đã hủy";
                PhieuXuat.NguoiCapNhat = nguoiUpdate;
                PhieuXuat.TimeUpDate = DateTime.Now;
                TempData["ThongBao"] = "Hủy phiếu thành công!";
            }
            else if (PhieuXuat.TrangThai == "Đã phê duyệt")
            {
                PhieuXuat.TrangThai = "Đang giao hàng";
                PhieuXuat.NguoiCapNhat = nguoiUpdate;
                PhieuXuat.TimeUpDate = DateTime.Now;
                TempData["ThongBao"] = "Cập nhật phiếu thành công!";
            }
            db.SaveChanges();
            return RedirectToAction("Index"); // hoặc trang xác nhận
        }

        // GET: PhieuXuat/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_PhieuXuat tbl_PhieuXuat = db.tbl_PhieuXuat.Find(id);
            if (tbl_PhieuXuat == null)
            {
                return HttpNotFound();
            }
            return View(tbl_PhieuXuat);
        }

        // POST: PhieuXuat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tbl_PhieuXuat tbl_PhieuXuat = db.tbl_PhieuXuat.Find(id);
            db.tbl_PhieuXuat.Remove(tbl_PhieuXuat);
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
