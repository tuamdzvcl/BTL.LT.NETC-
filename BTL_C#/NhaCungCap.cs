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

namespace BTL_C_
{
    public partial class NhaCungCap : Form
    {
        DBConnection DBConn = new DBConnection();

        public NhaCungCap()
        {
            InitializeComponent();
            this.ForeColor = Color.Black;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            LoadDanhSachNhaCC();
        }

        private void LoadDanhSachNhaCC()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maNCC = reader.GetString(0);
                        string tenNCC = reader.GetString(1);
                        string SDT = reader.GetString(2);
                        string diaChi = reader.GetString(3);
                        string ngayCapNhat = reader.GetString(4);

                        ListViewItem lvi = new ListViewItem(maNCC);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(diaChi);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void lsvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvDanhSach.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lsvDanhSach.SelectedItems[0];

                txtMaNCC.Text = lvi.SubItems[0].Text;
                txtTenNCC.Text = lvi.SubItems[1].Text;
                txtSDT.Text = lvi.SubItems[2].Text;
                txtDiaChi.Text = lvi.SubItems[3].Text;

                btnSua.Enabled = true;
                btnXoa.Enabled = true;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maNCC = txtMaNCC.Text;
            string tenNCC = txtTenNCC.Text;
            string sDT = txtSDT.Text;
            string diaChi = txtDiaChi.Text;
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maNCC == "" || tenNCC == "" || sDT == "" || diaChi == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNCC(maNCC))
            {
                MessageBox.Show("Mã nhà cung cấp bị trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ThemNhaCungCap(maNCC, tenNCC, sDT, diaChi, ngayCapNhat);
        }

        private void ThemNhaCungCap(string maNCC, string tenNCC, string sDT, string diaChi, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblNCC (MaNCC,TenNCC,SDT,DiaChi,NgayCapNhat) " +
                            "VALUES (@maNCC,@tenNCC,@sDT,@diaChi,@ngayCapNhat)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNCC", maNCC);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
                cmd.Parameters.AddWithValue("@sDT", sDT);
                cmd.Parameters.AddWithValue("@diaChi", diaChi);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Thêm nhà cung cấp thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhaCC();
                    ResetTextBox();
                }
                else MessageBox.Show("Thêm nhà cung cấp KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBConn.CloseConnection();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maNCC = txtMaNCC.Text;
            string tenNCC = txtTenNCC.Text;
            string sDT = txtSDT.Text;
            string diaChi = txtDiaChi.Text;
            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maNCC == "" || tenNCC == "" || sDT == "" || diaChi == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNCC(maNCC) == false)
            {
                MessageBox.Show("Mã nhà cung cấp không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SuaNhaCungCap(maNCC, tenNCC, sDT, diaChi, ngayCapNhat);
        }

        private void SuaNhaCungCap(string maNCC, string tenNCC, string sDT, string diaChi, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblNCC SET TenNCC = @tenNCC,SDT = @sDT,DiaChi = @diaChi,NgayCapNhat = @ngayCapNhat WHERE MaNCC = @maNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNCC", maNCC);
                cmd.Parameters.AddWithValue("@tenNCC", tenNCC);
                cmd.Parameters.AddWithValue("@sDT", sDT);
                cmd.Parameters.AddWithValue("@diaChi", diaChi);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Sửa thông tin nhà cung cấp thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhaCC();
                    ResetTextBox();
                }
                else MessageBox.Show("Sửa thông tin nhà cung cấp KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBConn.CloseConnection();
        }

        private bool KiemTraMaNCC(string maNCC)
        {
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNCC WHERE MaNCC = @maNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNCC", maNCC);

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

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc xóa nhà cung cấp ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;
            string maNCC = txtMaNCC.Text;
            if (maNCC == "")
            {
                MessageBox.Show("Vui lòng nhập Mã nhà cung cấp", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (KiemTraMaNCC(maNCC) == false)
            {
                MessageBox.Show("Mã nhà cung cấp không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XoaNhaCungCap(maNCC);
        }

        private void XoaNhaCungCap(string maNCC)
        {
            DBConn.GetConnection();

            string query = "DELETE FROM tblNCC WHERE MaNCC = @maNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNCC", maNCC);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Xóa nhà cung cấp thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachNhaCC();
                    ResetTextBox();
                }
                else MessageBox.Show("Xóa nhà cung cấp KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTextBox();
        }

        private void ResetTextBox()
        {
            txtMaNCC.Text = "";
            txtTenNCC.Text = "";
            txtSDT.Text = "";
            txtDiaChi.Text = "";

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tkMaNCC = txtTkMaNCC.Text;
            string tkTenNCC = txtTkTenNCC.Text;

            if (tkMaNCC != "" && tkTenNCC == "") TimKiemTheoMaNCC(tkMaNCC);
            if (tkMaNCC == "" && tkTenNCC != "") TimKiemTheoTenNCC(tkTenNCC);
            if (tkMaNCC != "" && tkTenNCC != "") TimKiemTheoMaNCC(tkMaNCC);
            if (tkMaNCC == "" && tkTenNCC == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachNhaCC();
            }
        }

        private void TimKiemTheoTenNCC(string tkTenNCC)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNCC WHERE TenNCC LIKE @tenNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenNCC", "%" + tkTenNCC + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maNCC = reader.GetString(0);
                        string tenNCC = reader.GetString(1);
                        string SDT = reader.GetString(2);
                        string diaChi = reader.GetString(3);
                        string ngayCapNhat = reader.GetString(4);

                        ListViewItem lvi = new ListViewItem(maNCC);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(diaChi);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaNCC(string tkMaNCC)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblNCC WHERE MaNCC = @maNCC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maNCC", tkMaNCC);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string maNCC = reader.GetString(0);
                        string tenNCC = reader.GetString(1);
                        string SDT = reader.GetString(2);
                        string diaChi = reader.GetString(3);
                        string ngayCapNhat = reader.GetString(4);

                        ListViewItem lvi = new ListViewItem(maNCC);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(SDT);
                        lvi.SubItems.Add(diaChi);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }
    }
}
