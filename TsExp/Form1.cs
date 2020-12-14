using DevExpress.Utils.Helpers;
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Filtering.Templates;
using DevExpress.Export;
using DevExpress.XtraPrinting;

namespace TsExp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var cont = new DemoDBEntities();
            var res = cont.TimeSheets.ToList();




            gridControl1.DataSource = res.OrderBy(x => Convert.ToDateTime(x.Date));

            gridView1.Columns[1].Caption = "EmpID";
            gridView1.Columns[3].Caption = "Reporting Manager";
            gridView1.Columns[6].Caption = "JobID";
            gridView1.Columns[9].Caption = "No of Hours";

            gridView1.RowUpdated += GridView1_RowUpdated;

            gridView1.RowCellStyle += GridView1_RowCellStyle;
        }

        private void GridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            //if (e.RowHandle != view.FocusedRowHandle &&
            //   ((e.RowHandle % 2 == 0 && e.Column.VisibleIndex % 2 == 1) ||
            //   (e.Column.VisibleIndex % 2 == 0 && e.RowHandle % 2 == 1)))
            //    e.Appearance.BackColor = Color.NavajoWhite;

            //if (e.CellValue.ToString() == "Holiday")
            //{
            //    e.Appearance.BackColor = Color.Yellow;
            //}

            GridView gv = sender as GridView;
            string value = Convert.ToString(gv.GetRowCellValue(e.RowHandle, "Client"));
            switch (value)
            {
                case "Holiday":
                    e.Appearance.BackColor = Color.Yellow;
                    break;
                case "Leave":
                    e.Appearance.BackColor = Color.Yellow;
                    break;
                case "leave":
                    e.Appearance.BackColor = Color.Yellow;
                    break;
            }

        }

        private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            var dateVal = "";
            var taskVal = "";
            var time = "";
            var id = 0;
            foreach (int rowHandle in selectedRows)
            {
                if (rowHandle >= 0)
                {
                    dateVal = gridView1.GetRowCellValue(rowHandle, "Date").ToString();
                    taskVal = gridView1.GetRowCellValue(rowHandle, "Tasks").ToString();
                    time = gridView1.GetRowCellValue(rowHandle, "No_of_Hours").ToString();
                    id = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "Id"));
                    using (var cont = new DemoDBEntities())
                    {
                        var res = cont.TimeSheets.Where(x => x.Id == id).First();
                        res.Date = dateVal;
                        res.Tasks = taskVal;
                        res.No_of_Hours = Convert.ToInt32(time);
                        cont.SaveChanges();
                    }
                }
            }
            MessageBox.Show("Updated!!!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            ac();
            var cont = new DemoDBEntities();
            var res = cont.TimeSheets.ToList();

            if (comboBox1.SelectedIndex == 0)
                gridControl1.DataSource = res.Where(x => Convert.ToDateTime(x.Date).Month == DateTime.Now.Month).OrderBy(x => Convert.ToDateTime(x.Date));
            else
                gridControl1.DataSource = res.Where(x => Convert.ToDateTime(x.Date).ToString("MM") == DateTime.Now.AddMonths(-1).ToString("MM")).OrderBy(x => Convert.ToDateTime(x.Date));

            gridControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            gridControl1.LookAndFeel.UseDefaultLookAndFeel = false; // <<<<<<<<  
            gridView1.Appearance.HeaderPanel.Options.UseBackColor = true;
            gridView1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.BurlyWood;
        }

        public void ac()
        {
            var context = new DemoDBEntities();
            var li = context.igis.Select(x => x.Client).ToList();
            AutoCompleteStringCollection acl = new AutoCompleteStringCollection();

            foreach (var re in li)
            {
                acl.Add(re);
            }

            txtClient.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtClient.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtClient.AutoCompleteCustomSource = acl;

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string FileName = @"C:\Users\vinay\Desktop\Timesheet - " + DateTime.Now.ToString("MMM yyyy") + ".xlsx";
            gridControl1.ExportToXlsx(FileName);
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            var date = "";
            var com = comboBox1.SelectedIndex;
            if (txtDate.Text != "")
            {
                date = Convert.ToDateTime(txtDate.Text).ToString("dd-MMM-yyyy");
            }
            else
            {
                MessageBox.Show("Date enter mado gaandu");
            }

            if (txtTask.Text == "" || string.IsNullOrEmpty(txtTask.Text) || string.IsNullOrWhiteSpace(txtTask.Text))
            {
                MessageBox.Show("Task enter mado gaandu");
            }
            else if (txtTime.Text == "" || string.IsNullOrEmpty(txtTime.Text) || string.IsNullOrWhiteSpace(txtTime.Text))
            {
                MessageBox.Show("Time enter mado gaandu");
            }
            else if (txtClient.Text == "" || string.IsNullOrEmpty(txtClient.Text) || string.IsNullOrWhiteSpace(txtClient.Text))
            {
                MessageBox.Show("Client enter mado gaandu");
            }
            else if (txtJobId.Text == "" || string.IsNullOrEmpty(txtJobId.Text) || string.IsNullOrWhiteSpace(txtJobId.Text))
            {
                MessageBox.Show("Job id enter mado gaandu");
            }
            else
            {
                try
                {
                    var cont = new DemoDBEntities();
                    var lastDate = cont.TimeSheets.Where(x => x.Id == cont.TimeSheets.Max(y => y.Id)).Select(x => x.Date).First();
                    var lh = cont.TimeSheets.Where(x => x.Id == cont.TimeSheets.Max(y => y.Id)).Select(x => x.Client).First();
                    // && 
                    int? total = 0;
                    var check = "";
                    var ld = Convert.ToDateTime(lastDate).ToString("dd-MMM-yyyy");
                    var ed = Convert.ToDateTime(txtDate.Text).ToString("dd-MMM-yyyy");
                    total = cont.TimeSheets.Where(x => x.Date == lastDate).Select(x => x.No_of_Hours).Sum();
                    if (ld == ed)
                    {
                        check = "";
                    }
                    else if((lh.ToLower() == "holiday" || lh.ToLower() == "Leave"))
                    {
                        check = "";
                    }
                    else
                    {
                        check = total < 8 ? "Previous Day Total Time is less than 8 hours." : total > 8 ? "Previous Day Total Time is more than 8 hours." : "";
                    }

                    if (check != "")
                    {
                        MessageBox.Show(check);
                    }
                    else
                    {
                        var ts = new TimeSheet()
                        {
                            Date = date,
                            EmpId = txtEmpId.Text,
                            Name = txtName.Text,
                            Reporting_Manager = txtRepMan.Text,
                            Product = txtProduct.Text,
                            Client = txtClient.Text,
                            JobID = txtJobId.Text,
                            Project = txtProject.Text,
                            Tasks = txtTask.Text.Trim(),
                            No_of_Hours = Convert.ToInt32(txtTime.Text)
                        };

                        cont.TimeSheets.Add(ts);
                        cont.SaveChanges();
                        var res = cont.TimeSheets.ToList();
                        gridControl1.DataSource = res.Where(x => Convert.ToDateTime(x.Date).Month == DateTime.Now.Month).OrderBy(x => Convert.ToDateTime(x.Date)); ;
                        txtDate.Text = "";
                        txtClient.Clear();
                        txtTask.Clear();
                        txtTime.Clear();
                    }

                    comboBox1.SelectedIndex = com;

                    //var cont = new DemoDBEntities();
                    var res1 = cont.TimeSheets.ToList();

                    if (comboBox1.SelectedIndex == 0)
                    {
                        gridControl1.DataSource = res1.Where(x => Convert.ToDateTime(x.Date).Month == DateTime.Now.Month).OrderBy(x => Convert.ToDateTime(x.Date));
                        txtDate.Text = DateTime.Now.ToString();
                    }

                    else
                    {
                        gridControl1.DataSource = res1.Where(x => Convert.ToDateTime(x.Date).ToString("MM") == DateTime.Now.AddMonths(-1).ToString("MM")).OrderBy(x => Convert.ToDateTime(x.Date));
                        txtDate.Text = DateTime.Now.AddMonths(-1).ToString();
                    }

                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }

            }
        }

        private void txtClient_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var cont = new DemoDBEntities();
                var li = cont.igis.Where(x => x.Client == txtClient.Text).Select(x => x.Job_IDS).First();
                if (li != "" || !string.IsNullOrEmpty(li) || !string.IsNullOrWhiteSpace(li))
                {
                    txtJobId.Text = li;
                }
            }
            catch
            {

            }
        }

        private void txtDate_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(txtDate.Text).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(txtDate.Text).DayOfWeek == DayOfWeek.Sunday)
            {
                txtTime.Text = "0";
                txtClient.Text = "Holiday";
            }
            else
            {
                txtClient.Clear();
                txtTime.Clear();
                txtJobId.Clear();
            }
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            string FileName = @"C:\Users\vinay\Desktop\Timesheet - " + DateTime.Now.ToString("MMM yyyy") + ".xlsx";
            gridView1.Columns[10].Visible = false;
            gridControl1.ExportToXlsx(FileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
            //gridControl1.ExportToXlsx(FileName);
        }

        private void btnMeet_Click(object sender, EventArgs e)
        {
            txtTask.Text = "Jira and Internal meeting.";
            txtClient.Text = "HealthEC 3D Express Analytics";
            txtTime.Text = "2";
        }

        private void btnWo_Click(object sender, EventArgs e)
        {
            txtClient.Clear();
            txtJobId.Clear();
            txtTime.Clear();
            txtTask.Text = "Worked on ";
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            txtClient.Clear();
            txtJobId.Clear();
            txtTask.Clear();
            txtTime.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cont = new DemoDBEntities();
            var res = cont.TimeSheets.ToList();

            if (comboBox1.SelectedIndex == 0)
            {
                gridControl1.DataSource = res.Where(x => Convert.ToDateTime(x.Date).Month == DateTime.Now.Month).OrderBy(x => Convert.ToDateTime(x.Date));
                txtDate.Text = DateTime.Now.ToString();
            }

            else
            {
                gridControl1.DataSource = res.Where(x => Convert.ToDateTime(x.Date).ToString("MM") == DateTime.Now.AddMonths(-1).ToString("MM")).OrderBy(x => Convert.ToDateTime(x.Date));
                txtDate.Text = DateTime.Now.AddMonths(-1).ToString();
            }

        }

        private void txtTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }
    }
}
