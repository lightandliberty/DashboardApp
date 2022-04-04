using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardApp.Db
{
    public abstract class DbConnection
    {
        private readonly string connectionString;

        // 생성자
        public DbConnection()
        {
            connectionString = "Server=(local); DataBase = NorthwindStore; Integrated Security=true";
        }

        // SqlConnection생성자에 connectionString을 전달해서, 새 SqlConnection객체를 생성해서 리턴.
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

    }
}
