using DashboardApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DashboardApp
{
    public partial class Form1 : Form
    {
        // 필드
        private Dashboard model;

        // 생성자
        public Form1()
        {
            InitializeComponent();
            // 초기값 - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btnLast7Days.Select(); // .Select() 컨트롤을 활성화

            model = new Dashboard();
            LoadData();
        }

        // private 메서드
        private void LoadData()
        {
            // Dashboard객체의 startDate멤버와 endDate멤버가 폼에서 선택한 날짜와 다르면, Dashboard객체의 날짜와 시간을 업데이트 하고,
            // 그에 따른 통계 데이터도 업데이트 한다.
            bool refreshData = model.LoadData(dtpStartDate.Value, dtpEndDate.Value);
            if (refreshData == true)
            {
                lblNumOrders.Text = model.NumOrders.ToString();
                lblTotalRevenue.Text = "$" + model.TotalRevenue.ToString();
                lblTotalProfit.Text = "$" + model.TotalProfit.ToString();

                lblNumCustomers.Text = model.NumCustomers.ToString();
                lblNumSuppliers.Text = model.NumSuppliers.ToString();
                lblNumProducts.Text = model.NumProducts.ToString();

                chartGrossRevenue.DataSource = model.GrossRevenueList;
                chartGrossRevenue.Series[0].XValueMember = "Date"; // 위 배열 원소 RevenueByDate객체의 멤버
                chartGrossRevenue.Series[0].YValueMembers = "TotalAmount";
                chartGrossRevenue.DataBind();

                chartTopProducts.DataSource = model.TopProductsList;
                chartTopProducts.Series[0].XValueMember = "Key";     // 위 배열 원소 KeyValuePair<string, int> 객체의 멤버
                chartTopProducts.Series[0].YValueMembers = "Value";
                chartTopProducts.DataBind();

                dgvUnderStock.DataSource = model.UnderstockList;
                dgvUnderStock.Columns[0].HeaderText = "Item";
                dgvUnderStock.Columns[1].HeaderText = "Units";

                Console.WriteLine("Loaded view :)");

            }
            else Console.WriteLine("View not loaded, same query");

        }

        private void DisableCustomDates()
        {
            dtpStartDate.Enabled = false;
            dtpEndDate.Enabled = false;
            btnOkCustomDate.Visible = false;
        }

        // 이벤트 메서드
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
        }

        private void btnLast7Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
        }

        private void btnLast30Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); // 올해, 이번달, 1일로 설정
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            DisableCustomDates();
        }

        private void btnCustomDate_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
        }

        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
