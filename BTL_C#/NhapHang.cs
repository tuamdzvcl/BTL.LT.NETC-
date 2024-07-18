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
    public partial class NhapHang : Form
    {
        DBConnection DBConn = new DBConnection();
        int chucVu = 0;
        string tenNV = "";
        int modSuaSoLuongSanPham;

        public NhapHang(int ChucVu, string TenNV)
        {
            InitializeComponent();
            this.ForeColor = Color.Black;
            tenNV = TenNV;
            txtTenNV.Text = TenNV;
            chucVu = ChucVu;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            LoadDanhSachDonNhap();
            LoadSanPhamLenComboBox();
            LoadTenNCCLenComboBox();
        }

        private void LoadTenNCCLenComboBox()
        {
            DBConn.GetConnection();

            string query = "SELECT MaNCC,TenNCC FROM tblNCC";
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, DBConn.conn))
            {
                DataTable data = new DataTable();
                adapter.Fill(data);
                cboTenNCC.DataSource = data;
                cboTenNCC.DisplayMember = "TenNCC";
                cboTenNCC.ValueMember = "MaNCC";
            }

            DBConn.CloseConnection();
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

        private void LoadDanhSachDonNhap()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNhapHang";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDN = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string tenNCC = reader.GetString(3);
                        string SLSP = reader.GetInt32(4).ToString();
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maDN);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }
            DBConn.CloseConnection();
        }

        private void btnNhapDon_Click(object sender, EventArgs e)
        {
            modSuaSoLuongSanPham = 0;

            string maDN = txtMaDN.Text;
            string tenNV = txtTenNV.Text;
            string loaiSP = cboLoaiSP.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            string tenNCC = cboTenNCC.Text;
            string maNCC = cboTenNCC.SelectedValue.ToString();
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

            if (maDN == "" || tenNV == "" || loaiSP == "" || tenNCC == "" || SLSP == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaDX(maDN) == true)
            {
                MessageBox.Show("Trùng mã đơn nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool _1 = SuaSoLuongSanPham(maSP, SLSP, maDN);
            bool _2 = ThemDonNhapVaoBang(maDN, tenNV, loaiSP, tenNCC, SLSP, ngayCapNhat);
            bool _3 = ThemDonNhapVaoBangThongKe(maDN, tenNV, loaiSP, SLSP, tenNCC, ngayCapNhat);

            if (_1 && _2 && _3)
            {
                MessageBox.Show("Nhập đơn hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachDonNhap();
                ResetTextBox();
            }
            else MessageBox.Show("Nhập đơn hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ThemDonNhapVaoBangThongKe(string maDN, string tenNV, string loaiSP, int sLSP, string tenNCC, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblThongKe (MaDH,TenKH,TenNCC,TenNV,LoaiSP,SLSP,NgayCapNhat,TrangThai) " +
                            "VALUES (@maDN,@tenKH,@tenNCC,@tenNV,@loaiSP,@sLSP,@ngayCapNhat,@trangThai)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
                cmd.Parameters.AddWithValue("@tenKH", "");
                cmd.Parameters.AddWithValue("@tenNV", tenNV);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@sLSP", sLSP);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);
                cmd.Parameters.AddWithValue("@trangThai", 0);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    return true;
                }
            }
            DBConn.CloseConnection();
            return false;
        }

        private bool ThemDonNhapVaoBang(string maDN, string tenNV, string loaiSP, string tenNCC, int SLSP, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblNhapHang (MaDN,TenNV,LoaiSP,TenNCC,SLSP,NgayCapNhat) " +
                            "VALUES (@maDN,@tenNV,@loaiSP,@tenNCC,@sLSP,@ngayCapNhat)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);
                cmd.Parameters.AddWithValue("@tenNV", tenNV);
                cmd.Parameters.AddWithValue("@loaiSP", loaiSP);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
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

        private bool SuaSoLuongSanPham(string maSP, int soLuongSanPhamMoi, string maDN)
        {
            if (modSuaSoLuongSanPham == 1)
            {
                int soLuongSanPhamTrongKho = LaySoLuongSanPham(maSP);
                int soLuongSanPhamTungXuat = LaySoLuongSanPhamTungNhap(maDN);
                if (soLuongSanPhamMoi <= 0)
                {
                    MessageBox.Show("Vui lòng nhập số lượng lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                int _soLuongSanPham = soLuongSanPhamTrongKho - soLuongSanPhamTungXuat + soLuongSanPhamMoi;
                bool _suaSoLuong = SuaSoLuong(maSP, _soLuongSanPham);
                if (_suaSoLuong) return true;
                return false;
            }
            int soLuongSanPhamCu = LaySoLuongSanPham(maSP);
            if (soLuongSanPhamMoi <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            int soLuongSanPham = soLuongSanPhamCu + soLuongSanPhamMoi;
            bool suaSoLuong = SuaSoLuong(maSP, soLuongSanPham);
            if (suaSoLuong) return true;
            return false;
        }

        private int LaySoLuongSanPhamTungNhap(string maDN)
        {
            DBConn.GetConnection();

            string query = "SELECT SLSP FROM tblNhapHang WHERE MaDN = @maDN";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);

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

                txtMaDN.Text = lvi.SubItems[0].Text;
                txtTenNV.Text = lvi.SubItems[1].Text;
                cboLoaiSP.Text = lvi.SubItems[2].Text;
                cboTenNCC.Text = lvi.SubItems[3].Text;
                txtSLSP.Text = lvi.SubItems[4].Text;

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                cboLoaiSP.Enabled = false;
                cboTenNCC.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextBox();
        }

        private void ResetTextBox()
        {
            txtMaDN.Text = "";
            txtTenNV.Text = "";
            cboLoaiSP.SelectedIndex = 0;
            cboTenNCC.SelectedIndex = 0;
            txtTenNV.Text = tenNV;
            txtSLSP.Text = "";

            btnXoa.Enabled = false;
            btnSua.Enabled = false;
            cboLoaiSP.Enabled = true;
            cboTenNCC.Enabled = true;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (chucVu != 1)
            {
                MessageBox.Show("Bạn không được quyền sửa thông tin đơn xuất hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            modSuaSoLuongSanPham = 1;

            string maDN = txtMaDN.Text;
            string tenNV = txtTenNV.Text;
            string loaiSP = cboLoaiSP.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            string tenNCC = cboTenNCC.Text;
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

            if (maDN == "" || tenNV == "" || loaiSP == "" || tenNCC == "" || SLSP == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaDX(maDN) == false)
            {
                MessageBox.Show("Mã đơn xuất hàng không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool _1 = SuaSoLuongSanPham(maSP, SLSP, maDN);
            bool _2 = SuaDonNhapHang(maDN, tenNV, loaiSP, tenNCC, SLSP, ngayCapNhat);
            bool _3 = SuaDonNhapHangOBangThongKe(maDN, tenNV, loaiSP, SLSP, tenNCC, ngayCapNhat);

            if (_1 && _2 && _3)
            {
                MessageBox.Show("Sửa thông tin nhập hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachDonNhap();
                ResetTextBox();
            }
            else MessageBox.Show("Sửa thông tin nhập hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool SuaDonNhapHangOBangThongKe(string maDN, string tenNV, string loaiSP, int sLSP, string tenNCC, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblThongKe SET TenNCC = @tenNCC,SLSP = @sLSP,NgayCapNhat = @ngayCapNhat WHERE MaDH = @maDN";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
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

        private bool SuaDonNhapHang(string maDN, string tenNV, string loaiSP, string tenNCC, int sLSP, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblNhapHang SET TenNCC = @tenNCC,SLSP = @sLSP,NgayCapNhat = @ngayCapNhat WHERE MaDN = @maDN";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
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
            string maDN = txtMaDN.Text;
            string maSP = cboLoaiSP.SelectedValue.ToString();
            DialogResult result = MessageBox.Show("Bạn muốn xóa đơn Xuất Hàng ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int soLuongTrongKho = LaySoLuongSanPham(maSP);
                int soLuongTungNhap = LaySoLuongSanPhamTungNhap(maDN);
                int slsp = soLuongTrongKho - soLuongTungNhap;
                SuaSoLuong(maSP, slsp);
                XoaDonNhapHang(maDN);
                LoadDanhSachDonNhap();
                ResetTextBox();
            }
        }

        private void XoaDonNhapHang(string maDN)
        {
            if (KiemTraMaDX(maDN) == false)
            {
                MessageBox.Show("Mã đơn Nhập hàng không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DBConn.GetConnection();

            string query = "DELETE FROM tblNhapHang WHERE MaDN = @maDN;" +
                            "DELETE FROM tblThongKe WHERE MaDH = @maDN AND TrangThai = 0";

            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Xóa đơn nhập hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("Xóa đơn nhập hàng KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();

        }

        private bool KiemTraMaDX(string maDN)
        {
            DBConn.GetConnection();
            string query = "SELECT * FROM tblNhapHang WHERE MaDN = @maDN";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", maDN);

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
            string tkMaDN = txtTkMaDN.Text;
            string tkTenNV = txtTkTenNV.Text;

            if (tkMaDN != "" && tkTenNV == "") TimKiemTheoMaDN(tkMaDN);
            if (tkMaDN == "" && tkTenNV != "") TimKiemTheoTenNV(tkTenNV);
            if (tkMaDN != "" && tkTenNV != "") TimKiemTheoMaDN(tkMaDN);
            if (tkMaDN == "" && tkTenNV == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachDonNhap();
            }
        }

        private void TimKiemTheoTenNV(string tkTenNV)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNhapHang WHERE TenNV LIKE @tenNV";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenNV", "%" + tkTenNV + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDN = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string tenNCC = reader.GetString(3);
                        string SLSP = reader.GetInt32(4).ToString();
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maDN);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaDN(string tkMaDN)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNhapHang WHERE MaDN = @maDN";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDN", tkMaDN);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string maDN = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string loaiSP = reader.GetString(2);
                        string tenNCC = reader.GetString(3);
                        string SLSP = reader.GetInt32(4).ToString();
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maDN);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(tenNCC);
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
