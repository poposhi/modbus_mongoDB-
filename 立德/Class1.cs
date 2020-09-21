using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;//debug msg在『輸出』視窗觀看

namespace modbus_mongoDB建立
{
    class Class1
    {
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
