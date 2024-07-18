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
    public partial class Login : Form
    {
        DBConnection DBConn = new DBConnection();

        public Login()
        {
            InitializeComponent();
            DBConn.GetConnection();
        }

        private void lbDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txtTaiKhoan.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (taiKhoan == "" || matKhau == "") MessageBox.Show("Vui lòng nhập đầy đủ thông tin","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
            else DangNhap(taiKhoan, matKhau);
        }

        private void DangNhap(string taiKhoan, string matKhau)
        {
            bool login = false;
            string tenNV = "";
            int ChucVu = 0;

            string query = @"SELECT * FROM tblTaiKhoan WHERE TaiKhoan = @taiKhoan
                             AND MatKhau = @matKhau";
            using(SQLiteCommand cmd = new SQLiteCommand(query,DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);
                cmd.Parameters.AddWithValue("@matKhau", matKhau);

                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tenNV = reader.GetString(1);
                        ChucVu = reader.GetInt32(2);
                        login = true;
                    }
                    else MessageBox.Show("Tài khoản/Mật khẩu không đúng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (login)
            {
                this.Hide();
                Form homeForm = new Home(ChucVu, tenNV);
                homeForm.ShowDialog();
                txtMatKhau.Text = "";
                this.Show();
            }
        }

        private void lbDangNhap_MouseHover(object sender, EventArgs e)
        {
            lbDangNhap.Cursor = Cursors.Hand;
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có thật sự muốn Thoát ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                e.Cancel = false;
                DBConn.CloseConnection();
            }
            else e.Cancel = true;
        }
    }
}
