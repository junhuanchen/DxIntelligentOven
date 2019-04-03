using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Sql
{
    public struct Data
    {
        public string 门状态;
        public string 温度;
        public string 倒数计时;
        public string 温度状态;
        public string 计时差;
        public string 系列号;
        public string 操作员工号;
        public string 操作机器号;
        public string 异常停机代码;
        public string 上下数分开记录信息;
        public string 扫描;

        public Data(Data tmp) : this()
        {
            this = tmp;
        }

        static public void PutSql(Data data, string ConnectStr = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Test;Data Source=LAPTOP-JUNHUAN")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectStr))
                {
                    conn.Open();
                    using (SqlCommand sqlCmd = conn.CreateCommand())
                    {
                        sqlCmd.CommandText = @"INSERTINTO[dbo].[oven_data]
                            ([collect_time]
                            ,[series_no]
                            ,[door_state]
                            ,[temperature]
                            ,[countdown]
                            ,[temperature_state]
                            ,[poor_timing]
                            ,[operator_worker]
                            ,[operator_machine]
                            ,[error_code]
                            ,[up_down_record]
                            ,[is_scan])VALUES(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12)
                        ";
                        SqlParameter[] sqlPara = new SqlParameter[] {
                            new SqlParameter("@1", DateTime.Now),
                            new SqlParameter("@2", data.系列号),
                            new SqlParameter("@3", data.门状态),
                            new SqlParameter("@4", data.温度),
                            new SqlParameter("@5", data.倒数计时),
                            new SqlParameter("@6", data.温度状态),
                            new SqlParameter("@7", data.计时差),
                            new SqlParameter("@8", data.操作员工号),
                            new SqlParameter("@9", data.操作机器号),
                            new SqlParameter("@10", data.异常停机代码),
                            new SqlParameter("@11", data.上下数分开记录信息),
                            new SqlParameter("@12", data.扫描),
                        };
                        sqlCmd.Parameters.AddRange(sqlPara); //把Paramerter 数组参数添加到sqlCmd中
                        sqlCmd.ExecuteNonQuery();
                    }
                    conn.Close();                                                                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    class Program
    {

       
        static void Main(string[] args)
        {
            Data.PutSql(new Data() { 上下数分开记录信息 = "", 倒数计时 = "", 异常停机代码 = "", 扫描 = "", 操作员工号 = "", 操作机器号 = "", 温度 = "", 温度状态 = "", 系列号 = "", 计时差 = "", 门状态 = "" });

            Console.ReadKey();
            return;

            //SQLServer 附加mdf文件
            string dataDir = AppDomain.CurrentDomain.BaseDirectory;
            if (dataDir.EndsWith(@"\bin\Debug\") || dataDir.EndsWith(@"\bin\Release\"))
            {
                dataDir = System.IO.Directory.GetParent(dataDir).Parent.Parent.FullName;
                AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
            }

            using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;"))
            {
                conn.Open();


                //写入一条数据
                string strUserName = "zhi";
                string strPWD = "Ab123456";
                using (SqlCommand sqlCmd = conn.CreateCommand())
                {
                    sqlCmd.CommandText = "insert into Mytable1(Name,Password) values (@UserName,@PWD) ";//连接字符串进行参数化
                    SqlParameter[] sqlPara = new SqlParameter[] {
                    new SqlParameter("UserName",strUserName),
                    new SqlParameter("PWD",strPWD)
                    };
                    sqlCmd.Parameters.AddRange(sqlPara); //把Paramerter 数组参数添加到sqlCmd中
                    sqlCmd.ExecuteNonQuery();
                    Console.WriteLine("Insert OK");
                }
                //从表中读取数据
                string strRead = "SELECT   ID, Name, Password FROM      MyTable1 ";
                using (SqlCommand sqlCmd = new SqlCommand(strRead, conn))
                {
                    //sqlDataReader 逐行读取数据
                    using (SqlDataReader sdr = sqlCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            int id = sdr.GetInt32(sdr.GetOrdinal("ID"));  //sdr.GetOrdinal 获取列的序号
                            string Name = sdr.GetString(sdr.GetOrdinal("Name"));
                            bool PWD = sdr.IsDBNull(sdr.GetOrdinal("Password"));
                            Console.WriteLine("ID:{0},Name:{1},PWD：{2}", id, Name, PWD);
                            Console.WriteLine(sdr.GetString(1));
                        }
                    }
                }
                conn.Close();//此处可以省略，Dispose()方法会自动检查
            }
            Console.ReadKey();
        }
    }
}
