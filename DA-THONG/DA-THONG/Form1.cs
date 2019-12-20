using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;
using Excel = Microsoft.Office.Interop.Excel;

namespace DA_THONG
{
    public partial class Form1 : Form
    {
        private MqttClient Client;
        private string message = "";
        private string[] mestmp = null;
        private string[] msnvtmp = new string[1000];
        private int stt,dis;
        
        public Form1()
        {
            InitializeComponent();
            try
            {

                Client = new MqttClient("m12.cloudmqtt.com", 17168,false, null, null, MqttSslProtocols.None);
                Client.ProtocolVersion = MqttProtocolVersion.Version_3_1;
                //string clientId = Guid.NewGuid().ToString();
                Client.Connect("datn", "frzzscrl", "clEabakgo1pU");

                Client.Subscribe(new String[] { "dkesp32" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                KillSpecificExcelFileProcess();


            }
            catch (MqttConnectionException)
            {
                
                MessageBox.Show("KHÔNG CÓ KẾT NỐI, KIỂM TRA KẾT NỐI MẠNG !");

            }
        }
        
        
        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            
            DateTime realtime = DateTime.Now;
            String hour = (realtime.ToString("hh"));
            String minute = (realtime.ToString("mm"));
            String dayss = (realtime.ToString("dd"));
            String monthss = (realtime.ToString("MM"));
            String yearss = (realtime.ToString("yyyy"));
            String gettime = dayss + ':' + monthss + ':' + yearss + ' '+ '-' +' '+ hour + ':' + minute;

            //************** EXCEL*************
            
            message = Encoding.Default.GetString(e.Message);
            Excel.Application app = new Excel.Application();

            Excel.Workbook workbook = app.Workbooks.Open("D:/quanlynhansu-EXCEL.xlsx");

            Excel.Worksheet xlWorksheet = (Excel.Worksheet)workbook.Sheets.get_Item(1);
            workbook.Activate();
            xlWorksheet.Activate();
            app.Visible = false;
            app.DisplayAlerts = false;
            xlWorksheet.Cells[1, 1] = "TÊN";
            xlWorksheet.Cells[1, 2] = "CHỨC VỤ";
            xlWorksheet.Cells[1, 3] = "MÃ SỐ";
            xlWorksheet.Cells[1, 4] = "GIỜ VÀO";
            xlWorksheet.Cells[1, 6] = "GIỜ RA";
            try
            {
                
                if (message != "ESP_reconnected")
                {
                    mestmp = message.Split('@');
                    Console.WriteLine("WORD_IN: " + message);

                    if (mestmp[1].Equals("IN"))
                    {
  
                        if (app == null)
                        {
                            MessageBox.Show("Excel is not properly installed!!");
                            return;
                        }
                        // ************* IN *****************
                        stt = Int32.Parse(mestmp[0]);
                        msnvtmp[stt] = mestmp[0];
                        
                        Console.WriteLine(msnvtmp[stt]);

                        SetText_sttIN(mestmp[0].ToString());
                        SetText_tenIN(mestmp[2].ToString()); SetText_chucvuIN(mestmp[3].ToString());
                        SetText_msIN(mestmp[4].ToString()); SetText_gioIN(gettime);
  
                        xlWorksheet.Cells[Int32.Parse(mestmp[0]) , 1] = mestmp[2];
                        xlWorksheet.Cells[Int32.Parse(mestmp[0]) , 2] = mestmp[3];
                        xlWorksheet.Cells[Int32.Parse(mestmp[0]) , 3] = mestmp[4];
                        xlWorksheet.Cells[Int32.Parse(mestmp[0]) , 4] = gettime;
                        
                        xlWorksheet.Columns.AutoFit();
                        //cleanup

                        workbook.Save();
                        workbook.Close();
                        app.Quit();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        KillSpecificExcelFileProcess();


                    }
                    if (mestmp[1].Equals("OUT"))
                    {
                        
                        if (app == null)
                        {
                            MessageBox.Show("Excel is not properly installed!!");
                            return;
                        }

                        // ************* OUT *****************
                       
                        
                        if (mestmp[0] == msnvtmp[Int32.Parse(mestmp[0])])
                        {
                            xlWorksheet.Cells[Int32.Parse(mestmp[0]) , 6] = gettime;
                        }
                        
                        xlWorksheet.Columns.AutoFit();
                        //cleanup

                        //cleanup

                        workbook.Save();
                        workbook.Close();
                        app.Quit();

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        KillSpecificExcelFileProcess();
                        SetText_sttOUT(mestmp[0].ToString());
                        SetText_tenOUT(mestmp[2].ToString()); SetText_chucvuOUT(mestmp[3].ToString());
                        SetText_msOUT(mestmp[4].ToString()); SetText_gioOUT(gettime);

                    }
                }
                if (dis == 1)
                {
                    Client.Disconnect();
                    Console.WriteLine("DIS-----------");
                    dis = 0;
                    Environment.Exit(1);
                }

            }
            catch (Exception) {
                
                workbook.Save();
                workbook.Close();
                app.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                KillSpecificExcelFileProcess();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.Unsubscribe(new String[] { "dkesp32" });
            Excel.Application app = new Excel.Application();

            Excel.Workbook workbook = app.Workbooks.Open("D:/quanlynhansu-EXCEL.xlsx");

            Excel.Worksheet xlWorksheet = (Excel.Worksheet)workbook.Sheets.get_Item(1);

                workbook.Activate();
                xlWorksheet.Activate();
                app.Visible = true;
                app.DisplayAlerts = false;
                //Environment.Exit(1);
            
        }
        delegate void SetTextCallback_tenIN(string text_tenIN); delegate void SetTextCallback_tenOUT(string text_tenOUT);
        delegate void SetTextCallback_chucvuIN(string text_chucvuIN); delegate void SetTextCallback_chucvuOUT(string text_chucvuOUT);
        delegate void SetTextCallback_msIN(string text_msIN); delegate void SetTextCallback_msOUT(string text_msOUT);
        delegate void SetTextCallback_gioIN(string text_gioIN); delegate void SetTextCallback_gioOUT(string text_gioOUT);
        delegate void SetTextCallback_sttIN(string text_sttIN); delegate void SetTextCallback_sttOUT(string text_sttOUT);

        //*********************** IN ****************************************
        private void SetText_sttIN(string text)
        {

            if (this.stt_IN.InvokeRequired)
            {
                SetTextCallback_sttIN d = new SetTextCallback_sttIN(SetText_sttIN);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.stt_IN.Text = text;
            }

        }
        private void SetText_tenIN(string text)
        {
            if (this.ten_IN.InvokeRequired)
            {
                SetTextCallback_tenIN d = new SetTextCallback_tenIN(SetText_tenIN);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.ten_IN.Text = text;
            }
        }
        private void SetText_gioIN(string text)
        {

            if (this.gio_IN.InvokeRequired)
            {
                SetTextCallback_gioIN d = new SetTextCallback_gioIN(SetText_gioIN);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.gio_IN.Text = text;
            }

        }
        private void SetText_chucvuIN(string text)
        {

            if (this.chucvu_IN.InvokeRequired)
            {
                SetTextCallback_chucvuIN d = new SetTextCallback_chucvuIN(SetText_chucvuIN);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.chucvu_IN.Text = text;
            }

        }
        private void SetText_msIN(string text)
        {

            if (this.maso_IN.InvokeRequired)
            {
                SetTextCallback_msIN d = new SetTextCallback_msIN(SetText_msIN);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.maso_IN.Text = text;
            }

        }

        //*********************** OUT ****************************************

        private void SetText_sttOUT(string text)
        {

            if (this.stt_OUT.InvokeRequired)
            {
                SetTextCallback_sttOUT d = new SetTextCallback_sttOUT(SetText_sttOUT);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.stt_OUT.Text = text;
            }

        }
        private void SetText_tenOUT(string text)
        {
            if (this.ten_OUT.InvokeRequired)
            {
                SetTextCallback_tenOUT d = new SetTextCallback_tenOUT(SetText_tenOUT);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.ten_OUT.Text = text;
            }
        }
        private void SetText_gioOUT(string text)
        {

            if (this.gio_OUT.InvokeRequired)
            {
                SetTextCallback_gioOUT d = new SetTextCallback_gioOUT(SetText_gioOUT);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.gio_OUT.Text = text;
            }

        }
        private void SetText_chucvuOUT(string text)
        {

            if (this.chucvu_OUT.InvokeRequired)
            {
                SetTextCallback_chucvuOUT d = new SetTextCallback_chucvuOUT(SetText_chucvuOUT);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.chucvu_OUT.Text = text;
            }

        }
        private void SetText_msOUT(string text)
        {

            if (this.maso_OUT.InvokeRequired)
            {
                SetTextCallback_msOUT d = new SetTextCallback_msOUT(SetText_msOUT);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.maso_OUT.Text = text;
            }

        }


        private void KillSpecificExcelFileProcess()
        {
            try
            {
                Process[] pl = Process.GetProcesses();
                foreach (Process p in pl)
                {
                    if (p.ProcessName.IndexOf("EXCEL") >= 0)
                    {
                        //Console.Write("EXCEL----------------");
                        p.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_restart_Click(object sender, EventArgs e)
        {
            Client.Subscribe(new String[] { "dkesp32" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            try
            {
                Client.Unsubscribe(new String[] { "dkesp32" });
                Environment.Exit(1);  
            }
            catch (Exception)
            {
                MessageBox.Show("error");

            }
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            try
            {              
                Client.Unsubscribe(new String[] { "dkesp32" });
                var x = MessageBox.Show("Đóng ứng dụng ? ",
                                     "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (x == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else
                    {

                    e.Cancel = false;    
                    Environment.Exit(1);
                
                    }
            }
            catch (Exception) {
                MessageBox.Show("error");

            }
        }

     
    }
}
