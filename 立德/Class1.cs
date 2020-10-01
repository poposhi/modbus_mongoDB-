using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;//debug msg在『輸出』視窗觀看

namespace modbus_mongoDB建立
{
    class RESET_Error //其實不知道這兩個Error是做什麼的 
    {
        public static double error_count = 0;
        public static double count = 0;
        public static int EMS_Error = 0;
        public static int EMS_Error1 = 0;
        public static bool EMS_Flag = false;
    }
    class Error
    {
        public static double error_count = 0;
        public static double count = 0;
        public static int EMS_Error = 0;
        public static int EMS_Error1 = 0;
        public static bool EMS_Flag = false;
    }
    public class grid
    {
        public static int v_base = 380;
        public static int S_rated = 1;
    }
    public class PCS
    {
        private string device_ID = "";
        private double error_count = 0;
        private bool read_or_not;
        private string communication_error = "";
        private double count = 0;
        private bool ems_flag = false;                          //判斷是否缺值
        private int ems_error = 0;
        private ushort[] holding_register = new ushort[53];
        private ushort error1 = 0;
        private ushort error2 = 0;
        private ushort error3 = 0;
        private ushort error4 = 0;
        private double v_grid1 = 0;
        private double v_grid2 = 0;
        private double v_grid3 = 0;
        private double v_out1 = 0;
        private double v_out2 = 0;
        private double v_out3 = 0;
        private double i_out1 = 0;
        private double i_out2 = 0;
        private double i_out3 = 0;
        private double f_offgrid = 0;
        private double f_grid = 0;
        private double i_n = 0;
        private double temp_inner = 0;
        private double temp_sink = 0;
        private double v_dc = 0;
        private double i_dc = 0;
        private double p_dc = 0;
        private double s_sum = 0;
        private double p_sum = 0;
        private double q_sum = 0;
        private double s_out1 = 0;
        private double p_out1 = 0;
        private double pf_out1 = 0;
        private double s_out2 = 0;
        private double p_out2 = 0;
        private double pf_out2 = 0;
        private double s_out3 = 0;
        private double p_out3 = 0;
        private double pf_out3 = 0;
        private double kwh_chg = 0;
        private double kwh_dischg = 0;
        private double status_operation = 0;
        private double status_grid = 0;
        private ushort[] errorbit2ushort = new ushort[3];
        private bool[,] error_bit = new bool[3, 16];
        public double Continuous_Communication_ErrorSeconds { get; set; }
        public string communication_w_error { get; set; }
        public double error_w_count { get; set; }
        public double S_rated { get; set; }
        public bool control_pcs { get; set; }
        public double Error_count { get { return error_count; } set { error_count = value; } }
        public string Communication_error { get { return communication_error; } set { communication_error = value; } }
        public double Count { get { return count; } set { count = value; } }
        public bool Ems_flag { get { return ems_flag; } set { ems_flag = value; } }                      //判斷是否缺值
        public int Ems_error { get { return ems_error; } set { ems_error = value; } }
        public ushort[] Holding_register { get { return holding_register; } set { holding_register = value; } }
        public ushort Error1 { get { return error1; } set { error1 = value; } }
        public ushort Error2 { get { return error2; } set { error2 = value; } }
        public ushort Error3 { get { return error3; } set { error3 = value; } }
        public ushort Error4 { get { return error4; } set { error4 = value; } }
        public double V_grid1 { get { return v_grid1; } set { v_grid1 = value; } }
        public double V_grid2 { get { return v_grid2; } set { v_grid2 = value; } }
        public double V_grid3 { get { return v_grid3; } set { v_grid3 = value; } }
        public double V_out1 { get { return v_out1; } set { v_out1 = value; } }
        public double V_out2 { get { return v_out2; } set { v_out2 = value; } }
        public double V_out3 { get { return v_out3; } set { v_out3 = value; } }
        public double I_out1 { get { return i_out1; } set { i_out1 = value; } }
        public double I_out2 { get { return i_out2; } set { i_out2 = value; } }
        public double I_out3 { get { return i_out3; } set { i_out3 = value; } }
        public double F_offgrid { get { return f_offgrid; } set { f_offgrid = value; } }
        public double F_grid { get { return f_grid; } set { f_grid = value; } }
        public double I_n { get { return i_n; } set { i_n = value; } }
        public double Temp_inner { get { return temp_inner; } set { temp_inner = value; } }
        public double Temp_sink { get { return temp_sink; } set { temp_sink = value; } }
        public double V_dc { get { return v_dc; } set { v_dc = value; } }
        public double I_dc { get { return i_dc; } set { i_dc = value; } }
        public double P_dc { get { return p_dc; } set { p_dc = value; } }
        public double S_sum { get { return s_sum; } set { s_sum = value; } }
        public double P_sum { get { return p_sum; } set { p_sum = value; } }
        public double Q_sum { get { return q_sum; } set { q_sum = value; } }
        public double S_out1 { get { return s_out1; } set { s_out1 = value; } }
        public double P_out1 { get { return p_out1; } set { p_out1 = value; } }
        public double Pf_out1 { get { return pf_out1; } set { pf_out1 = value; } }
        public double S_out2 { get { return s_out2; } set { s_out2 = value; } }
        public double P_out2 { get { return p_out2; } set { p_out2 = value; } }
        public double Pf_out2 { get { return pf_out2; } set { pf_out2 = value; } }
        public double S_out3 { get { return s_out3; } set { s_out3 = value; } }
        public double P_out3 { get { return p_out3; } set { p_out3 = value; } }
        public double Pf_out3 { get { return pf_out3; } set { pf_out3 = value; } }
        public double Kwh_chg { get { return kwh_chg; } set { kwh_chg = value; } }
        public double Kwh_dischg { get { return kwh_dischg; } set { kwh_dischg = value; } }
        public double Status_operation { get { return status_operation; } set { status_operation = value; } }
        public double Status_grid { get { return status_grid; } set { status_grid = value; } }
        public ushort[] Errorbit2ushort { get => errorbit2ushort; set => errorbit2ushort = value; }
        public bool[,] Error_bit { get => error_bit; set => error_bit = value; }
        public string Device_ID { get => device_ID; set => device_ID = value; }
        public bool Read_or_not { get => read_or_not; set => read_or_not = value; }

        private static double Negative_two_num(ushort num1, ushort num2)
        {
            double result = 0;
            if (num1 >= 32768)
            {
                result = (double)(num1 - 65535) * 65536 + (num2 - 65536);
            }
            else
            {
                result = num1 * 65536 + num2;
            }
            return result;
        }
        private static double Negative_num(ushort num)
        {
            double result = 0;
            if (num >= 32768)
            {
                result = num - 65535;
            }
            else
            {
                result = num;
            }
            return result;
        }
        public void Put_Data1()
        {
            status_operation = holding_register[0];
            Error1 = holding_register[1];
            Error2 = holding_register[2];
            Error3 = holding_register[3];
            Error4 = holding_register[4];
            v_grid1 = Negative_num(holding_register[5]) * 0.1;
            v_grid2 = Negative_num(holding_register[6]) * 0.1;
            v_grid3 = Negative_num(holding_register[7]) * 0.1;
            v_out1 = Negative_num(holding_register[8]) * 0.1;
            v_out2 = Negative_num(holding_register[9]) * 0.1;
            v_out3 = Negative_num(holding_register[10]) * 0.1;
            i_out1 = Negative_num(holding_register[11]) * 0.1;
            i_out2 = Negative_num(holding_register[12]) * 0.1;
            i_out3 = Negative_num(holding_register[13]) * 0.1;
            f_offgrid = holding_register[14] * 0.01;
            f_grid = holding_register[15] * 0.01;
            i_n = Negative_num(holding_register[16]) * 0.1;
            temp_inner = Negative_num(holding_register[17]) * 0.1;
            temp_sink = Negative_num(holding_register[18]) * 0.1;
            v_dc = holding_register[19] * 0.1;
            i_dc = Negative_num(holding_register[20]) * 0.1;
            p_dc = Negative_num(holding_register[21]) * 0.1;
            s_sum = holding_register[22] * 0.1;
            p_sum = Negative_num(holding_register[23]) * 0.1;
            q_sum = Negative_num(holding_register[24]) * 0.1;
            s_out1 = holding_register[25] * 0.1;
            p_out1 = Negative_num(holding_register[26]) * 0.1;
            pf_out1 = Negative_num(holding_register[27]) * 0.01;
            s_out2 = holding_register[29] * 0.1;
            p_out2 = Negative_num(holding_register[30]) * 0.1;
            pf_out2 = Negative_num(holding_register[31]) * 0.01;
            s_out3 = holding_register[33] * 0.1;
            p_out3 = Negative_num(holding_register[34]) * 0.1;
            pf_out3 = Negative_num(holding_register[35]) * 0.01;
            kwh_chg = (holding_register[44] * 65536 + holding_register[45]) * 0.1;
            kwh_dischg = (holding_register[46] * 65536 + holding_register[47]) * 0.1;
            status_grid = holding_register[52];
        }
    }
    public class MBMS  //////尚無資料
    {
        private string device_ID = "";
        private bool read_or_not;
        private double s_rated = 375;
        private double continuous_Communication_ErrorSeconds = 0;
        private double error_count = 0;
        private string communication_error = "";
        private double count = 0;
        private bool ems_flag = false;                          //判斷是否缺值
        private int ems_error = 0;
        private double i_rated = 100;

        //家彰新增 
        public double System_MaximumPermissibleCurrent_Charging { get; set; } = 0;   //最大允許充電電流
        public double System_MaximumPermissibleCurrent_Discharging { get; set; } = 0;   //最大允許放電電流
        public bool Positive_Contactor { get; set; } = false;          //bit0主正接觸器狀態
        public bool Precharge_Contactor { get; set; } = false;          //bit1預充接觸器狀態
        public bool Negative_Contactor { get; set; } = false;          //CATL沒有負接觸器狀態
        //到這裡結束 --------
        public double S_rated { get => s_rated; set => s_rated = value; }
        public double Continuous_Communication_ErrorSeconds { get => continuous_Communication_ErrorSeconds; set => continuous_Communication_ErrorSeconds = value; }
        public double Error_count { get { return error_count; } set { error_count = value; } }
        public string Communication_error { get { return communication_error; } set { communication_error = value; } }
        public double Count { get { return count; } set { count = value; } }
        public bool Ems_flag { get { return ems_flag; } set { ems_flag = value; } }                       //判斷是否缺值
        public int Ems_error { get { return ems_error; } set { ems_error = value; } }
        public double I_rated { get => i_rated; set => i_rated = value; }

        public string Device_ID { get => device_ID; set => device_ID = value; }
        public bool Read_or_not { get => read_or_not; set => read_or_not = value; }
        public double System_SOH { get; set; } = 0;                           //SOH
        public bool[,] Error_bit { get; set; } = new bool[6, 24];       //用在MBMS_Error_log()，判斷故障碼是否變更

        //BMS"Put_Data1()"計算共用(有量科技、CATL)
        public ushort[] holding_register { get; set; } = new ushort[125];
        public ushort[] holding_register_2 { get; set; } = new ushort[125];
        public ushort[] CAN_DataRegister { get; set; } = new ushort[4];
        public byte[] ArrayDate { get; set; } = new byte[2];           //用於將Word拆成兩個Byte(要注意高低位元)
                                                                       //holding_register[0]，Global.arraydate[0] & Global.arraydate[1]
                                                                       //     0xa1b1                0xb1                0xa1
        public System.Collections.BitArray bitStatus { get; set; } = new System.Collections.BitArray(8, false);      //建立一個大小為8的點陣列

        //BMS只能取其一(有量科技 or CATL)，沒用到必須"//"
        #region 有量科技
        public double System_V { get; set; } = 655;                      //總電壓
        public double System_A { get; set; } = 0;                      //總電流
        public double System_P { get; set; } = 0;                      //總功率
        public double SOC { get; set; } = 0;                           //SOC
        public byte RealyStatus { get; set; } = 0;                   //MBMS_Realy狀態
        public bool RealyStatus_NegativeRelay { get; set; } = false;       //負極Realy狀態
        public bool RealyStatus_PositiveRelay { get; set; } = false;       //正極Realy狀態
        public byte WorkStatus { get; set; } = 0;       //運行狀態
        public bool Work_DischargingCharging { get; set; } = false;       //Discharging：放電，Charging：放電
        public byte TargetStatus { get; set; } = 0;       //命令狀態(PCS下達充、放狀態)
        public bool Target_DischargingCharging { get; set; } = false;       //Discharging：放電，Charging：放電
        public double Cell_maxV { get; set; } = 0;                     //Cell最高電壓
        public double Cell_minV { get; set; } = 0;                     //Cell最低電壓
        public double Cell_maxTemp { get; set; } = 0;                  //模組最高溫度
        public double Cell_minTemp { get; set; } = 0;                  //模組最低溫度

        public BMS_Rack BMS01 = new BMS_Rack();
        public BMS_Rack BMS02 = new BMS_Rack();
        public BMS_Rack BMS03 = new BMS_Rack();
        public BMS_Rack BMS04 = new BMS_Rack();
        public BMS_Rack BMS05 = new BMS_Rack();
        public BMS_Rack BMS06 = new BMS_Rack();
        public class BMS_Rack
        {
            public double V { get; set; } = 0;              //
            public double A { get; set; } = 0;              //
            public double SOC { get; set; } = 0;                           //SOC
            public byte RealyStatus { get; set; } = 0;              //BMS_Realy狀態
            public bool RealyStatus_NegativeRelay { get; set; } = false;       //負極Realy狀態
            public bool RealyStatus_PositiveRelay { get; set; } = false;       //正極Realy狀態
            public byte WorkStatus { get; set; } = 0;       //運行狀態
            public bool Work_DischargingCharging { get; set; } = false;       //Discharging：放電，Charging：放電
            public byte TargetStatus { get; set; } = 0;       //命令狀態(PCS下達充、放狀態)
            public bool Target_DischargingCharging { get; set; } = false;       //Discharging：放電，Charging：放電
            public double Cell_maxV { get; set; } = 0;                 //BMS_Cell最高電壓
            public double Cell_minV { get; set; } = 0;                 //BMS_Cell最低電壓
            public double Cell_maxTemp { get; set; } = 0;              //BMS_模組最高溫度
            public double Cell_minTemp { get; set; } = 0;              //BMS_模組最低溫度
            public byte Fault_Falg_V { get; set; } = 0;              //電壓錯誤旗標
            public byte Fault_Falg_A { get; set; } = 0;              //電流錯誤旗標
            public byte Fault_Falg_Temp { get; set; } = 0;           //溫度錯誤旗標
            public byte Fault_Falg_V_LastTime { get; set; } = 0;     //上次電壓錯誤旗標
            public byte Fault_Falg_A_LastTime { get; set; } = 0;     //上次電流錯誤旗標
            public byte Fault_Falg_Temp_LastTime { get; set; } = 0;  //上次溫度錯誤旗標

            public bool[] Error { get; set; } = new bool[24];       //將Error資訊(bit)全部集中
        }


        public void Put_Data1()
        {
            //===========================================================================
            //ID：0x1C01EDEF
            CAN_DataRegister[0] = holding_register[3];
            CAN_DataRegister[1] = holding_register[4];
            CAN_DataRegister[2] = holding_register[5];
            CAN_DataRegister[3] = holding_register[6];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            System_V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            System_V = Negative_num((ushort)System_V);
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            System_A = (ArrayDate[0] * 256 + ArrayDate[1]);
            System_A = (Negative_num((ushort)System_A)) * 0.1;
            //---------------------------------------------
            System_P = System_V * System_A * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);

            Debug.Print("ArrayDate" + ArrayDate.ToString());
            Debug.Print("ArrayDate[1]"+ ArrayDate[1].ToString());
            Debug.Print("ArrayDate[0]" + ArrayDate[0].ToString());
            SOC = ArrayDate[1];
            RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(RealyStatus));     //把一byte值存至點陣列中
            RealyStatus_NegativeRelay = bitStatus[0];
            RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            WorkStatus = ArrayDate[1];
            if (WorkStatus == 0)
            {
                Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (WorkStatus == 1)
            {
                Work_DischargingCharging = true;               //1 = Charging充電
            }
            TargetStatus = ArrayDate[0];
            if (TargetStatus == 0)
            {
                Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (TargetStatus == 1)
            {
                Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF01
            CAN_DataRegister[0] = holding_register[12];
            CAN_DataRegister[1] = holding_register[13];
            CAN_DataRegister[2] = holding_register[14];
            CAN_DataRegister[3] = holding_register[15];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS01.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS01.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS01.A = (Negative_num((ushort)BMS01.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS01.SOC = ArrayDate[1];
            BMS01.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.RealyStatus));     //把一byte值存至點陣列中
            BMS01.RealyStatus_NegativeRelay = bitStatus[0];
            BMS01.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS01.WorkStatus = ArrayDate[1];
            if (BMS01.WorkStatus == 0)
            {
                BMS01.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS01.WorkStatus == 1)
            {
                BMS01.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS01.TargetStatus = ArrayDate[0];
            if (BMS01.TargetStatus == 0)
            {
                BMS01.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS01.TargetStatus == 1)
            {
                BMS01.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF02
            CAN_DataRegister[0] = holding_register[21];
            CAN_DataRegister[1] = holding_register[22];
            CAN_DataRegister[2] = holding_register[23];
            CAN_DataRegister[3] = holding_register[24];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS02.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS02.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS02.A = (Negative_num((ushort)BMS02.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS02.SOC = ArrayDate[1];
            BMS02.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.RealyStatus));     //把一byte值存至點陣列中
            BMS02.RealyStatus_NegativeRelay = bitStatus[0];
            BMS02.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS02.WorkStatus = ArrayDate[1];
            if (BMS02.WorkStatus == 0)
            {
                BMS02.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS02.WorkStatus == 1)
            {
                BMS02.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS02.TargetStatus = ArrayDate[0];
            if (BMS02.TargetStatus == 0)
            {
                BMS02.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS02.TargetStatus == 1)
            {
                BMS02.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF03
            CAN_DataRegister[0] = holding_register[30];
            CAN_DataRegister[1] = holding_register[31];
            CAN_DataRegister[2] = holding_register[32];
            CAN_DataRegister[3] = holding_register[33];
            //---------------------------------------------Bms的電壓 
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS03.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS03.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS03.A = (Negative_num((ushort)BMS03.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS03.SOC = ArrayDate[1];
            BMS03.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.RealyStatus));     //把一byte值存至點陣列中
            BMS03.RealyStatus_NegativeRelay = bitStatus[0];
            BMS03.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS03.WorkStatus = ArrayDate[1];
            if (BMS03.WorkStatus == 0)
            {
                BMS03.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS03.WorkStatus == 1)
            {
                BMS03.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS03.TargetStatus = ArrayDate[0];
            if (BMS03.TargetStatus == 0)
            {
                BMS03.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS03.TargetStatus == 1)
            {
                BMS03.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF04
            CAN_DataRegister[0] = holding_register[39];
            CAN_DataRegister[1] = holding_register[40];
            CAN_DataRegister[2] = holding_register[41];
            CAN_DataRegister[3] = holding_register[42];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS04.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS04.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS04.A = (Negative_num((ushort)BMS04.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS04.SOC = ArrayDate[1];
            BMS04.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.RealyStatus));     //把一byte值存至點陣列中
            BMS04.RealyStatus_NegativeRelay = bitStatus[0];
            BMS04.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS04.WorkStatus = ArrayDate[1];
            if (BMS04.WorkStatus == 0)
            {
                BMS04.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS04.WorkStatus == 1)
            {
                BMS04.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS04.TargetStatus = ArrayDate[0];
            if (BMS04.TargetStatus == 0)
            {
                BMS04.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS04.TargetStatus == 1)
            {
                BMS04.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF05
            CAN_DataRegister[0] = holding_register[48];
            CAN_DataRegister[1] = holding_register[49];
            CAN_DataRegister[2] = holding_register[50];
            CAN_DataRegister[3] = holding_register[51];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS05.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS05.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS05.A = (Negative_num((ushort)BMS05.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS05.SOC = ArrayDate[1];
            BMS05.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS05.RealyStatus));     //把一byte值存至點陣列中
            BMS05.RealyStatus_NegativeRelay = bitStatus[0];
            BMS05.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS05.WorkStatus = ArrayDate[1];
            if (BMS05.WorkStatus == 0)
            {
                BMS05.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS05.WorkStatus == 1)
            {
                BMS05.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS05.TargetStatus = ArrayDate[0];
            if (BMS05.TargetStatus == 0)
            {
                BMS05.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS05.TargetStatus == 1)
            {
                BMS05.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C01EF06
            CAN_DataRegister[0] = holding_register[57];
            CAN_DataRegister[1] = holding_register[58];
            CAN_DataRegister[2] = holding_register[59];
            CAN_DataRegister[3] = holding_register[60];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS06.V = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS06.A = (ArrayDate[0] * 256 + ArrayDate[1]);
            BMS06.A = (Negative_num((ushort)BMS06.A)) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS06.SOC = ArrayDate[1];
            BMS06.RealyStatus = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS06.RealyStatus));     //把一byte值存至點陣列中
            BMS06.RealyStatus_NegativeRelay = bitStatus[0];
            BMS06.RealyStatus_PositiveRelay = bitStatus[1];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS06.WorkStatus = ArrayDate[1];
            if (BMS06.WorkStatus == 0)
            {
                BMS06.Work_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS06.WorkStatus == 1)
            {
                BMS06.Work_DischargingCharging = true;               //1 = Charging充電
            }
            BMS06.TargetStatus = ArrayDate[0];
            if (BMS06.TargetStatus == 0)
            {
                BMS06.Target_DischargingCharging = false;              //0 = Discharging放電
            }
            if (BMS06.TargetStatus == 1)
            {
                BMS06.Target_DischargingCharging = true;               //1 = Charging充電
            }
            //===========================================================================
            //ID：0x1C02EDEF
            CAN_DataRegister[0] = holding_register[66];
            CAN_DataRegister[1] = holding_register[67];
            CAN_DataRegister[2] = holding_register[68];
            CAN_DataRegister[3] = holding_register[69];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF01
            CAN_DataRegister[0] = holding_register[75];
            CAN_DataRegister[1] = holding_register[76];
            CAN_DataRegister[2] = holding_register[77];
            CAN_DataRegister[3] = holding_register[78];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS01.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS01.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS01.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS01.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF02
            CAN_DataRegister[0] = holding_register[84];
            CAN_DataRegister[1] = holding_register[85];
            CAN_DataRegister[2] = holding_register[86];
            CAN_DataRegister[3] = holding_register[87];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS02.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS02.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS02.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS02.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF03
            CAN_DataRegister[0] = holding_register[93];
            CAN_DataRegister[1] = holding_register[94];
            CAN_DataRegister[2] = holding_register[95];
            CAN_DataRegister[3] = holding_register[96];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS03.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS03.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS03.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS03.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF04
            CAN_DataRegister[0] = holding_register[102];
            CAN_DataRegister[1] = holding_register[103];
            CAN_DataRegister[2] = holding_register[104];
            CAN_DataRegister[3] = holding_register[105];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS04.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS04.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS04.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS04.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF05
            CAN_DataRegister[0] = holding_register[111];
            CAN_DataRegister[1] = holding_register[112];
            CAN_DataRegister[2] = holding_register[113];
            CAN_DataRegister[3] = holding_register[114];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS05.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS05.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS05.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS05.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C02EF06
            CAN_DataRegister[0] = holding_register[120];
            CAN_DataRegister[1] = holding_register[121];
            CAN_DataRegister[2] = holding_register[122];
            CAN_DataRegister[3] = holding_register[123];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS06.Cell_maxV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS06.Cell_minV = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.001;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS06.Cell_maxTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
            BMS06.Cell_minTemp = (ArrayDate[0] * 256 + ArrayDate[1]) * 0.1;
            //===========================================================================
            //ID：0x1C03EF01
            CAN_DataRegister[0] = holding_register_2[4];
            CAN_DataRegister[1] = holding_register_2[5];
            CAN_DataRegister[2] = holding_register_2[6];
            CAN_DataRegister[3] = holding_register_2[7];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS01.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS01.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS01.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS01.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS01.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS01.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS01.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS01.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS01.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS01.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS01.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS01.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS01.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS01.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS01.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS01.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS01.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS01.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS01.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS01.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS01.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS01.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS01.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS01.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS01.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS01.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS01.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS01.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS01.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS01.Fault_Falg_Temp_LastTime = ArrayDate[0];
            //===========================================================================
            //ID：0x1C03EF02
            CAN_DataRegister[0] = holding_register_2[13];
            CAN_DataRegister[1] = holding_register_2[14];
            CAN_DataRegister[2] = holding_register_2[15];
            CAN_DataRegister[3] = holding_register_2[16];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS02.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS02.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS02.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS02.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS02.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS02.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS02.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS02.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS02.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS02.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS02.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS02.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS02.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS02.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS02.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS02.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS02.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS02.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS02.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS02.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS02.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS02.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS02.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS02.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS02.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS02.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS02.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS02.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS02.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS02.Fault_Falg_Temp_LastTime = ArrayDate[0];
            //===========================================================================
            //ID：0x1C03EF03
            CAN_DataRegister[0] = holding_register_2[22];
            CAN_DataRegister[1] = holding_register_2[23];
            CAN_DataRegister[2] = holding_register_2[24];
            CAN_DataRegister[3] = holding_register_2[25];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS03.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS03.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS03.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS03.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS03.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS03.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS03.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS03.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS03.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS03.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS03.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS03.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS03.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS03.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS03.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS03.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS03.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS03.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS03.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS03.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS03.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS03.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS03.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS03.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS03.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS03.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS03.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS03.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS03.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS03.Fault_Falg_Temp_LastTime = ArrayDate[0];
            //===========================================================================
            //ID：0x1C03EF04
            CAN_DataRegister[0] = holding_register_2[31];
            CAN_DataRegister[1] = holding_register_2[32];
            CAN_DataRegister[2] = holding_register_2[33];
            CAN_DataRegister[3] = holding_register_2[34];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS04.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS04.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS04.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS04.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS04.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS04.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS04.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS04.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS04.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS04.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS04.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS04.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS04.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS04.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS04.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS04.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS04.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS04.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS04.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS04.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS04.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS04.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS04.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS04.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS04.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS04.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS04.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS04.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS04.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS04.Fault_Falg_Temp_LastTime = ArrayDate[0];
            //===========================================================================
            //ID：0x1C03EF05
            CAN_DataRegister[0] = holding_register_2[40];
            CAN_DataRegister[1] = holding_register_2[41];
            CAN_DataRegister[2] = holding_register_2[42];
            CAN_DataRegister[3] = holding_register_2[43];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS05.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS05.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS05.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS05.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS05.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS05.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS05.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS05.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS05.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS05.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS05.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS05.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS05.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS05.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS05.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS05.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS05.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS05.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS05.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS05.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS05.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS05.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS05.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS05.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS05.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS05.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS05.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS05.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS05.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS05.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS05.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS05.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS05.Fault_Falg_Temp_LastTime = ArrayDate[0];
            //===========================================================================
            //ID：0x1C03EF06
            CAN_DataRegister[0] = holding_register_2[49];
            CAN_DataRegister[1] = holding_register_2[50];
            CAN_DataRegister[2] = holding_register_2[51];
            CAN_DataRegister[3] = holding_register_2[52];
            //---------------------------------------------
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
            BMS06.Fault_Falg_V = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS06.Fault_Falg_V));     //把一byte值存至點陣列中
            BMS06.Error[0] = bitStatus[0];  //Over Voltage Protection
            BMS06.Error[1] = bitStatus[1];  //Under Voltage Protection
            BMS06.Error[2] = bitStatus[2];  //Cell drops below the 2V threshold
            BMS06.Error[3] = bitStatus[3];  //Large Voltage Difference
            BMS06.Error[4] = bitStatus[4];  //MBMS Communication Error
            BMS06.Error[5] = bitStatus[5];  //BMU Communication Error
            BMS06.Error[6] = bitStatus[6];  //Slave BMS Error
            BMS06.Error[7] = bitStatus[7];  //Slave BMS Relay Error
            BMS06.Fault_Falg_A = ArrayDate[0];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS06.Fault_Falg_A));     //把一byte值存至點陣列中
            BMS06.Error[8] = bitStatus[0];  //Charging Over Current Protection Safety Level A
            BMS06.Error[9] = bitStatus[1];  //Charging Over Current Protection Safety Level B
            BMS06.Error[10] = bitStatus[2]; //Discharging Over Current Protection Safety Level A
            BMS06.Error[11] = bitStatus[3]; //Discharging Over Current Protection Safety Level B
            BMS06.Error[12] = bitStatus[4]; //Discharging Over Current Protection Safety Level C
            BMS06.Error[13] = bitStatus[5]; //Current Sensor Communication Error
            BMS06.Error[14] = bitStatus[6]; //Fault_Falg_A Bit6
            BMS06.Error[15] = bitStatus[7]; //Fault_Falg_A Bit7
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
            BMS06.Fault_Falg_Temp = ArrayDate[1];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS06.Fault_Falg_Temp));     //把一byte值存至點陣列中
            BMS06.Error[16] = bitStatus[0]; //Charging Over Temperature Protection
            BMS06.Error[17] = bitStatus[1]; //Charging Under Temperature Protection
            BMS06.Error[18] = bitStatus[2]; //Discharging Over Temperature Protection
            BMS06.Error[19] = bitStatus[3]; //Discharging Under Temperature Protection
            BMS06.Error[20] = bitStatus[4]; //Large Temperature Difference
            BMS06.Error[21] = bitStatus[5]; //Fault_Falg_Temp Bit5
            BMS06.Error[22] = bitStatus[6]; //Fault_Falg_Temp Bit6
            BMS06.Error[23] = bitStatus[7]; //Fault_Falg_Temp Bit7
            BMS06.Fault_Falg_V_LastTime = ArrayDate[0];
            ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
            BMS06.Fault_Falg_A_LastTime = ArrayDate[1];
            BMS06.Fault_Falg_Temp_LastTime = ArrayDate[0];
        }
        #endregion

        #region CATL
        //public double System_Status { get; set; } = 0;                      //電池工作狀態
        //public double Charging_maxA { get; set; } = 0;                      //最大充電電流
        //public double Discharging_maxA { get; set; } = 0;                      //最大放電電流
        //public double System_V { get; set; } = 0;                      //總電壓
        //public double System_A { get; set; } = 0;                      //總電流
        //public double SOC { get; set; } = 0;                           //SOC
        //public BMS_Subcircuit Sub01 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub02 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub03 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub04 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub05 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub06 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub07 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub08 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub09 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub10 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub11 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub12 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub13 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub14 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub15 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub16 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub17 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub18 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub19 = new BMS_Subcircuit();
        //public BMS_Subcircuit Sub20 = new BMS_Subcircuit();
        //public class BMS_Subcircuit
        //{
        //    public double RealyStatus { get; set; } = 0;              //支路Realy狀態
        //    public double SOC { get; set; } = 0;                        //支路SOC
        //    public double A { get; set; } = 0;                      //支路電流
        //}

        //public void Put_Data1()
        //{
        //    //===========================================================================
        //    //ID：0x000C0000
        //    CAN_DataRegister[0] = holding_register[3];
        //    CAN_DataRegister[1] = holding_register[4];
        //    CAN_DataRegister[2] = holding_register[5];
        //    CAN_DataRegister[3] = holding_register[6];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    System_Status = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    System_Status = Negative_num((ushort)System_Status);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Charging_maxA = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Charging_maxA = Negative_num((ushort)Charging_maxA);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Discharging_maxA = (ArrayDate[1] * 256 + ArrayDate[0]); 
        //    Discharging_maxA = Negative_num((ushort)Discharging_maxA);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    System_V = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    System_V = (Negative_num((ushort)System_V)) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C0004
        //    CAN_DataRegister[0] = holding_register[12];
        //    CAN_DataRegister[1] = holding_register[13];
        //    CAN_DataRegister[2] = holding_register[14];
        //    CAN_DataRegister[3] = holding_register[15];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    System_A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    System_A = Negative_num((ushort)System_A) *0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    SOC = Negative_num((ushort)SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub01.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub01.RealyStatus = Negative_num((ushort)Sub01.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub02.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub02.RealyStatus = Negative_num((ushort)Sub02.RealyStatus);
        //    //===========================================================================
        //    //ID：0x000C0008
        //    CAN_DataRegister[0] = holding_register[21];
        //    CAN_DataRegister[1] = holding_register[22];
        //    CAN_DataRegister[2] = holding_register[23];
        //    CAN_DataRegister[3] = holding_register[24];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub03.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub03.RealyStatus = Negative_num((ushort)Sub03.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub04.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub04.RealyStatus = Negative_num((ushort)Sub04.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub05.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub05.RealyStatus = Negative_num((ushort)Sub05.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub06.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub06.RealyStatus = Negative_num((ushort)Sub06.RealyStatus);
        //    //===========================================================================
        //    //ID：0x000C000C(12)
        //    CAN_DataRegister[0] = holding_register[30];
        //    CAN_DataRegister[1] = holding_register[31];
        //    CAN_DataRegister[2] = holding_register[32];
        //    CAN_DataRegister[3] = holding_register[33];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub07.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub07.RealyStatus = Negative_num((ushort)Sub07.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub08.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub08.RealyStatus = Negative_num((ushort)Sub08.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub09.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub09.RealyStatus = Negative_num((ushort)Sub09.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub10.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub10.RealyStatus = Negative_num((ushort)Sub10.RealyStatus);
        //    //===========================================================================
        //    //ID：0x000C0010(16)
        //    CAN_DataRegister[0] = holding_register[39];
        //    CAN_DataRegister[1] = holding_register[40];
        //    CAN_DataRegister[2] = holding_register[41];
        //    CAN_DataRegister[3] = holding_register[42];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub11.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub11.RealyStatus = Negative_num((ushort)Sub11.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub12.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub12.RealyStatus = Negative_num((ushort)Sub12.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub13.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub13.RealyStatus = Negative_num((ushort)Sub13.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub14.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub14.RealyStatus = Negative_num((ushort)Sub14.RealyStatus);
        //    //===========================================================================
        //    //ID：0x000C0014(20)
        //    CAN_DataRegister[0] = holding_register[48];
        //    CAN_DataRegister[1] = holding_register[49];
        //    CAN_DataRegister[2] = holding_register[50];
        //    CAN_DataRegister[3] = holding_register[51];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub15.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub15.RealyStatus = Negative_num((ushort)Sub15.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub16.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub16.RealyStatus = Negative_num((ushort)Sub16.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub17.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub17.RealyStatus = Negative_num((ushort)Sub17.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub18.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub18.RealyStatus = Negative_num((ushort)Sub18.RealyStatus);
        //    //===========================================================================
        //    //ID：0x000C0018(24)
        //    CAN_DataRegister[0] = holding_register[57];
        //    CAN_DataRegister[1] = holding_register[58];
        //    CAN_DataRegister[2] = holding_register[59];
        //    CAN_DataRegister[3] = holding_register[60];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub19.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub19.RealyStatus = Negative_num((ushort)Sub19.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub20.RealyStatus = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub20.RealyStatus = Negative_num((ushort)Sub20.RealyStatus);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub01.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub01.SOC = Negative_num((ushort)Sub01.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub02.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub02.SOC = Negative_num((ushort)Sub02.SOC);
        //    //===========================================================================
        //    //ID：0x000C001C(28)
        //    CAN_DataRegister[0] = holding_register[66];
        //    CAN_DataRegister[1] = holding_register[67];
        //    CAN_DataRegister[2] = holding_register[68];
        //    CAN_DataRegister[3] = holding_register[69];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub03.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub03.SOC = Negative_num((ushort)Sub03.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub04.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub04.SOC = Negative_num((ushort)Sub04.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub05.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub05.SOC = Negative_num((ushort)Sub05.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub06.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub06.SOC = Negative_num((ushort)Sub06.SOC);
        //    //===========================================================================
        //    //ID：0x000C0020(32)
        //    CAN_DataRegister[0] = holding_register[75];
        //    CAN_DataRegister[1] = holding_register[76];
        //    CAN_DataRegister[2] = holding_register[77];
        //    CAN_DataRegister[3] = holding_register[78];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub07.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub07.SOC = Negative_num((ushort)Sub07.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub08.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub08.SOC = Negative_num((ushort)Sub08.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub09.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub09.SOC = Negative_num((ushort)Sub09.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub10.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub10.SOC = Negative_num((ushort)Sub10.SOC);
        //    //===========================================================================
        //    //ID：0x000C0024(36)
        //    CAN_DataRegister[0] = holding_register[84];
        //    CAN_DataRegister[1] = holding_register[85];
        //    CAN_DataRegister[2] = holding_register[86];
        //    CAN_DataRegister[3] = holding_register[87];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub11.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub11.SOC = Negative_num((ushort)Sub11.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub12.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub12.SOC = Negative_num((ushort)Sub12.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub13.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub13.SOC = Negative_num((ushort)Sub13.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub14.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub14.SOC = Negative_num((ushort)Sub14.SOC);
        //    //===========================================================================
        //    //ID：0x000C0028(40)
        //    CAN_DataRegister[0] = holding_register[93];
        //    CAN_DataRegister[1] = holding_register[94];
        //    CAN_DataRegister[2] = holding_register[95];
        //    CAN_DataRegister[3] = holding_register[96];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub15.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub15.SOC = Negative_num((ushort)Sub15.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub16.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub16.SOC = Negative_num((ushort)Sub16.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub17.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub17.SOC = Negative_num((ushort)Sub17.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub18.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub18.SOC = Negative_num((ushort)Sub18.SOC);
        //    //===========================================================================
        //    //ID：0x000C002C(44)
        //    CAN_DataRegister[0] = holding_register[102];
        //    CAN_DataRegister[1] = holding_register[103];
        //    CAN_DataRegister[2] = holding_register[104];
        //    CAN_DataRegister[3] = holding_register[105];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub19.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub19.SOC = Negative_num((ushort)Sub19.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub20.SOC = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub20.SOC = Negative_num((ushort)Sub20.SOC);
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub01.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub01.A = Negative_num((ushort)Sub01.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub02.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub02.A = Negative_num((ushort)Sub02.A) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C0030(48)
        //    CAN_DataRegister[0] = holding_register[111];
        //    CAN_DataRegister[1] = holding_register[112];
        //    CAN_DataRegister[2] = holding_register[113];
        //    CAN_DataRegister[3] = holding_register[114];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub03.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub03.A = Negative_num((ushort)Sub03.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub04.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub04.A = Negative_num((ushort)Sub04.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub05.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub05.A = Negative_num((ushort)Sub05.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub06.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub06.A = Negative_num((ushort)Sub06.A) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C0034(52)
        //    CAN_DataRegister[0] = holding_register[120];
        //    CAN_DataRegister[1] = holding_register[121];
        //    CAN_DataRegister[2] = holding_register[122];
        //    CAN_DataRegister[3] = holding_register[123];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub07.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub07.A = Negative_num((ushort)Sub07.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub08.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub08.A = Negative_num((ushort)Sub08.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub09.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub09.A = Negative_num((ushort)Sub09.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub10.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub10.A = Negative_num((ushort)Sub10.A) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C0038(56)
        //    CAN_DataRegister[0] = holding_register_2[4];
        //    CAN_DataRegister[1] = holding_register_2[5];
        //    CAN_DataRegister[2] = holding_register_2[6];
        //    CAN_DataRegister[3] = holding_register_2[7];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub11.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub11.A = Negative_num((ushort)Sub11.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub12.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub12.A = Negative_num((ushort)Sub12.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub13.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub13.A = Negative_num((ushort)Sub13.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub14.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub14.A = Negative_num((ushort)Sub14.A) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C003C(60)
        //    CAN_DataRegister[0] = holding_register_2[13];
        //    CAN_DataRegister[1] = holding_register_2[14];
        //    CAN_DataRegister[2] = holding_register_2[15];
        //    CAN_DataRegister[3] = holding_register_2[16];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub15.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub15.A = Negative_num((ushort)Sub15.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub16.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub16.A = Negative_num((ushort)Sub16.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);
        //    Sub17.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub17.A = Negative_num((ushort)Sub17.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);
        //    Sub18.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub18.A = Negative_num((ushort)Sub18.A) * 0.1;
        //    //===========================================================================
        //    //ID：0x000C0040(64)
        //    CAN_DataRegister[0] = holding_register_2[22];
        //    CAN_DataRegister[1] = holding_register_2[23];
        //    CAN_DataRegister[2] = holding_register_2[24];
        //    CAN_DataRegister[3] = holding_register_2[25];
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[0]);
        //    Sub19.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub19.A = Negative_num((ushort)Sub19.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[1]);
        //    Sub20.A = (ArrayDate[1] * 256 + ArrayDate[0]);
        //    Sub20.A = Negative_num((ushort)Sub20.A) * 0.1;
        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[2]);

        //    //---------------------------------------------
        //    ArrayDate = BitConverter.GetBytes(CAN_DataRegister[3]);

        //}
        #endregion

        private double Negative_two_num(ushort num1, ushort num2)
        {
            double result = 0;
            if (num1 >= 32768)
            {
                result = (double)(num1 - 65535) * 65536 + (num2 - 65536);
            }
            else
            {
                result = num1 * 65536 + num2;
            }
            return result;
        }
        private double Negative_num(ushort num)
        {
            double result = 0;
            if (num >= 32768)
            {
                result = num - 65535;
            }
            else
            {
                result = num;
            }
            return result;
        }

    }

    public class MBMS_CATL  //////尚無資料
    {
        private string device_ID = "";
        private bool read_or_not;
        private double s_rated = 375;
        private double continuous_Communication_ErrorSeconds = 0;
        private double error_count = 0;
        private string communication_error = "";
        private double count = 0;
        private bool ems_flag = false;                          //判斷是否缺值
        private int ems_error = 0;
        private double i_rated = 800;


        public double S_rated { get => s_rated; set => s_rated = value; }
        public double Continuous_Communication_ErrorSeconds { get => continuous_Communication_ErrorSeconds; set => continuous_Communication_ErrorSeconds = value; }
        public double Error_count { get { return error_count; } set { error_count = value; } }
        public string Communication_error { get { return communication_error; } set { communication_error = value; } }
        public double Count { get { return count; } set { count = value; } }
        public bool Ems_flag { get { return ems_flag; } set { ems_flag = value; } }                       //判斷是否缺值
        public int Ems_error { get { return ems_error; } set { ems_error = value; } }
        public double I_rated { get => i_rated; set => i_rated = value; }

        public string Device_ID { get => device_ID; set => device_ID = value; }
        public bool Read_or_not { get => read_or_not; set => read_or_not = value; }


        public bool[,] Error_bit { get; set; } = new bool[5, 96];       //用在MBMS_Error_log()，判斷故障碼是否變更


        public ushort[] holding_register { get; set; } = new ushort[5];
        public ushort[] holding_register_2 { get; set; } = new ushort[29];
        public ushort[] holding_register_3 { get; set; } = new ushort[22];
        public ushort[] holding_register_Rank1_1 { get; set; } = new ushort[22];
        public ushort[] holding_register_Rank1_2_1 { get; set; } = new ushort[125];
        public ushort[] holding_register_Rank1_2_2 { get; set; } = new ushort[113];
        public ushort[] holding_register_Rank1_3 { get; set; } = new ushort[68];
        public ushort[] holding_register_Rank1_4 { get; set; } = new ushort[8];
        public ushort[] holding_register_Rank2_1 { get; set; } = new ushort[22];
        public ushort[] holding_register_Rank2_2_1 { get; set; } = new ushort[125];
        public ushort[] holding_register_Rank2_2_2 { get; set; } = new ushort[113];
        public ushort[] holding_register_Rank2_3 { get; set; } = new ushort[68];
        public ushort[] holding_register_Rank2_4 { get; set; } = new ushort[8];
        public ushort[] holding_register_Rank3_1 { get; set; } = new ushort[22];
        public ushort[] holding_register_Rank3_2_1 { get; set; } = new ushort[125];
        public ushort[] holding_register_Rank3_2_2 { get; set; } = new ushort[113];
        public ushort[] holding_register_Rank3_3 { get; set; } = new ushort[68];
        public ushort[] holding_register_Rank3_4 { get; set; } = new ushort[8];
        public ushort[] holding_register_Rank4_1 { get; set; } = new ushort[22];
        public ushort[] holding_register_Rank4_2_1 { get; set; } = new ushort[125];
        public ushort[] holding_register_Rank4_2_2 { get; set; } = new ushort[113];
        public ushort[] holding_register_Rank4_3 { get; set; } = new ushort[68];
        public ushort[] holding_register_Rank4_4 { get; set; } = new ushort[8];

        //public byte[] ArrayDate { get; set; } = new byte[2];           //用於將Word拆成兩個Byte(要注意高低位元)
        //holding_register[0]，Global.arraydate[0] & Global.arraydate[1]
        //     0xa1b1                0xb1                0xa1
        public System.Collections.BitArray bitStatus { get; set; } = new System.Collections.BitArray(16, false);      //建立一個大小為8的點陣列


        //System Configuration
        public double System_One_Key_Parallel_Switch { get; set; } = 0;           //1:啟用，2:結束
        public double System_Cluster_1_is_use { get; set; } = 0;           //1:使用，2不使用
        public double System_Cluster_2_is_use { get; set; } = 0;           //
        public double System_Cluster_3_is_use { get; set; } = 0;           //
        public double System_Cluster_4_is_use { get; set; } = 0;           //

        //System Status
        public double System_P { get; set; } = 0;                      //總功率(System_V * System_A)

        public double System_ChargingDischarging_State { get; set; } = 0;           //0:靜置，1:放電，2:充電
        public double System_A { get; set; } = 0;                      //總電流
        public double System_Spare1 { get; set; } = 0;                      //備用1
        public double System_SOC { get; set; } = 0;                           //SOC
        public double System_Run_State { get; set; } = 0;       //運行狀態
        public double System_V { get; set; } = 0;                      //總電壓
        public double System_Insulation { get; set; } = 0;                      //系統絕緣
        public double System_Spare2 { get; set; } = 0;                      //備用2
        public double System_Spare3 { get; set; } = 0;                      //備用3
        public double System_Spare4 { get; set; } = 0;                      //備用4
        public double System_MaximumPermissibleCurrent_Charging { get; set; } = 0;   //最大允許充電電流
        public double System_MaximumPermissibleCurrent_Discharging { get; set; } = 0;   //最大允許放電電流
        public double System_SOH { get; set; } = 0;                           //SOH
        public double Cell_maxV_Index_Rank { get; set; } = 0;     //
        public double Cell_maxV_Index_Module { get; set; } = 0;   //
        public double Cell_maxV_Index_Cell { get; set; } = 0;     //
        public double Cell_maxV { get; set; } = 0;                //
        public double Cell_minV_Index_Rank { get; set; } = 0;     //
        public double Cell_minV_Index_Module { get; set; } = 0;   //
        public double Cell_minV_Index_Cell { get; set; } = 0;     //
        public double Cell_minV { get; set; } = 0;                //
        public double Cell_maxTemp_Index_Rank { get; set; } = 0;      //
        public double Cell_maxTemp_Index_Module { get; set; } = 0;    //
        public double Cell_maxTemp_Index_Cell { get; set; } = 0;      //
        public double Cell_maxTemp { get; set; } = 0;                 //
        public double Cell_minTemp_Index_Rank { get; set; } = 0;      //
        public double Cell_minTemp_Index_Module { get; set; } = 0;    //
        public double Cell_minTemp_Index_Cell { get; set; } = 0;      //
        public double Cell_minTemp { get; set; } = 0;                 //

        //System Error
        public ushort System_Error { get; set; } = 0;            //
        public ushort Ranks_Communication_Error { get; set; } = 0;      //
        public ushort System_Error_Warning { get; set; } = 0;           //
        public ushort System_Error_Stop { get; set; } = 0;              //
        public ushort System_Error_Protection { get; set; } = 0;        //


        public bool[] Error { get; set; } = new bool[80];       //將System_Error資訊(bit)全部集中


        public BMS_Rack BMS01 = new BMS_Rack();
        public BMS_Rack BMS02 = new BMS_Rack();
        public BMS_Rack BMS03 = new BMS_Rack();
        public BMS_Rack BMS04 = new BMS_Rack();
        public class BMS_Rack
        {
            //BMS Status
            public double V { get; set; } = 0;                          //
            public double A { get; set; } = 0;                          //
            public double BMS_ChargingDischarging_State { get; set; } = 0; //0:靜置，1:放電，2:充電
            public double SOC { get; set; } = 0;                        //
            public double SOH { get; set; } = 0;                        //
            public double Cell_maxV_Index_Serial { get; set; } = 0;          //
            public double Cell_maxV { get; set; } = 0;                       //
            public double Cell_minV_Index_Serial { get; set; } = 0;          //
            public double Cell_minV { get; set; } = 0;                       //
            public double Cell_maxTemp_Index_Serial { get; set; } = 0;       //
            public double Cell_maxTemp { get; set; } = 0;                    //
            public double Cell_minTemp_Index_Serial { get; set; } = 0;       //
            public double Cell_minTemp { get; set; } = 0;                    //
            public double Cell_avgV { get; set; } = 0;                       //
            public double Insulation { get; set; } = 0;                 //絕緣
            public double Max_Current_Charging { get; set; } = 0;       //最大充電電流
            public double Max_Current_Discharging { get; set; } = 0;    //最大放電電流
            public double Insulation_Positive_Electrode { get; set; } = 0;  //正極絕緣值
            public double Insulation_Negative_Electrode { get; set; } = 0;  //負極絕緣值
            public double Cell_avgTemp { get; set; } = 0;               //
            //BMS V
            public double[] Cell_V { get; set; } = new double[238];     //共238串
            //BMS Temp
            public double[] Cell_Temp { get; set; } = new double[68];   //共68串
            //BMS Contactors Status
            public ushort Contactors_Status { get; set; } = 0;          //
            public bool Positive_Contactor { get; set; } = false;          //bit0主正接觸器狀態
            public bool Precharge_Contactor { get; set; } = false;          //bit1預充接觸器狀態
            public bool Negative_Contactor { get; set; } = false;          //CATL沒有負接觸器狀態
            //BMS Run State
            public ushort BMS_Run_State { get; set; } = 0;          //
            //BMS Error
            public ushort BMS_Error { get; set; } = 0;            //
            public ushort Module_Error { get; set; } = 0;            //
            public ushort BMS_Error_Warning { get; set; } = 0;           //
            public ushort BMS_Error_Stop { get; set; } = 0;              //
            public ushort BMS_Error_Protection { get; set; } = 0;        //
            public ushort BMS_Other_Error { get; set; } = 0;            //


            public bool[] Error { get; set; } = new bool[96];       //將Error資訊(bit)全部集中
        }

        public void Put_Data1()
        {
            //===========================================================================

            //System Configuration
            System_One_Key_Parallel_Switch = holding_register[0];
            System_Cluster_1_is_use = holding_register[1];
            System_Cluster_2_is_use = holding_register[2];
            System_Cluster_3_is_use = holding_register[3];
            System_Cluster_4_is_use = holding_register[4];
            //---------------------------------------------

            //System Status
            System_ChargingDischarging_State = holding_register_2[0];
            //---------------------
            System_A = holding_register_2[1];
            System_A = (Negative_num((ushort)System_A)) * 0.1;
            //---------------------
            System_Spare1 = holding_register_2[2];
            System_SOC = holding_register_2[3];                                 //0%～100%
            System_Run_State = holding_register_2[4];                           //0：正常(系統可以放電，可以充電)
                                                                                //1：充滿(系統可以放電，不能充電)
                                                                                //2：放空(系統可以充電，不能放電)
                                                                                //3：待機(系統不能充放，接觸器閉合)
                                                                                //4：停機(系統不能充放，接觸器斷開)
                                                                                //---------------------
            System_V = holding_register_2[5];
            System_V = System_V * 0.1;
            //---------------------
            System_P = System_V * System_A * 0.001;
            //---------------------
            System_Insulation = holding_register_2[6];                          //單位：KΩ
            System_Spare2 = holding_register_2[7];
            System_Spare3 = holding_register_2[8];
            System_Spare4 = holding_register_2[9];
            //---------------------
            System_MaximumPermissibleCurrent_Charging = holding_register_2[10];
            System_MaximumPermissibleCurrent_Charging = (Negative_num((ushort)System_MaximumPermissibleCurrent_Charging)) * 0.1;
            //---------------------
            System_MaximumPermissibleCurrent_Discharging = holding_register_2[11];
            System_MaximumPermissibleCurrent_Discharging = (Negative_num((ushort)System_MaximumPermissibleCurrent_Discharging)) * 0.1;
            //---------------------
            System_SOH = holding_register_2[12];                                //0‰～1000‰
            System_SOH = System_SOH * 0.1;                                      //依照EMS要求改成0%~100%
            Cell_maxV_Index_Rank = holding_register_2[13];                      //1～4 Rank
            Cell_maxV_Index_Module = holding_register_2[14];                    //1～17 Module
            Cell_maxV_Index_Cell = holding_register_2[15];                      //1～14 Cell
            //---------------------
            Cell_maxV = holding_register_2[16];                                 //mV
            Cell_maxV = Cell_maxV * 0.001;                                      //V
            //---------------------
            Cell_minV_Index_Rank = holding_register_2[17];
            Cell_minV_Index_Module = holding_register_2[18];
            Cell_minV_Index_Cell = holding_register_2[19];
            //---------------------
            Cell_minV = holding_register_2[20];                                 //mV
            Cell_minV = Cell_minV * 0.001;                                      //V
            //---------------------
            Cell_maxTemp_Index_Rank = holding_register_2[21];
            Cell_maxTemp_Index_Module = holding_register_2[22];
            Cell_maxTemp_Index_Cell = holding_register_2[23];
            //---------------------
            Cell_maxTemp = holding_register_2[24];
            Cell_maxTemp = (Negative_num((ushort)Cell_maxTemp)) * 0.1;          //℃
            //---------------------
            Cell_minTemp_Index_Rank = holding_register_2[25];
            Cell_minTemp_Index_Module = holding_register_2[26];
            Cell_minTemp_Index_Cell = holding_register_2[27];
            //---------------------
            Cell_minTemp = holding_register_2[28];
            Cell_minTemp = (Negative_num((ushort)Cell_minTemp)) * 0.1;          //℃
            //---------------------------------------------

            //System Error
            System_Error = holding_register_3[0];                                   //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(System_Error));     //把一ushort值存至點陣列中
            Error[0] = bitStatus[0];                //總控EEPROM初始化故障
            Error[1] = bitStatus[1];                //總控FLASH初始化故障
            Error[2] = bitStatus[2];                //總控網路模塊初始化故障
            Error[3] = bitStatus[3];                //總控RTC初始化故障
            Error[4] = bitStatus[4];
            Error[5] = bitStatus[5];
            Error[6] = bitStatus[6];
            Error[7] = bitStatus[7];
            Error[8] = bitStatus[8];
            Error[9] = bitStatus[9];
            Error[10] = bitStatus[10];
            Error[11] = bitStatus[11];
            Error[12] = bitStatus[12];
            Error[13] = bitStatus[13];
            Error[14] = bitStatus[14];
            Error[15] = bitStatus[15];
            //---------------------
            // = holding_register_3[1];                                               //備用
            //---------------------
            Ranks_Communication_Error = holding_register_3[2];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(Ranks_Communication_Error));     //把一ushort值存至點陣列中
            Error[16] = bitStatus[0];                //主控1通訊故障
            Error[17] = bitStatus[1];                //主控2通訊故障
            Error[18] = bitStatus[2];                //主控3通訊故障
            Error[19] = bitStatus[3];                //主控4通訊故障
            Error[20] = bitStatus[4];
            Error[21] = bitStatus[5];
            Error[22] = bitStatus[6];
            Error[23] = bitStatus[7];
            Error[24] = bitStatus[8];
            Error[25] = bitStatus[9];
            Error[26] = bitStatus[10];
            Error[27] = bitStatus[11];
            Error[28] = bitStatus[12];
            Error[29] = bitStatus[13];
            Error[30] = bitStatus[14];
            Error[31] = bitStatus[15];
            //---------------------
            // = holding_register_3[3];                                               //預留(內部使用)
            // = holding_register_3[4];
            // = holding_register_3[5];
            // = holding_register_3[6];
            // = holding_register_3[7];
            // = holding_register_3[8];
            // = holding_register_3[9];
            // = holding_register_3[10];
            // = holding_register_3[11];
            // = holding_register_3[12];
            // = holding_register_3[13];
            // = holding_register_3[14];
            // = holding_register_3[15];
            // = holding_register_3[16];
            // = holding_register_3[17];
            // = holding_register_3[18];
            //---------------------
            System_Error_Warning = holding_register_3[19];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(System_Error_Warning));     //把一ushort值存至點陣列中
            Error[32] = bitStatus[0];                //堆內各組單體過壓一級報警匯總(電池即將充滿)
            Error[33] = bitStatus[1];                //堆內各組總電壓過壓一級報警匯總(電池即將充滿)
            Error[34] = bitStatus[2];                //堆內各組充電過流一級報警匯總
            Error[35] = bitStatus[3];                //堆內各組單體欠壓一級報警匯總(電池即將放空)
            Error[36] = bitStatus[4];                //堆內各組總電壓欠壓一級報警匯總(電池即將放空)
            Error[37] = bitStatus[5];                //堆內各組放電過流一級報警匯總
            Error[38] = bitStatus[6];                //堆內各組充電溫度過高一級報警匯總
            Error[39] = bitStatus[7];                //堆內各組充電溫度過低一級報警匯總
            Error[40] = bitStatus[8];                //堆內各組SOC過低一級報警匯總
            Error[41] = bitStatus[9];                //堆內各組溫度差異過大一級報警匯總
            Error[42] = bitStatus[10];               //堆內各組柱極溫度過高一級報警匯總
            Error[43] = bitStatus[11];               //堆內各組單體差異過大一級報警匯總
            Error[44] = bitStatus[12];               //堆內各組絕緣過低一級報警匯總
            Error[45] = bitStatus[13];               //堆內各組總壓差異過大一級報警匯總
            Error[46] = bitStatus[14];               //堆內各組放電溫度過高一級報警匯總
            Error[47] = bitStatus[15];               //堆內各組放電溫度過低一級報警匯總
            //---------------------
            System_Error_Stop = holding_register_3[20];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(System_Error_Stop));     //把一ushort值存至點陣列中
            Error[48] = bitStatus[0];                //堆內各組單體過壓二級報警匯總(電池即將充滿)
            Error[49] = bitStatus[1];                //堆內各組總電壓過壓二級報警匯總(電池即將充滿)
            Error[50] = bitStatus[2];                //堆內各組充電過流二級報警匯總
            Error[51] = bitStatus[3];                //堆內各組單體欠壓二級報警匯總(電池即將放空)
            Error[52] = bitStatus[4];                //堆內各組總電壓欠壓二級報警匯總(電池即將放空)
            Error[53] = bitStatus[5];                //堆內各組放電過流二級報警匯總
            Error[54] = bitStatus[6];                //堆內各組充電溫度過高二級報警匯總
            Error[55] = bitStatus[7];                //堆內各組充電溫度過低二級報警匯總
            Error[56] = bitStatus[8];                //堆內各組SOC過低二級報警匯總
            Error[57] = bitStatus[9];                //堆內各組溫度差異過大二級報警匯總
            Error[58] = bitStatus[10];               //堆內各組柱極溫度過高二級報警匯總
            Error[59] = bitStatus[11];               //堆內各組單體差異過大二級報警匯總
            Error[60] = bitStatus[12];               //堆內各組絕緣過低二級報警匯總
            Error[61] = bitStatus[13];               //堆內各組總壓差異過大二級報警匯總
            Error[62] = bitStatus[14];               //堆內各組放電溫度過高二級報警匯總
            Error[63] = bitStatus[15];               //堆內各組放電溫度過低二級報警匯總
            //---------------------
            System_Error_Protection = holding_register_3[21];
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(System_Error_Protection));     //把一ushort值存至點陣列中
            Error[64] = bitStatus[0];                //堆內各組單體過壓三級報警匯總(電池即將充滿)
            Error[65] = bitStatus[1];                //堆內各組總電壓過壓三級報警匯總(電池即將充滿)
            Error[66] = bitStatus[2];                //堆內各組充電過流三級報警匯總
            Error[67] = bitStatus[3];                //堆內各組單體欠壓三級報警匯總(電池即將放空)
            Error[68] = bitStatus[4];                //堆內各組總電壓欠壓三級報警匯總(電池即將放空)
            Error[69] = bitStatus[5];                //堆內各組放電過流三級報警匯總
            Error[70] = bitStatus[6];                //堆內各組充電溫度過高三級報警匯總
            Error[71] = bitStatus[7];                //堆內各組充電溫度過低三級報警匯總
            Error[72] = bitStatus[8];                //堆內各組SOC過低三級報警匯總
            Error[73] = bitStatus[9];                //堆內各組溫度差異過大三級報警匯總
            Error[74] = bitStatus[10];               //堆內各組柱極溫度過高三級報警匯總
            Error[75] = bitStatus[11];               //堆內各組單體差異過大三級報警匯總
            Error[76] = bitStatus[12];               //堆內各組絕緣過低三級報警匯總
            Error[77] = bitStatus[13];               //堆內各組總壓差異過大三級報警匯總
            Error[78] = bitStatus[14];               //堆內各組放電溫度過高三級報警匯總
            Error[79] = bitStatus[15];               //堆內各組放電溫度過低三級報警匯總
            //===========================================================================

            //BMS Status
            BMS01.V = holding_register_Rank1_1[0];
            BMS01.V = BMS01.V * 0.1;
            //---------------------
            BMS01.A = holding_register_Rank1_1[1];
            BMS01.A = (Negative_num((ushort)BMS01.A)) * 0.1;
            //---------------------
            BMS01.BMS_ChargingDischarging_State = holding_register_Rank1_1[2];
            BMS01.SOC = holding_register_Rank1_1[3];
            //---------------------
            BMS01.SOH = holding_register_Rank1_1[4];
            BMS01.SOH = BMS01.SOH * 0.1;
            //---------------------
            BMS01.Cell_maxV_Index_Serial = holding_register_Rank1_1[5];
            //---------------------
            BMS01.Cell_maxV = holding_register_Rank1_1[6];
            BMS01.Cell_maxV = BMS01.Cell_maxV * 0.001;
            //---------------------
            BMS01.Cell_minV_Index_Serial = holding_register_Rank1_1[7];
            //---------------------
            BMS01.Cell_minV = holding_register_Rank1_1[8];
            BMS01.Cell_minV = BMS01.Cell_minV * 0.001;
            //---------------------
            BMS01.Cell_maxTemp_Index_Serial = holding_register_Rank1_1[9];
            //---------------------
            BMS01.Cell_maxTemp = holding_register_Rank1_1[10];
            BMS01.Cell_maxTemp = (Negative_num((ushort)BMS01.Cell_maxTemp)) * 0.1;
            //---------------------
            BMS01.Cell_minTemp_Index_Serial = holding_register_Rank1_1[11];
            //---------------------
            BMS01.Cell_minTemp = holding_register_Rank1_1[12];
            BMS01.Cell_minTemp = (Negative_num((ushort)BMS01.Cell_minTemp)) * 0.1;
            //---------------------
            BMS01.Cell_avgV = holding_register_Rank1_1[13];
            BMS01.Cell_avgV = BMS01.Cell_avgV * 0.001;
            //---------------------
            BMS01.Insulation = holding_register_Rank1_1[14];
            BMS01.Max_Current_Charging = holding_register_Rank1_1[15];
            BMS01.Max_Current_Discharging = holding_register_Rank1_1[16];
            BMS01.Insulation_Positive_Electrode = holding_register_Rank1_1[17];     //
            BMS01.Insulation_Negative_Electrode = holding_register_Rank1_1[18];     //
            // = holding_register_Rank1_1[19];                                        //備用
            //---------------------
            BMS01.Cell_avgTemp = holding_register_Rank1_1[20];
            BMS01.Cell_avgTemp = (Negative_num((ushort)BMS01.Cell_avgTemp)) * 0.1;
            //---------------------
            // = holding_register_Rank1_1[21];                                        //備用
            //---------------------------------------------
            /*
            //BMS V
            for(int i=0; i < 125; i++)
            {
                BMS01.Cell_V[i] = holding_register_Rank1_2_1[i];
                BMS01.Cell_V[i] = BMS01.Cell_V[i] * 0.001;
            }
            for (int i = 0; i < 113; i++)
            {
                BMS01.Cell_V[i+125] = holding_register_Rank1_2_2[i];
                BMS01.Cell_V[i + 125] = BMS01.Cell_V[i + 125] * 0.001;
            }
            //---------------------------------------------

            //BMS Temp
            for (int i = 0; i < 68; i++)
            {
                BMS01.Cell_Temp[i] = holding_register_Rank1_3[i];
                BMS01.Cell_Temp[i] = (Negative_num((ushort)BMS01.Cell_Temp[i])) * 0.1;
            }
            //---------------------------------------------
            */
            //BMS Contactors Status
            BMS01.Contactors_Status = holding_register_Rank1_4[0];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.Contactors_Status));     //把一ushort值存至點陣列中
            BMS01.Positive_Contactor = bitStatus[0];
            BMS01.Precharge_Contactor = bitStatus[1];
            //---------------------

            //BMS Run State
            BMS01.BMS_Run_State = holding_register_Rank1_4[1];                      //0：正常(系統可以放電，可以充電)
                                                                                    //1：充滿(系統可以放電，不能充電)
                                                                                    //2：放空(系統可以充電，不能放電)
                                                                                    //3：待機(系統不能充放，接觸器閉合)
                                                                                    //4：停機(系統不能充放，接觸器斷開)
                                                                                    //---------------------

            //BMS Error
            BMS01.BMS_Error = holding_register_Rank1_4[2];                          //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.BMS_Error));     //把一ushort值存至點陣列中
            BMS01.Error[0] = bitStatus[0];          //主控EEPROM故障
            BMS01.Error[1] = bitStatus[1];
            BMS01.Error[2] = bitStatus[2];
            BMS01.Error[3] = bitStatus[3];          //主控FLASH自檢狀態
            BMS01.Error[4] = bitStatus[4];          //主控RTC自檢狀態
            BMS01.Error[5] = bitStatus[5];
            BMS01.Error[6] = bitStatus[6];
            BMS01.Error[7] = bitStatus[7];
            BMS01.Error[8] = bitStatus[8];
            BMS01.Error[9] = bitStatus[9];
            BMS01.Error[10] = bitStatus[10];
            BMS01.Error[11] = bitStatus[11];
            BMS01.Error[12] = bitStatus[12];
            BMS01.Error[13] = bitStatus[13];
            BMS01.Error[14] = bitStatus[14];
            BMS01.Error[15] = bitStatus[15];
            //---------------------
            BMS01.Module_Error = holding_register_Rank1_4[3];                       //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.Module_Error));     //把一ushort值存至點陣列中
            BMS01.Error[16] = bitStatus[0];         //從控採樣線故障
            BMS01.Error[17] = bitStatus[1];         //從控連接線故障
            BMS01.Error[18] = bitStatus[2];         //從控LTC6803故障
            BMS01.Error[19] = bitStatus[3];         //從控電壓採樣故障
            BMS01.Error[20] = bitStatus[4];         //從控溫度採樣故障
            BMS01.Error[21] = bitStatus[5];         //從控溫度傳感器故障
            BMS01.Error[22] = bitStatus[6];         //從控極耳溫度故障
            BMS01.Error[23] = bitStatus[7];         //從控保留
            BMS01.Error[24] = bitStatus[8];         //從控均衡模塊故障
            BMS01.Error[25] = bitStatus[9];         //從控溫度採樣線故障
            BMS01.Error[26] = bitStatus[10];        //從控內網通信故障
            BMS01.Error[27] = bitStatus[11];        //從控EEPROM故障
            BMS01.Error[28] = bitStatus[12];        //從控初始化故障
            BMS01.Error[29] = bitStatus[13];
            BMS01.Error[30] = bitStatus[14];
            BMS01.Error[31] = bitStatus[15];
            //---------------------
            BMS01.BMS_Error_Warning = holding_register_Rank1_4[4];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.BMS_Error_Warning));     //把一ushort值存至點陣列中
            BMS01.Error[32] = bitStatus[0];        //電池即將充滿(單體過壓一級報警)
            BMS01.Error[33] = bitStatus[1];        //電池即將充滿(總電壓過壓一級報警)
            BMS01.Error[34] = bitStatus[2];        //充電過流一級報警
            BMS01.Error[35] = bitStatus[3];        //電池即將放空(單體欠壓一級報警)
            BMS01.Error[36] = bitStatus[4];        //電池即將放空(總電壓欠壓一級報警)
            BMS01.Error[37] = bitStatus[5];        //放電過流一級報警
            BMS01.Error[38] = bitStatus[6];        //充電溫度過高一級報警
            BMS01.Error[39] = bitStatus[7];        //充電溫度過低一級報警
            BMS01.Error[40] = bitStatus[8];        //SOC過低一級報警
            BMS01.Error[41] = bitStatus[9];        //溫度差異過大一級報警
            BMS01.Error[42] = bitStatus[10];       //極柱溫度過高一級報警
            BMS01.Error[43] = bitStatus[11];       //單體差異過大一級報警
            BMS01.Error[44] = bitStatus[12];       //絕緣過低一級報警
            BMS01.Error[45] = bitStatus[13];       //總壓差異過大一級報警
            BMS01.Error[46] = bitStatus[14];       //放電溫度過高一級報警
            BMS01.Error[47] = bitStatus[15];       //放電溫度過低一級報警
            //---------------------
            BMS01.BMS_Error_Stop = holding_register_Rank1_4[5];                     //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.BMS_Error_Stop));     //把一ushort值存至點陣列中
            BMS01.Error[48] = bitStatus[0];        //電池已充滿(單體過壓二級報警)
            BMS01.Error[49] = bitStatus[1];        //電池已充滿(總電壓過壓二級報警)
            BMS01.Error[50] = bitStatus[2];        //充電過流二級報警
            BMS01.Error[51] = bitStatus[3];        //電池已放空(單體欠壓二級報警)
            BMS01.Error[52] = bitStatus[4];        //電池已放空(總電壓欠壓二級報警)
            BMS01.Error[53] = bitStatus[5];        //放電過流二級報警
            BMS01.Error[54] = bitStatus[6];        //充電溫度過高二級報警
            BMS01.Error[55] = bitStatus[7];        //充電溫度過低二級報警
            BMS01.Error[56] = bitStatus[8];        //SOC過低二級報警
            BMS01.Error[57] = bitStatus[9];        //溫度差異過大二級報警
            BMS01.Error[58] = bitStatus[10];       //極柱溫度過高二級報警
            BMS01.Error[59] = bitStatus[11];       //單體差異過大二級報警
            BMS01.Error[60] = bitStatus[12];       //絕緣過低二級報警
            BMS01.Error[61] = bitStatus[13];       //總壓差異過大二級報警
            BMS01.Error[62] = bitStatus[14];       //放電溫度過高二級報警
            BMS01.Error[63] = bitStatus[15];       //放電溫度過低二級報警
            //---------------------
            BMS01.BMS_Error_Protection = holding_register_Rank1_4[6];               //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.BMS_Error_Protection));     //把一ushort值存至點陣列中
            BMS01.Error[64] = bitStatus[0];        //單體過壓三級報警
            BMS01.Error[65] = bitStatus[1];        //系統過壓三級報警
            BMS01.Error[66] = bitStatus[2];        //充電過流三級報警
            BMS01.Error[67] = bitStatus[3];        //單體欠壓三級報警
            BMS01.Error[68] = bitStatus[4];        //系統欠壓三級報警
            BMS01.Error[69] = bitStatus[5];        //放電過流三級報警
            BMS01.Error[70] = bitStatus[6];        //充電溫度過高三級報警
            BMS01.Error[71] = bitStatus[7];        //充電溫度過低三級報警
            BMS01.Error[72] = bitStatus[8];        //SOC過低三級報警
            BMS01.Error[73] = bitStatus[9];        //溫差過大三級報警
            BMS01.Error[74] = bitStatus[10];       //極柱溫度過高三級報警
            BMS01.Error[75] = bitStatus[11];       //單體差異過大三級報警
            BMS01.Error[76] = bitStatus[12];       //絕緣過低三級報警
            BMS01.Error[77] = bitStatus[13];       //總壓差異過大三級報警
            BMS01.Error[78] = bitStatus[14];       //放電溫度過高三級報警
            BMS01.Error[79] = bitStatus[15];       //放電溫度過低三級報警
            //---------------------
            BMS01.BMS_Other_Error = holding_register_Rank1_4[7];                    //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS01.BMS_Other_Error));     //把一ushort值存至點陣列中
            BMS01.Error[80] = bitStatus[0];        //與總控通信故障
            BMS01.Error[81] = bitStatus[1];        //與從控通信故障
            BMS01.Error[82] = bitStatus[2];
            BMS01.Error[83] = bitStatus[3];        //從控設備故障
            BMS01.Error[84] = bitStatus[4];
            BMS01.Error[85] = bitStatus[5];
            BMS01.Error[86] = bitStatus[6];
            BMS01.Error[87] = bitStatus[7];
            BMS01.Error[88] = bitStatus[8];
            BMS01.Error[89] = bitStatus[9];
            BMS01.Error[90] = bitStatus[10];
            BMS01.Error[91] = bitStatus[11];
            BMS01.Error[92] = bitStatus[12];
            BMS01.Error[93] = bitStatus[13];
            BMS01.Error[94] = bitStatus[14];
            BMS01.Error[95] = bitStatus[15];
            //===========================================================================

            //BMS Status
            BMS02.V = holding_register_Rank2_1[0];
            BMS02.V = BMS02.V * 0.1;
            //---------------------
            BMS02.A = holding_register_Rank2_1[1];
            BMS02.A = (Negative_num((ushort)BMS02.A)) * 0.1;
            //---------------------
            BMS02.BMS_ChargingDischarging_State = holding_register_Rank2_1[2];
            BMS02.SOC = holding_register_Rank2_1[3];
            //---------------------
            BMS02.SOH = holding_register_Rank2_1[4];
            BMS02.SOH = BMS02.SOH * 0.1;
            //---------------------
            BMS02.Cell_maxV_Index_Serial = holding_register_Rank2_1[5];
            //---------------------
            BMS02.Cell_maxV = holding_register_Rank2_1[6];
            BMS02.Cell_maxV = BMS02.Cell_maxV * 0.001;
            //---------------------
            BMS02.Cell_minV_Index_Serial = holding_register_Rank2_1[7];
            //---------------------
            BMS02.Cell_minV = holding_register_Rank2_1[8];
            BMS02.Cell_minV = BMS02.Cell_minV * 0.001;
            //---------------------
            BMS02.Cell_maxTemp_Index_Serial = holding_register_Rank2_1[9];
            //---------------------
            BMS02.Cell_maxTemp = holding_register_Rank2_1[10];
            BMS02.Cell_maxTemp = (Negative_num((ushort)BMS02.Cell_maxTemp)) * 0.1;
            //---------------------
            BMS02.Cell_minTemp_Index_Serial = holding_register_Rank2_1[11];
            //---------------------
            BMS02.Cell_minTemp = holding_register_Rank2_1[12];
            BMS02.Cell_minTemp = (Negative_num((ushort)BMS02.Cell_minTemp)) * 0.1;
            //---------------------
            BMS02.Cell_avgV = holding_register_Rank2_1[13];
            BMS02.Cell_avgV = BMS02.Cell_avgV * 0.001;
            //---------------------
            BMS02.Insulation = holding_register_Rank2_1[14];
            BMS02.Max_Current_Charging = holding_register_Rank2_1[15];
            BMS02.Max_Current_Discharging = holding_register_Rank2_1[16];
            BMS02.Insulation_Positive_Electrode = holding_register_Rank2_1[17];     //
            BMS02.Insulation_Negative_Electrode = holding_register_Rank2_1[18];     //
            // = holding_register_Rank2_1[19];                                        //備用
            //---------------------
            BMS02.Cell_avgTemp = holding_register_Rank2_1[20];
            BMS02.Cell_avgTemp = (Negative_num((ushort)BMS02.Cell_avgTemp)) * 0.1;
            //---------------------
            // = holding_register_Rank2_1[21];                                        //備用
            //---------------------------------------------
            /*
            //BMS V
            for (int i = 0; i < 125; i++)
            {
                BMS02.Cell_V[i] = holding_register_Rank2_2_1[i];
                BMS02.Cell_V[i] = BMS02.Cell_V[i] * 0.001;
            }
            for (int i = 0; i < 113; i++)
            {
                BMS02.Cell_V[i + 125] = holding_register_Rank2_2_2[i];
                BMS02.Cell_V[i + 125] = BMS02.Cell_V[i + 125] * 0.001;
            }
            //---------------------------------------------

            //BMS Temp
            for (int i = 0; i < 68; i++)
            {
                BMS02.Cell_Temp[i] = holding_register_Rank2_3[i];
                BMS02.Cell_Temp[i] = (Negative_num((ushort)BMS02.Cell_Temp[i])) * 0.1;
            }
            //---------------------------------------------
            */
            //BMS Contactors Status
            BMS02.Contactors_Status = holding_register_Rank2_4[0];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.Contactors_Status));     //把一ushort值存至點陣列中
            BMS02.Positive_Contactor = bitStatus[0];
            BMS02.Precharge_Contactor = bitStatus[1];
            //---------------------

            //BMS Run State
            BMS02.BMS_Run_State = holding_register_Rank2_4[1];                      //0：正常(系統可以放電，可以充電)
                                                                                    //1：充滿(系統可以放電，不能充電)
                                                                                    //2：放空(系統可以充電，不能放電)
                                                                                    //3：待機(系統不能充放，接觸器閉合)
                                                                                    //4：停機(系統不能充放，接觸器斷開)
                                                                                    //---------------------

            //BMS Error
            BMS02.BMS_Error = holding_register_Rank2_4[2];                          //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.BMS_Error));     //把一ushort值存至點陣列中
            BMS02.Error[0] = bitStatus[0];          //主控EEPROM故障
            BMS02.Error[1] = bitStatus[1];
            BMS02.Error[2] = bitStatus[2];
            BMS02.Error[3] = bitStatus[3];          //主控FLASH自檢狀態
            BMS02.Error[4] = bitStatus[4];          //主控RTC自檢狀態
            BMS02.Error[5] = bitStatus[5];
            BMS02.Error[6] = bitStatus[6];
            BMS02.Error[7] = bitStatus[7];
            BMS02.Error[8] = bitStatus[8];
            BMS02.Error[9] = bitStatus[9];
            BMS02.Error[10] = bitStatus[10];
            BMS02.Error[11] = bitStatus[11];
            BMS02.Error[12] = bitStatus[12];
            BMS02.Error[13] = bitStatus[13];
            BMS02.Error[14] = bitStatus[14];
            BMS02.Error[15] = bitStatus[15];
            //---------------------
            BMS02.Module_Error = holding_register_Rank2_4[3];                       //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.Module_Error));     //把一ushort值存至點陣列中
            BMS02.Error[16] = bitStatus[0];         //從控採樣線故障
            BMS02.Error[17] = bitStatus[1];         //從控連接線故障
            BMS02.Error[18] = bitStatus[2];         //從控LTC6803故障
            BMS02.Error[19] = bitStatus[3];         //從控電壓採樣故障
            BMS02.Error[20] = bitStatus[4];         //從控溫度採樣故障
            BMS02.Error[21] = bitStatus[5];         //從控溫度傳感器故障
            BMS02.Error[22] = bitStatus[6];         //從控極耳溫度故障
            BMS02.Error[23] = bitStatus[7];         //從控保留
            BMS02.Error[24] = bitStatus[8];         //從控均衡模塊故障
            BMS02.Error[25] = bitStatus[9];         //從控溫度採樣線故障
            BMS02.Error[26] = bitStatus[10];        //從控內網通信故障
            BMS02.Error[27] = bitStatus[11];        //從控EEPROM故障
            BMS02.Error[28] = bitStatus[12];        //從控初始化故障
            BMS02.Error[29] = bitStatus[13];
            BMS02.Error[30] = bitStatus[14];
            BMS02.Error[31] = bitStatus[15];
            //---------------------
            BMS02.BMS_Error_Warning = holding_register_Rank2_4[4];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.BMS_Error_Warning));     //把一ushort值存至點陣列中
            BMS02.Error[32] = bitStatus[0];        //電池即將充滿(單體過壓一級報警)
            BMS02.Error[33] = bitStatus[1];        //電池即將充滿(總電壓過壓一級報警)
            BMS02.Error[34] = bitStatus[2];        //充電過流一級報警
            BMS02.Error[35] = bitStatus[3];        //電池即將放空(單體欠壓一級報警)
            BMS02.Error[36] = bitStatus[4];        //電池即將放空(總電壓欠壓一級報警)
            BMS02.Error[37] = bitStatus[5];        //放電過流一級報警
            BMS02.Error[38] = bitStatus[6];        //充電溫度過高一級報警
            BMS02.Error[39] = bitStatus[7];        //充電溫度過低一級報警
            BMS02.Error[40] = bitStatus[8];        //SOC過低一級報警
            BMS02.Error[41] = bitStatus[9];        //溫度差異過大一級報警
            BMS02.Error[42] = bitStatus[10];       //極柱溫度過高一級報警
            BMS02.Error[43] = bitStatus[11];       //單體差異過大一級報警
            BMS02.Error[44] = bitStatus[12];       //絕緣過低一級報警
            BMS02.Error[45] = bitStatus[13];       //總壓差異過大一級報警
            BMS02.Error[46] = bitStatus[14];       //放電溫度過高一級報警
            BMS02.Error[47] = bitStatus[15];       //放電溫度過低一級報警
            //---------------------
            BMS02.BMS_Error_Stop = holding_register_Rank2_4[5];                     //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.BMS_Error_Stop));     //把一ushort值存至點陣列中
            BMS02.Error[48] = bitStatus[0];        //電池已充滿(單體過壓二級報警)
            BMS02.Error[49] = bitStatus[1];        //電池已充滿(總電壓過壓二級報警)
            BMS02.Error[50] = bitStatus[2];        //充電過流二級報警
            BMS02.Error[51] = bitStatus[3];        //電池已放空(單體欠壓二級報警)
            BMS02.Error[52] = bitStatus[4];        //電池已放空(總電壓欠壓二級報警)
            BMS02.Error[53] = bitStatus[5];        //放電過流二級報警
            BMS02.Error[54] = bitStatus[6];        //充電溫度過高二級報警
            BMS02.Error[55] = bitStatus[7];        //充電溫度過低二級報警
            BMS02.Error[56] = bitStatus[8];        //SOC過低二級報警
            BMS02.Error[57] = bitStatus[9];        //溫度差異過大二級報警
            BMS02.Error[58] = bitStatus[10];       //極柱溫度過高二級報警
            BMS02.Error[59] = bitStatus[11];       //單體差異過大二級報警
            BMS02.Error[60] = bitStatus[12];       //絕緣過低二級報警
            BMS02.Error[61] = bitStatus[13];       //總壓差異過大二級報警
            BMS02.Error[62] = bitStatus[14];       //放電溫度過高二級報警
            BMS02.Error[63] = bitStatus[15];       //放電溫度過低二級報警
            //---------------------
            BMS02.BMS_Error_Protection = holding_register_Rank2_4[6];               //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.BMS_Error_Protection));     //把一ushort值存至點陣列中
            BMS02.Error[64] = bitStatus[0];        //單體過壓三級報警
            BMS02.Error[65] = bitStatus[1];        //系統過壓三級報警
            BMS02.Error[66] = bitStatus[2];        //充電過流三級報警
            BMS02.Error[67] = bitStatus[3];        //單體欠壓三級報警
            BMS02.Error[68] = bitStatus[4];        //系統欠壓三級報警
            BMS02.Error[69] = bitStatus[5];        //放電過流三級報警
            BMS02.Error[70] = bitStatus[6];        //充電溫度過高三級報警
            BMS02.Error[71] = bitStatus[7];        //充電溫度過低三級報警
            BMS02.Error[72] = bitStatus[8];        //SOC過低三級報警
            BMS02.Error[73] = bitStatus[9];        //溫差過大三級報警
            BMS02.Error[74] = bitStatus[10];       //極柱溫度過高三級報警
            BMS02.Error[75] = bitStatus[11];       //單體差異過大三級報警
            BMS02.Error[76] = bitStatus[12];       //絕緣過低三級報警
            BMS02.Error[77] = bitStatus[13];       //總壓差異過大三級報警
            BMS02.Error[78] = bitStatus[14];       //放電溫度過高三級報警
            BMS02.Error[79] = bitStatus[15];       //放電溫度過低三級報警
            //---------------------
            BMS02.BMS_Other_Error = holding_register_Rank2_4[7];                    //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS02.BMS_Other_Error));     //把一ushort值存至點陣列中
            BMS02.Error[80] = bitStatus[0];        //與總控通信故障
            BMS02.Error[81] = bitStatus[1];        //與從控通信故障
            BMS02.Error[82] = bitStatus[2];
            BMS02.Error[83] = bitStatus[3];        //從控設備故障
            BMS02.Error[84] = bitStatus[4];
            BMS02.Error[85] = bitStatus[5];
            BMS02.Error[86] = bitStatus[6];
            BMS02.Error[87] = bitStatus[7];
            BMS02.Error[88] = bitStatus[8];
            BMS02.Error[89] = bitStatus[9];
            BMS02.Error[90] = bitStatus[10];
            BMS02.Error[91] = bitStatus[11];
            BMS02.Error[92] = bitStatus[12];
            BMS02.Error[93] = bitStatus[13];
            BMS02.Error[94] = bitStatus[14];
            BMS02.Error[95] = bitStatus[15];
            //===========================================================================

            //BMS Status
            BMS03.V = holding_register_Rank3_1[0];
            BMS03.V = BMS03.V * 0.1;
            //---------------------
            BMS03.A = holding_register_Rank3_1[1];
            BMS03.A = (Negative_num((ushort)BMS03.A)) * 0.1;
            //---------------------
            BMS03.BMS_ChargingDischarging_State = holding_register_Rank3_1[2];
            BMS03.SOC = holding_register_Rank3_1[3];
            //---------------------
            BMS03.SOH = holding_register_Rank3_1[4];
            BMS03.SOH = BMS03.SOH * 0.1;
            //---------------------
            BMS03.Cell_maxV_Index_Serial = holding_register_Rank3_1[5];
            //---------------------
            BMS03.Cell_maxV = holding_register_Rank3_1[6];
            BMS03.Cell_maxV = BMS03.Cell_maxV * 0.001;
            //---------------------
            BMS03.Cell_minV_Index_Serial = holding_register_Rank3_1[7];
            //---------------------
            BMS03.Cell_minV = holding_register_Rank3_1[8];
            BMS03.Cell_minV = BMS03.Cell_minV * 0.001;
            //---------------------
            BMS03.Cell_maxTemp_Index_Serial = holding_register_Rank3_1[9];
            //---------------------
            BMS03.Cell_maxTemp = holding_register_Rank3_1[10];
            BMS03.Cell_maxTemp = (Negative_num((ushort)BMS03.Cell_maxTemp)) * 0.1;
            //---------------------
            BMS03.Cell_minTemp_Index_Serial = holding_register_Rank3_1[11];
            //---------------------
            BMS03.Cell_minTemp = holding_register_Rank3_1[12];
            BMS03.Cell_minTemp = (Negative_num((ushort)BMS03.Cell_minTemp)) * 0.1;
            //---------------------
            BMS03.Cell_avgV = holding_register_Rank3_1[13];
            BMS03.Cell_avgV = BMS03.Cell_avgV * 0.001;
            //---------------------
            BMS03.Insulation = holding_register_Rank3_1[14];
            BMS03.Max_Current_Charging = holding_register_Rank3_1[15];
            BMS03.Max_Current_Discharging = holding_register_Rank3_1[16];
            BMS03.Insulation_Positive_Electrode = holding_register_Rank3_1[17];     //
            BMS03.Insulation_Negative_Electrode = holding_register_Rank3_1[18];     //
            // = holding_register_Rank3_1[19];                                        //備用
            //---------------------
            BMS03.Cell_avgTemp = holding_register_Rank3_1[20];
            BMS03.Cell_avgTemp = (Negative_num((ushort)BMS03.Cell_avgTemp)) * 0.1;
            //---------------------
            // = holding_register_Rank3_1[21];                                        //備用
            //---------------------------------------------
            /*
            //BMS V
            for (int i = 0; i < 125; i++)
            {
                BMS03.Cell_V[i] = holding_register_Rank3_2_1[i];
                BMS03.Cell_V[i] = BMS03.Cell_V[i] * 0.001;
            }
            for (int i = 0; i < 113; i++)
            {
                BMS03.Cell_V[i + 125] = holding_register_Rank3_2_2[i];
                BMS03.Cell_V[i + 125] = BMS03.Cell_V[i + 125] * 0.001;
            }
            //---------------------------------------------

            //BMS Temp
            for (int i = 0; i < 68; i++)
            {
                BMS03.Cell_Temp[i] = holding_register_Rank3_3[i];
                BMS03.Cell_Temp[i] = (Negative_num((ushort)BMS03.Cell_Temp[i])) * 0.1;
            }
            //---------------------------------------------
            */
            //BMS Contactors Status
            BMS03.Contactors_Status = holding_register_Rank3_4[0];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.Contactors_Status));     //把一ushort值存至點陣列中
            BMS03.Positive_Contactor = bitStatus[0];
            BMS03.Precharge_Contactor = bitStatus[1];
            //---------------------

            //BMS Run State
            BMS03.BMS_Run_State = holding_register_Rank3_4[1];                      //0：正常(系統可以放電，可以充電)
                                                                                    //1：充滿(系統可以放電，不能充電)
                                                                                    //2：放空(系統可以充電，不能放電)
                                                                                    //3：待機(系統不能充放，接觸器閉合)
                                                                                    //4：停機(系統不能充放，接觸器斷開)
                                                                                    //---------------------

            //BMS Error
            BMS03.BMS_Error = holding_register_Rank3_4[2];                          //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.BMS_Error));     //把一ushort值存至點陣列中
            BMS03.Error[0] = bitStatus[0];          //主控EEPROM故障
            BMS03.Error[1] = bitStatus[1];
            BMS03.Error[2] = bitStatus[2];
            BMS03.Error[3] = bitStatus[3];          //主控FLASH自檢狀態
            BMS03.Error[4] = bitStatus[4];          //主控RTC自檢狀態
            BMS03.Error[5] = bitStatus[5];
            BMS03.Error[6] = bitStatus[6];
            BMS03.Error[7] = bitStatus[7];
            BMS03.Error[8] = bitStatus[8];
            BMS03.Error[9] = bitStatus[9];
            BMS03.Error[10] = bitStatus[10];
            BMS03.Error[11] = bitStatus[11];
            BMS03.Error[12] = bitStatus[12];
            BMS03.Error[13] = bitStatus[13];
            BMS03.Error[14] = bitStatus[14];
            BMS03.Error[15] = bitStatus[15];
            //---------------------
            BMS03.Module_Error = holding_register_Rank3_4[3];                       //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.Module_Error));     //把一ushort值存至點陣列中
            BMS03.Error[16] = bitStatus[0];         //從控採樣線故障
            BMS03.Error[17] = bitStatus[1];         //從控連接線故障
            BMS03.Error[18] = bitStatus[2];         //從控LTC6803故障
            BMS03.Error[19] = bitStatus[3];         //從控電壓採樣故障
            BMS03.Error[20] = bitStatus[4];         //從控溫度採樣故障
            BMS03.Error[21] = bitStatus[5];         //從控溫度傳感器故障
            BMS03.Error[22] = bitStatus[6];         //從控極耳溫度故障
            BMS03.Error[23] = bitStatus[7];         //從控保留
            BMS03.Error[24] = bitStatus[8];         //從控均衡模塊故障
            BMS03.Error[25] = bitStatus[9];         //從控溫度採樣線故障
            BMS03.Error[26] = bitStatus[10];        //從控內網通信故障
            BMS03.Error[27] = bitStatus[11];        //從控EEPROM故障
            BMS03.Error[28] = bitStatus[12];        //從控初始化故障
            BMS03.Error[29] = bitStatus[13];
            BMS03.Error[30] = bitStatus[14];
            BMS03.Error[31] = bitStatus[15];
            //---------------------
            BMS03.BMS_Error_Warning = holding_register_Rank3_4[4];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.BMS_Error_Warning));     //把一ushort值存至點陣列中
            BMS03.Error[32] = bitStatus[0];        //電池即將充滿(單體過壓一級報警)
            BMS03.Error[33] = bitStatus[1];        //電池即將充滿(總電壓過壓一級報警)
            BMS03.Error[34] = bitStatus[2];        //充電過流一級報警
            BMS03.Error[35] = bitStatus[3];        //電池即將放空(單體欠壓一級報警)
            BMS03.Error[36] = bitStatus[4];        //電池即將放空(總電壓欠壓一級報警)
            BMS03.Error[37] = bitStatus[5];        //放電過流一級報警
            BMS03.Error[38] = bitStatus[6];        //充電溫度過高一級報警
            BMS03.Error[39] = bitStatus[7];        //充電溫度過低一級報警
            BMS03.Error[40] = bitStatus[8];        //SOC過低一級報警
            BMS03.Error[41] = bitStatus[9];        //溫度差異過大一級報警
            BMS03.Error[42] = bitStatus[10];       //極柱溫度過高一級報警
            BMS03.Error[43] = bitStatus[11];       //單體差異過大一級報警
            BMS03.Error[44] = bitStatus[12];       //絕緣過低一級報警
            BMS03.Error[45] = bitStatus[13];       //總壓差異過大一級報警
            BMS03.Error[46] = bitStatus[14];       //放電溫度過高一級報警
            BMS03.Error[47] = bitStatus[15];       //放電溫度過低一級報警
            //---------------------
            BMS03.BMS_Error_Stop = holding_register_Rank3_4[5];                     //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.BMS_Error_Stop));     //把一ushort值存至點陣列中
            BMS03.Error[48] = bitStatus[0];        //電池已充滿(單體過壓二級報警)
            BMS03.Error[49] = bitStatus[1];        //電池已充滿(總電壓過壓二級報警)
            BMS03.Error[50] = bitStatus[2];        //充電過流二級報警
            BMS03.Error[51] = bitStatus[3];        //電池已放空(單體欠壓二級報警)
            BMS03.Error[52] = bitStatus[4];        //電池已放空(總電壓欠壓二級報警)
            BMS03.Error[53] = bitStatus[5];        //放電過流二級報警
            BMS03.Error[54] = bitStatus[6];        //充電溫度過高二級報警
            BMS03.Error[55] = bitStatus[7];        //充電溫度過低二級報警
            BMS03.Error[56] = bitStatus[8];        //SOC過低二級報警
            BMS03.Error[57] = bitStatus[9];        //溫度差異過大二級報警
            BMS03.Error[58] = bitStatus[10];       //極柱溫度過高二級報警
            BMS03.Error[59] = bitStatus[11];       //單體差異過大二級報警
            BMS03.Error[60] = bitStatus[12];       //絕緣過低二級報警
            BMS03.Error[61] = bitStatus[13];       //總壓差異過大二級報警
            BMS03.Error[62] = bitStatus[14];       //放電溫度過高二級報警
            BMS03.Error[63] = bitStatus[15];       //放電溫度過低二級報警
            //---------------------
            BMS03.BMS_Error_Protection = holding_register_Rank3_4[6];               //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.BMS_Error_Protection));     //把一ushort值存至點陣列中
            BMS03.Error[64] = bitStatus[0];        //單體過壓三級報警
            BMS03.Error[65] = bitStatus[1];        //系統過壓三級報警
            BMS03.Error[66] = bitStatus[2];        //充電過流三級報警
            BMS03.Error[67] = bitStatus[3];        //單體欠壓三級報警
            BMS03.Error[68] = bitStatus[4];        //系統欠壓三級報警
            BMS03.Error[69] = bitStatus[5];        //放電過流三級報警
            BMS03.Error[70] = bitStatus[6];        //充電溫度過高三級報警
            BMS03.Error[71] = bitStatus[7];        //充電溫度過低三級報警
            BMS03.Error[72] = bitStatus[8];        //SOC過低三級報警
            BMS03.Error[73] = bitStatus[9];        //溫差過大三級報警
            BMS03.Error[74] = bitStatus[10];       //極柱溫度過高三級報警
            BMS03.Error[75] = bitStatus[11];       //單體差異過大三級報警
            BMS03.Error[76] = bitStatus[12];       //絕緣過低三級報警
            BMS03.Error[77] = bitStatus[13];       //總壓差異過大三級報警
            BMS03.Error[78] = bitStatus[14];       //放電溫度過高三級報警
            BMS03.Error[79] = bitStatus[15];       //放電溫度過低三級報警
            //---------------------
            BMS03.BMS_Other_Error = holding_register_Rank3_4[7];                    //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS03.BMS_Other_Error));     //把一ushort值存至點陣列中
            BMS03.Error[80] = bitStatus[0];        //與總控通信故障
            BMS03.Error[81] = bitStatus[1];        //與從控通信故障
            BMS03.Error[82] = bitStatus[2];
            BMS03.Error[83] = bitStatus[3];        //從控設備故障
            BMS03.Error[84] = bitStatus[4];
            BMS03.Error[85] = bitStatus[5];
            BMS03.Error[86] = bitStatus[6];
            BMS03.Error[87] = bitStatus[7];
            BMS03.Error[88] = bitStatus[8];
            BMS03.Error[89] = bitStatus[9];
            BMS03.Error[90] = bitStatus[10];
            BMS03.Error[91] = bitStatus[11];
            BMS03.Error[92] = bitStatus[12];
            BMS03.Error[93] = bitStatus[13];
            BMS03.Error[94] = bitStatus[14];
            BMS03.Error[95] = bitStatus[15];
            //===========================================================================

            //BMS Status
            BMS04.V = holding_register_Rank4_1[0];
            BMS04.V = BMS04.V * 0.1;
            //---------------------
            BMS04.A = holding_register_Rank4_1[1];
            BMS04.A = (Negative_num((ushort)BMS04.A)) * 0.1;
            //---------------------
            BMS04.BMS_ChargingDischarging_State = holding_register_Rank4_1[2];
            BMS04.SOC = holding_register_Rank4_1[3];
            //---------------------
            BMS04.SOH = holding_register_Rank4_1[4];
            BMS04.SOH = BMS04.SOH * 0.1;
            //---------------------
            BMS04.Cell_maxV_Index_Serial = holding_register_Rank4_1[5];
            //---------------------
            BMS04.Cell_maxV = holding_register_Rank4_1[6];
            BMS04.Cell_maxV = BMS04.Cell_maxV * 0.001;
            //---------------------
            BMS04.Cell_minV_Index_Serial = holding_register_Rank4_1[7];
            //---------------------
            BMS04.Cell_minV = holding_register_Rank4_1[8];
            BMS04.Cell_minV = BMS04.Cell_minV * 0.001;
            //---------------------
            BMS04.Cell_maxTemp_Index_Serial = holding_register_Rank4_1[9];
            //---------------------
            BMS04.Cell_maxTemp = holding_register_Rank4_1[10];
            BMS04.Cell_maxTemp = (Negative_num((ushort)BMS04.Cell_maxTemp)) * 0.1;
            //---------------------
            BMS04.Cell_minTemp_Index_Serial = holding_register_Rank4_1[11];
            //---------------------
            BMS04.Cell_minTemp = holding_register_Rank4_1[12];
            BMS04.Cell_minTemp = (Negative_num((ushort)BMS04.Cell_minTemp)) * 0.1;
            //---------------------
            BMS04.Cell_avgV = holding_register_Rank4_1[13];
            BMS04.Cell_avgV = BMS04.Cell_avgV * 0.001;
            //---------------------
            BMS04.Insulation = holding_register_Rank4_1[14];
            BMS04.Max_Current_Charging = holding_register_Rank4_1[15];
            BMS04.Max_Current_Discharging = holding_register_Rank4_1[16];
            BMS04.Insulation_Positive_Electrode = holding_register_Rank4_1[17];     //
            BMS04.Insulation_Negative_Electrode = holding_register_Rank4_1[18];     //
            // = holding_register_Rank4_1[19];                                        //備用
            //---------------------
            BMS04.Cell_avgTemp = holding_register_Rank4_1[20];
            BMS04.Cell_avgTemp = (Negative_num((ushort)BMS04.Cell_avgTemp)) * 0.1;
            //---------------------
            // = holding_register_Rank4_1[21];                                        //備用
            //---------------------------------------------
            /*
            //BMS V
            for (int i = 0; i < 125; i++)
            {
                BMS04.Cell_V[i] = holding_register_Rank4_2_1[i];
                BMS04.Cell_V[i] = BMS04.Cell_V[i] * 0.001;
            }
            for (int i = 0; i < 113; i++)
            {
                BMS04.Cell_V[i + 125] = holding_register_Rank4_2_2[i];
                BMS04.Cell_V[i + 125] = BMS04.Cell_V[i + 125] * 0.001;
            }
            //---------------------------------------------

            //BMS Temp
            for (int i = 0; i < 68; i++)
            {
                BMS04.Cell_Temp[i] = holding_register_Rank4_3[i];
                BMS04.Cell_Temp[i] = (Negative_num((ushort)BMS04.Cell_Temp[i])) * 0.1;
            }
            //---------------------------------------------
            */
            //BMS Contactors Status
            BMS04.Contactors_Status = holding_register_Rank4_4[0];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.Contactors_Status));     //把一ushort值存至點陣列中
            BMS04.Positive_Contactor = bitStatus[0];
            BMS04.Precharge_Contactor = bitStatus[1];
            //---------------------

            //BMS Run State
            BMS04.BMS_Run_State = holding_register_Rank4_4[1];                      //0：正常(系統可以放電，可以充電)
                                                                                    //1：充滿(系統可以放電，不能充電)
                                                                                    //2：放空(系統可以充電，不能放電)
                                                                                    //3：待機(系統不能充放，接觸器閉合)
                                                                                    //4：停機(系統不能充放，接觸器斷開)
                                                                                    //---------------------

            //BMS Error
            BMS04.BMS_Error = holding_register_Rank4_4[2];                          //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.BMS_Error));     //把一ushort值存至點陣列中
            BMS04.Error[0] = bitStatus[0];          //主控EEPROM故障
            BMS04.Error[1] = bitStatus[1];
            BMS04.Error[2] = bitStatus[2];
            BMS04.Error[3] = bitStatus[3];          //主控FLASH自檢狀態
            BMS04.Error[4] = bitStatus[4];          //主控RTC自檢狀態
            BMS04.Error[5] = bitStatus[5];
            BMS04.Error[6] = bitStatus[6];
            BMS04.Error[7] = bitStatus[7];
            BMS04.Error[8] = bitStatus[8];
            BMS04.Error[9] = bitStatus[9];
            BMS04.Error[10] = bitStatus[10];
            BMS04.Error[11] = bitStatus[11];
            BMS04.Error[12] = bitStatus[12];
            BMS04.Error[13] = bitStatus[13];
            BMS04.Error[14] = bitStatus[14];
            BMS04.Error[15] = bitStatus[15];
            //---------------------
            BMS04.Module_Error = holding_register_Rank4_4[3];                       //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.Module_Error));     //把一ushort值存至點陣列中
            BMS04.Error[16] = bitStatus[0];         //從控採樣線故障
            BMS04.Error[17] = bitStatus[1];         //從控連接線故障
            BMS04.Error[18] = bitStatus[2];         //從控LTC6803故障
            BMS04.Error[19] = bitStatus[3];         //從控電壓採樣故障
            BMS04.Error[20] = bitStatus[4];         //從控溫度採樣故障
            BMS04.Error[21] = bitStatus[5];         //從控溫度傳感器故障
            BMS04.Error[22] = bitStatus[6];         //從控極耳溫度故障
            BMS04.Error[23] = bitStatus[7];         //從控保留
            BMS04.Error[24] = bitStatus[8];         //從控均衡模塊故障
            BMS04.Error[25] = bitStatus[9];         //從控溫度採樣線故障
            BMS04.Error[26] = bitStatus[10];        //從控內網通信故障
            BMS04.Error[27] = bitStatus[11];        //從控EEPROM故障
            BMS04.Error[28] = bitStatus[12];        //從控初始化故障
            BMS04.Error[29] = bitStatus[13];
            BMS04.Error[30] = bitStatus[14];
            BMS04.Error[31] = bitStatus[15];
            //---------------------
            BMS04.BMS_Error_Warning = holding_register_Rank4_4[4];                  //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.BMS_Error_Warning));     //把一ushort值存至點陣列中
            BMS04.Error[32] = bitStatus[0];        //電池即將充滿(單體過壓一級報警)
            BMS04.Error[33] = bitStatus[1];        //電池即將充滿(總電壓過壓一級報警)
            BMS04.Error[34] = bitStatus[2];        //充電過流一級報警
            BMS04.Error[35] = bitStatus[3];        //電池即將放空(單體欠壓一級報警)
            BMS04.Error[36] = bitStatus[4];        //電池即將放空(總電壓欠壓一級報警)
            BMS04.Error[37] = bitStatus[5];        //放電過流一級報警
            BMS04.Error[38] = bitStatus[6];        //充電溫度過高一級報警
            BMS04.Error[39] = bitStatus[7];        //充電溫度過低一級報警
            BMS04.Error[40] = bitStatus[8];        //SOC過低一級報警
            BMS04.Error[41] = bitStatus[9];        //溫度差異過大一級報警
            BMS04.Error[42] = bitStatus[10];       //極柱溫度過高一級報警
            BMS04.Error[43] = bitStatus[11];       //單體差異過大一級報警
            BMS04.Error[44] = bitStatus[12];       //絕緣過低一級報警
            BMS04.Error[45] = bitStatus[13];       //總壓差異過大一級報警
            BMS04.Error[46] = bitStatus[14];       //放電溫度過高一級報警
            BMS04.Error[47] = bitStatus[15];       //放電溫度過低一級報警
            //---------------------
            BMS04.BMS_Error_Stop = holding_register_Rank4_4[5];                     //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.BMS_Error_Stop));     //把一ushort值存至點陣列中
            BMS04.Error[48] = bitStatus[0];        //電池已充滿(單體過壓二級報警)
            BMS04.Error[49] = bitStatus[1];        //電池已充滿(總電壓過壓二級報警)
            BMS04.Error[50] = bitStatus[2];        //充電過流二級報警
            BMS04.Error[51] = bitStatus[3];        //電池已放空(單體欠壓二級報警)
            BMS04.Error[52] = bitStatus[4];        //電池已放空(總電壓欠壓二級報警)
            BMS04.Error[53] = bitStatus[5];        //放電過流二級報警
            BMS04.Error[54] = bitStatus[6];        //充電溫度過高二級報警
            BMS04.Error[55] = bitStatus[7];        //充電溫度過低二級報警
            BMS04.Error[56] = bitStatus[8];        //SOC過低二級報警
            BMS04.Error[57] = bitStatus[9];        //溫度差異過大二級報警
            BMS04.Error[58] = bitStatus[10];       //極柱溫度過高二級報警
            BMS04.Error[59] = bitStatus[11];       //單體差異過大二級報警
            BMS04.Error[60] = bitStatus[12];       //絕緣過低二級報警
            BMS04.Error[61] = bitStatus[13];       //總壓差異過大二級報警
            BMS04.Error[62] = bitStatus[14];       //放電溫度過高二級報警
            BMS04.Error[63] = bitStatus[15];       //放電溫度過低二級報警
            //---------------------
            BMS04.BMS_Error_Protection = holding_register_Rank4_4[6];               //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.BMS_Error_Protection));     //把一ushort值存至點陣列中
            BMS04.Error[64] = bitStatus[0];        //單體過壓三級報警
            BMS04.Error[65] = bitStatus[1];        //系統過壓三級報警
            BMS04.Error[66] = bitStatus[2];        //充電過流三級報警
            BMS04.Error[67] = bitStatus[3];        //單體欠壓三級報警
            BMS04.Error[68] = bitStatus[4];        //系統欠壓三級報警
            BMS04.Error[69] = bitStatus[5];        //放電過流三級報警
            BMS04.Error[70] = bitStatus[6];        //充電溫度過高三級報警
            BMS04.Error[71] = bitStatus[7];        //充電溫度過低三級報警
            BMS04.Error[72] = bitStatus[8];        //SOC過低三級報警
            BMS04.Error[73] = bitStatus[9];        //溫差過大三級報警
            BMS04.Error[74] = bitStatus[10];       //極柱溫度過高三級報警
            BMS04.Error[75] = bitStatus[11];       //單體差異過大三級報警
            BMS04.Error[76] = bitStatus[12];       //絕緣過低三級報警
            BMS04.Error[77] = bitStatus[13];       //總壓差異過大三級報警
            BMS04.Error[78] = bitStatus[14];       //放電溫度過高三級報警
            BMS04.Error[79] = bitStatus[15];       //放電溫度過低三級報警
            //---------------------
            BMS04.BMS_Other_Error = holding_register_Rank4_4[7];                    //
            bitStatus = new System.Collections.BitArray(BitConverter.GetBytes(BMS04.BMS_Other_Error));     //把一ushort值存至點陣列中
            BMS04.Error[80] = bitStatus[0];        //與總控通信故障
            BMS04.Error[81] = bitStatus[1];        //與從控通信故障
            BMS04.Error[82] = bitStatus[2];
            BMS04.Error[83] = bitStatus[3];        //從控設備故障
            BMS04.Error[84] = bitStatus[4];
            BMS04.Error[85] = bitStatus[5];
            BMS04.Error[86] = bitStatus[6];
            BMS04.Error[87] = bitStatus[7];
            BMS04.Error[88] = bitStatus[8];
            BMS04.Error[89] = bitStatus[9];
            BMS04.Error[90] = bitStatus[10];
            BMS04.Error[91] = bitStatus[11];
            BMS04.Error[92] = bitStatus[12];
            BMS04.Error[93] = bitStatus[13];
            BMS04.Error[94] = bitStatus[14];
            BMS04.Error[95] = bitStatus[15];
            //===========================================================================
        }

        private double Negative_two_num(ushort num1, ushort num2)
        {
            double result = 0;
            if (num1 >= 32768)
            {
                result = (double)(num1 - 65535) * 65536 + (num2 - 65536);
            }
            else
            {
                result = num1 * 65536 + num2;
            }
            return result;
        }
        private double Negative_num(ushort num)
        {
            double result = 0;
            if (num >= 32768)
            {
                result = num - 65535;
            }
            else
            {
                result = num;
            }
            return result;
        }

    }
    public class TMM70
    {
        private bool read_or_not;
        private string device_ID = "";
        private double error_count = 0;
        private string communication_error = "";
        private double count = 0;
        private bool disconct_err = false;
        private bool ems_flag = false;
        private int ems_error = 0;
        private ushort[] holding_register = new ushort[68];
        private double f = 0;
        private double v_avg = 0;
        private double vl_avg = 0;
        private double i_avg = 0;
        private double i_n = 0;
        private double p_sum = 0;
        private double q_sum = 0;
        private double s_sum = 0;
        private double pf_avg = 0;
        private double v1 = 0;
        private double v2 = 0;
        private double v3 = 0;
        private double v12 = 0;
        private double v23 = 0;
        private double v31 = 0;
        private double i1 = 0;
        private double i2 = 0;
        private double i3 = 0;
        private double p1 = 0;
        private double p2 = 0;
        private double p3 = 0;
        private double q1 = 0;
        private double q2 = 0;
        private double q3 = 0;
        private double s1 = 0;
        private double s2 = 0;
        private double s3 = 0;
        private double pf1 = 0;
        private double pf2 = 0;
        private double pf3 = 0;
        private double kwh_in = 0;
        private double kwh_out = 0;
        private double kwh_total = 0;
        private double kwh_net = 0;
        private double kvarh_in = 0;
        private double kvarh_out = 0;
        private double kvarh_total = 0;
        private double kvarh_net = 0;
        private double thd_u12 = 0;
        private double thd_u23 = 0;
        private double thd_u31 = 0;
        private double thd_u_avg = 0;
        private double thd_i1 = 0;
        private double thd_i2 = 0;
        private double thd_i3 = 0;
        private double thd_i_avg = 0;




        public int Ems_error_Sec { get; set; } = 0;     ///補遺資料有哪些，0,1
        public bool Ems_flag_Sec { get; set; } = false;       //判斷是否缺值，須補遺,true,false
        public double Error_count { get { return error_count; } set { error_count = value; } }  ///判斷連續斷線次數
        public string Communication_error { get { return communication_error; } set { communication_error = value; } }
        public double Count { get { return count; } set { count = value; } }
        public bool Disconct_err { get { return disconct_err; } set { disconct_err = value; } }
        public bool Ems_flag { get { return ems_flag; } set { ems_flag = value; } }          //判斷是否缺值，須補遺,true,false
        public int Ems_error { get { return ems_error; } set { ems_error = value; } }        ///補遺資料有哪些，0,1
        public ushort[] Holding_register { get { return holding_register; } set { holding_register = value; } }
        public double F { get { return f; } set { f = value; } }
        public double V_avg { get { return v_avg; } set { v_avg = value; } }
        public double Vl_avg { get { return vl_avg; } set { vl_avg = value; } }
        public double I_avg { get { return i_avg; } set { i_avg = value; } }
        public double I_n { get { return i_n; } set { i_n = value; } }
        public double P_sum { get { return p_sum; } set { p_sum = value; } }
        public double Q_sum { get { return q_sum; } set { q_sum = value; } }
        public double S_sum { get { return s_sum; } set { s_sum = value; } }
        public double Pf_avg { get { return pf_avg; } set { pf_avg = value; } }
        public double V1 { get { return v1; } set { v1 = value; } }
        public double V2 { get { return v2; } set { v2 = value; } }
        public double V3 { get { return v3; } set { v3 = value; } }
        public double V12 { get { return v12; } set { v12 = value; } }
        public double V23 { get { return v23; } set { v23 = value; } }
        public double V31 { get { return v31; } set { v31 = value; } }
        public double I1 { get { return i1; } set { i1 = value; } }
        public double I2 { get { return i2; } set { i2 = value; } }
        public double I3 { get { return i3; } set { i3 = value; } }
        public double P1 { get { return p1; } set { p1 = value; } }
        public double P2 { get { return p2; } set { p2 = value; } }
        public double P3 { get { return p3; } set { p3 = value; } }
        public double Q1 { get { return q1; } set { q1 = value; } }
        public double Q2 { get { return q2; } set { q2 = value; } }
        public double Q3 { get { return q3; } set { q3 = value; } }
        public double S1 { get { return s1; } set { s1 = value; } }
        public double S2 { get { return s2; } set { s2 = value; } }
        public double S3 { get { return s3; } set { s3 = value; } }
        public double Pf1 { get { return pf1; } set { pf1 = value; } }
        public double Pf2 { get { return pf2; } set { pf2 = value; } }
        public double Pf3 { get { return pf3; } set { pf3 = value; } }
        public double Kwh_in { get { return kwh_in; } set { kwh_in = value; } }
        public double Kwh_out { get { return kwh_out; } set { kwh_out = value; } }
        public double Kwh_total { get { return kwh_total; } set { kwh_total = value; } }
        public double Kwh_net { get { return kwh_net; } set { kwh_net = value; } }
        public double Kvarh_in { get { return kvarh_in; } set { kvarh_in = value; } }
        public double Kvarh_out { get { return kvarh_out; } set { kvarh_out = value; } }
        public double Kvarh_total { get { return kvarh_total; } set { kvarh_total = value; } }
        public double Kvarh_net { get { return kvarh_net; } set { kvarh_net = value; } }

        public double Thd_u12 { get => thd_u12; set => thd_u12 = value; }
        public double Thd_u23 { get => thd_u23; set => thd_u23 = value; }
        public double Thd_u31 { get => thd_u31; set => thd_u31 = value; }
        public double Thd_u_avg { get => thd_u_avg; set => thd_u_avg = value; }
        public double Thd_i1 { get => thd_i1; set => thd_i1 = value; }
        public double Thd_i2 { get => thd_i2; set => thd_i2 = value; }
        public double Thd_i3 { get => thd_i3; set => thd_i3 = value; }
        public double Thd_i_avg { get => thd_i_avg; set => thd_i_avg = value; }
        public string Device_ID { get => device_ID; set => device_ID = value; }
        public bool Read_or_not { get => read_or_not; set => read_or_not = value; }

        private double Negative_two_num(ushort num1, ushort num2)
        {
            double result = 0;
            if (num1 >= 32768)
            {
                result = (double)(num1 - 65535) * 65536 + (num2 - 65536);
            }
            else
            {
                result = num1 * 65536 + num2;
            }
            return result;
        }
        private double Negative_num(ushort num)
        {
            double result = 0;
            if (num >= 32768)
            {
                result = num - 65535;
            }
            else
            {
                result = num;
            }
            return result;
        }
        public void Put_Data1()
        {
            f = holding_register[0] * 0.01;
            v1 = (holding_register[1] * 65536 + holding_register[2]) * 0.1;
            v2 = (holding_register[3] * 65536 + holding_register[4]) * 0.1;
            v3 = (holding_register[5] * 65536 + holding_register[6]) * 0.1;
            v_avg = (holding_register[7] * 65536 + holding_register[8]) * 0.1;
            v12 = (holding_register[9] * 65536 + holding_register[10]) * 0.1;
            v23 = (holding_register[11] * 65536 + holding_register[12]) * 0.1;
            v31 = (holding_register[13] * 65536 + holding_register[14]) * 0.1;
            vl_avg = (holding_register[15] * 65536 + holding_register[16]) * 0.1;
            i1 = Negative_two_num(holding_register[17], holding_register[18]) * 0.001;
            i2 = Negative_two_num(holding_register[19], holding_register[20]) * 0.001;
            i3 = Negative_two_num(holding_register[21], holding_register[22]) * 0.001;
            i_avg = Negative_two_num(holding_register[23], holding_register[24]) * 0.001;
            i_n = Negative_two_num(holding_register[25], holding_register[26]) * 0.001;
            p1 = Negative_two_num(holding_register[27], holding_register[28]);
            p2 = Negative_two_num(holding_register[29], holding_register[30]);
            p3 = Negative_two_num(holding_register[31], holding_register[32]);
            p_sum = Negative_two_num(holding_register[33], holding_register[34]);
            q1 = Negative_two_num(holding_register[35], holding_register[36]);
            q2 = Negative_two_num(holding_register[37], holding_register[38]);
            q3 = Negative_two_num(holding_register[39], holding_register[40]);
            q_sum = Negative_two_num(holding_register[41], holding_register[42]);
            s1 = Negative_two_num(holding_register[43], holding_register[44]);
            s2 = Negative_two_num(holding_register[45], holding_register[46]);
            s3 = Negative_two_num(holding_register[47], holding_register[48]);
            s_sum = Negative_two_num(holding_register[49], holding_register[50]);
            pf1 = Negative_num(holding_register[51]) * 0.001;
            pf2 = Negative_num(holding_register[52]) * 0.001;
            pf3 = Negative_num(holding_register[53]) * 0.001;
            pf_avg = Negative_num(holding_register[54]) * 0.001;
            kwh_in = Negative_two_num(holding_register[81], holding_register[82]) * 0.1;
            kwh_out = Negative_two_num(holding_register[83], holding_register[84]) * 0.1;
            kwh_total = Negative_two_num(holding_register[85], holding_register[86]) * 0.1;
            kwh_net = Negative_two_num(holding_register[87], holding_register[88]) * 0.1;
            kvarh_in = Negative_two_num(holding_register[89], holding_register[90]) * 0.1;
            kvarh_out = Negative_two_num(holding_register[91], holding_register[92]) * 0.1;
            kvarh_total = Negative_two_num(holding_register[93], holding_register[94]) * 0.1;
            kvarh_net = Negative_two_num(holding_register[95], holding_register[96]) * 0.1;
        }
        public void Put_Data2()
        {
            p_sum = Negative_two_num(holding_register[0], holding_register[1]);
            q_sum = Negative_two_num(holding_register[8], holding_register[9]);
            s_sum = Negative_two_num(holding_register[16], holding_register[17]);
        }
        public void Put_Data3()
        {
            Thd_u12 = holding_register[0] * 0.1;
            Thd_u23 = holding_register[1] * 0.1;
            Thd_u31 = holding_register[2] * 0.1;
            Thd_u_avg = holding_register[3] * 0.1;
            Thd_i1 = holding_register[4] * 0.1;
            Thd_i2 = holding_register[5] * 0.1;
            Thd_i3 = holding_register[6] * 0.1;
            Thd_i_avg = holding_register[7] * 0.1;
        }
        public void Put_Data4()
        {
            f = holding_register[0] * 0.01;
            v1 = (holding_register[1] * 65536 + holding_register[2]) * 0.1;
            v2 = (holding_register[3] * 65536 + holding_register[4]) * 0.1;
            v3 = (holding_register[5] * 65536 + holding_register[6]) * 0.1;
            v_avg = (holding_register[7] * 65536 + holding_register[8]) * 0.1;
            v12 = (holding_register[9] * 65536 + holding_register[10]) * 0.1;
            v23 = (holding_register[11] * 65536 + holding_register[12]) * 0.1;
            v31 = (holding_register[13] * 65536 + holding_register[14]) * 0.1;
            vl_avg = (holding_register[15] * 65536 + holding_register[16]) * 0.1;
            i1 = Negative_two_num(holding_register[17], holding_register[18]) * 0.001;
            i2 = Negative_two_num(holding_register[19], holding_register[20]) * 0.001;
            i3 = Negative_two_num(holding_register[21], holding_register[22]) * 0.001;
            i_avg = Negative_two_num(holding_register[23], holding_register[24]) * 0.001;
            i_n = Negative_two_num(holding_register[25], holding_register[26]) * 0.001;
            p1 = Negative_two_num(holding_register[27], holding_register[28]);
            p2 = Negative_two_num(holding_register[29], holding_register[30]);
            p3 = Negative_two_num(holding_register[31], holding_register[32]);
            p_sum = Negative_two_num(holding_register[33], holding_register[34]);
            q1 = Negative_two_num(holding_register[35], holding_register[36]);
            q2 = Negative_two_num(holding_register[37], holding_register[38]);
            q3 = Negative_two_num(holding_register[39], holding_register[40]);
            q_sum = Negative_two_num(holding_register[41], holding_register[42]);
            s1 = Negative_two_num(holding_register[43], holding_register[44]);
            s2 = Negative_two_num(holding_register[45], holding_register[46]);
            s3 = Negative_two_num(holding_register[47], holding_register[48]);
            s_sum = Negative_two_num(holding_register[49], holding_register[50]);
            pf1 = Negative_num(holding_register[51]) * 0.001;
            pf2 = Negative_num(holding_register[52]) * 0.001;
            pf3 = Negative_num(holding_register[53]) * 0.001;
            pf_avg = Negative_num(holding_register[54]) * 0.001;
        }
    }
    class Vq_Control
    {
        public static double v1_set = 0;          //頻率點F1
        public static double v2_set = 0;          //頻率點F2
        public static double v3_set = 0;          //頻率點F3
        public static double v4_set = 0;          //頻率點F4
        public static double v5_set = 0;          //頻率點F5
        public static double v6_set = 0;          //頻率點F6
        public static double q1_set = 0;          //功率點P1
        public static double q2_set = 0;          //功率點P2
        public static double q3_set = 0;          //功率點P3
        public static double q4_set = 0;         //功率點P4
        public static double q5_set = 0;         //功率點P5
        public static double q6_set = 0;         //功率點P6
        public static double q_base = 0;         //頻率實功基底值
        public static double v_base = 0;
        //public static bool FR_Enable = false;    ///頻率實功模式參數勾選
        public static double q_tr = 0;

        /////////////////////////////////////////
        public static double Hys_line = 1;
        public static double grid_v_last;
        public static double q_val_last;
        ///////測試用
        public static double test_flag = 0;

    }
    class Smooth
    {
        public static double p_variance = 0;    //平滑化功率變動限制
        //public static double pv_rate = 250;
        public static DateTime BaseTime;
        public static double pv_p_avg; //	每3秒平均輸出功率 
        public static double pv_rated = 1000;
        public static double count; //計數器 
        public static double p_variance_cal = 0;
        //////AC耦合

        public static double p_last; //上一次輸出功率
        public static double p_pv_max;
        public static double p_pv_min;

        ///////考慮逆送
        public static double p_limit;
        public static double p_limit_new;
        public static double ramp_down;
        public static double anti_pv_rated = 1000;
        public static double Flow_Back_p_variance_cal = 0; //沒有用到 
    }
    class Grid_Control
    {
        //grid
        public static double Grid_f = 0;
        public static double Grid_v = 0;
        public static int control_mode = 0;  ///目前模式:0:remote、1:loacal
        public static int schdule_mode = 0;  ///目前模式:0:pq、1:schdule
        //pcs
        public static double soc_max = 0;         //SOC最大值
        public static double soc_min = 0;         //SOC最小值
        public static double control_mode_p = 0;         //pq模式 的p
        public static double control_mode_q = 0;  //pq模式 的q

        // 穩定輸出模式需要的變數 
        public static bool mode_change = true; ///////模式是否更改
        public static double total_output = 0; //pv +ess  現在輸出功率 
        public static double p_diff = 0; //儲能系統輸出補償功率
        public static double PCS_p = 0;       ///////PCS輸出功率
        public static double fianl_ess_output = 0; //儲能系統輸出補償功率

        //平滑化
        public static double p_tr = 0;
        //各種功能合成的輸出 
        public static double AddUpEssPower = 0;
    }
    class control_mode
    {
        //輸入遲滯曲線旗標(=1) 輸出功率 上一次頻率 上一次p  基本p 電網頻率 ...12個設定點     回傳P_tr (輸出功率)
        public static void Pf_control(ref double Hys_line, ref double p_tr, ref double grid_f_last, ref double p_val_last, double bat_rate_p, double grid_f, double fset1, double fset2, double fset3, double fset4, double fset5, double fset6, double pset1, double pset2, double pset3, double pset4, double pset5, double pset6)
        {

            ///////1為橘線    0為藍線  2為區域
            double p_val = 0;
            //if (Grid_Control.flag != 5)
            /*if (Grid_Control.mode_change == true)
            {
                grid_f_last = grid_f;
                Grid_Control.flag = 5;
            }*/
            // Debug.Print($" {0}hz 最外層, grid_f");
            if (grid_f <= fset1)            ///////////////磁滯以外
            {
                p_val = pset1;
                grid_f_last = grid_f;
            }
            else if (grid_f <= fset6 && grid_f > fset1)
            {
                p_val = (grid_f - fset1) * (pset6 - pset1) / (fset6 - fset1) + pset1;
                grid_f_last = grid_f;
            }
            else if (grid_f >= fset3 && grid_f < fset4)
            {
                p_val = (grid_f - fset3) * (pset4 - pset3) / (fset4 - fset3) + pset3;
                grid_f_last = grid_f;
            }
            else if (grid_f >= fset4)
            {
                p_val = pset4;
                grid_f_last = grid_f;
            }
            /////////////////////////遲滯部分            
            else if (fset6 < grid_f && grid_f < fset3) /////電壓介於遲滯
            {
                if (grid_f >= grid_f_last)    ////電壓增加
                {
                    if (Hys_line == 1)       /////藍曲線
                    {
                        if (grid_f < fset2)                   ///////上邊界
                        {
                            p_val = pset2;
                        }
                        else                 ////右邊界
                        {
                            p_val = (grid_f - fset2) * (pset3 - pset2) / (fset3 - fset2) + pset2;
                            grid_f_last = grid_f;
                        }
                    }
                    else if (Hys_line == 0)  /////橘曲線
                    {
                        if (grid_f <= (p_val_last - pset2) * (fset3 - fset2) / (pset3 - pset2) + fset2)  /////遲滯保持不變
                        {
                            grid_f_last = grid_f;
                            p_val = p_val_last;
                        }
                        else                                                                           /////到達藍曲線
                        {
                            grid_f_last = grid_f;
                            p_val = (grid_f - fset2) * (pset3 - pset2) / (fset3 - fset2) + pset2;
                            Hys_line = 1;
                        }
                    }
                }

                else if (grid_f < grid_f_last)    ////f減少
                {
                    if (Hys_line == 0)  /////橘曲線
                    {
                        if (grid_f >= fset5)                   ///////上邊界
                        {
                            Debug.Print("f減少 {0}hz Hys_line == 0", grid_f);
                            p_val = pset5;
                        }
                        else                 ////右邊界
                        {
                            Debug.Print("f減少 {0}hz  Hys_line == 0 else", grid_f);
                            p_val = (grid_f - fset6) * (pset5 - pset6) / (fset5 - fset6) + pset6;
                            grid_f_last = grid_f;
                        }
                    }
                    else if (Hys_line == 1)  /////藍曲線
                    {
                        if (grid_f >= (p_val_last - pset5) * (fset6 - fset5) / (pset6 - pset5) + fset5)  /////遲滯保持不變                           
                        {
                            Debug.Print("f減少 {0}hz Hys_line == 1", grid_f);
                            p_val = p_val_last;
                        }
                        else                                                                           /////到達藍曲線
                        {
                            Debug.Print("f減少 {0}hz Hys_line == 1 else", grid_f);
                            p_val = (grid_f - fset6) * (pset5 - pset6) / (fset5 - fset6) + pset6;
                            Hys_line = 0;
                        }
                    }
                }
            }
            p_val_last = p_val;
            p_tr = bat_rate_p * p_val * 0.01;
            if (Vq_Control.q_tr > 50)
            {
                Vq_Control.q_tr = Vq_Control.q_tr - 50;
            }
            else if (Vq_Control.q_tr < -50)
            {
                Vq_Control.q_tr = Vq_Control.q_tr + 50;
            }
            else
            {
                Vq_Control.q_tr = 0;
            }

        }
        //  輸入遲滯曲線旗標(=1) 輸出q 上一次電壓 上一次q 基本q基本v  ...12個設定點   回傳q_tr 輸出q 
        public static void Vq_control(ref double Hys_line, ref double q_tr, ref double grid_v_last, ref double q_val_last, double bat_rate_q, double base_v, double grid_v, double vset1, double vset2, double vset3, double vset4, double vset5, double vset6, double qset1, double qset2, double qset3, double qset4, double qset5, double qset6)
        { /////需定義一開始為藍線和儲存Grid_v_last值
            //int Hys_line = 1;              ///////1為藍線    0為橘線  2為區域
            grid_v = grid_v * 100 / base_v;
            double q_val = 0;
            /////////似乎非必要??
            //////////////// 我把這裡除掉 也把這個輸入除掉 
            ////if (Grid_Control.flag != 6)
            /*if (Grid_Control.mode_change == true)
            {
                grid_v_last = grid_v;
                Grid_Control.flag = 6;

            }
            
            if (pq_flag == false)
            {
                if (p_diff > 50)
                {
                    p_diff = p_diff - 50;
                }
                else if (p_diff < -50)
                {
                    p_diff = p_diff + 50;
                }
                else
                {
                    p_diff = 0;
                }

            }*/
            Debug.Print("grid_v{0}v vset1{1}v", grid_v, vset1);
            if (grid_v <= vset1)            ///////////////磁滯以外
            {
                q_val = qset1;
                grid_v_last = grid_v;
            }
            else if (grid_v <= vset6 && grid_v > vset1)
            {
                q_val = (grid_v - vset1) * (qset6 - qset1) / (vset6 - vset1) + qset1;
                grid_v_last = grid_v;
            }
            else if (grid_v >= vset3 && grid_v < vset4)
            {
                q_val = (grid_v - vset3) * (qset4 - qset3) / (vset4 - vset3) + qset3;
                grid_v_last = grid_v;
            }
            else if (grid_v >= vset4)
            {
                q_val = qset4;
                grid_v_last = grid_v;
            }
            /////////////////////////遲滯部分            
            else if (vset6 < grid_v && grid_v < vset3) /////電壓介於遲滯
            {
                Debug.Print("進入遲滯");
                if (grid_v >= grid_v_last)    ////電壓增加
                {
                    if (Hys_line == 1)       /////藍曲線
                    {
                        if (grid_v < vset2)                   ///////上邊界
                        {
                            q_val = qset2;
                        }
                        else                 ////右邊界
                        {
                            q_val = (grid_v - vset2) * (qset3 - qset2) / (vset3 - vset2) + qset2;
                            grid_v_last = grid_v;
                        }
                    }
                    else if (Hys_line == 0)  /////橘曲線
                    {
                        if (grid_v <= (q_val_last - qset2) * (vset3 - vset2) / (qset3 - qset2) + vset2)  /////遲滯保持不變
                        {
                            q_val = q_val_last;
                        }
                        else                                                                           /////到達藍曲線
                        {
                            q_val = (grid_v - vset2) * (qset3 - qset2) / (vset3 - vset2) + qset2;
                            Hys_line = 1;
                        }
                    }
                }

                else if (grid_v < grid_v_last)    ////電壓減少
                {
                    if (Hys_line == 0)  /////橘曲線
                    {
                        if (grid_v >= vset5)                   ///////上邊界
                        {
                            q_val = qset5;
                        }
                        else                 ////右邊界
                        {
                            q_val = (grid_v - vset6) * (qset5 - qset6) / (vset5 - vset6) + qset6;
                            grid_v_last = grid_v;
                        }
                    }
                    else if (Hys_line == 1)  /////藍曲線
                    {
                        if (grid_v >= (q_val_last - qset5) * (vset6 - vset5) / (qset6 - qset5) + vset5)  /////遲滯保持不變                           
                        {
                            q_val = q_val_last;
                        }
                        else                                                                           /////到達藍曲線
                        {
                            q_val = (grid_v - vset6) * (qset5 - qset6) / (vset5 - vset6) + qset6;
                            Hys_line = 0;
                        }
                    }
                }
            }
            q_val_last = q_val;
            q_tr = bat_rate_q * q_val * 0.01;
            //grid_v_last = grid_v;

        }
        public static void Smoothing_mode(double p_pv, double bat_p, double p_variance, double p_soc_compenstation)
        {
            //bat_p = Grid_Control.p_diff;  ///////測試用途
            p_variance = p_variance * 0.01;
            DateTime time_now = DateTime.Now;
            #region 註解  改變基準功率  
            //if (p_pv + bat_p < 100)
            //{
            //    Smooth.pv_p_rate = 100;
            //}
            //else if (p_pv + bat_p >= 400)
            //{
            //    Smooth.pv_p_rate = 500;
            //}
            //else if (p_pv + bat_p >= 300)
            //{
            //    Smooth.pv_p_rate = 400;
            //}
            //else if (p_pv + bat_p >= 200)
            //{
            //    Smooth.pv_p_rate = 300;
            //}
            //else if (p_pv + bat_p >= 100)
            //{
            //    Smooth.pv_p_rate = 200;
            //}
            #endregion
            #region 初始化  執行時機 ::只有變動模式才會執行
            if (Grid_Control.mode_change == true)
            {

                //Grid_Control.meter_p_last = new double[2] { p_pv, p_pv };//沒有用到 
                //Smooth.BaseTime = DateTime.Now; //沒有用到 
                Smooth.pv_p_avg = 0;            //平均功率 
                Smooth.p_last = p_pv + bat_p;   //初始條件，最一開始的目標值 
                //Smooth.pv_rated = Smooth.p_last;  //////若基底為目前平均功率
                Smooth.p_pv_max = Smooth.p_last + p_variance * Smooth.pv_rated * 0.1;//功率變動率10%當作上緩衝區
                Smooth.p_pv_min = Smooth.p_last - p_variance * Smooth.pv_rated * 0.1;//功率變動率10%當作下緩衝區
                Grid_Control.p_tr = Smooth.p_last; //設定目標值 
            }
            //Smooth.pv_p_rate = p_pv + bat_p;   //////若基底為目前平均功率 當此值為負數，將會異常
            #endregion

            #region 正常功能區  每3秒會執行一次  代表每次可以變動 3秒功率變動率的8成(剩下20%拿去做緩衝區 )
            if (Smooth.count >= 3)
            {
                Smooth.pv_p_avg = Smooth.pv_p_avg / 3 + p_soc_compenstation; // 計算前3秒PV輸出平均功率
                Smooth.count = 0;
                //Smooth.BaseTime = DateTime.Now;
                #region 過去3秒假如平均功率超過變動量的上下限制 ，就要把目標值設定在上/下限制 沒有超過 目標值就等於平均功率 
                if (Smooth.pv_p_avg - Smooth.p_last > p_variance * Smooth.pv_rated * 0.04) //////每3秒平均值變動範圍為  (80%功率變動率) /20 = (20%*80% )/20  =0.04
                {
                    Smooth.p_last = Smooth.p_last + p_variance * Smooth.pv_rated * 0.04;
                }
                else if (Smooth.pv_p_avg - Smooth.p_last < -p_variance * Smooth.pv_rated * 0.04)
                {
                    Smooth.p_last = Smooth.p_last - p_variance * Smooth.pv_rated * 0.04;
                }
                else //假如功率變動很小 
                {
                    Smooth.p_last = Smooth.pv_p_avg;
                }
                #endregion
                #region 功率變動率的10% 當作  功率變動緩衝區 減少pcs輸出改變次數 
                Smooth.p_pv_max = Smooth.p_last + p_variance * Smooth.pv_rated * 0.1;
                Smooth.p_pv_min = Smooth.p_last - p_variance * Smooth.pv_rated * 0.1;
                Smooth.pv_p_avg = 0;
                #endregion
            }

            #region soc補償量 計算最終的輸出功率  假如計算出來的結果 超出限制值 就不補償 
            double p_pv_soc = p_pv + p_soc_compenstation;
            if (p_pv_soc > Smooth.p_pv_max)//&& 計算出來結果 假如大於  最大功率 就限制在最大功率 
            {
                Grid_Control.p_tr = Smooth.p_pv_max;
            }
            else if (p_pv_soc < Smooth.p_pv_min)// && 計算出來結果 假如小於  最小功率 就限制在最小功率 
            {
                Grid_Control.p_tr = Smooth.p_pv_min;
            }
            else
            {
                Grid_Control.p_tr = p_pv_soc; //假如沒有超過上下限制值 就輸出
            }
            #endregion
            #endregion
            #region 每秒鐘會執行一次 累積計算PV輸出功率  計算計數器 
            //p_tr = SOC_limit(Grid_Control.soc_max, Grid_Control.soc_min, Grid_Control.soc_now, p_tr);
            Grid_Control.p_diff = Grid_Control.p_tr - p_pv; //儲能系統輸出功率 =目標功率- pv輸出功率 
            Smooth.pv_p_avg = Smooth.pv_p_avg + p_pv;
            Smooth.count = Smooth.count + 1;
            #region 假如有輸出虛功應該要慢慢的歸零 
            #endregion

            #endregion
        }

    }
}
