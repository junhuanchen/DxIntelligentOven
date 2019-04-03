using DevComponents.DotNetBar.Charts;
using DevComponents.DotNetBar.Controls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Oven.Properties;
using Sql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ZtsyLib;

namespace Oven
{
    public partial class Index : DevComponents.DotNetBar.Metro.MetroAppForm
    {
        string ConnectStr = Settings.Default.ConnectSql;

        struct OutXls
        {
            public string OutPath;
            public string ϵ�к�;
        }

        ZtsyLib.Dave dave = new Dave();
        volatile bool Alive = false;
        Data Cache��һ�� = new Data();

        List<OutXls> ListOutXls = new List<OutXls>();

        System.Timers.Timer timer��һ�� = new System.Timers.Timer(30000);

        private const string PathName = @"../DATA";

        string ClearPath(string rPath)
        {
            StringBuilder rBuilder = new StringBuilder(rPath);
            foreach (char rInvalidChar in Path.GetInvalidPathChars())
            {
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            }
            return rBuilder.ToString();
        }

        public Index()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            if (!Directory.Exists(PathName))
            {
                Directory.CreateDirectory(PathName);  //����Ŀ¼
            }
            
            textBoxX3Timer.Elapsed += textBoxX3TimerOnTimedEvent;

            KeepConnect();

            timer��һ��.Elapsed += timer��һ��_Tick;

            // ���������ļ�
            
            textBoxX5.Text = Settings.Default.MachineNumber.ToString();

            textBoxX2.Text = Settings.Default.MachineWorker.ToString();

        }

        ~Index()
        {
            dave.Stop();

            // ���������ļ�

        }

        void KeepConnect()
        {
            lock (dave)
            {
                if (false == Alive)
                {
                    new Thread(() =>
                    {
                        dave.Stop();
                        Alive = dave.Start("192.168.2.1", 102);
                    }).Start();
                    ribbonTextBox2.Text = "PLC �Ͽ����������ڳ�������......";
                }
            }
        }

        DevComponents.DotNetBar.Controls.TextBoxX temp = new TextBoxX();

        void CheckLock()
        {
            var max = Settings.Default.MachineMax;
            var min = Settings.Default.MachineMin;
            
            if (max > min)
            {
                max--;
                Settings.Default.MachineMax = max;
            }
            else
            {
                var res = new InputName(temp, "ϵͳ����ά��������ϵ������").ShowDialog();
                if(res == DialogResult.Yes)
                {
                    var tmp = int.Parse(temp.Text.ToString());
                    switch (tmp)
                    {
                        case 518646: // 576891 784523
                            tmp = 6000;
                            break;
                        case 214369: // 218976 548732
                            tmp = 9000;
                            break;
                        case 745632: // 548321 648735
                            tmp = 12000;
                            break;
                        case 756699: // 853419 485169
                            tmp = 15000;
                            break;
                        case 083254: // 784538 214865
                            tmp = 18000;
                            break;
                        case 155836: // 243158 351876
                            tmp = int.MaxValue;
                            break;
                        default:
                            tmp = 1;
                            break;
                    }
                    
                    if (Settings.Default.MachineTmp < tmp)
                    {
                        Settings.Default.MachineMin = Settings.Default.MachineTmp;
                        Settings.Default.MachineMax = tmp;
                        Settings.Default.MachineTmp = Settings.Default.MachineMax;
                    }
                }
                CheckLock();
            } 
            Settings.Default.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            KeepConnect();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Alive)
            {
                metroTileItem9.TitleText = "������";
                metroTileItem9.TitleTextColor = Color.Green;
                
                ribbonTextBox2.Text = "PLC �������������ڻ�ȡ����......";

                Cache��һ��.ϵ�к� = metroTileItem4.TitleText;

                Cache��һ��.���������� = textBoxX5.Text;

                Cache��һ��.����Ա���� = textBoxX2.Text;

                Cache��һ��.�������ֿ���¼��Ϣ = "";

                Cache��һ��.ɨ�� = "yes";

                float VD110 = 0;
                Alive = (0 == dave.GetVD(110, ref VD110));
                if (Alive)
                {
                    metroTileItem6.TitleText = VD110.ToString();
                    integerInput1.Value = (int)VD110;
                }

                float VD160 = 0;
                Alive = (0 == dave.GetVD(160, ref VD160));
                if (Alive)
                {
                    metroTileItem6.TitleText += "(��" + VD160.ToString() + ")��";
                    integerInput3.Value = (int)VD160;
                }

                ushort VW10 = 0;
                Alive = (0 == dave.GetVW(10, ref VW10));
                if (Alive)
                {
                    metroTileItem2.TitleText = new DateTime().AddSeconds(VW10 / 10).ToString("HH:mm:ss");
                    integerInput2.Value = VW10 / 600;
                }

                if (true == metroTabItem4.Checked)
                {
                    ushort VW40 = 0;
                    Alive = (0 == dave.GetVW(40, ref VW40));
                    if (Alive)
                    {
                        integerInput5.Value = VW40 / 600;
                    }
                }
                else if (true == metroTabItem3.Checked)
                {
                    float VD100 = 0;
                    Alive = (0 == dave.GetVD(100, ref VD100));
                    if (Alive)
                    {

                        new Thread(r => {
                            Cache��һ��.�¶� = VD100.ToString("F1");

                            metroTileItem20.TitleText = VD100.ToString("F1") + "��";
                            var chart = chartControl1.ChartPanel.ChartContainers;
                            if (chart.Count != 0)
                            {
                                ChartXy chartXy = (ChartXy)chart[0];
                                var chartSerie = chartXy.ChartSeries[1];

                                if (chartSerie.SeriesPoints.Count > 60)
                                {
                                    chartSerie.SeriesPoints.RemoveAt(0);
                                }

                                chartSerie.SeriesPoints.Add(new SeriesPoint(DateTime.Now, new object[] { VD100 }));
                            }
                        }).Start();
                    }

                    byte TimeFlag = 0;
                    Alive = (0 == dave.GetMB(4, ref TimeFlag));

                    ushort VW20 = 0;
                    Alive = (0 == dave.GetVW(20, ref VW20));
                    if (Alive)
                    {
                        if (0x01 == (TimeFlag & 0x01))
                        {
                            metroTileItem3.Text = "<font color=\"#E1E1E1\"><h2>��ʱʱ��</h2><h6>Time Overtime</h6></font>";
                            metroTileItem3.TitleText = new DateTime().AddSeconds(VW20 / 10).ToString("-HH:mm:ss");
                        }
                        else
                        {
                            metroTileItem3.Text = "<font color=\"#E1E1E1\"><h2>ʣ��ʱ��</h2><h6>Time Remaining</h6></font>";
                            metroTileItem3.TitleText = new DateTime().AddSeconds(VW20 / 10).ToString("HH:mm:ss");
                        }

                        Cache��һ��.������ʱ = metroTileItem3.TitleText;
                    }
                    
                }

                byte MB10 = 0;
                Alive = (0 == dave.GetMB(10, ref MB10));
                if (Alive)
                {
                    if (0x08 == (MB10 & 0x08))
                    {
                        if (true == timer��һ��.Enabled)
                        {
                            timer��һ��.Enabled = false;
                            ListOutXls.Clear();
                        }
                    }
                }

                byte MB3 = 0;
                Alive = (0 == dave.GetMB(3, ref MB3));
                if (Alive)
                {
                    ribbonTextBox21.Text = MB3.ToString("X");

                    Cache��һ��.�쳣ͣ������ = ribbonTextBox21.Text;
                }

                bool IO0 = false;
                Alive = (0 == dave.GetIO(0, ref IO0));
                if (Alive)
                {
                    Cache��һ��.��״̬ = IO0 ? "closed" : "open";
                }
                
                byte MB4 = 0;
                Alive = (0 == dave.GetMB(4, ref MB4));
                if (Alive)
                {
                    Cache��һ��.��ʱ�� = (MB4 & 0x1) != 0 ? "yes" : "no";
                }

                byte MB5 = 0;
                Alive = (0 == dave.GetMB(5, ref MB5));
                if (Alive)
                {
                    Cache��һ��.�¶�״̬ = MB5 != 0 ? "yes" : "no";
                }

                byte MB0 = 0;
                Alive = (0 == dave.GetMB(0, ref MB0));
                if (Alive)
                {
                    metroTileItem26.TitleText = "Running";
                    metroTileItem26.TitleTextColor = Color.Green;

                    metroTileItem1.TitleText = "ֹͣ";
                    metroTileItem1.TitleTextColor = Color.Red;

                    if (0x04 == (MB0 & 0x04))
                    {
                        // metroTileItem26.TitleText = "First";
                        metroTileItem1.TitleText = "������";
                        metroTileItem1.TitleTextColor = Color.Green;

                        if (false == timer��һ��.Enabled && Cache��һ��.ϵ�к�.Length > 0)
                        {
                            timer��һ��.Enabled = true;
                            new Thread(r => {
                                Ouput��һ��Xls = string.Format("{0}/{1}.xls", PathName, ClearPath(Cache��һ��.ϵ�к�));
                                var tmp = new Data(Cache��һ��) { ϵ�к� = textBoxX3.Text };
                                OutPutReportData(Ouput��һ��Xls, tmp, DateTime.Now);
                            }).Start();
                        }
                    }
                    if (0x02 == (MB0 & 0x02))
                    {
                        // metroTileItem26.TitleText = "Error";
                        // metroTileItem26.TitleTextColor = Color.Red;
                    }

                    if (0x01 == (MB0 & 0x01))
                    {
                        metroTileItem26.TitleText = "Stoping";
                        metroTileItem26.TitleTextColor = Color.Red;
                    }

                }

            }
            else
            {
                metroTileItem9.TitleText = "δ����";
                metroTileItem9.TitleTextColor = Color.Red;
            }
            
            textBoxX6.Text = ListOutXls.Count.ToString();
            ribbonTextBox1.Text = DateTime.Now.ToString("yyyy��MM��dd��HHʱmm��ss��");
        }

        #region textBoxX3
        private DateTime textBoxX3InputBak = DateTime.Now;
        private System.Timers.Timer textBoxX3Timer = new System.Timers.Timer(1500);
        private void textBoxX3_KeyPress(object sender, KeyPressEventArgs e)
        {
            DateTime tmp = DateTime.Now; // �����������֮��ļ����ȷ��Ϊ���˹����롣
            if (tmp.Subtract(textBoxX3InputBak).Milliseconds > 50)
            {
                lock (textBox3)
                {
                    textBoxX3.Text = "";
                }
            }
            textBoxX3InputBak = tmp;
        }

        private void textBoxX3TimerOnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.Subtract(textBoxX3InputBak).Seconds > 1)
            {
                lock(textBox3)
                {
                    if(textBoxX3.Text.Length > 3)
                    {
                        textBoxX3Timer.Stop();
                        metroTileItem4.TitleText = textBoxX3.Text;

                        if (metroTileItem4.TitleText.Length > 16)
                        {
                            metroTileItem4.TitleText = metroTileItem4.TitleText.Insert(16, "\r\n");
                        }

                        CheckLock();

                        if (Alive)
                        {
                            Alive = (0 == dave.SetMB(1, 0xFF));
                            Thread.Sleep(50);
                            Alive = (0 == dave.SetMB(1, 0));
                        }

                        textBoxX3.Text = ClearPath(textBoxX3.Text);
                        Ouput��һ��Xls = string.Format("{0}/{1}.xls", PathName, textBoxX3.Text);
                        var tmp = new Data(Cache��һ��) { ϵ�к� = textBoxX3.Text };
                        // MessageBox.Show(string.Format("{0}\n{1}", Ouput��һ��Xls, tmp.ϵ�к�));
                        OutPutReportData(Ouput��һ��Xls, tmp, DateTime.Now);
                        ListOutXls.Add(new OutXls() { OutPath = Ouput��һ��Xls, ϵ�к� = textBoxX3.Text });
                        textBoxX3.Text = "";

                        if (false == timer��һ��.Enabled)
                        {
                            timer��һ��.Enabled = true;
                        }

                    }
                }
            }
        }
        private void textBoxX3_TextChanged(object sender, EventArgs e)
        {
            lock (textBox3)
            {
                if (Alive && false == textBoxX3Timer.Enabled && textBoxX3.Text.Length > 1)
                {
                    textBoxX3Timer.Start();
                }
            }
        }
        #endregion
        
        private void integerInput1_Click(object sender, EventArgs e)
        {
            dave.SetVD(110, (float)(integerInput1.Value));
        }

        private void integerInput2_Click(object sender, EventArgs e)
        {
            dave.SetVW(10, (ushort)(integerInput2.Value * 600));
        }

        private void integerInput3_Click(object sender, EventArgs e)
        {
            dave.SetVD(160, (float)(integerInput3.Value));
        }

        private void integerInput5_Click(object sender, EventArgs e)
        {
            dave.SetVW(40, (ushort)(integerInput5.Value * 600));
        }

        //#endregion

        void OutPutReportData(string ExcelName, Data Data, DateTime Now)
        {
        RE:
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(new FileStream(ExcelName, FileMode.Open, FileAccess.Read));

                ISheet sheet = hssfworkbook.GetSheet("Data");

                //if (sheet.LastRowNum > 0)
                //{
                //    sheet.ShiftRows(sheet.FirstRowNum + 1, sheet.LastRowNum, 1);

                //    //if (sheet.LastRowNum > 5000)
                //    //{
                //    //    System.IO.File.Move(PathName + ExcelName, PathName + ExcelName + Now.ToString("yyyy��MM��dd��HHʱmm��ss��"));
                //    //    // sheet.RemoveRow(sheet.GetRow(5000));
                //    //}
                //}

                // ADD
                var cells = sheet.CreateRow(sheet.LastRowNum + 1);
                cells.CreateCell(0).SetCellValue(Now.ToString("HHʱmm��ss��"));
                var fields = Data.GetType().GetFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    var cell = cells.CreateCell(i + 1);
                    cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    if (fields[i].Name == "�¶�")
                    {
                        cell.SetCellValue(double.Parse(fields[i].GetValue(Data).ToString()));
                    }
                    else
                    {
                        cell.SetCellValue(fields[i].GetValue(Data).ToString());
                    }
                }

                FileStream file = new FileStream(ExcelName, FileMode.Open, FileAccess.ReadWrite, FileShare.Write);

                hssfworkbook.Write(file);

                file.Close();
            }
            catch (FileNotFoundException ex)
            {
                try
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                    ISheet sheet = hssfworkbook.CreateSheet("Data");
                    // INIT
                    var top_cells = sheet.CreateRow(0);
                    top_cells.CreateCell(0).SetCellValue("          " + "ʱ��" + "          ");
                    sheet.AutoSizeColumn(0);
                    var fields = Data.GetType().GetFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        top_cells.CreateCell(i + 1).SetCellValue("          " + fields[i].Name + "          ");
                        sheet.AutoSizeColumn(i);
                    }
                    FileStream file = new FileStream(ExcelName, FileMode.OpenOrCreate);
                    hssfworkbook.Write(file);
                    file.Close();
                    goto RE;
                }
                catch (Exception e)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
            var res = new InputName(textBoxX2, "�����������Ա����").ShowDialog();
            
            Settings.Default.MachineWorker = textBoxX2.Text;
            Settings.Default.Save();
        }

        private void metroTabItem4_CheckedChanged(object sender, EventArgs e)
        {
            if(true == metroTabItem4.Checked)
            {
                var res = new Verification().ShowDialog();
                if(res != DialogResult.Yes)
                {
                    metroTabItem4.Checked = false;
                    metroTabItem3.Checked = true;
                }
            }
        }

        string Ouput��һ��Xls = PathName;
        private void timer��һ��_Tick(Object source, ElapsedEventArgs e)
        {
            foreach (var Out in ListOutXls)
            {
                // MessageBox.Show(string.Format("{0}\n{1}", Out.OutPath, Out.ϵ�к�));
                var tmp = new Data(Cache��һ��) { ϵ�к� = Out.ϵ�к� };
                OutPutReportData(Out.OutPath, tmp, DateTime.Now);
                new Thread(r => {
                    Data.PutSql(tmp, ConnectStr);
                }).Start();
            }
            // OutPutReportData(Ouput��һ��Xls, Cache��һ��, DateTime.Now);
        }
        
        private void integerInput4_ValueChanged(object sender, EventArgs e)
        {
            // �洢��������
            Settings.Default.MachineNumber = integerInput4.Value;
            Settings.Default.Save();
            textBoxX5.Text = integerInput4.Value.ToString();
        }

        private void metroTileItem1_Click(object sender, EventArgs e)
        {

        }

        private void metroTilePanel1_ItemClick(object sender, EventArgs e)
        {

        }

        private void metroTileItem9_Click(object sender, EventArgs e)
        {

        }
    }
}
