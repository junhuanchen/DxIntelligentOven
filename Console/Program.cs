using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZtsyLib;

namespace ConsoleUnitTest
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
    }

    public class ZwDave
    {
        ZtsyLib.Dave dave = new Dave();
        public bool Alive = false;
        Data tmp = new Data()
        {
            门状态 = "",
            温度 = "",
            倒数计时 = "",
            温度状态 = "",
            计时差 = "",
            系列号 = "",
            操作员工号 = "",
            操作机器号 = "",
            异常停机代码 = "",
            上下数分开记录信息 = "",
            扫描 = "",
        };


        public bool try_connect()
        {
            lock (dave) {
                if (Alive == false) {

                    Console.WriteLine("未连接，plc 开始联机.....");
                    Alive = dave.Start("192.168.2.1", 102);
                }

            }
           return true; 

        }
        public void get_data(Data tmp)
        {
            lock (dave)
            {
                if (Alive)
                {
                    Console.WriteLine("PLC 保持联机，正在获取数据......");

                    float VD110 = 0;
                    Alive = (0 == dave.GetVD(110, ref VD110));
                    if (Alive)
                    {
                        ;
                    }
                    //textbox1.text = "已联机";

                    //textbox2.text = "plc 保持联机，正在获取数据......";

                    //float VD110 = 0;
                    //Alive = (0 == dave.GetVD(110, ref VD110));
                    //if (Alive)
                    //{
                    //    textBox3.Text = "VD值：" + VD110.ToString();

                    //}

                    //float VD160 = 0;
                    //Alive = (0 == dave.GetVD(160, ref VD160));
                    //if (Alive)
                    //{
                    //    textBox4.Text += "温度值：" + "(±" + VD160.ToString() + ")℃";

                    //}
                    //ushort VW10 = 0;
                    //Alive = (0 == dave.GetVW(10, ref VW10));
                    //if (Alive)
                    //{
                    //    textBox5.Text = new DateTime().AddSeconds(VW10 / 10).ToString("HH:mm:ss");
                    //    //integerInput2.Value = VW10 / 600;
                    //}
                }
            }

       }

        public void set_data(Data tmp) 
        {
            if (Alive) {
                 ;
            }
        }

        public void output_data(Data tmp)
        {
            /* 打印 Book1 信息 */
            Console.WriteLine("门状态 : {0}", tmp.门状态);
            Console.WriteLine("温度: {0}", tmp.温度);
            Console.WriteLine("倒数计时: {0}", tmp.倒数计时);
            Console.WriteLine("温度状态: {0}", tmp.温度状态);
            Console.WriteLine("计时差 : {0}", tmp.计时差);
            Console.WriteLine("系列号: {0}", tmp.系列号);
            Console.WriteLine("操作员工号: {0}", tmp.操作员工号);
            Console.WriteLine("操作机器号： {0}", tmp.操作机器号);
            Console.WriteLine("异常停机代码: {0}", tmp.异常停机代码);
            Console.WriteLine("上下数分开记录信息: {0}", tmp.上下数分开记录信息);
            Console.WriteLine("扫描: {0}", tmp.扫描);

        }
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            var ts = new ZwDave();
            Data tmp = new Data()
            {
                门状态 = "close",
                温度 = "27",
                倒数计时 = "",
                温度状态 = "",
                计时差 = "",
                系列号 = "",
                操作员工号 = "",
                操作机器号 = "",
                异常停机代码 = "",
                上下数分开记录信息 = "",
                扫描 = "",
            };
            while (true)
            {
                Thread.Sleep(1000);
                if (ts.try_connect())
                {
                    ts.get_data(tmp);
                    ts.output_data(tmp);

                }
                
            }
        }
    }
}
