using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using AlgorithmTool;

namespace Others
{
    public class Ini
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section,
        string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section,
        string key, string def, StringBuilder retVal,
        int size, string filePath);

        /** \brief Define: PID Power Coeffient Kp   */
        public double PID_POWER_COE_KP;

        /** \brief Define: PID Coefficient for Theta Offset Kp   */
        public double PID_THETA_OFFSET_COE_KP;

        /** \brief Define: PID Coefficient for Car Angle to motor angle Kp   */
        public double PID_ANGLE_CAR_MOTOR_COE_KP;


        /** \brief Define: PID Power Coeffient Kp   */
        public double PID_POWER_COE_KI;

        /** \brief Define: PID Coefficient for Theta Offset Kp   */
        public double PID_THETA_OFFSET_COE_KI;

        /** \brief Define: PID Coefficient for Car Angle to motor angle Kp   */
        public double PID_ANGLE_CAR_MOTOR_COE_KI;



        /** \brief Define: PID Power Coeffient Kp   */
        public double PID_POWER_COE_KD;

        /** \brief Define: PID Coefficient for Theta Offset Kp   */
        public double PID_THETA_OFFSET_COE_KD;

        /** \brief Define: PID Coefficient for Car Angle to motor angle Kp   */
        public double PID_ANGLE_CAR_MOTOR_COE_KD;


        /** \brief Define: Radius of smooth mode: 旋轉半徑 (判斷是否到達定點 開始準備轉向動作)   */
        public int RADIUS_SMOOTH;

        public string CAR_ID;

        /** \brief Define: 兩輪中心到後輪距離   */
        public int CAR_LENGTH;

        /** \brief Define: 兩輪距離   */
        public int CAR_WIDTH_WHEEL;

        /** \brief 偵測到物體幾秒後自走車要停止*/
        public int STOP_DETECTION_TIME = 10;

        /** \brief 自走車慢速速度*/
        public int SLOW_SPEED = 50;


        /** \brief Define: file name1   */
        public const string filename_Cfg1 = "Cfg_1.ini";

        public void Read_ini_Cfg1()
        {
            int size = 3000;//temp file source size
            StringBuilder temp = new StringBuilder(size); //temp file source

            #region P資訊
            GetPrivateProfileString("Motor Control Cfg", "PID_POWER_COE_KP", "", temp, size, ".\\" + filename_Cfg1);
            PID_POWER_COE_KP = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_THETA_OFFSET_COE_KP", "", temp, size, ".\\" + filename_Cfg1);
            PID_THETA_OFFSET_COE_KP = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_ANGLE_CAR_MOTOR_COE_KP", "", temp, size, ".\\" + filename_Cfg1);
            PID_ANGLE_CAR_MOTOR_COE_KP = Convert.ToDouble(temp.ToString());
            #endregion

            #region I資訊
            GetPrivateProfileString("Motor Control Cfg", "PID_POWER_COE_KI", "", temp, size, ".\\" + filename_Cfg1);
            PID_POWER_COE_KI = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_THETA_OFFSET_COE_KI", "", temp, size, ".\\" + filename_Cfg1);
            PID_THETA_OFFSET_COE_KI = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_ANGLE_CAR_MOTOR_COE_KI", "", temp, size, ".\\" + filename_Cfg1);
            PID_ANGLE_CAR_MOTOR_COE_KI = Convert.ToDouble(temp.ToString());
            #endregion

            #region D資訊
            GetPrivateProfileString("Motor Control Cfg", "PID_POWER_COE_KD", "", temp, size, ".\\" + filename_Cfg1);
            PID_POWER_COE_KD = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_THETA_OFFSET_COE_KD", "", temp, size, ".\\" + filename_Cfg1);
            PID_THETA_OFFSET_COE_KD = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Motor Control Cfg", "PID_ANGLE_CAR_MOTOR_COE_KD", "", temp, size, ".\\" + filename_Cfg1);
            PID_ANGLE_CAR_MOTOR_COE_KD = Convert.ToDouble(temp.ToString());
            #endregion

            GetPrivateProfileString("Motor Control Cfg", "RADIUS_SMOOTH", "", temp, size, ".\\" + filename_Cfg1);
            RADIUS_SMOOTH = Convert.ToInt32(temp.ToString());

            GetPrivateProfileString("Car Cfg", "CAR_ID", "", temp, size, ".\\" + filename_Cfg1);
            CAR_ID = temp.ToString();

            GetPrivateProfileString("Car Cfg", "CAR_LENGTH", "", temp, size, ".\\" + filename_Cfg1);
            CAR_LENGTH = Convert.ToInt32(temp.ToString());

            GetPrivateProfileString("Car Cfg", "CAR_WIDTH_WHEEL", "", temp, size, ".\\" + filename_Cfg1);
            CAR_WIDTH_WHEEL = Convert.ToInt32(temp.ToString());
        }

        /** \brief Define: 區域個數   */
        public int Region_Num;

        /** \brief Define: 第1區節點個數   */
        public int Node_Num_Local;

        /** \brief Define: 有幾區的貨架群集   */
        public int Container_Number_Region;
        
        /** \brief Define: 第1區有幾個貨架   */
        public int Container_Number;

        /** \brief Define: 第1區有幾個區域   */
        public int Block_Number;

        /** \brief Define: file name2   */
        public const string filename_Cfg2 = "Cfg_2.ini";


        public rtAGV_Control Read_ini_Cfg2(rtAGV_Control DeliverData)
        {
            //rtAGV_Control宣告
            DeliverData = new rtAGV_Control();
            DeliverData.tIS_Cfg.tMapCfg.Init();

            int size = 3000;//temp file source size
            StringBuilder temp = new StringBuilder(size); //temp file source

            #region 自走車地圖設定
            GetPrivateProfileString("AGV MAP Cfg", "Region_Num", "", temp, size, ".\\" + filename_Cfg2);
            Region_Num = Convert.ToInt32(temp.ToString());

            //宣告使用到的Region數量
            DeliverData.tIS_Cfg.tMapCfg.alRegionNum = Region_Num;

            //宣告節點使用到的空間
            //DeliverData.tAGV_Cfg.tMapCfg.atNodeLocal = new rtIS_MAP_node[Region_Num][];
            //DeliverData.tAGV_Cfg.tMapCfg.alPathTableLocal = new int[Region_Num][];
            DeliverData.tIS_Cfg.tMapCfg.atRegionCfg = new RegionOfMap[Region_Num];

            for (int i = 0; i < Region_Num; i++)
            {
                GetPrivateProfileString("AGV MAP Cfg", "Node_Num_Local_" + i.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                Node_Num_Local = Convert.ToInt32(temp.ToString());
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].lNodeNum = Node_Num_Local;
                // DeliverData.tAGV_Cfg.tMapCfg.atNodeLocal[i] = new rtIS_MAP_node[Node_Num_Local];

                //第i區的路徑權重 (有Node_Num_Local個節點，所以table大小是 Node_Num_Local x Node_Num_Local = )
                int Array_count = 0;
                int[] Array_Region_Path_Table = new int[Node_Num_Local * Node_Num_Local];
                for (int j = 0; j < Node_Num_Local; j++)
                {
                    for (int k = 0; k < Node_Num_Local; k++)
                    {
                        GetPrivateProfileString("AGV MAP Cfg", "Region_Path_Table_" + i.ToString() + "_" + j.ToString() + "_" + k.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                        Array_Region_Path_Table[Array_count++] = Convert.ToInt32(temp.ToString());
                    }
                }
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].alPathTableNode = Array_Region_Path_Table;

                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atNode = new rtNode[Node_Num_Local];
                for (int j = 0; j < Node_Num_Local; j++)
                {
                    GetPrivateProfileString("AGV MAP Cfg", "Region_Coordinate_X_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atNode[j].tCoordinate.eX = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("AGV MAP Cfg", "Region_Coordinate_Y_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atNode[j].tCoordinate.eY = Convert.ToInt32(temp.ToString());
                }
            }

            #endregion

            #region 區域資訊設定

            for (int i = 0; i < Region_Num; i++)
            {
                GetPrivateProfileString("Block Info", "Block_Number_" + i.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                Block_Number = Convert.ToInt32(temp.ToString());
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].lBlockNum = Block_Number;

                /*int iiii = DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[0].lBlockNum;
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[0].lBlockNum = 9; // by ancre ???????
  */
                //DeliverData.tAGV_Cfg.tMapCfg.atNodeLocal[i] = new rtIS_MAP_node[Node_Num_Local];

                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atBlockCfg = new rtBlockInfo[Block_Number];

                for (int j = 0; j < Block_Number; j++)
                {
                    GetPrivateProfileString("Block Info", "Block_X_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atBlockCfg[j].tCoordinate.eX = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Block Info", "Block_Y_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atBlockCfg[j].tCoordinate.eY = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Block Info", "Block_Width_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atBlockCfg[j].eWidth = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Block Info", "Block_Height_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atBlockCfg[j].eHeight = Convert.ToInt32(temp.ToString());
                }
            }     

            #endregion

            #region 貨倉資訊設定
            /*GetPrivateProfileString("Warehousing Info", "Container_Number_Region", "", temp, size, ".\\" + filename_Cfg2);
            Container_Number_Region = Region_Num;*/

            /*GetPrivateProfileString("Warehousing Info", "Container_Number_0", "", temp, size, ".\\" + filename_Cfg2);
            Container_Number_0 = Convert.ToInt32(temp.ToString());*/

            for (int i = 0; i < Region_Num; i++)
            {
                GetPrivateProfileString("Warehousing Info", "Container_Number_" + i.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                Container_Number = Convert.ToInt32(temp.ToString());
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].lWarehouseNum = Container_Number;
                DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg = new rtWarehouseInfo[Container_Number];

                for (int j = 0; j < Container_Number; j++)
                {
                    GetPrivateProfileString("Warehousing Info", "Container_Region_Id_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].lRegion = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_Index_Id_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].lNodeId = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_Height_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].eHeight = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_Depth_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].DistanceDepth = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_X_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].tCoordinate.eX = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_Y_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].tCoordinate.eY = Convert.ToInt32(temp.ToString());

                    GetPrivateProfileString("Warehousing Info", "Container_Direction_" + i.ToString() + "_" + j.ToString(), "", temp, size, ".\\" + filename_Cfg2);
                    DeliverData.tIS_Cfg.tMapCfg.atRegionCfg[i].atWarehouseCfg[j].eDirection = Convert.ToInt32(temp.ToString());
                }
            }
            #endregion
            return DeliverData;
        }




        /** \brief Define: 光達精確度   */
        public double Lidar_Accuracy;

        /** \brief Define: 光達中心   */
        public int Lidar_Center;

        /** \brief Define: 光達半徑   */
        public float Lidar_Radius;

        /** \brief Define: 障礙物高度   */
        public double Lidar_Obstacle_Height;

        /** \brief Define: 障礙物數量   */
        public int Lidar_Obstacle_Number;


        /** \brief Define: file name3   */
        public const string filename_Cfg3 = "Cfg_3.ini";


        public void Read_ini_Cfg3()
        {
            int size = 3000;//temp file source size
            StringBuilder temp = new StringBuilder(size); //temp file source

            #region 光達資訊
            GetPrivateProfileString("Lidar", "Lidar_Accuracy", "", temp, size, ".\\" + filename_Cfg3);
            Lidar_Accuracy = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Lidar", "Lidar_Center", "", temp, size, ".\\" + filename_Cfg3);
            Lidar_Center = Convert.ToInt16(temp.ToString());

            GetPrivateProfileString("Lidar", "Lidar_Radius", "", temp, size, ".\\" + filename_Cfg3);
            if (Convert.ToDouble(temp.ToString()) <= 1)
            {
                Lidar_Radius = (float)1.1;
            }
            else
            {
                Lidar_Radius = (float)Convert.ToDouble(temp.ToString());
            }

            GetPrivateProfileString("Lidar", "Lidar_Obstacle_Height", "", temp, size, ".\\" + filename_Cfg3);
            Lidar_Obstacle_Height = Convert.ToDouble(temp.ToString());

            GetPrivateProfileString("Lidar", "Lidar_Obstacle_Number", "", temp, size, ".\\" + filename_Cfg3);
            Lidar_Obstacle_Number = Convert.ToInt16(temp.ToString());

            GetPrivateProfileString("Lidar", "Stop_Detection_Time", "", temp, size, ".\\" + filename_Cfg3);
            STOP_DETECTION_TIME = Convert.ToInt16(temp.ToString());

            GetPrivateProfileString("Lidar", "Slow_Speed", "", temp, size, ".\\" + filename_Cfg3);
            SLOW_SPEED = Convert.ToInt16(temp.ToString());

            #endregion
        }

    }
}
