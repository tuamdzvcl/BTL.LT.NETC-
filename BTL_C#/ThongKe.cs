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
using System.Windows.Forms.VisualStyles;
using Excel = Microsoft.Office.Interop.Excel;

namespace BTL_C_
{
    public partial class ThongKe : Form
    {
        DBConnection DBConn = new DBConnection();

        public ThongKe()
        {
            InitializeComponent();
            this.ForeColor = Color.Black;

            LoadDanhSachThongKe();
            lbSLDN.Text = LaySoLuong(0);
            lbSLDX.Text = LaySoLuong(1);
        }

        private string LaySoLuong(int tt)
        {
            int sl = 0;
            DBConn.GetConnection();

            string query = "SELECT TrangThai FROM tblThongKe";
            using(SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                using(SQLiteDataReader reader= cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int trangThai = reader.GetInt32(0);
                        if (trangThai == tt) sl++;
                    }
                }
            }

            DBConn.CloseConnection();
            return sl.ToString();
        }

        private void LoadDanhSachThongKe()
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblThongKe";
            using(SQLiteCommand cmd = new SQLiteCommand(query,DBConn.conn))
            {
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDH = reader.GetString(0);
                        string trangThai = "";
                        int _trangThai = reader.GetInt32(7);
                        if (_trangThai == 0) trangThai = "Đơn nhập";
                        else trangThai = "Đơn xuất";
                        string tenKH = reader.GetString(1);
                        string tenNCC = reader.GetString(2);
                        string tenNV = reader.GetString(3);
                        string loaiSP = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDH);
                        lvi.SubItems.Add(trangThai);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void lsvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvDanhSach.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lsvDanhSach.SelectedItems[0];

                txtMaDH.Text = lvi.SubItems[0].Text;
                txtTrangThai.Text = lvi.SubItems[1].Text;
                txtTenKH.Text = lvi.SubItems[2].Text;
                txtTenNCC.Text = lvi.SubItems[3].Text;
                txtTenNV.Text = lvi.SubItems[4].Text;
                txtTenSP.Text = lvi.SubItems[5].Text;
                txtSLSP.Text = lvi.SubItems[6].Text;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMaDH.Text = "";
            txtTrangThai.Text = "";
            txtTenKH.Text = "";
            txtTenNCC.Text = "";
            txtTenNV.Text = "";
            txtTenSP.Text = "";
            txtSLSP.Text = "";
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tkMaDH = txtTkMaDH.Text;
            string tkTenNV_KH = txtTkTenNV_KH.Text;

            if (tkMaDH != "" && tkTenNV_KH == "") TimKiemTheoMaDH(tkMaDH);
            if (tkMaDH == "" && tkTenNV_KH != "") TimKiemTheoTenNV_KH(tkTenNV_KH);
            if (tkMaDH != "" && tkTenNV_KH != "") TimKiemTheoMaDH(tkMaDH);
            if (tkMaDH == "" && tkTenNV_KH == "")
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadDanhSachThongKe();
            }
        }

        private void TimKiemTheoTenNV_KH(string tkTenNV_KH)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblThongKe WHERE TenKH LIKE @tenKH OR TenNV LIKE @tenNV";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@tenKH", "%" + tkTenNV_KH + "%");
                cmd.Parameters.AddWithValue("@tenNV", "%" + tkTenNV_KH + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDH = reader.GetString(0);
                        string trangThai = "";
                        int _trangThai = reader.GetInt32(7);
                        if (_trangThai == 0) trangThai = "Đơn nhập";
                        else trangThai = "Đơn xuất";
                        string tenKH = reader.GetString(1);
                        string tenNCC = reader.GetString(2);
                        string tenNV = reader.GetString(3);
                        string loaiSP = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDH);
                        lvi.SubItems.Add(trangThai);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void TimKiemTheoMaDH(string tkMaDH)
        {
            lsvDanhSach.Items.Clear();
            DBConn.GetConnection();

            string query = "SELECT * FROM tblThongKe WHERE MaDH = @maDH";
            using (SQLiteCommand cmd = new SQLiteCommand(query, DBConn.conn))
            {
                cmd.Parameters.AddWithValue("@maDH", tkMaDH);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string maDH = reader.GetString(0);
                        string trangThai = "";
                        int _trangThai = reader.GetInt32(7);
                        if (_trangThai == 0) trangThai = "Đơn nhập";
                        else trangThai = "Đơn xuất";
                        string tenKH = reader.GetString(1);
                        string tenNCC = reader.GetString(2);
                        string tenNV = reader.GetString(3);
                        string loaiSP = reader.GetString(4);
                        string SLSP = reader.GetInt32(5).ToString();
                        string ngayCapNhat = reader.GetString(6);

                        ListViewItem lvi = new ListViewItem(maDH);
                        lvi.SubItems.Add(trangThai);
                        lvi.SubItems.Add(tenKH);
                        lvi.SubItems.Add(tenNCC);
                        lvi.SubItems.Add(tenNV);
                        lvi.SubItems.Add(loaiSP);
                        lvi.SubItems.Add(SLSP);
                        lvi.SubItems.Add(ngayCapNhat);

                        lsvDanhSach.Items.Add(lvi);
                    }
                }
            }

            DBConn.CloseConnection();
        }

        private void btnXuatFile_Click(object sender, EventArgs e)
        {
            XuatRaExcel(lsvDanhSach);
        }

        private void XuatRaExcel(ListView listView)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;

            worksheet.Cells.Font.Name = "Times New Roman";
            worksheet.Cells.Font.Size = 14;
            worksheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            worksheet.Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            // Ghi tiêu đề cột
            for (int i = 0; i < listView.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = listView.Columns[i].Text;
            }

            // Ghi dữ liệu từ ListView
            for (int i = 0; i < listView.Items.Count; i++)
            {
                for (int j = 0; j < listView.Items[i].SubItems.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = listView.Items[i].SubItems[j].Text;
                }
            }

            worksheet.Cells.EntireRow.AutoFit();
            worksheet.Cells.EntireColumn.AutoFit();
        }
    }
}
