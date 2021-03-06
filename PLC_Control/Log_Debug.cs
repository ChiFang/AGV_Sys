﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.IO;

//使用excel資料讀取須宣告
using System.Data.OleDb;

//使用演算法須宣告
using AlgorithmTool;

namespace PLC_Control
{
    public partial class Log_Debug : UserControl
    {
        public Panel ShowLogDebugPanel;

        DataSet CarInfo = new DataSet();
        DataSet Main_Data = new DataSet();
        /** \brief是否要開預測紀錄*/
        public static rtAGV_Control DeliverData;

        public Log_Debug()
        {
            InitializeComponent();
            txtDataIndex.Text = "1";
            btnReadData.Enabled = false;//防止資料尚未開啟就進行運算導致程式錯誤

            if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.BigCar)
            {
                dataGridView_Log.Rows.Clear();
                dataGridView_Log.Columns.Clear();

                dataGridView_Log.Columns.Add("Index", "Index");
                dataGridView_Log.Columns.Add("P_X", "P_X");
                dataGridView_Log.Columns.Add("P_Y", "P_Y");
                dataGridView_Log.Columns.Add("Angle", "Angle");
                dataGridView_Log.Columns.Add("R_X", "R_X");
                dataGridView_Log.Columns.Add("R_Y", "R_Y");
                dataGridView_Log.Columns.Add("L_X", "L_X");
                dataGridView_Log.Columns.Add("L_Y", "L_Y");
                dataGridView_Log.Columns.Add("S_L", "S_L");
                dataGridView_Log.Columns.Add("S_R", "S_R");
                dataGridView_Log.Columns.Add("Src", "Src");
                dataGridView_Log.Columns.Add("Dst", "Dst");
            }
            else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.MediumCar)
            {

            }
            else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.SmallCar)
            {
                dataGridView_Log.Rows.Clear();
                dataGridView_Log.Columns.Clear();

                dataGridView_Log.Columns.Add("Index", "Index");
                dataGridView_Log.Columns.Add("P_X", "P_X");
                dataGridView_Log.Columns.Add("P_Y", "P_Y");
                dataGridView_Log.Columns.Add("Angle", "Angle");
                /*dataGridView_Log.Columns.Add("R_X", "R_X");
                dataGridView_Log.Columns.Add("R_Y", "R_Y");
                dataGridView_Log.Columns.Add("L_X", "L_X");
                dataGridView_Log.Columns.Add("L_Y", "L_Y");*/
                dataGridView_Log.Columns.Add("S_L", "S_L");
                dataGridView_Log.Columns.Add("S_R", "S_R");
                dataGridView_Log.Columns.Add("Src", "Src");
                dataGridView_Log.Columns.Add("Dst", "Dst");
            }
            else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.Other)
            {

            }


        }

        private void btnLogBack_Click(object sender, EventArgs e)
        {
            ShowLogDebugPanel.Visible = false;
            MainForm MachineType = new MainForm();
            MachineType.MachineType_Chang(true);
            
        }
        string filename;
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            CarInfo = new DataSet();
            Main_Data = new DataSet();

            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "(*.xlsx)|*.xlsx";
            if (OFD.ShowDialog() != DialogResult.OK) return;
            //if (!File.Exists(OFD.FileName)) return;
            filename = OFD.SafeFileName;
            //OFD.SafeFileName
            OFD.Dispose();

            string strCon = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filename + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'"; //此連接可以操作.xls與.xlsx文件
            using (OleDbConnection OleConn = new OleDbConnection(strCon))
            {
                OleConn.Open();
                String sql = "SELECT * FROM [CarInfo$]";
                OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(sql, OleConn);
                OleDaExcel.Fill(CarInfo);

                sql = "SELECT * FROM [Main_Data$]";
                OleDaExcel = new OleDbDataAdapter(sql, OleConn);
                OleDaExcel.Fill(Main_Data);

                OleConn.Close();
                btnReadData.Enabled = true;//防止資料尚未開啟就進行運算導致程式錯誤
            }
        }
        object[] obj;
        private void btnReadData_Click(object sender, EventArgs e)
        {
            if (txtDataIndex.Text == "")
            {
                MessageBox.Show("請輸入數值");
                return;
            }
            int Select_Index = Convert.ToInt16(txtDataIndex.Text) + 1;
            DeliverData = new rtAGV_Control();
            dataGridView_Log.Rows.Clear();
            try
            {
                //更新車體資訊
                DeliverData.tAGV_Data.tCarInfo.tPosition.eX = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][2].ToString());
                DeliverData.tAGV_Data.tCarInfo.tPosition.eY = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][3].ToString());
                DeliverData.tAGV_Data.tCarInfo.eAngle = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][0].ToString());

                DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedLeft = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][10].ToString());
                DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedRight = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][11].ToString());

                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eX = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][4].ToString());
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eY = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][5].ToString());
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eX = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][6].ToString());
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eY = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][7].ToString());
                DeliverData.tAGV_Data.tCarInfo.tMotorPosition.eX = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][8].ToString());
                DeliverData.tAGV_Data.tCarInfo.tMotorPosition.eY = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][9].ToString());

                //更新車Sensor資訊
                DeliverData.tAGV_Data.tCarInfo.eWheelAngle = Convert.ToDouble(CarInfo.Tables[0].Rows[Select_Index][1].ToString());
                //DeliverData.tAGV_Data.tSensorData.tForkInputData.height = (int)GlobalVar.ForkCurrentHeight;

                //取得Path資訊
                string[] DataSrc = Main_Data.Tables[0].Rows[Select_Index][9].ToString().Split('/');
                string[] DataDst = Main_Data.Tables[0].Rows[Select_Index][10].ToString().Split('/');
                DeliverData.tAGV_Data.atPathInfo = new rtPath_Info[1];
                DeliverData.tAGV_Data.atPathInfo[0].tSrc.eX = Convert.ToDouble(DataSrc[0]);
                DeliverData.tAGV_Data.atPathInfo[0].tSrc.eY = Convert.ToDouble(DataSrc[1]);
                DeliverData.tAGV_Data.atPathInfo[0].tDest.eX = Convert.ToDouble(DataDst[0]);
                DeliverData.tAGV_Data.atPathInfo[0].tDest.eY = Convert.ToDouble(DataDst[1]);
                DeliverData.tAGV_Data.atPathInfo[0].ucTurnType = 2;
                DeliverData.tAGV_Data.atPathInfo[0].ucStatus = 1;



                if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.BigCar)
                {
                    obj = new object[12] { Select_Index,  DeliverData.tAGV_Data.tCarInfo.tPosition.eX,  DeliverData.tAGV_Data.tCarInfo.tPosition.eY, 
                        DeliverData.tAGV_Data.tCarInfo.eAngle,   DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eX,   DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eY, 
                        DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eX,  DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eY,  DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedLeft,
                        DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedRight,  DeliverData.tAGV_Data.atPathInfo[0].tSrc.eX+"/"+ DeliverData.tAGV_Data.atPathInfo[0].tSrc.eY, 
                        DeliverData.tAGV_Data.atPathInfo[0].tDest.eX + "/" +  DeliverData.tAGV_Data.atPathInfo[0].tDest.eY};

                }
                else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.MediumCar)
                {

                }
                else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.SmallCar)
                {
                    obj = new object[8] { Select_Index,  DeliverData.tAGV_Data.tCarInfo.tPosition.eX,  DeliverData.tAGV_Data.tCarInfo.tPosition.eY, 
                        DeliverData.tAGV_Data.tCarInfo.eAngle, DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedLeft,
                        DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedRight,  DeliverData.tAGV_Data.atPathInfo[0].tSrc.eX+"/"+ DeliverData.tAGV_Data.atPathInfo[0].tSrc.eY, 
                        DeliverData.tAGV_Data.atPathInfo[0].tDest.eX + "/" +  DeliverData.tAGV_Data.atPathInfo[0].tDest.eY};

                }
                else if (MainForm.comboBox_MachineType_Num == (byte)rtAGV_Control.Type_Self_Carriage.Other)
                {

                }

                
                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.CreateCells(dataGridView_Log, obj);
                dgvr.Height = 35;
                dgvr.DefaultCellStyle.BackColor = Color.LightBlue;
                dataGridView_Log.Rows.Add(dgvr);


                //判斷現在車種為何
                DeliverData.rtAGV_Chang_Type_Self_Carriage(MainForm.comboBox_MachineType_Num);

                DeliverData.rtAGV_MotorCtrl(ref DeliverData.tAGV_Data.atPathInfo, 0, true);

                //Console.WriteLine("Power:" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorPower.ToString());
                //Console.WriteLine("lMotorAngle:" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorAngle.ToString());

                label_LogPower.Text = "Power：" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorPower.ToString();
                label_LogMotorAngle.Text = "MotorAngle：" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorAngle.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("此筆數據為非導航狀態");
            }
        }
    }
}
