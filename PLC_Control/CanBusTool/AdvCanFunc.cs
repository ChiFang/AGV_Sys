using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PLC_Control;
using Others;

namespace CanBusTool
{
    public class AdvBusFunc
    {
        public static UInt32 m_devtype = 4;//USBCAN2
        public static UInt32 m_bOpen = 0;

        //Channel1
        public static UInt32 m_devind = 0;
        public static UInt32 m_canind = 0;
        public static VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[1000];
        public static UInt32[] m_arrdevtype = new UInt32[20];

        //Channel2
        public static UInt32 m_devind_2 = 0;
        public static UInt32 m_canind_2 = 0;
        public static VCI_CAN_OBJ[] m_recobj_2 = new VCI_CAN_OBJ[1000];
        public static UInt32[] m_arrdevtype_2 = new UInt32[20];

        /** \brief 轉向-堆高機電腦資料   */
        public static byte[] ReceiveComputer;

        /** \brief 轉向-堆高機電腦暫存資料   */
        public static byte[] ReceiveComputer_Storage;

        /** \brief 轉向-堆高機馬達資料   */
        public static byte[] ReceiveMotor;

        /** \brief 轉向-堆高機馬達暫存資料   */
        public static byte[] ReceiveMotor_Storage;

        /** \brief 暫存資料，計算Check資料用   */
        public static byte[] BufferData;

        /** \brief 轉向-給主控修改的資料   */
        public static byte[] TransData;

        /** \brief 行走-堆高機電腦資料   */
        public static byte[] ReceiveMoveComputer;

        /** \brief 行走-堆高機電腦暫存資料   */
        public static byte[] ReceiveMoveComputer_Storage;

        /** \brief 行走-堆高機馬達資料   */
        public static byte[] ReceiveMoveMotor;

        /** \brief 行走-堆高機馬達暫存資料   */
        public static byte[] ReceiveMoveMotor_Storage;

        /** \brief 行走-給主控修改的資料   */
        public static byte[] TransMoveData;

        /** \brief 監聽堆高機電腦的執行緒   */
        public static Thread ComputerThread;

        /** \brief 監聽堆高機馬達的執行緒   */
        public static Thread MotorThread;

        //手動控制
        public static bool CanControl = false;
        public static bool isSend = false;
        public static bool isMoveSend = false;
        public static bool CANBUS_TurnLeft = false;
        public static bool CANBUS_TurnRight = false;

        //2016.09.09彥昌新增
        public static AdvCANIO Device = new AdvCANIO();
        public static bool m_bRun = false;
        public static bool syncflag = false;
        public static uint nMsgCount = 1;
        public static Thread MotorThread_Adv;
        public static ThreadStart MotorThreadST_Adv;


        //CanBusFunc CanBusFun = new CanBusFunc();

        public static Thread ComputerThread_Adv;
        public static ThreadStart ComputerThreadST_Adv;

        public static AdvCANIO Device_CommandToMotor = new AdvCANIO();
        public static AdvCANIO Device_MotorToCommand = new AdvCANIO();

        public static string CanPortName1 = "can1";
        public static string CanPortName2 = "can2";
        
        //2016.09.09彥昌新增

        /// <summary>
        /// CanBus初始數據
        /// </summary>
        public static void CanInit()
        {
            ReceiveComputer = new byte[8];
            ReceiveMotor = new byte[8];
            ReceiveComputer_Storage = new byte[8];
            ReceiveMotor_Storage = new byte[8];
            BufferData = new byte[8];
            TransData = new byte[8];
            TransData[0] = 0xca;
            TransData[1] = 0x01;
            TransData[4] = GlobalVar.RotateSpeed;
            TransData[5] = GlobalVar.RotateSpeed;

            //行走馬達儲存空間宣告
            ReceiveMoveComputer = new byte[8];
            ReceiveMoveComputer_Storage = new byte[8];
            ReceiveMoveMotor = new byte[8];
            ReceiveMoveMotor_Storage = new byte[8];
            TransMoveData = new byte[8];
        }
        
        /// <summary>
        /// CanBus連線
        /// </summary>
        /// <returns> 連線成功回傳1，否則回傳0</returns>
        public static int CanConnect()
        {
            //2016.09.09彥昌新增
            int nRet1 = 0;
            int nRet2 = 0;
            Byte byBtr0, byBtr1;

            nRet1 = Device_CommandToMotor.acCanOpen("can1", false, 500, 500);             //Open CAN port
            nRet2 = Device_MotorToCommand.acCanOpen("can2", false, 500, 500);             //Open CAN port
            if (nRet1 < 0 || nRet2 < 0)
            {
                m_bOpen = 0;
            }
            else
            {
                m_bOpen = 1;
            }

            nRet1 = Device_CommandToMotor.acEnterResetMode();                                 //Enter reset mode     
            nRet2 = Device_MotorToCommand.acEnterResetMode();                                 //Enter reset mode     
            if (nRet1 < 0 || nRet2 < 0)
            {
                Device_CommandToMotor.acCanClose();
                Device_MotorToCommand.acCanClose();
                m_bOpen = 0;
            }
            else
            {
                m_bOpen = 1;
            }

            byBtr0 = Convert.ToByte("00", 16);
            byBtr1 = Convert.ToByte("1c", 16);

            nRet1 = Device_CommandToMotor.acSetBaudRegister(byBtr0, byBtr1);                //Set Baud Rate
            nRet2 = Device_MotorToCommand.acSetBaudRegister(byBtr0, byBtr1);                //Set Baud Rate
            if (nRet1 < 0 || nRet2 < 0)
            {
                Device_CommandToMotor.acCanClose();
                Device_MotorToCommand.acCanClose();
                m_bOpen = 0;
            }
            else
            {
                m_bOpen = 1;
            }

            nRet1 = Device_CommandToMotor.acSetTimeOut(3000, 3000);     //Set timeout
            nRet2 = Device_MotorToCommand.acSetTimeOut(3000, 3000);     //Set timeout
            if (nRet1 < 0 || nRet2 < 0)
            {
                Device_CommandToMotor.acCanClose();
                Device_MotorToCommand.acCanClose();
                m_bOpen = 0;
            }
            else
            {
                m_bOpen = 1;
            }

            nRet1 = Device_CommandToMotor.acEnterWorkMode();                                     //Enter work mdoe
            nRet2 = Device_MotorToCommand.acEnterWorkMode();                                     //Enter work mdoe
            if (nRet1 < 0 || nRet2 < 0)
            {
                Device_CommandToMotor.acCanClose();
                Device_MotorToCommand.acCanClose();
                m_bOpen = 0;
            }
            else
            {
                m_bOpen = 1;
            }

            ComputerThreadST_Adv = new ThreadStart(StartComputerMonitorThread_Adv);            //Create a new thread
            ComputerThread_Adv = new Thread(ComputerThreadST_Adv);
            ComputerThread_Adv.Priority = ThreadPriority.Normal;
            

            //ComputerThread = new Thread(StartComputerMonitorThread);
            //MotorThread = new Thread(StartMotorMonitorThread);

            MotorThreadST_Adv = new ThreadStart(StartMotorMonitorThread_Adv);            //Create a new thread
            MotorThread_Adv = new Thread(MotorThreadST_Adv);
            MotorThread_Adv.Priority = ThreadPriority.Normal;
            
            //2016.09.09彥昌新增

            
            if (m_bOpen == 1) return 1;//CAN連線
            else return 0;//CAN斷線
        }

        /// <summary>
        /// 開啟CanBus通訊
        /// </summary>
        /// <returns>連線成功回傳1，否則回傳0</returns>
        public static int OpenCan()
        {
            //開啟CanBus
            

            ComputerThread_Adv.Start();//New thread starts
            MotorThread_Adv.Start();  //New thread starts
            return 1;
            //else return 0;
        }

        /// <summary>
        /// 關閉CanBus通訊
        /// </summary>
        public static void ResetCan()
        {
            if (m_bOpen == 0) return;

            Device_MotorToCommand.acCanClose();
            Device_CommandToMotor.acCanClose();

            //CanBusTool.VCI_ResetCAN(m_devtype, m_devind, m_canind);
            //CanBusTool.VCI_ResetCAN(m_devtype, m_devind_2, m_canind_2);
        }

        /// <summary>
        /// 攔截堆高機電腦傳送過來資料，修改後傳送給馬達 
        /// </summary>
        /*unsafe private static void  StartComputerMonitorThread()
        {
            while (true)
            {
                UInt32 res = new UInt32();
                res = CanBusTool.VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0], 1000, 100);
                if (res > 10000 || res < 0) return;
                for (UInt32 i = 0; i < res; i++)
                {
                    if (!CanControl) //初始化
                    {
                        //收到電腦傳送過來資料，直接傳送給馬達 
                        VCI_CAN_OBJ recobj = m_recobj[i];
                        string StrID = Convert.ToString(recobj.ID, 16);
                        //Console.WriteLine("ReceiveComputerID: " + StrID);

                        byte Datalen = (byte)(m_recobj[i].DataLen % 9);
                        byte[] TransData = new byte[Datalen];
                        for (int k = 0; k < TransData.Length; k++) TransData[k] = recobj.Data[k];
                        SendCommandToMotor(recobj.ID, Datalen, TransData);
                        //ID 207 轉向馬達
                        if (recobj.ID == 519)
                        {
                            Array.Copy(TransData, ReceiveComputer_Storage, TransData.Length);
                            Array.Copy(TransData, ReceiveComputer, TransData.Length);
                        }
                        //ID 20B 行走馬達
                        if (recobj.ID == 523)
                        {
                            Array.Copy(TransData, ReceiveMoveComputer_Storage, TransData.Length);
                            Array.Copy(TransData, ReceiveMoveComputer, TransData.Length);
                        }
                    }
                    else //開始介入
                    {
                        VCI_CAN_OBJ recobj = m_recobj[i];
                        string StrID = Convert.ToString(recobj.ID, 16);
                        byte Datalen = (byte)(recobj.DataLen % 9);
                        //收到ID 207
                        if (recobj.ID == 519) //轉向馬達站號
                        {
                            //將上次傳送結果複製給比較buffer
                            Array.Copy(ReceiveComputer, BufferData, BufferData.Length);
                            //更改ID207資料
                            if (isSend)
                            {
                                Array.Copy(TransData, ReceiveComputer, TransData.Length);//傳送資料
                                isSend = false;
                            }
                            //計算ID207的Check的資料
                            CalCheckData();
                            //傳送指令給馬達
                            SendCommandToMotor(recobj.ID, ReceiveComputer.Length, ReceiveComputer);
                            //回傳給電腦虛擬馬達ID187資料
                            SendCommand(391, ReceiveMotor_Storage.Length, ReceiveMotor_Storage);
                        }
                        if (recobj.ID == 523) //行走馬達站號
                        {
                            //修改ID523資料
                            if (isMoveSend)
                            {
                                Array.Copy(TransMoveData, ReceiveMoveComputer, TransMoveData.Length);//傳送資料
                                isMoveSend = false;
                            }
                            //傳送指令給馬達
                            SendCommandToMotor(recobj.ID, ReceiveMoveComputer.Length, ReceiveMoveComputer);
                            //回傳給電腦虛擬馬達ID18B資料
                            SendCommand(395, ReceiveMoveMotor_Storage.Length, ReceiveMoveMotor_Storage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 收到堆高機馬達傳送過來資料，修改後傳送給電腦 
        /// </summary>
        unsafe public static void StartMotorMonitorThread()
        {
            while (true)
            {
                UInt32 res = new UInt32();
                res = CanBusTool.VCI_Receive(m_devtype, m_devind_2, m_canind_2, ref m_recobj_2[0], 1000, 100);
                if (res > 10000 || res < 0) return;
                if (!CanControl) //初始化
                {
                    for (UInt32 i = 0; i < res; i++)
                    {
                        //收到馬達傳送過來資料，傳送給電腦 
                        VCI_CAN_OBJ recobj = m_recobj_2[i];
                        byte Datalen = (byte)(m_recobj_2[i].DataLen % 9);
                        byte[] TransData = new byte[Datalen];
                        for (int k = 0; k < TransData.Length; k++) TransData[k] = recobj.Data[k];
                        SendCommand(recobj.ID, Datalen, TransData);
                        if (recobj.ID == 391) //ID187 轉向站號
                        {
                            Array.Copy(TransData, ReceiveMotor, TransData.Length);
                            Array.Copy(TransData, ReceiveMotor_Storage, TransData.Length);
                        }
                        if (recobj.ID == 395) //ID18B 行走站號
                        {
                            Array.Copy(TransData, ReceiveMoveMotor, TransData.Length);
                            Array.Copy(TransData, ReceiveMoveMotor_Storage, TransData.Length);
                        }
                    }
                }
                else
                {
                    if (GlobalVar.isCanBusDebug) //記錄回傳角度資訊
                    {
                        for (UInt32 i = 0; i < res; i++)
                        {
                            VCI_CAN_OBJ recobj = m_recobj_2[i];

                            //分析馬達回傳資料
                            AnalysisMotorData(recobj, i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析馬達回傳資料
        /// </summary>
        /// <param name="recobj">[IN] recobj 參數</param>
        /// <param name="i">[IN] Index</param>
        unsafe public static void AnalysisMotorData(VCI_CAN_OBJ recobj, uint i)
        {
            if (recobj.ID == 391) //ID187 轉向站號
            {
                byte Datalen = (byte)(m_recobj_2[i].DataLen % 9);
                int LowByte = Convert.ToInt16(recobj.Data[4]);
                int HiByte = Convert.ToInt16(recobj.Data[5]);
                LowByte -= MainForm.L_Error;
                HiByte -= MainForm.H_Error;
                double LowDegree = (LowByte * 1.40625) / 255;
                double HiDegree = HiByte * 1.40625;
                double AngleTemp = LowDegree + HiDegree;
                if (AngleTemp > 180) AngleTemp = AngleTemp - 360;
                GlobalVar.RealMotorAngle = AngleTemp;
            }

            if (recobj.ID == 395) //ID187 行走站號
            {
                byte Datalen = (byte)(m_recobj_2[i].DataLen % 9);
                int LowByte = Convert.ToInt16(recobj.Data[2]);
                int HiByte = Convert.ToInt16(recobj.Data[3]);
                if (HiByte > 128)
                {
                    HiByte = 255 - HiByte;
                    LowByte = 255 - LowByte;
                    double Temp = -((double)HiByte * (double)256 + (double)LowByte) / 6.97265625;
                    if (Temp < -255) Temp = -255;
                    GlobalVar.RealMotorPower = Temp;
                }
                else
                {
                    double Temp = ((double)HiByte * (double)256 + (double)LowByte) / 6.97265625;
                    if (Temp > 255) Temp = 255;
                    GlobalVar.RealMotorPower = Temp;
                }
            }
        }
        */
         
        /// <summary>
        /// 傳送資料給堆高機的電腦
        /// </summary>
        /// <param name="ID">IN] 傳送ID</param>[
        /// <param name="len">IN] 數據長度</param>[
        /// <param name="data">[IN] 傳送數據</param>
        unsafe public static void SendMotorToCommand(uint ID, int len, byte[] data, int k)
        {
            if (m_bOpen == 0) return;

            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];
            msgWrite[k].flags = 0;
            msgWrite[k].id = ID;
            msgWrite[k].length = System.Convert.ToByte(len);
            msgWrite[k].data = new byte[AdvCan.DATALENGTH];

            for (int i = 0; i < len; i++) msgWrite[k].data[i] = data[i];

            int nRet = Device_MotorToCommand.acCanWrite(msgWrite, nMsgCount, ref pulNumberofWritten_Device_CommandToMotor); //Send frames

            /*VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.RemoteFlag = 0;
            sendobj.ExternFlag = 0;
            sendobj.ID = ID;
            sendobj.DataLen = System.Convert.ToByte(len);
            for (int i = 0; i < len; i++) sendobj.Data[i] = data[i];
            CanBusTool.VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1);*/
        }

        /// <summary>
        /// 傳送資料給堆高機的馬達
        /// </summary>
        /// <param name="ID">[IN] 傳送ID</param>
        /// <param name="len">[IN] 數據長度</param>
        /// <param name="data">[IN] 傳送數據</param>
        unsafe public static void SendCommandToMotor(uint ID, int len, byte[] data,int k)
        {
            if (m_bOpen == 0) return;
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];
            msgWrite[k].flags = 0;
            msgWrite[k].id = ID;
            msgWrite[k].length = System.Convert.ToByte(len);
            msgWrite[k].data = new byte[AdvCan.DATALENGTH];

            for (int i = 0; i < len; i++) msgWrite[k].data[i] = data[i];

            int nRet = Device_CommandToMotor.acCanWrite(msgWrite, nMsgCount, ref pulNumberofWritten_Device_MotorToCommand); //Send frames


            /*VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.RemoteFlag = 0;
            sendobj.ExternFlag = 0;
            sendobj.ID = ID;
            sendobj.DataLen = System.Convert.ToByte(len);*/
            //for (int i = 0; i < len; i++) msgWrite[k].Data[i] = data[i];
            //CanBusTool.VCI_Transmit(m_devtype, m_devind_2, m_canind_2, ref sendobj, 1);
        }

        /// <summary>
        /// 利用BufferData(上一筆)與ReceiveComputer(當前要傳送的資料)來計算ID 207之Check資料
        /// </summary>
        public static void CalCheckData()
        {
            int dis = 0;
            for (int i = 0; i < 6; i++)
            {
                if (ReceiveComputer[i] >= BufferData[i])
                    dis += ReceiveComputer[i] - BufferData[i];
                else
                    dis += (256 - BufferData[i] + ReceiveComputer[i]);
            }
            int value = BufferData[7] + 6 + dis;
            value = value % 256;
            int fixdata = BufferData[6] + 6;
            fixdata = fixdata % 256;
            ReceiveComputer[6] = Convert.ToByte(fixdata);
            ReceiveComputer[7] = Convert.ToByte(value);
        }




        //2016.09.09彥昌新增

        public static bool rtr = true;
        public static bool count = false;
        public static uint pulNumberofWritten_Device_CommandToMotor = 0;
        public static uint pulNumberofWritten_Device_MotorToCommand = 0;



        unsafe public static void StartComputerMonitorThread_Adv()
        {

            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];                 //Package for write   
            //AdvCan.canmsg_t[] tmp_msgWrite = new AdvCan.canmsg_t[nMsgCount];
            AdvCan.canmsg_t[] msgRead = new AdvCan.canmsg_t[nMsgCount];
            uint nWriteCount = 0;
            //uint pulNumberofWritten = 0;
            //uint SendIndex = 0;

            //string strTemp;
            byte[] data = new byte[8];
            //int i;

            //Initialize msg
            for (int j = 0; j < nMsgCount; j++)
            {
                msgWrite[j].flags = AdvCan.MSG_EXT;
                msgWrite[j].cob = 0;
                msgWrite[j].id = 0;
                msgWrite[j].length = (short)AdvCan.DATALENGTH;
                msgWrite[j].data = new byte[AdvCan.DATALENGTH];
                if (rtr)
                {
                    msgWrite[j].flags += AdvCan.MSG_RTR;
                    msgWrite[j].length = 0;
                }
            }

            //string MotorMonitor_Adv = "";
            uint nReadCount = nMsgCount;
            uint pulNumberofRead = 0;
            //uint ReceiveIndex = 0;

            //myDelegate SetList = new myDelegate(ListDelegate);
            //myDelegate SetButton = new myDelegate(ButtonDelegate);




            //ReceiveIndex = 0;
            m_bRun = true;

            while (m_bRun)
            {
                int nRet;
                //string StrID = "";
                //MotorMonitor_Adv = "";
                nRet = Device_CommandToMotor.acCanRead(msgRead, nReadCount, ref pulNumberofRead); //Receiving frames
                if (nRet == AdvCANIO.TIME_OUT)
                {
                    //labelCANStatus.
                    //ShowStatus.Invoke(SetList, ReceiveStatus);//Package receiving timeout
                }
                else if (nRet == AdvCANIO.OPERATION_ERROR)
                {
                    //ShowStatus.Invoke(SetList, ReceiveStatus);
                }
                else
                {
                    for (int j = 0; j < pulNumberofRead; j++)
                    {
                        if (msgRead[j].id == AdvCan.ERRORID)
                        {
                            //ReceiveStatus += "a incorrect package";
                            //ShowStatus.Invoke(SetList, ReceiveStatus);
                        }
                        else
                        {
                            if ((msgRead[j].flags & AdvCan.MSG_RTR) > 0)
                            {
                                //ReceiveStatus += "a RTR package";
                            }
                            else
                            {
                                //for (int k = 0; k < msgRead[j].length; k++)
                                {
                                    if (!CanControl) //初始化
                                    {
                                        string StrID = Convert.ToString(msgRead[j].id, 16);
                                        data = msgRead[j].data;
                                        for (int i = 0; i < data.Length; i++)
                                        {
                                            msgWrite[j].data[i] = Convert.ToByte(data[i]);
                                        }
                                        msgWrite[j].length = (short)data.Length;
                                        nWriteCount = nMsgCount;
                                        //Array.Copy(msgWrite, 0, tmp_msgWrite, 0, nMsgCount);

                                        //nRet = Device_MotorToCommand.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames

                                        SendCommandToMotor(msgWrite[j].id, msgWrite[j].length, msgWrite[j].data, j);

                                        if (msgRead[j].id == 519)
                                        {
                                            Array.Copy(msgWrite[j].data, ReceiveComputer_Storage, data.Length);
                                            Array.Copy(msgWrite[j].data, ReceiveComputer, data.Length);
                                        }
                                        //ID 20B 行走馬達
                                        if (msgRead[j].id == 523)
                                        {
                                            Array.Copy(msgWrite[j].data, ReceiveMoveComputer_Storage, data.Length);
                                            Array.Copy(msgWrite[j].data, ReceiveMoveComputer, data.Length);
                                        }
                                    }
                                    else //開始介入
                                    {
                                        //VCI_CAN_OBJ recobj = msgRead[j];
                                        //string StrID = Convert.ToString(msgRead[j].id, 16);
                                        /*for (int i = 0; i < data.Length; i++)
                                        {
                                            msgWrite[k].data[i] = Convert.ToByte(data[i] - 48);
                                        }
                                        msgWrite[k].length = (short)data.Length;
                                        nWriteCount = nMsgCount;
                                        Array.Copy(msgWrite, 0, tmp_msgWrite, 0, nMsgCount);*/

                                        //收到ID 207
                                        if (msgRead[j].id == 519) //轉向馬達站號
                                        {
                                            //將上次傳送結果複製給比較buffer
                                            Array.Copy(ReceiveComputer, BufferData, BufferData.Length);
                                            //更改ID207資料
                                            if (isSend)
                                            {
                                                Array.Copy(TransData, ReceiveComputer, TransData.Length);//傳送資料
                                                isSend = false;
                                            }
                                            //計算ID207的Check的資料
                                            CalCheckData();
                                            //傳送指令給馬達

                                            SendCommandToMotor(msgRead[j].id, ReceiveComputer.Length, ReceiveComputer, j);

                                            //nRet = Device_MotorToCommand.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames

                                            //回傳給電腦虛擬馬達ID187資料
                                            SendMotorToCommand(391, ReceiveMotor_Storage.Length, ReceiveMotor_Storage, j);

                                            //nRet = Device_CommandToMotor.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames
                                        }
                                        if (msgRead[j].id == 523) //行走馬達站號
                                        {
                                            //修改ID523資料
                                            if (isMoveSend)
                                            {
                                                Array.Copy(TransMoveData, ReceiveMoveComputer, TransMoveData.Length);//傳送資料
                                                isMoveSend = false;
                                            }
                                            //傳送指令給馬達
                                            SendCommandToMotor(msgRead[j].id, ReceiveMoveComputer.Length, ReceiveMoveComputer, j);
                                            //回傳給電腦虛擬馬達ID18B資料
                                            SendMotorToCommand(395, ReceiveMoveMotor_Storage.Length, ReceiveMoveMotor_Storage, j);
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }





            ///舊的
            /*while (m_bRun)
            {
                UInt32 res = new UInt32();
                res = CanBusTool.VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0], 1000, 100);
                if (res > 10000 || res < 0) return;
                for (UInt32 i = 0; i < res; i++)
                {
                    if (!CanControl) //初始化
                    {
                        //收到電腦傳送過來資料，直接傳送給馬達 
                        VCI_CAN_OBJ recobj = m_recobj[i];
                        string StrID = Convert.ToString(recobj.ID, 16);
                        //Console.WriteLine("ReceiveComputerID: " + StrID);

                        byte Datalen = (byte)(m_recobj[i].DataLen % 9);
                        byte[] TransData = new byte[Datalen];
                        for (int k = 0; k < TransData.Length; k++) TransData[k] = recobj.Data[k];
                        SendCommandToMotor(recobj.ID, Datalen, TransData);
                        //ID 207 轉向馬達
                        if (recobj.ID == 519)
                        {
                            Array.Copy(TransData, ReceiveComputer_Storage, TransData.Length);
                            Array.Copy(TransData, ReceiveComputer, TransData.Length);
                        }
                        //ID 20B 行走馬達
                        if (recobj.ID == 523)
                        {
                            Array.Copy(TransData, ReceiveMoveComputer_Storage, TransData.Length);
                            Array.Copy(TransData, ReceiveMoveComputer, TransData.Length);
                        }
                    }
                    else //開始介入
                    {
                        VCI_CAN_OBJ recobj = m_recobj[i];
                        string StrID = Convert.ToString(recobj.ID, 16);
                        byte Datalen = (byte)(recobj.DataLen % 9);
                        //收到ID 207
                        if (recobj.ID == 519) //轉向馬達站號
                        {
                            //將上次傳送結果複製給比較buffer
                            Array.Copy(ReceiveComputer, BufferData, BufferData.Length);
                            //更改ID207資料
                            if (isSend)
                            {
                                Array.Copy(TransData, ReceiveComputer, TransData.Length);//傳送資料
                                isSend = false;
                            }
                            //計算ID207的Check的資料
                            CalCheckData();
                            //傳送指令給馬達
                            SendCommandToMotor(recobj.ID, ReceiveComputer.Length, ReceiveComputer);
                            //回傳給電腦虛擬馬達ID187資料
                            SendCommand(391, ReceiveMotor_Storage.Length, ReceiveMotor_Storage);
                        }
                        if (recobj.ID == 523) //行走馬達站號
                        {
                            //修改ID523資料
                            if (isMoveSend)
                            {
                                Array.Copy(TransMoveData, ReceiveMoveComputer, TransMoveData.Length);//傳送資料
                                isMoveSend = false;
                            }
                            //傳送指令給馬達
                            SendCommandToMotor(recobj.ID, ReceiveMoveComputer.Length, ReceiveMoveComputer);
                            //回傳給電腦虛擬馬達ID18B資料
                            SendCommand(395, ReceiveMoveMotor_Storage.Length, ReceiveMoveMotor_Storage);
                        }
                    }
                }
            }*/
            ///舊的
















            ////可用AGV
            /*int nRet;
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];                 //Package for write   
            AdvCan.canmsg_t[] tmp_msgWrite = new AdvCan.canmsg_t[nMsgCount];
            uint nWriteCount = 0;
            uint pulNumberofWritten = 0;
            uint SendIndex = 0;

            string strTemp;
            char[] data = new char[8];
            int i;

            //Initialize msg
            for (int j = 0; j < nMsgCount; j++)
            {
                msgWrite[j].flags = AdvCan.MSG_EXT;
                msgWrite[j].cob = 0;
                msgWrite[j].id = 0;
                msgWrite[j].length = (short)AdvCan.DATALENGTH;
                msgWrite[j].data = new byte();
                if (rtr)
                {
                    msgWrite[j].flags += AdvCan.MSG_RTR;
                    msgWrite[j].length = 0;
                }
            }

            m_bRun = true;
            while (m_bRun)
            {
                if (count == true)
                {
                    if (nWriteCount > 0)
                    {
                        Array.Copy(msgWrite, nWriteCount, tmp_msgWrite, 0, nMsgCount - nWriteCount);
                    }
                    else
                    {
                        for (int j = 0; j < nMsgCount; j++)
                        {
                            strTemp = Convert.ToString(SendIndex + 1 + j);
                            data = strTemp.ToCharArray();
                            for (i = 0; i < data.Length; i++)
                            {
                                msgWrite[j].data = Convert.ToByte(data[i] - 48);
                            }
                            msgWrite[j].length = (short)data.Length;
                            nWriteCount = nMsgCount;
                            Array.Copy(msgWrite, 0, tmp_msgWrite, 0, nMsgCount);
                        }
                    }
                    count = false;
                }


                nRet = Device_CommandToMotor.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames
                if (nRet == AdvCANIO.TIME_OUT)
                {

                }
                else if (nRet == AdvCANIO.OPERATION_ERROR)
                {

                }
                else
                {
                    nWriteCount -= pulNumberofWritten;
                    SendIndex += pulNumberofWritten;
                }

                Thread.Sleep(400);
            }*/
        }



        /// <summary>
        /// 
        /// </summary>
        unsafe public static void StartMotorMonitorThread_Adv()
        {
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];                 //Package for write   
            //AdvCan.canmsg_t[] tmp_msgWrite = new AdvCan.canmsg_t[nMsgCount];
            AdvCan.canmsg_t[] msgRead = new AdvCan.canmsg_t[nMsgCount];
            uint nWriteCount = 0;
            //uint pulNumberofWritten = 0;
            //uint SendIndex = 0;

            //string strTemp;
            char[] data = new char[8];
            //int i;

            //Initialize msg
            for (int j = 0; j < nMsgCount; j++)
            {
                msgWrite[j].flags = AdvCan.MSG_EXT;
                msgWrite[j].cob = 0;
                msgWrite[j].id = 0;
                msgWrite[j].length = (short)AdvCan.DATALENGTH;
                msgWrite[j].data = new byte[AdvCan.DATALENGTH];
                if (rtr)
                {
                    msgWrite[j].flags += AdvCan.MSG_RTR;
                    msgWrite[j].length = 0;
                }
            }

            //string MotorMonitor_Adv = "";
            uint nReadCount = nMsgCount;
            uint pulNumberofRead = 0;
            //uint ReceiveIndex = 0;

            //myDelegate SetList = new myDelegate(ListDelegate);
            //myDelegate SetButton = new myDelegate(ButtonDelegate);




            //ReceiveIndex = 0;
            m_bRun = true;

            while (m_bRun)
            {
                int nRet;
                //string StrID = "";
                //MotorMonitor_Adv = "";
                nRet = Device_MotorToCommand.acCanRead(msgRead, nReadCount, ref pulNumberofRead); //Receiving frames
                if (nRet == AdvCANIO.TIME_OUT)
                {
                    //labelCANStatus.
                    //ShowStatus.Invoke(SetList, ReceiveStatus);//Package receiving timeout
                }
                else if (nRet == AdvCANIO.OPERATION_ERROR)
                {
                    //ShowStatus.Invoke(SetList, ReceiveStatus);
                }
                else
                {
                    for (int j = 0; j < pulNumberofRead; j++)
                    {
                        if (msgRead[j].id == AdvCan.ERRORID)
                        {
                            //ReceiveStatus += "a incorrect package";
                            //ShowStatus.Invoke(SetList, ReceiveStatus);
                        }
                        else
                        {
                            if ((msgRead[j].flags & AdvCan.MSG_RTR) > 0)
                            {
                                //ReceiveStatus += "a RTR package";
                            }
                            else
                            {
                                for (int k = 0; k < msgRead[j].length; k++)
                                {
                                    if (!CanControl) //初始化
                                    {
                                        string StrID = Convert.ToString(msgRead[j].id, 16);
                                        data = StrID.ToCharArray();
                                        for (int i = 0; i < data.Length; i++)
                                        {
                                            msgWrite[k].data[i] = Convert.ToByte(data[i] - 48);
                                        }
                                        msgWrite[k].length = (short)data.Length;
                                        nWriteCount = nMsgCount;

                                        SendMotorToCommand(msgWrite[k].id, msgWrite[k].length, msgWrite[k].data, k);

                                        if (msgRead[j].id == 319)
                                        {
                                            Array.Copy(msgWrite[k].data, ReceiveComputer_Storage, data.Length);
                                            Array.Copy(msgWrite[k].data, ReceiveComputer, data.Length);
                                        }
                                        //ID 20B 行走馬達
                                        if (msgRead[j].id == 395)
                                        {
                                            Array.Copy(msgWrite[k].data, ReceiveMoveComputer_Storage, data.Length);
                                            Array.Copy(msgWrite[k].data, ReceiveMoveComputer, data.Length);
                                        }
                                    }
                                    else //開始介入
                                    {
                                        if (GlobalVar.isCanBusDebug) //記錄回傳角度資訊
                                        {
                                            //for (UInt32 i = 0; i < res; i++)
                                            {
                                                ;

                                                //分析馬達回傳資料
                                                AnalysisMotorData(msgRead[j], j);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        unsafe public static void AnalysisMotorData(AdvCan.canmsg_t msgWrite, int i)
        {
            if (msgWrite.id == 391) //ID187 轉向站號
            {
                byte Datalen = (byte)(m_recobj_2[i].DataLen % 9);
                int LowByte = Convert.ToInt16(msgWrite.data[4]);
                int HiByte = Convert.ToInt16(msgWrite.data[5]);
                LowByte -= MainForm.L_Error;
                HiByte -= MainForm.H_Error;
                double LowDegree = (LowByte * 1.40625) / 255;
                double HiDegree = HiByte * 1.40625;
                double AngleTemp = LowDegree + HiDegree;
                if (AngleTemp > 180) AngleTemp = AngleTemp - 360;
                GlobalVar.RealMotorAngle = AngleTemp;
            }
            if (msgWrite.id == 395) //ID187 行走站號
            {
                byte Datalen = (byte)(m_recobj_2[i].DataLen % 9);
                int LowByte = Convert.ToInt16(msgWrite.data[2]);
                int HiByte = Convert.ToInt16(msgWrite.data[3]);
                if (HiByte > 128)
                {
                    HiByte = 255 - HiByte;
                    LowByte = 255 - LowByte;
                    double Temp = -((double)HiByte * (double)256 + (double)LowByte) / 6.97265625;
                    if (Temp < -255) Temp = -255;
                    GlobalVar.RealMotorPower = Temp;
                }
                else
                {
                    double Temp = ((double)HiByte * (double)256 + (double)LowByte) / 6.97265625;
                    if (Temp > 255) Temp = 255;
                    GlobalVar.RealMotorPower = Temp;
                }
            }
        }
        //2016.09.09彥昌新增

    }
}
