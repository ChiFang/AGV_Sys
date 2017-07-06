using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Others;
using AlgorithmTool;

namespace LidarTool
{
    //矩形檢測範圍
    class DetectedRectRegion
    {
        //偵測範圍
        public float DetectRegionX1 = 0;
        public float DetectRegionX2 = 0;
        public float DetectRegionY1 = 0;
        public float DetectRegionY2 = 0;
        public float DetectRegionZ1 = 0;
        public float DetectRegionZ2 = 0;

        public DetectedRectRegion()
        { 
        
        }

        public DetectedRectRegion(float RegionX1, float RegionX2, float RegionY1, float RegionY2, float RegionZ1, float RegionZ2)
        {
            if (RegionX1 < RegionX2)
            {
                DetectRegionX1 = RegionX1;
                DetectRegionX2 = RegionX2;
            }
            else
            {
                DetectRegionX1 = RegionX2;
                DetectRegionX2 = RegionX1;
            }

            if (RegionY1 < RegionY2)
            {
                DetectRegionY1 = RegionY1;
                DetectRegionY2 = RegionY2;
            }
            else
            {
                DetectRegionY1 = RegionY2;
                DetectRegionY2 = RegionY1;
            }

            if (RegionY1 < RegionY2)
            {
                DetectRegionZ1 = RegionZ1;
                DetectRegionZ2 = RegionZ2;
            }
            else
            {
                DetectRegionZ1 = RegionZ2;
                DetectRegionZ2 = RegionZ1;
            }
        }
    }

    //圓形檢測範圍
    class DetectCircleRegion
    {
        public float CenterX = 0;
        public float CenterY = 0;
        public float ZUp = 0;
        public float ZDown = 0;
        public float Radius = 0;

        public DetectCircleRegion(float CircleCenterX, float CircleCenterY, float HUp, float HDown, float CirckeRadius)
        {
            CenterX = CircleCenterX;
            CenterY = CircleCenterY;
            if (HUp > HDown)
            {
                ZUp = HUp;
                ZDown = HDown;
            }
            else
            {
                ZUp = HDown;
                ZDown = HUp;
            }
            Radius = CirckeRadius;
        }

        public DetectCircleRegion()
        { }
    }

    //障礙物資料
    class ObstacleData
    {
        //障礙物位置
        public List<float> ObstaclePosX = new List<float>();
        public List<float> ObstaclePosY = new List<float>();
        public List<float> ObstaclePosZ = new List<float>();
        public List<double> ObstacleDistance = new List<double>();
    }

    

    class ObstacleFunc
    {
        public Form MainForm = new Form();
        public Label DetectObjectlabel = new Label();

        //偵測障礙物用
        public Thread ObDetect = null;

        //偵測範圍矩形
        public DetectedRectRegion rAlarmRegion = new DetectedRectRegion(-2.9f, -1.5f, 0.7f, 2.5f, -0.3f, 0.8f);
        public DetectedRectRegion rDetectRegion = new DetectedRectRegion(-2.1f, -1.5f, 1.2f, 2.0f, -0.3f, 0.8f);

        //偵測範圍圓形
        public DetectCircleRegion rAlarmCircleRegion;
        public DetectCircleRegion rWarningCircleRegion;
        //rCircleRegion = new DetectCircleRegion(-1.8f, 1.8f, -0.3f, 0.8f, Obstacle_Ini.Lidar_Radius);

        //障礙物資料
        public ObstacleData AlarmObjectData = new ObstacleData();
        public ObstacleData ObjectData = new ObstacleData();

        public static int Information_Obstacle;

        public delegate void ShowResult(bool Isdetect);
        public delegate void ShowGradingResult(int DetectResult);
        public bool IsContinueDetected = false;

        Ini Obstacle_Ini = new Ini();

        //執行檢測
        public void RunObstacleDetect()
        {
            Obstacle_Ini.Read_ini_Cfg3();

            rWarningCircleRegion = new DetectCircleRegion(0f, 0f, -0.3f, 0.8f, Obstacle_Ini.Lidar_Radius);

            if (Obstacle_Ini.Lidar_Radius / 3 <= 1)
            {

                rAlarmCircleRegion = new DetectCircleRegion(0f, 0f, -0.3f, 0.8f, 1.1f);
            }
            else
            {
                rAlarmCircleRegion = new DetectCircleRegion(0f, 0f, -0.3f, 0.8f, (Obstacle_Ini.Lidar_Radius / 3));
            }

            while (IsContinueDetected)
            {
                //bool HaveObject = ObstacleDetected(LidarFunc.rLidarData, rDetectRegion, out ObjectData);
                Information_Obstacle = CircleObstacleDetected(LidarFunc.rLidarData, rWarningCircleRegion, rAlarmCircleRegion, out AlarmObjectData, out ObjectData);
                //MainForm.BeginInvoke(new ShowResult(ShowDetectResult), HaveObject);
                MainForm.BeginInvoke(new ShowGradingResult(ShowDetectResult), Information_Obstacle);
                //Console.WriteLine("Is there an obstacle ? " + HaveObject.ToString());
                /*int result = ObstacleDetected(LidarFunc.rLidarData, rAlarmRegion, rDetectRegion, out AlarmObjectData, out ObjectData);
                MainForm.BeginInvoke(new ShowGradingResult(ShowDetectResult), result);*/
            }
        }

        //更新UI
        public void ShowDetectResult(bool Isdetect)
        {
            if (Isdetect)
            {
                DetectObjectlabel.ForeColor = Color.Red;
                DetectObjectlabel.Text = "Have obstacle!";
            }
            else
            {
                DetectObjectlabel.ForeColor = Color.Black;
                DetectObjectlabel.Text = "No obstacle!";
            }
        }

        //更新UI
        public void ShowDetectResult(int DetectResult)
        {
            if (DetectResult==0)
            {
                DetectObjectlabel.ForeColor = Color.Black;
                DetectObjectlabel.Text = "No obstacle!";
            }
            else if (DetectResult == 1)
            {
                DetectObjectlabel.ForeColor = Color.YellowGreen;
                DetectObjectlabel.Text = "Warning! Have obstacle!";
            }
            else if (DetectResult == 2)
            {
                DetectObjectlabel.ForeColor = Color.Red;
                DetectObjectlabel.Text = "Stop! Have obstacle!";
            }
        }

        public bool ObstacleDetected(LidarData Data, DetectedRectRegion Region, out ObstacleData ObData)
        {
            bool Isdetect = false;
            ObData = new ObstacleData();
            double Distance = 0;
            for (int index = 0; index < Data.X.Length; index+=100)
            {
                if (Region.DetectRegionX1 < Data.X[index] && Data.X[index] < Region.DetectRegionX2 && Region.DetectRegionY1 < Data.Y[index] && Data.Y[index] < Region.DetectRegionY2 && Region.DetectRegionZ1 < Data.Z[index] && Data.Z[index] < Region.DetectRegionZ2)
                {
                    Distance = Math.Sqrt(Data.X[index] * Data.X[index] + Data.Y[index] * Data.Y[index] + Data.Z[index] * Data.Z[index]);
                    Isdetect = true;
                    ObData.ObstaclePosX.Add(Data.X[index]);
                    ObData.ObstaclePosY.Add(Data.Y[index]);
                    ObData.ObstaclePosZ.Add(Data.Z[index]);
                    ObData.ObstacleDistance.Add(Distance);
                }
            }
            return Isdetect;
        }

        public int CircleObstacleDetected(LidarData Data, DetectCircleRegion WarningCircleRegion, DetectCircleRegion AlarmCircleRegion, out ObstacleData WarningObData, out ObstacleData AlarmObData)
        {
            
            //bool Isdetect = false;
            int Result = 0;
            WarningObData = new ObstacleData();
            AlarmObData = new ObstacleData();

            double Distance = 0;
            double radius = 0;
            for (int index = 0; index < Data.X.Length; index++)
            {
                radius = Math.Sqrt((Data.X[index] - WarningCircleRegion.CenterX) * (Data.X[index] - WarningCircleRegion.CenterX) + (Data.Y[index] - WarningCircleRegion.CenterY) * (Data.Y[index] - WarningCircleRegion.CenterY));
                if (Data.X[index] != 0 && Data.Y[index] != 0 && Data.Z[index] != 0)
                {
                    if (WarningCircleRegion.ZDown <= Data.Z[index] && Data.Z[index] <= WarningCircleRegion.ZUp && radius <= WarningCircleRegion.Radius)
                    {
                        if (Data.Z[index] >= Obstacle_Ini.Lidar_Obstacle_Height)
                        {
                            //if (BanRegion.DetectRegionX1 < Data.X[index] && Data.X[index] < BanRegion.DetectRegionX2 && BanRegion.DetectRegionY1 < Data.Y[index] && Data.Y[index] < BanRegion.DetectRegionY2 && BanRegion.DetectRegionZ1 < Data.Z[index] && Data.Z[index] < BanRegion.DetectRegionZ2)
                            if (radius < WarningCircleRegion.Radius && radius < AlarmCircleRegion.Radius && radius > 1)
                            {
                                //當在警戒範圍內時(降速)
                                AlarmObData.ObstaclePosX.Add(Data.X[index]);
                                AlarmObData.ObstaclePosY.Add(Data.Y[index]);
                                AlarmObData.ObstaclePosZ.Add(Data.Z[index]);
                                AlarmObData.ObstacleDistance.Add(Distance);
                            }
                            else if (radius < WarningCircleRegion.Radius && radius > AlarmCircleRegion.Radius && radius > 1)
                            {
                                //當在危險範圍內時(停止)
                                WarningObData.ObstaclePosX.Add(Data.X[index]);
                                WarningObData.ObstaclePosY.Add(Data.Y[index]);
                                WarningObData.ObstaclePosZ.Add(Data.Z[index]);
                                WarningObData.ObstacleDistance.Add(Distance);
                            }
                            /*Distance = Math.Sqrt(Data.X[index] * Data.X[index] + Data.Y[index] * Data.Y[index] + Data.Z[index] * Data.Z[index]);
                            Isdetect = true;
                            ObData.ObstaclePosX.Add(Data.X[index]);
                            ObData.ObstaclePosY.Add(Data.Y[index]);
                            ObData.ObstaclePosZ.Add(Data.Z[index]);
                            ObData.ObstacleDistance.Add(Distance);*/
                        }
                    }
                }
            }

            if (AlarmObData.ObstaclePosX.Count != 0)
                Result = 2;
            else if (WarningObData.ObstaclePosX.Count != 0)
                Result = 1;

            return Result;
            //return Isdetect;
        }

        public int ObstacleDetected(LidarData Data, DetectedRectRegion AlarmRegion, DetectedRectRegion BanRegion, out ObstacleData AlarmObData, out ObstacleData BanObData)
        {
            int Result = 0;
            AlarmObData = new ObstacleData();
            BanObData = new ObstacleData();
            double Distance = 0;

            for (int index = 0; index < Data.X.Length; index++)
            {
                if (AlarmRegion.DetectRegionX1 < Data.X[index] && Data.X[index] < AlarmRegion.DetectRegionX2 && AlarmRegion.DetectRegionY1 < Data.Y[index] && Data.Y[index] < AlarmRegion.DetectRegionY2 && AlarmRegion.DetectRegionZ1 < Data.Z[index] && Data.Z[index] < AlarmRegion.DetectRegionZ2)
                {
                    Distance = Math.Sqrt(Data.X[index] * Data.X[index] + Data.Y[index] * Data.Y[index] + Data.Z[index] * Data.Z[index]);
                    if (Data.X[index] != 0 && Data.Y[index] != 0 && Data.Z[index] != 0)
                    {
                        if (Data.Z[index] >= Obstacle_Ini.Lidar_Obstacle_Height)
                        {
                            if (BanRegion.DetectRegionX1 < Data.X[index] && Data.X[index] < BanRegion.DetectRegionX2 && BanRegion.DetectRegionY1 < Data.Y[index] && Data.Y[index] < BanRegion.DetectRegionY2 && BanRegion.DetectRegionZ1 < Data.Z[index] && Data.Z[index] < BanRegion.DetectRegionZ2)
                            {
                                BanObData.ObstaclePosX.Add(Data.X[index]);
                                BanObData.ObstaclePosY.Add(Data.Y[index]);
                                BanObData.ObstaclePosZ.Add(Data.Z[index]);
                                BanObData.ObstacleDistance.Add(Distance);
                            }
                            else
                            {
                                AlarmObData.ObstaclePosX.Add(Data.X[index]);
                                AlarmObData.ObstaclePosY.Add(Data.Y[index]);
                                AlarmObData.ObstaclePosZ.Add(Data.Z[index]);
                                AlarmObData.ObstacleDistance.Add(Distance);
                            }
                        }
                    }
                }
            }
            if (BanObData.ObstaclePosX.Count != 0)
                Result = 2;
            else if (AlarmObData.ObstaclePosX.Count != 0)
                Result = 1;

            //回傳0 為無障礙物
            //回傳1 為警告區域內有障礙物
            //回傳2 為禁止區域有障礙物

            return Result;
        }

        public bool ObstacleDetected(float[] X, float[] Y, float[] Z, double DetectDistance, int DetectAngleRangeMin, int DetectAngleRangeMax)
        {
            //DetectAngleRangeMin, DetectAngleRangeMax 偵測的角度範圍(未用到)

            double Distance = 0;
            for (int index = 0; index < X.Length; index++)
            {
                Distance = Math.Sqrt(X[index] * X[index] + Y[index] * Y[index]);
                if (Distance < DetectDistance)
                    return true;
            }
            return false;
        }

    }
}
