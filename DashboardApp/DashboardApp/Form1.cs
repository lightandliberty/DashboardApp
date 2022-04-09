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
        private Button currentButton;

        // 생성자
        public Form1()
        {
            InitializeComponent();
            // 초기값 - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btnLast7Days.Select(); // .Select() 컨트롤을 활성화
            SetDateMenuButtonUI(btnLast7Days); // 최근 7일간 버튼 선택을 UI에도 반영
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
                lblTotalRevenue.Text = "$" + model.TotalRevenue.ToString(); // 수익
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

        // 선택할 때마다 버튼을 조명하기 위해 currentButton생성
        private void SetDateMenuButtonUI(object button)
        {
            Button btn = button as Button;
            // 버튼을 하이라이트 (버튼의 색을 last30Days의 테두리 색으로 바꾸고, 글자를 흰색으로 설정)
            btn.BackColor = btnLast30Days.FlatAppearance.BorderColor;
            btn.ForeColor = Color.White;
            // 하이라이트 끔 (버튼을 처음 선택하는 게 아니고, 이전에 선택했던 버튼이랑 지금 버튼(매개변수로 전달된 버튼)이 다르면) (처음 선택하는 게 아니어야 이전에 선택했던 버튼을 끌 수 있고, 같은 버튼을 두번 클릭하면, 불필요하게 껐다켰다 하게 되므로)
            if(currentButton != null && currentButton != btn)
            {
                currentButton.BackColor = this.BackColor;
                currentButton.ForeColor = Color.FromArgb(124, 141, 181); // 원래 버튼의 글자색
            }
            // 지금 선택한 버튼을 이전에 선택한 버튼으로 저장
            currentButton = btn; // Set current button


            // 커스텀 기간 선택을 눌렀으면, 날짜 선택을 가능하게 만듦.
            if(btn == btnCustomDate)
            {
                dtpStartDate.Enabled = true;
                dtpEndDate.Enabled = true;
                btnOkCustomDate.Visible = true; // OK버튼을 보이게
                lblStartDate.Cursor = Cursors.Hand;   // 날짜 선택 레이블의 아이콘을 손 모양 커서로
                lblEndDate.Cursor = Cursors.Hand;
            }
            else // 커스텀 기간 선택 버튼이 아니면, 날짜 선택을 불가능하게 만들고, OK버튼을 사라지게 하고, 손모양 커서를 원래 커서로 되돌림
            {
                dtpStartDate.Enabled = false;
                dtpEndDate.Enabled = false;
                btnOkCustomDate.Visible = false; // OK버튼을 보이게
                lblStartDate.Cursor = Cursors.Default;   // 날짜 선택 레이블의 아이콘을 손 모양 커서로
                lblEndDate.Cursor = Cursors.Default;
            }


            //dtpStartDate.Enabled = false;
            //dtpEndDate.Enabled = false;
            //btnOkCustomDate.Visible = false;
        }

        // 이벤트 메서드
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonUI(sender);
        }

        private void btnLast7Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonUI(sender);
        }

        private void btnLast30Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonUI(sender);
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); // 올해, 이번달, 1일로 설정
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetDateMenuButtonUI(sender);
        }

        private void btnCustomDate_Click(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
            SetDateMenuButtonUI(sender);
        }

        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void lblStartDate_Click(object sender, EventArgs e)
        {
            if(currentButton == btnCustomDate)
            {
                dtpStartDate.Select();
                SendKeys.Send("%{DOWN}"); // 키 입력을 활성 응용프로그램에 보냄
            }
        }

        private void lblEndDate_Click(object sender, EventArgs e)
        {
            if (currentButton == btnCustomDate)
            {
                dtpEndDate.Select();
                SendKeys.Send("%{DOWN}"); // 키 입력을 활성 응용프로그램에 보냄
            }
        }

        // DateTimePicker의 값이 변하면, 그 위의 lblStartDate의 Text에 반영
        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            lblStartDate.Text = dtpStartDate.Text;
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            lblEndDate.Text = dtpEndDate.Text;
        }

        // 폼이 로드될 때 초기화
        private void Form1_Load(object sender, EventArgs e)
        {
            lblStartDate.Text = dtpStartDate.Text;
            lblEndDate.Text = dtpEndDate.Text;
            dgvUnderStock.Columns[1].Width = 50;
        }
    }
}
