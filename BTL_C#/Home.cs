using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_C_
{
    public partial class Home : Form
    {
        Form formCon = null;
        int _chucVu = 0;
        string _tenNV = "";

        public Home(int ChucVu, string tenNV)
        {
            InitializeComponent();

            lbTenNV.Text = tenNV;
            _tenNV = tenNV;
            _chucVu = ChucVu;
            if (ChucVu != 1)
            {
                lbChucVu.Text = "Chức Vụ: Nhân Viên";
                lbNhaCungCap.Enabled = false;
                lbNhanVien.Enabled = false;
                lbThongKe.Enabled = false;
            }
            else lbChucVu.Text = "Chức Vụ: Quản Lý";
            lbNgayGio.Text = $"{DateTime.Now.DayOfWeek} ({DateTime.Now.ToString("dd-MM-yyyy")})";
            lbCopyright.Text = $"Copyright © {DateTime.Now.Year} | By Nhóm 4";
            lbTrangChu.ForeColor = Color.Yellow;
        }

        private void LoadFormCon(Form form)
        {
            if (formCon != null)
            {
                formCon.Close();
            }
            formCon = form;
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            content_right.Controls.Add(form);
            content_right.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void lbTrangChu_MouseHover(object sender, EventArgs e)
        {
            lbTrangChu.Cursor = Cursors.Hand;
        }

        private void lbSanPham_MouseHover(object sender, EventArgs e)
        {
            lbSanPham.Cursor = Cursors.Hand;
        }

        private void lbXuatHang_MouseHover(object sender, EventArgs e)
        {
            lbXuatHang.Cursor = Cursors.Hand;
        }

        private void lbNhapHang_MouseHover(object sender, EventArgs e)
        {
            lbNhapHang.Cursor = Cursors.Hand;
        }

        private void lbNhaCungCap_MouseHover(object sender, EventArgs e)
        {
            lbNhaCungCap.Cursor = Cursors.Hand;
        }

        private void lbNhanVien_MouseHover(object sender, EventArgs e)
        {
            lbNhanVien.Cursor = Cursors.Hand;
        }

        private void lbThongKe_MouseHover(object sender, EventArgs e)
        {
            lbThongKe.Cursor = Cursors.Hand;
        }

        private void lbSanPham_Click(object sender, EventArgs e)
        {
            lbSanPham.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbNhapHang.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;
            Form sanPham = new SanPham(_chucVu);
            LoadFormCon(sanPham);
        }

        private void lbTrangChu_Click(object sender, EventArgs e)
        {
            lbTrangChu.ForeColor = Color.Yellow;

            lbNhapHang.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;

            if (formCon != null)
            {
                formCon.Close();
            }
        }

        private void lbXuatHang_Click(object sender, EventArgs e)
        {
            lbXuatHang.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbNhapHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;
            Form xuatHang = new XuatHang(_chucVu,_tenNV);
            LoadFormCon(xuatHang);
        }

        private void lbNhapHang_Click(object sender, EventArgs e)
        {
            lbNhapHang.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;

            Form nhapHang = new NhapHang(_chucVu, _tenNV);
            LoadFormCon(nhapHang);
        }

        private void lbNhaCungCap_Click(object sender, EventArgs e)
        {
            lbNhaCungCap.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhapHang.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;

            Form nhaCC = new NhaCungCap();
            LoadFormCon(nhaCC);
        }

        private void lbNhanVien_Click(object sender, EventArgs e)
        {
            lbNhanVien.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhapHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbThongKe.ForeColor = Color.White;

            Form nhanVien = new NhanVien();
            LoadFormCon(nhanVien);
        }

        private void lbThongKe_Click(object sender, EventArgs e)
        {
            lbThongKe.ForeColor = Color.Yellow;

            lbTrangChu.ForeColor = Color.White;
            lbSanPham.ForeColor = Color.White;
            lbXuatHang.ForeColor = Color.White;
            lbNhapHang.ForeColor = Color.White;
            lbNhaCungCap.ForeColor = Color.White;
            lbNhanVien.ForeColor = Color.White;

            Form thongKe = new ThongKe();
            LoadFormCon(thongKe);
        }
    }
}
