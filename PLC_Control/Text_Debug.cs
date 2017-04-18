using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//使用演算法須宣告
using AlgorithmTool;

namespace PLC_Control
{
    public partial class Text_Debug : UserControl
    {
        public Panel ShowTextDebugPanel;

        /** \brief是否要開預測紀錄*/
        public static rtAGV_Control DeliverData;

        public static string PositionX;
        public static string PositionY;
        public static string Angle;
        public static string CarTireSpeedLeft;
        public static string CarTireSpeedRight;
        public static string CarTirepositionR_X;
        public static string CarTirepositionR_Y;
        public static string CarTirepositionL_X;
        public static string CarTirepositionL_Y;
        public static string MotorPosition_X;
        public static string MotorPosition_Y;
        public static string WheelAngle;
        public static string TurnType;
        public static string Src_X;
        public static string Src_Y;
        public static string Dest_X;
        public static string Dest_Y;

        /// <summary>
        /// 讀取數值到文字輸入除錯頁面
        /// </summary>
        public static void CheckConfig()
        {
            //讀取設定檔
            if (File.Exists(Application.StartupPath + @"\config.boltun"))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + @"\config.boltun");

                PositionX = file.ReadLine();
                PositionY = file.ReadLine();
                Angle = file.ReadLine();
                CarTireSpeedLeft = file.ReadLine();
                CarTireSpeedRight = file.ReadLine();
                CarTirepositionR_X = file.ReadLine();
                CarTirepositionR_Y = file.ReadLine();
                CarTirepositionL_X = file.ReadLine();
                CarTirepositionL_Y = file.ReadLine();
                MotorPosition_X = file.ReadLine();
                MotorPosition_Y = file.ReadLine();
                WheelAngle = file.ReadLine();
                TurnType = file.ReadLine();
                Src_X = file.ReadLine();
                Src_Y = file.ReadLine();
                Dest_X = file.ReadLine();
                Dest_Y = file.ReadLine();

                file.Close();
            }
            else//讀不到設定檔時顯示預設資料
            {
                PositionX = "2383";
                PositionY = "-1686";
                Angle = "1";
                CarTireSpeedLeft = "0";
                CarTireSpeedRight = "0";
                CarTirepositionR_X = "2393";
                CarTirepositionR_Y = "-2285";
                CarTirepositionL_X = "2373";
                CarTirepositionL_Y = "-1087";
                MotorPosition_X = "884";
                MotorPosition_Y = "-1712";
                WheelAngle = "-0.308823529411765";
                TurnType = "2";
                Src_X = "2250";
                Src_Y = "-1550";
                Dest_X = "17080";
                Dest_Y = "-1550";
            }
        }

        /// <summary>
        /// 將文字輸入除錯頁面數值儲存起來
        /// </summary>
        public static void SaveConfig()
        {
            //儲存設定檔
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\config.boltun"))
            {
                sw.WriteLine(PositionX);
                sw.WriteLine(PositionY);
                sw.WriteLine(Angle);
                sw.WriteLine(CarTireSpeedLeft);
                sw.WriteLine(CarTireSpeedRight);
                sw.WriteLine(CarTirepositionR_X);
                sw.WriteLine(CarTirepositionR_Y);
                sw.WriteLine(CarTirepositionL_X);
                sw.WriteLine(CarTirepositionL_Y);
                sw.WriteLine(MotorPosition_X);
                sw.WriteLine(MotorPosition_Y);
                sw.WriteLine(WheelAngle);
                sw.WriteLine(TurnType);
                sw.WriteLine(Src_X);
                sw.WriteLine(Src_Y);
                sw.WriteLine(Dest_X);
                sw.WriteLine(Dest_Y);
                sw.WriteLine(Dest_Y);
            }
        }

        /// <summary>
        /// 將讀取到的數值放置相對應的文字框內
        /// </summary>
        public Text_Debug()
        {
            InitializeComponent();

            textBox_PositionX.Text = PositionX;
            textBox_PositionY.Text = PositionY;
            textBox_Angle.Text = Angle;
            textBox_CarTireSpeedLeft.Text = CarTireSpeedLeft;
            textBox_CarTireSpeedRight.Text = CarTireSpeedRight;
            textBox_CarTirepositionR_X.Text = CarTirepositionR_X;
            textBox_CarTirepositionR_Y.Text = CarTirepositionR_Y;
            textBox_CarTirepositionL_X.Text = CarTirepositionL_X;
            textBox_CarTirepositionL_Y.Text = CarTirepositionL_Y;
            textBox_MotorPosition_X.Text = MotorPosition_X;
            textBox_MotorPosition_Y.Text = MotorPosition_Y;
            textBox_WheelAngle.Text = WheelAngle;
            textBox_TurnType.Text = TurnType;
            textBox_Src_X.Text = Src_X;
            textBox_Src_Y.Text = Src_Y;
            textBox_Dest_X.Text = Dest_X;
        }
        
        private void btnTextBack_Click(object sender, EventArgs e)
        {
            ShowTextDebugPanel.Visible = false;
        }

        private void btnCalculationResults_Click(object sender, EventArgs e)
        {
            if (textBox_PositionX.Text == "" || textBox_PositionY.Text == "" || textBox_Angle.Text == ""
             || textBox_CarTireSpeedLeft.Text == "" || textBox_CarTireSpeedRight.Text == "" || textBox_CarTirepositionR_X.Text == ""
             || textBox_CarTirepositionR_Y.Text == "" || textBox_CarTirepositionL_X.Text == "" || textBox_CarTirepositionL_Y.Text == ""
             || textBox_MotorPosition_X.Text == "" || textBox_MotorPosition_Y.Text == "" || textBox_WheelAngle.Text == ""
             || textBox_Src_X.Text == "" || textBox_Src_Y.Text == "" || textBox_Dest_X.Text == "" || textBox_Dest_Y.Text == "")
            {
                MessageBox.Show("請輸入數值");
                return;
            }
            if (textBox_PositionX.Text == "")
            {
                return;
            }

            //int Select_Index = 1;
            DeliverData = new rtAGV_Control();
            dataGridView_Text.Rows.Clear();
            try
            {
                //更新車體資訊
                DeliverData.tAGV_Data.tCarInfo.tPosition.eX = Convert.ToDouble(textBox_PositionX.Text);
                DeliverData.tAGV_Data.tCarInfo.tPosition.eY = Convert.ToDouble(textBox_PositionY.Text);
                DeliverData.tAGV_Data.tCarInfo.eAngle = Convert.ToDouble(textBox_Angle.Text);

                DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedLeft = Convert.ToDouble(textBox_CarTireSpeedLeft.Text);
                DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedRight = Convert.ToDouble(textBox_CarTireSpeedRight.Text);

                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eX = Convert.ToDouble(textBox_CarTirepositionR_X.Text);
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eY = Convert.ToDouble(textBox_CarTirepositionR_Y.Text);
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eX = Convert.ToDouble(textBox_CarTirepositionL_X.Text);
                DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eY = Convert.ToDouble(textBox_CarTirepositionL_Y.Text);
                DeliverData.tAGV_Data.tCarInfo.tMotorPosition.eX = Convert.ToDouble(textBox_MotorPosition_X.Text);
                DeliverData.tAGV_Data.tCarInfo.tMotorPosition.eY = Convert.ToDouble(textBox_MotorPosition_Y.Text);

                //更新車Sensor資訊
                DeliverData.tAGV_Data.tCarInfo.eWheelAngle = Convert.ToDouble(textBox_WheelAngle.Text);

                //取得Path資訊
                DeliverData.tAGV_Data.atPathInfo = new rtPath_Info[1];
                DeliverData.tAGV_Data.atPathInfo[0].tSrc.eX = Convert.ToDouble(textBox_Src_X.Text);
                DeliverData.tAGV_Data.atPathInfo[0].tSrc.eY = Convert.ToDouble(textBox_Src_Y.Text);
                DeliverData.tAGV_Data.atPathInfo[0].tDest.eX = Convert.ToDouble(textBox_Dest_X.Text);
                DeliverData.tAGV_Data.atPathInfo[0].tDest.eY = Convert.ToDouble(textBox_Dest_Y.Text);
                DeliverData.tAGV_Data.atPathInfo[0].ucTurnType = Convert.ToByte(textBox_TurnType.Text);
                DeliverData.tAGV_Data.atPathInfo[0].ucStatus = 1;

                object[] obj = new object[11] { DeliverData.tAGV_Data.tCarInfo.tPosition.eX,  DeliverData.tAGV_Data.tCarInfo.tPosition.eY, 
                        DeliverData.tAGV_Data.tCarInfo.eAngle,   DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eX,   DeliverData.tAGV_Data.tCarInfo.tCarTirepositionR.eY, 
                        DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eX,  DeliverData.tAGV_Data.tCarInfo.tCarTirepositionL.eY,  DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedLeft,
                        DeliverData.tAGV_Data.tCarInfo.eCarTireSpeedRight,  DeliverData.tAGV_Data.atPathInfo[0].tSrc.eX+"/"+ DeliverData.tAGV_Data.atPathInfo[0].tSrc.eY, 
                        DeliverData.tAGV_Data.atPathInfo[0].tDest.eX + "/" +  DeliverData.tAGV_Data.atPathInfo[0].tDest.eY};

                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.CreateCells(dataGridView_Text, obj);
                dgvr.Height = 35;
                dgvr.DefaultCellStyle.BackColor = Color.LightBlue;
                dataGridView_Text.Rows.Add(dgvr);

                DeliverData.rtAGV_MotorCtrl(ref DeliverData.tAGV_Data.atPathInfo, 0, true);

                //Console.WriteLine("Power:" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorPower.ToString());
                //Console.WriteLine("lMotorAngle:" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorAngle.ToString());

                label_TextPower.Text = "Power：" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorPower.ToString();
                label_TextMotorAngle.Text = "MotorAngle：" + DeliverData.tAGV_Data.CMotor.tMotorData.lMotorAngle.ToString();

                //將數值寫回儲存資料內
                PositionX = textBox_PositionX.Text;
                PositionY = textBox_PositionY.Text;
                Angle = textBox_Angle.Text;
                CarTireSpeedLeft = textBox_CarTireSpeedLeft.Text;
                CarTireSpeedRight = textBox_CarTireSpeedRight.Text;
                CarTirepositionR_X = textBox_CarTirepositionR_X.Text;
                CarTirepositionR_Y = textBox_CarTirepositionR_Y.Text;
                CarTirepositionL_X = textBox_CarTirepositionL_X.Text;
                CarTirepositionL_Y = textBox_CarTirepositionL_Y.Text;
                MotorPosition_X = textBox_MotorPosition_X.Text;
                MotorPosition_Y = textBox_MotorPosition_Y.Text;
                WheelAngle = textBox_WheelAngle.Text;
                TurnType = textBox_TurnType.Text;
                Src_X = textBox_Src_X.Text;
                Src_Y = textBox_Src_Y.Text;
                Dest_X = textBox_Dest_X.Text;
                Dest_Y = textBox_Dest_Y.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("請正確輸入數值");
            }
        }
    }
}
