using System.Globalization;
using DashboardApp.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardApp.Models
{
    public class Dashboard : DbConnection
    {
        public struct RevenueByDate
        {

            public string Date { get; set; }
            public decimal TotalAmount { get; set; }
        }

        //필드 & 속성들
        private DateTime startDate;
        private DateTime endDate;
        private int numberDays;

        public int NumCustomers { get; private set; }
        public int NumSuppliers { get; private set; }
        public int NumProducts { get; private set; }
        public List<KeyValuePair<string, int>> TopProductsList { get; private set; }
        public List<KeyValuePair<string, int>> UnderstockList { get; private set; }
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public int NumOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        // 생성자
        public Dashboard()
        {

        }

        // Private methods
        private void GetNumberItems()
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    // Get total number of Customers
                    command.CommandText = "select count(id) from Customer";
                    NumCustomers = (int)command.ExecuteScalar();

                    // Get Total Number of Suppliers
                    command.CommandText = "select count(id) from Supplier";
                    NumSuppliers = (int)command.ExecuteScalar();

                    // Get Total Number of Products
                    command.CommandText = "select count(id) from Product";
                    NumProducts = (int)command.ExecuteScalar();

                    // Get Total Number of Orders
                    command.CommandText = @"select count(id) from [Order]" +
                        "Where OrderDate between @fromDate and @toDate";
                    // 매개변수 연결
                    command.Parameters.Add("@fromData", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    NumOrders = (int)command.ExecuteScalar();
                }
            }
        }  // GetNumberItems() 끝
        private void GetOrderAnalisys() // Order Analisys (주문 분석)
        {
            // 쿼리를 하기 전에 초기화를 해야 함
            GrossRevenueList = new List<RevenueByDate>();
            TotalProfit = 0;
            TotalRevenue = 0;

            using (SqlConnection connection = GetConnection()) // new SqlConnection("Server=(local); DataBase = NorthwindStore; Integrated Security=true") 을 반환
            {
                connection.Open(); // 연결을 오픈하고,
                using (var command = new SqlCommand())
                {
                    command.Connection = connection; // 연결 부분에 connection 대입
                    command.CommandText = @"select OrderDate, sum(TotalAmount)
                                            from[Order] 
                                            where OrderDate between @fromDate and @toDate            
                                            group by OrderDate";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    SqlDataReader reader = command.ExecuteReader();
                    // 쿼리 결과를 저장할 객체 생성
                    List<KeyValuePair<DateTime, decimal>> resultTable = new List<KeyValuePair<DateTime, decimal>>();
                    while(reader.Read()) // .Read()는 다음 행으로 이동하고, 행이 더 있으면 true.
                    {
                        // 행이 더 있는 경우이므로, reader[0]과 reader[1]을 쌍으로 저장.
                        resultTable.Add(new KeyValuePair<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1]));
                        TotalRevenue += (decimal)reader[0];
                    }
                    TotalProfit = TotalRevenue * 0.2m; // 20%
                    reader.Close();

                    // Group by Days
                    if(numberDays <= 30)
                    {
                        foreach(var item in resultTable)
                        {
                            // RevenueByDate는 string, decimal을 가지는 객체임
                            GrossRevenueList.Add(new RevenueByDate() {Date=item.Key.ToString("dd MMM"), TotalAmount=item.Value }); // " MMM" 월(약식)

                        }
                    }
                    // Group by Weeks
                    else if(numberDays <= 92) 
                    {   // LINQ
                        GrossRevenueList = (from orderList in resultTable // System.Globalization 참조
                                            group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = "Week " + order.Key.ToString(), 
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    #region LINQ 설명
                    /*
                       모든 결과 테이블에 대해, By문으로 Key값을 만들어, order배열에 저장하고,
                       그 order배열의 .Sum메서드로 각각의 원소의 .Value값을 더해서 객체를 만들어, List로 내보내면,
                       모든 결과 테이블의 모든 행들이 By문의 조건의 Key값으로 분류된 후,
                       RevenueByDate객체의 .Sum합계의 객체들로 새롭게 분류되어 GrossRevenueList에 저장되게 되는 듯 하다.
                     */
                    #endregion LINQ 설명 끝
                    // Group By Months
                    else if (numberDays <= (365 * 2))
                    {
                        bool isYear = numberDays <= 365 ? true : false;
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("MMM yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }

                    // Group by Years
                    else
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();

                    }





                }
            }

        }
        private void GetProductAnalisys()
        {
            TopProductsList = new List<KeyValuePair<string, int>>();
            UnderstockList = new List<KeyValuePair<string, int>>();
            // Using문을 꼭 사용해야 한다.( * 중요 * )
//            using(var connection = )
        }
    }
}
