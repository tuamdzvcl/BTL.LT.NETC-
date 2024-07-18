using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_C_
{
    public partial class XuatHang : Form
    {
        DBConnection DBConn = new DBConnection();
        int chucVu = 0;
        string tenNV = "";
        int modSuaSoLuongSanPham;
        public XuatHang(int ChucVu, string TenNV)
        {
            InitializeComponent();
            this.ForeColor = Color.Black;
            tenNV = TenNV;
            txtTenNV.Text = TenNV;
            chucVu = ChucVu;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            LoadDanhSachDonXuat();
            LoadSanPhamLenComboBox();
        }

        private void LoadSanPhamLenComboBox()
        {
            DBConn.GetConnection();

            string query = "SELECT MaSP,TenSP,DungLuong,MauSac FROM tblSanPham";
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, DBConn.conn))
            {
                DataTable data = new DataTable();
                adapter.Fill(data);
                data.Columns.Add("TenSP,MauSac,DungLuong", typeof(string), "TenSP + '-' + MauSac + '-' + DungLuong");
                cboLoaiSP.DataSource = data;
                cboLoaiSP.DisplayMember = "TenSP,MauSac,DungLuong";
                cboLoaiSP.ValueMember = "MaSP";
            }

            DBConn.CloseConnection();
        }

        private void LoadDanhSachDonXuat()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblXuatHang";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDX = reader.GetString(0);
                        string tenKH = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string SDT = reader.GetString(3);
                        string NVXH = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDX);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(NVXH);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }
            DBConn.CloseConnection();
        }

        private void btnXuatDon_Click(object sender, EventArgs e)
        {
            modSuaSoLuongSanPham = 0;

            string maDX = txtMaDX.Text;
            string tenKH = txtTenKH.Text;
            string loaiSP = cboLoaiSP.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            string SDT = txtSDT.Text;
            string NVXH = txtTenNV.Text;
            int SLSP = 0;
            try
            {
                SLSP = Convert.ToInt32(txtSLSP.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vui lòng nhập SLSP là số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maDX == "" || tenKH == "" || loaiSP == "" || SDT == "" || SLSP == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaDX(maDX) == true)
            {
                MessageBox.Show("Trùng mã đơn xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool _1 = SuaSoLuongSanPham(maSP, SLSP, maDX);
            bool _2 = ThemDonXuatVaoBang(maDX, tenKH, loaiSP, SDT, NVXH, SLSP, ngayCapNhat);
            bool _3 = ThemDonXuatVaoBangThongKe(maDX, tenKH, loaiSP, SLSP, NVXH, ngayCapNhat);

            if (_1 && _2 && _3)
            {
                MessageBox.Show("Xuất đơn hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachDonXuat();
                ResetTextBox();
            }
            else MessageBox.Show("Xuất đơn hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ThemDonXuatVaoBangThongKe(string maDX, string tenKH, string loaiSP, int sLSP, string nVXH, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblThongKe (MaDH,TenKH,TenNCC,TenNV,LoaiSP,SLSP,NgayCapNhat,TrangThai) " +
                            "VALUES (@maDX,@tenKH,@tenNCC,@tenNV,@loaiSP,@sLSP,@ngayCapNhat,@trangThai)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);
                cmd.Parameters.AddWithValue("@tenKH", tenKH);
                cmd.Parameters.AddWithValue("@tenNCC", "");
                cmd.Parameters.AddWithValue("@tenNV", nVXH);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@sLSP", sLSP);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);
                cmd.Parameters.AddWithValue("@trangThai", 1);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    return true;
                }
            }
            DBConn.CloseConnection();
            return false;
        }

        private bool ThemDonXuatVaoBang(string maDX, string tenKH, string loaiSP, string sDT, string nVXH, int SLSP, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblXuatHang (MaDX,TenKH,LoaiSP,SDT,NVXH,SLSP,NgayCapNhat) " +
                            "VALUES (@maDX,@tenKH,@loaiSP,@sDT,@nVXH,@sLSP,@ngayCapNhat)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);
                cmd.Parameters.AddWithValue("@tenKH", tenKH);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@sDT", sDT);
                cmd.Parameters.AddWithValue("@nVXH", nVXH);
                cmd.Parameters.AddWithValue("@sLSP", SLSP);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    return true;
                }
            }
            DBConn.CloseConnection();

            return false;
        }

        private bool SuaSoLuongSanPham(string maSP, int soLuongSanPhamMoi, string maDX)
        {
            if (modSuaSoLuongSanPham == 1)
            {
                int soLuongSanPhamTrongKho = LaySoLuongSanPham(maSP);
                int soLuongSanPhamTungXuat = LaySoLuongSanPhamTungXuat(maDX);
                if (soLuongSanPhamMoi > soLuongSanPhamTrongKho)
                {
                    MessageBox.Show("Không đủ sản phẩm để xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                int _soLuongSanPham = soLuongSanPhamTrongKho + soLuongSanPhamTungXuat - soLuongSanPhamMoi;
                bool _suaSoLuong = SuaSoLuong(maSP, _soLuongSanPham);
                if (_suaSoLuong) return true;
                return false;
            }
            int soLuongSanPhamCu = LaySoLuongSanPham(maSP);
            if (soLuongSanPhamMoi > soLuongSanPhamCu)
            {
                MessageBox.Show("Không đủ sản phẩm để xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            int soLuongSanPham = soLuongSanPhamCu - soLuongSanPhamMoi;
            bool suaSoLuong = SuaSoLuong(maSP, soLuongSanPham);
            if (suaSoLuong) return true;
            return false;
        }

        private int LaySoLuongSanPhamTungXuat(string maDX)
        {
            DBConn.GetConnection();

            string query = "SELECT SLSP FROM tblXuatHang WHERE MaDX = @maDX";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int soLuong = reader.GetInt32(0);
                        return soLuong;
                    }
                }
            }

            DBConn.CloseConnection();

            return -1;
        }

        private bool SuaSoLuong(string maSP, int soLuongSanPham)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblSanPham SET SoLuong = @soLuong WHERE MaSP = @maSP";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@soLuong", soLuongSanPham);
                cmd.Parameters.AddWithValue("@maSP", maSP);

                int check = cmd.ExecuteNonQuery();
                if (check > 0) return true;
            }

            DBConn.CloseConnection();

            return false;
        }

        private int LaySoLuongSanPham(string maSP)
        {
            DBConn.GetConnection();

            string query = "SELECT SoLuong FROM tblSanPham WHERE MaSP = @maSP";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maSP", maSP);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int soLuong = reader.GetInt32(0);
                        return soLuong;
                    }
                }
            }

            DBConn.CloseConnection();

            return -1;
        }

        private void lsvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvDanhSach.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lsvDanhSach.SelectedItems[0];

                txtMaDX.Text = lvi.SubItems[0].Text;
                txtTenKH.Text = lvi.SubItems[1].Text;
                cboLoaiSP.Text = lvi.SubItems[2].Text;
                txtSDT.Text = lvi.SubItems[3].Text;
                txtTenNV.Text = lvi.SubItems[4].Text;
                txtSLSP.Text = lvi.SubItems[5].Text;

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                cboLoaiSP.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextBox();
        }

        private void ResetTextBox()
        {
            txtMaDX.Text = "";
            txtTenKH.Text = "";
            cboLoaiSP.SelectedIndex = 0;
            txtSDT.Text = "";
            txtTenNV.Text = tenNV;
            txtSLSP.Text = "";

            btnXoa.Enabled = false;
            btnSua.Enabled = false;
            cboLoaiSP.Enabled = true;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (chucVu != 1)
            {
                MessageBox.Show("Bạn không được quyền sửa thông tin đơn xuất hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            modSuaSoLuongSanPham = 1;

            string maDX = txtMaDX.Text;
            string tenKH = txtTenKH.Text;
            string loaiSP = cboLoaiSP.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            string SDT = txtSDT.Text;
            string NVXH = txtTenNV.Text;
            int SLSP = 0;
            try
            {
                SLSP = Convert.ToInt32(txtSLSP.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vui lòng nhập SLSP là số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maDX == "" || tenKH == "" || loaiSP == "" || SDT == "" || SLSP == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaDX(maDX) == false)
            {
                MessageBox.Show("Mã đơn xuất hàng không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool _1 = SuaSoLuongSanPham(maSP, SLSP, maDX);
            bool _2 = SuaDonXuatHang(maDX, tenKH, loaiSP, SDT, NVXH, SLSP, ngayCapNhat);
            bool _3 = SuaDonXuatHangOBangThongKe(maDX, tenKH, loaiSP, SLSP, NVXH, ngayCapNhat);

            if (_1 && _2 && _3)
            {
                MessageBox.Show("Sửa thông tin xuất hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachDonXuat();
                ResetTextBox();
            }
            else MessageBox.Show("Sửa thông tin xuất hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool SuaDonXuatHangOBangThongKe(string maDX, string tenKH, string loaiSP, int sLSP, string nVXH, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblThongKe SET TenKH = @tenKH,LoaiSP = @loaiSP,SLSP = @sLSP,NgayCapNhat = @ngayCapNhat WHERE MaDH = @maDX";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);
                cmd.Parameters.AddWithValue("@tenKH", tenKH);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@sLSP", sLSP);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    return true;
                }
            }
            DBConn.CloseConnection();
            return false;
        }

        private bool SuaDonXuatHang(string maDX, string tenKH, string loaiSP, string sDT, string nVXH, int sLSP, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblXuatHang SET TenKH = @tenKH,LoaiSP = @loaiSP,SDT = @sDT,SLSP = @sLSP,NgayCapNhat = @ngayCapNhat WHERE MaDX = @maDX";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);
                cmd.Parameters.AddWithValue("@tenKH", tenKH);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@sDT", sDT);
                cmd.Parameters.AddWithValue("@sLSP", sLSP);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    return true;
                }
            }
            DBConn.CloseConnection();

            return false;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (chucVu != 1)
            {
                MessageBox.Show("Bạn không được quyền xóa đơn xuất hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string maDX = txtMaDX.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            DialogResult result = MessageBox.Show("Bạn muốn xóa đơn Xuất Hàng ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int soLuongTrongKho = LaySoLuongSanPham(maSP);
                int soLuongTungXuat = LaySoLuongSanPhamTungXuat(maDX);
                int slsp = soLuongTrongKho + soLuongTungXuat;
                SuaSoLuong(maSP, slsp);
                XoaDonXuatHang(maDX);
                LoadDanhSachDonXuat();
                ResetTextBox();
            }
        }

        private void XoaDonXuatHang(string maDX)
        {
            if (KiemTraMaDX(maDX) == false)
            {
                MessageBox.Show("Mã đơn xuất hàng không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DBConn.GetConnection();

            string query = "DELETE FROM tblXuatHang WHERE MaDX = @maDX;" +
                            "DELETE FROM tblThongKe WHERE MaDH = @maDX AND TrangThai = 1;";

            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Xóa đơn xuất hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("Xóa đơn xuất hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();

        }

        private bool KiemTraMaDX(string maDX)
        {
            DBConn.GetConnection();
            string query = "SELECT * FROM tblXuatHang WHERE MaDX = @maDX";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", maDX);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            DBConn.CloseConnection();
            return false;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tkMaDX = txtTkMaDX.Text;
            string tkTenKH = txtTkTenKH.Text;

            if (tkMaDX != "" && tkTenKH == "") TimKiemTheoMaDX(tkMaDX);
            if (tkMaDX == "" && tkTenKH != "") TimKiemTheoTenKH(tkTenKH);
            if (tkMaDX != "" && tkTenKH != "") TimKiemTheoMaDX(tkMaDX);
            if (tkMaDX == "" && tkTenKH == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachDonXuat();
            }
        }

        private void TimKiemTheoTenKH(string tkTenKH)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblXuatHang WHERE TenKH LIKE @tenKH";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenKH", "%" + tkTenKH + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDX = reader.GetString(0);
                        string tenKH = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string SDT = reader.GetString(3);
                        string NVXH = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDX);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(NVXH);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaDX(string tkMaDX)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblXuatHang WHERE MaDX = @maDX";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDX", tkMaDX);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string maDX = reader.GetString(0);
                        string tenKH = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string SDT = reader.GetString(3);
                        string NVXH = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDX);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(NVXH);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }
    }
}
