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
    public partial class SanPham : Form
    {
        DBConnection DBConn = new DBConnection();
        int chucVu = 0;
        public SanPham(int ChucVu)
        {
            InitializeComponent();
            this.ForeColor = Color.Black;
            chucVu = ChucVu;
            if (ChucVu != 1)
            {
                btnThem.Enabled = false;
            }
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            LoadDanhSachSanPham();
        }

        private void LoadDanhSachSanPham()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblSanPham";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maSP = reader.GetString(0);
                        string tenSP = reader.GetString(1);
                        string hang = reader.GetString(2);
                        string mauSac = reader.GetString(3);
                        string dungLuong = reader.GetInt32(4).ToString();
                        string soLuong = reader.GetInt32(5).ToString();
                        string giaBan = ChenDauCham(reader.GetDouble(6).ToString()) + " VNĐ";
                        string ngayCapNhat = reader.GetString(7);

                        ListViewItem lvi = new ListViewItem(maSP);
                        lvi.SubItems.Add(tenSP);
                        lvi.SubItems.Add(hang);
                        lvi.SubItems.Add(mauSac);
                        lvi.SubItems.Add(dungLuong);
                        lvi.SubItems.Add(soLuong);
                        lvi.SubItems.Add(giaBan);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }
            DBConn.CloseConnection();
        }

        private string ChenDauCham(string so)
        {
            int doDai = so.Length;
            int soDauCham = doDai / 3;

            for (int i = 1; i <= soDauCham; i++)
            {
                so = so.Insert(doDai - i * 3, ".");
            }

            return so;
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text;
            string tenSP = txtTenSP.Text;
            string hangSP = txtHang.Text;
            string mauSac = txtMauSac.Text;
            int dungLuong = 0;
            int soLuong = 0;
            double giaBan = 0;
            try
            {
                dungLuong = Convert.ToInt32(txtDungLuong.Text);
                giaBan = Convert.ToDouble(txtGiaBan.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vui lòng nhập 'Dung lượng','Giá bán' là số ");
                return;
            }

            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maSP == "" || tenSP == "" || hangSP == "" || mauSac == "" || dungLuong == 0 || giaBan == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (KiemTraMaSP(maSP) == false)
            {
                ThemSanPham(maSP, tenSP, hangSP, mauSac, dungLuong, soLuong, giaBan, ngayCapNhat);
            }
            else MessageBox.Show("Mã sản phẩm bị trùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ThemSanPham(string maSP, string tenSP, string hangSP, string mauSac, int dungLuong, int soLuong, double giaBan, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "INSERT INTO tblSanPham (MaSP,TenSP,Hang,MauSac,DungLuong,SoLuong,GiaBan,NgayCapNhat)" +
                        " VALUES (@maSP,@tenSP,@hangSP,@mauSac,@dungLuong,@soLuong,@giaBan,@ngayCapNhat)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maSP", maSP);
                cmd.Parameters.AddWithValue("@tenSP", tenSP);
                cmd.Parameters.AddWithValue("@hangSP", hangSP);
                cmd.Parameters.AddWithValue("@mauSac", mauSac);
                cmd.Parameters.AddWithValue("@dungLuong", dungLuong);
                cmd.Parameters.AddWithValue("@soLuong", soLuong);
                cmd.Parameters.AddWithValue("@giaBan", giaBan);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Thêm sản phẩm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachSanPham();
                    XoaTrangTextBox();
                }
                else MessageBox.Show("Thêm sản phẩm KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();
        }

        private bool KiemTraMaSP(string maSP)
        {
            DBConn.GetConnection();

            string query = "SELECT * FROM tblSanPham WHERE MaSP = @maSP";
            using(SQLiteCommand cmd = new SQLiteCommand(@query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maSP", maSP);
                
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return true;
                    else return false;
                }
            }
        }

        private void lsvDanhSach_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (lsvDanhSach.SelectedItems.Count > 0)
            {
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                ListViewItem lvi = lsvDanhSach.SelectedItems[0];

                txtMaSP.Text = lvi.SubItems[0].Text;
                txtTenSP.Text = lvi.SubItems[1].Text;
                txtHang.Text = lvi.SubItems[2].Text;
                txtMauSac.Text = lvi.SubItems[3].Text;
                txtDungLuong.Text = lvi.SubItems[4].Text;
                txtSoLuong.Text = lvi.SubItems[5].Text;
                txtGiaBan.Text = lvi.SubItems[6].Text;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if(chucVu != 1)
            {
                MessageBox.Show("Bạn không được quyền sửa thông tin sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maSP = txtMaSP.Text;
            string tenSP = txtTenSP.Text;
            string hangSP = txtHang.Text;
            string mauSac = txtMauSac.Text;
            int dungLuong = 0;
            double giaBan = 0;
            try
            {
                dungLuong = Convert.ToInt32(txtDungLuong.Text);
                giaBan = Convert.ToDouble(txtGiaBan.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vui lòng nhập 'Dung lượng','Giá bán' là số ");
                return;
            }

            DateTime date = DateTime.Now;
            string ngayCapNhat = $"{date.ToString("HH:mm (dd-MM-yy)")}";

            if (maSP == "" || tenSP == "" || hangSP == "" || mauSac == "" || dungLuong == 0 || giaBan == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (KiemTraMaSP(maSP)) SuaSanPham(maSP, tenSP, hangSP, mauSac, dungLuong, giaBan, ngayCapNhat);
            else MessageBox.Show("Không tồn tại mã sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SuaSanPham(string maSP, string tenSP, string hangSP, string mauSac, int dungLuong, double giaBan, string ngayCapNhat)
        {
            DBConn.GetConnection();

            string query = "UPDATE tblSanPham SET TenSP = @tenSP,Hang = @hangSP,MauSac = @mauSac,DungLuong = @dungLuong,GiaBan = @giaBan,NgayCapNhat = @ngayCapNhat WHERE MaSP = @maSP";
            using(SQLiteCommand cmd = new SQLiteCommand(query,DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenSP", tenSP);
                cmd.Parameters.AddWithValue("@hangSP", hangSP);
                cmd.Parameters.AddWithValue("@mauSac", mauSac);
                cmd.Parameters.AddWithValue("@dungLuong", dungLuong);
                cmd.Parameters.AddWithValue("@giaBan", giaBan);
                cmd.Parameters.AddWithValue("@ngayCapNhat", ngayCapNhat);
                cmd.Parameters.AddWithValue("@maSP", maSP);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Sửa thông tin sản phẩm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachSanPham();
                    btnSua.Enabled = false;
                    btnXoa.Enabled = false;
                    XoaTrangTextBox();
                }
                else MessageBox.Show("Sửa thông tin sản phẩm KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            DBConn.CloseConnection();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (chucVu != 1)
            {
                MessageBox.Show("Bạn không được quyền xóa sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn muốn xóa sản phẩm ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            string maSP = txtMaSP.Text;
            if (KiemTraMaSP(maSP)) XoaSanPham(maSP);
            else MessageBox.Show("Không tồn tại mã sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void XoaSanPham(string maSP)
        {
            DBConn.GetConnection();

            string query = "DELETE FROM tblSanPham WHERE MaSP = @maSP";
            using(SQLiteCommand cmd = new SQLiteCommand(query,DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maSP", maSP);

                int check = cmd.ExecuteNonQuery();
                if (check > 0)
                {
                    MessageBox.Show("Xóa sản phẩm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachSanPham();
                    btnSua.Enabled = false;
                    btnXoa.Enabled = false;
                    XoaTrangTextBox();
                }
                else MessageBox.Show("Xóa sản phẩm KHÔNG thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DBConn.CloseConnection();
        }

        private void XoaTrangTextBox()
        {
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtMaSP.Text = "";
            txtTenSP.Text = "";
            txtHang.Text = "";
            txtMauSac.Text = "";
            txtDungLuong.Text = "";
            txtSoLuong.Text = "";
            txtGiaBan.Text = "";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            XoaTrangTextBox();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tkMaSP = txtTkMaSP.Text;
            string tkTenSP = txtTkTenSP.Text;

            if (tkMaSP != "" && tkTenSP == "") TimKiemTheoMaSP(tkMaSP);
            if (tkMaSP == "" && tkTenSP != "") TimKiemTheoTenSP(tkTenSP);
            if (tkMaSP != "" && tkTenSP != "") TimKiemTheoMaSP(tkMaSP);
            if (tkMaSP == "" && tkTenSP == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachSanPham();
            }
        }

        private void TimKiemTheoTenSP(string tkTenSP)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblSanPham WHERE TenSP LIKE @tenSP";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenSP", "%"+tkTenSP+"%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maSP = reader.GetString(0);
                        string tenSP = reader.GetString(1);
                        string hang = reader.GetString(2);
                        string mauSac = reader.GetString(3);
                        string dungLuong = reader.GetInt32(4).ToString();
                        string soLuong = reader.GetInt32(5).ToString();
                        string giaBan = ChenDauCham(reader.GetDouble(6).ToString()) + " VNĐ";
                        string ngayCapNhat = reader.GetString(7);

                        ListViewItem lvi = new ListViewItem(maSP);
                        lvi.SubItems.Add(tenSP);
                        lvi.SubItems.Add(hang);
                        lvi.SubItems.Add(mauSac);
                        lvi.SubItems.Add(dungLuong);
                        lvi.SubItems.Add(soLuong);
                        lvi.SubItems.Add(giaBan);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaSP(string tkMaSP)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblSanPham WHERE MaSP = @maSP";
            using(SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maSP", tkMaSP);

                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.Read()) 
                    {
                        string maSP = reader.GetString(0);
                        string tenSP = reader.GetString(1);
                        string hang = reader.GetString(2);
                        string mauSac = reader.GetString(3);
                        string dungLuong = reader.GetInt32(4).ToString();
                        string soLuong = reader.GetInt32(5).ToString();
                        string giaBan = ChenDauCham(reader.GetDouble(6).ToString()) + " VNĐ";
                        string ngayCapNhat = reader.GetString(7);

                        ListViewItem lvi = new ListViewItem(maSP);
                        lvi.SubItems.Add(tenSP);
                        lvi.SubItems.Add(hang);
                        lvi.SubItems.Add(mauSac);
                        lvi.SubItems.Add(dungLuong);
                        lvi.SubItems.Add(soLuong);
                        lvi.SubItems.Add(giaBan);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }
    }
}
