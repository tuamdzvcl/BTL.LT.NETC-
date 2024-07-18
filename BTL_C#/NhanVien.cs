using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace BTL_C_
{
    public partial class NhanVien : Form
    {
        DBConnection DBConn = new DBConnection();

        public NhanVien()
        {
            InitializeComponent();
            this.ForeColor = Color.Black;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            radNhanVien.Checked = true;
            LoadDanhSachNhanVien();
        }

        private void LoadDanhSachNhanVien()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblTaiKhoan";
            using(SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read()) 
                    { 
                        string maNV = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string chucVu = "";
                        int _chucVu = reader.GetInt32(2);
                        if (_chucVu == 1)
                        {
                            chucVu = "Quản lý";
                        }
                        else chucVu = "Nhân viên";
                        string taiKhoan = reader.GetString(3);
                        string matKhau = reader.GetString(4);
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maNV);

                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(chucVu);
                        lvi.SubItems.Add(taiKhoan);
                        lvi.SubItems.Add(matKhau);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text;
            string tenNV = txtTenNV.Text;
            int chucVu = 0;
            if (radQuanLy.Checked) chucVu = 1;
            string taiKhoan = txtTaiKhoan.Text;
            string matKhau = txtMatKhau.Text;
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maNV == "" || tenNV == "" || taiKhoan == "" || matKhau == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNV(maNV))
            {
                MessageBox.Show("Mã nhà cung cấp bị trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ThemNhanVien(maNV, tenNV, chucVu, taiKhoan, matKhau, ngayCapNhat);
        }

        private void ThemNhanVien(string maNV, string tenNV, int chucVu, string taiKhoan, string matKhau, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblTaiKhoan (MaNV,TenNV,ChucVu,TaiKhoan,MatKhau,NgayCapNhat) " +
                            "VALUES (@maNV,@tenNV,@chucVu,@taiKhoan,@matKhau,@ngayCapNhat)";
            using(SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNV", maNV);
                cmd.Parameters.AddWithValue("@tenNV", tenNV);
                cmd.Parameters.AddWithValue("@chucVu", chucVu);
                cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);
                cmd.Parameters.AddWithValue("@matKhau", matKhau);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if(check > 0)
                {
                    MessageBox.Show("Thêm nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhanVien();
                    ResetTextBox();
                }
                else MessageBox.Show("Thêm nhân viên KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBConn.CloseConnection();
        }

        private void ResetTextBox()
        {
            txtMaNV.Text = "";
            txtTenNV.Text = "";
            radNhanVien.Checked = true;
            txtTaiKhoan.Text = "";
            txtMatKhau.Text = "";

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private bool KiemTraMaNV(string maNV)
        {
            DBConn.GetConnection();

            string query = "SELECT * FROM tblTaiKhoan WHERE MaNV = @maNV";
            using(SQLiteCommand cmd = new SQLiteCommand(query,DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNV", maNV);
                using(SQLiteDataReader reader =  cmd.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        return true;
                    }
                }
            }

            DBConn.CloseConnection();
            return false;
        }

        private void lsvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvDanhSach.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lsvDanhSach.SelectedItems[0];

                txtMaNV.Text = lvi.SubItems[0].Text;
                txtTenNV.Text = lvi.SubItems[1].Text;
                string chucVu = lvi.SubItems[2].Text;
                if (chucVu == "Quản lý") radQuanLy.Checked = true;
                else radNhanVien.Checked = true;
                txtTaiKhoan.Text = lvi.SubItems[3].Text;
                txtMatKhau.Text = lvi.SubItems[4].Text;

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextBox();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text;
            string tenNV = txtTenNV.Text;
            int chucVu = 0;
            if (radQuanLy.Checked) chucVu = 1;
            string taiKhoan = txtTaiKhoan.Text;
            string matKhau = txtMatKhau.Text;
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maNV == "" || tenNV == "" || taiKhoan == "" || matKhau == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNV(maNV) == false)
            {
                MessageBox.Show("Mã nhân viên không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SuaThongTinNhanVien(maNV, tenNV, chucVu, taiKhoan, matKhau, ngayCapNhat);
        }

        private void SuaThongTinNhanVien(string maNV, string tenNV, int chucVu, string taiKhoan, string matKhau, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblTaiKhoan SET TenNV = @tenNV,ChucVu = @chucVu,TaiKhoan = @taiKhoan,MatKhau = @matKhau,NgayCapNhat = @ngayCapNhat WHERE MaNV = @maNV";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNV", maNV);
                cmd.Parameters.AddWithValue("@tenNV", tenNV);
                cmd.Parameters.AddWithValue("@chucVu", chucVu);
                cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);
                cmd.Parameters.AddWithValue("@matKhau", matKhau);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Sửa thông tin nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhanVien();
                    ResetTextBox();
                }
                else MessageBox.Show("Sửa thông tin nhân viên KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text;
            if(maNV == "")
            {
                MessageBox.Show("Mã nhân viên không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNV(maNV) == false)
            {
                MessageBox.Show("Mã nhân viên không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XoaNhanVien(maNV);
        }

        private void XoaNhanVien(string maNV)
        {
            DBConn.GetConnection();

            string query = "DELETE FROM tblTaiKhoan WHERE MaNV = @maNV";
            using(SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNV", maNV);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Xóa nhân viên thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhanVien();
                    ResetTextBox();
                }
                else MessageBox.Show("Xóa nhân viên KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBConn.CloseConnection();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tkMaNV = txtTkMaNV.Text;
            string tkTenNV = txtTkTenNV.Text;

            if (tkMaNV != "" && tkTenNV == "") TimKiemTheoMaNV(tkMaNV);
            if (tkMaNV == "" && tkTenNV != "") TimKiemTheoTenNV(tkTenNV);
            if (tkMaNV != "" && tkTenNV != "") TimKiemTheoMaNV(tkMaNV);
            if (tkMaNV == "" && tkTenNV == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachNhanVien();
            }
        }

        private void TimKiemTheoTenNV(string tkTenNV)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblTaiKhoan WHERE TenNV LIKE @tenNV";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenNV", "%" + tkTenNV + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maNV = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string chucVu = "";
                        int _chucVu = reader.GetInt32(2);
                        if (_chucVu == 1)
                        {
                            chucVu = "Quản lý";
                        }
                        else chucVu = "Nhân viên";
                        string taiKhoan = reader.GetString(3);
                        string matKhau = reader.GetString(4);
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maNV);

                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(chucVu);
                        lvi.SubItems.Add(taiKhoan);
                        lvi.SubItems.Add(matKhau);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaNV(string tkMaNV)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblTaiKhoan WHERE MaNV = @maNV";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNV", tkMaNV);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string maNV = reader.GetString(0);
                        string tenNV = reader.GetString(1);
                        string chucVu = "";
                        int _chucVu = reader.GetInt32(2);
                        if (_chucVu == 1)
                        {
                            chucVu = "Quản lý";
                        }
                        else chucVu = "Nhân viên";
                        string taiKhoan = reader.GetString(3);
                        string matKhau = reader.GetString(4);
                        string ngayCapNhat = reader.GetString(5);

                        ListViewItem lvi = new ListViewItem(maNV);

                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(chucVu);
                        lvi.SubItems.Add(taiKhoan);
                        lvi.SubItems.Add(matKhau);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }
    }
}
