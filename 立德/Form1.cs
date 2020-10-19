///這個程式主要是用來快速建立一個電網控制器 ，能夠用文字編輯器設定 是否開啟，本地遠端資料庫，pcs modbus設定，多少秒讀取一次 各項變數是第幾個資料 
///
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#region 常用
using System.Diagnostics;//debug msg在『輸出』視窗觀看
using System.Threading;  // 可以讓整個執行緒停止  Thread.Sleep(2000);
using ThreadingTimer = System.Threading.Timer;  //可以開一個平行緒計算時間
using System.IO; // 讀取寫入文字檔 

#endregion
#region Modbus
using Modbus;
using Modbus.Device;
using Modbus.Data;
using Modbus.Message;
using System.IO.Ports;  //for serial port

#endregion
// 資料庫連線設定 讀取一個collection 並且上傳到自己的資料庫 
#region mongodb 1
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
#endregion

namespace modbus_mongoDB建立
{
    public partial class Form1 : Form
    {
        //使用一些旗標變數 來建立Modbus slave master 
        #region 各種變數 旗標 
        string local_db_name = "solar"; //本地資料庫名稱 
        string remote_db_name = "solar";
        //遠端資料庫IP 
        public PCS PCS1 = new PCS();
        public MBMS MBMS1 = new MBMS();
        int time_offset = 8;
        #endregion
        #region Modbus 2 建立相關物件 
        //照理來說立德計劃應該只需要pcs還有bms 還有電表 
        SerialPort serialPort_dg = new SerialPort(), serialPort_pv1 = new SerialPort(), serialPort_pcs = new SerialPort(), serialPort_load = new SerialPort();
        ModbusSerialMaster master_dg;
        ModbusSerialMaster master_pv1;
        ModbusSerialMaster master_pcs;
        ModbusSerialMaster master_load;
        string port_pcs = "pcs_com";
        string port_pv = "pv_com";
        string port_dg = "DG_com";
        string port_load = "load_com";
        #endregion
        #region MongoDB2 宣告變數 
        private MongoClient dbconn;
        public static IMongoDatabase local_db;//為了要讓其他的物件也可以使用 
        private MongoClient ems_dbconn;
        private IMongoDatabase ems_db;
        //private string mlabconn = "mongodb://localhost:27017/?wtimeoutMS=200";  //mlab提供的連線字串 
        //private string mlabconn = "mongodb://tsai_user:0000@localhost:27017";
        #endregion
        #region 地址 
        ushort bat_v_address = 3141;//電池電壓 
        ushort ac_v_address = 3113;//電池電流
        ushort commend_mode = 3400;//工作模式  1 充電 2 放電 
        ushort commend_charge_i = 3402;//充電電流指令  
        ushort commend_discharge_i = 3404;//放電電流指令  

        #endregion
        public Form1()
        {
            InitializeComponent();
            InitialLv();
            #region MongoDB3 連線建立 連線設定

            ////Local端，MongoDB連線Timeout設定
            MongoClientSettings settings = new MongoClientSettings();
            settings.WaitQueueSize = int.MaxValue;
            settings.ConnectTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.SocketTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.WaitQueueTimeout = new TimeSpan(0, 0, 0, 0, 0100);
            //settings.Server = new MongoServerAddress("localhost");
            settings.Server = new MongoServerAddress("localhost");
            this.dbconn = new MongoClient(settings);
            local_db = dbconn.GetDatabase(local_db_name);  //資料庫名稱 


            ////Server端，MongoDB連線Timeout設定
            MongoIdentity identity = new MongoInternalIdentity("admin", "root"); // 資料庫名稱  使用者名稱
             MongoIdentityEvidence evidence = new PasswordEvidence("pc152");
            MongoClientSettings esettings = new MongoClientSettings();
            esettings.WaitQueueSize = int.MaxValue;
            esettings.ConnectTimeout = new TimeSpan(0, 0, 0, 0, 200);
            esettings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 0, 200);
            esettings.SocketTimeout = new TimeSpan(0, 0, 0, 0, 200);
            esettings.WaitQueueTimeout = new TimeSpan(0, 0, 0, 0, 200);
            //esettings.Server = new MongoServerAddress("140.118.172.75");        //EMS(ntust)
            //esettings.Server = new MongoServerAddress("140.118.172.154");       //MGC(ntust)
            esettings.Server = new MongoServerAddress("140.118.207.49");         
            //esettings.Server = new MongoServerAddress("192.168.1.8");           //EMS
            esettings.Credential = new MongoCredential(null, identity, evidence);
            this.ems_dbconn = new MongoClient(esettings);
            this.ems_db = ems_dbconn.GetDatabase("solar");  //資料庫名稱 

            #endregion
            #region Modbus 3 rtu 連線
            try //開啟pcs串列 
            { set_serial(ref serialPort_pcs, ref master_pcs, port_pcs, 9600); }
            catch (Exception e)

            { Debug.Print(e.Message); }

            #endregion
            timertrickAtBeginning.Enabled = true;
        }
        #region mongodb 使用
        //讀取整個collection，並且上傳到本地端 並沒有完成 
        private void read_all_coll()
        {
            DateTime time_now = DateTime.Now;

            //讀取資料庫 
            var coll = ems_db.GetCollection<BsonDocument>("equipment");  //指定寫入給"categories"此collection  
            var sort = Builders<BsonDocument>.Sort.Descending("time");
            //var cursor = coll.Find(new BsonDocument()).Sort(sort).Limit(1).ToList();
            var cursor = coll.Find(new BsonDocument());
            var cursor_list = cursor.ToList();
            foreach (var item in cursor_list)
            {
                Debug.Print(item.ToString());
            }
            var coll_local = local_db.GetCollection<BsonDocument>("equipment");  //指定寫入給"categories"此collection  
            coll_local.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset) } });
        }
        //測試是否可以上傳資料 
        private void mongo_test(DateTime time_now, string coll_name)
        {
            int time_offset = 8;
            var coll = local_db.GetCollection<BsonDocument>(coll_name);  //指定寫入給"categories"此collection  
            coll.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset) } });
        }
        //
        private void Mongo_PCS(IMongoDatabase db, PCS Device, DateTime time_now)
        {
            //////{"Error", new BsonArray {new BsonDocument("Error1", PCS1.Error1),new BsonDocument("Error2", PCS1.Error2),new BsonDocument("Error3", PCS1.Error3),new BsonDocument("Error4", PCS1.Error4)}}
            // ushort[] a = new ushort[3] { PCS1.Error1, PCS1.Error2, PCS1.Error3 };
            try
            {
                var coll = db.GetCollection<BsonDocument>("pcs");  //指定寫入給"categories"此collection  
                coll.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset)}, { "ID", Device.Device_ID} ,{ "Error1", Device.Error1 },{ "Error2", Device.Error2 },{ "Error3", Device.Error3 },{"Error4", Device.Error4}
                ,{"v_grid1",Device.V_grid1},{"v_grid2",Device.V_grid2},{"v_grid3",Device.V_grid3},{"v_grid_per",(Device.V_grid1+Device.V_grid2+Device.V_grid3)/grid.v_base },{"f", Device.F_grid}
                ,{"v_out1",Device.V_out1},{"v_out2",Device.V_out2},{"v_out3",Device.V_out3},{"i_out1",Device.I_out1},{"i_out2",Device.I_out2},{"i_out3",Device.I_out3},{"f_offgrid",Device.F_offgrid}
                ,{"f_grid",Device.F_grid},{"i_n",Device.I_n},{"temp_inner",Device.Temp_inner},{"temp_sink",Device.Temp_sink},{"v_dc",Device.V_dc},{"i_dc",Device.I_dc},{"p_dc",Device.P_dc}
                ,{"s_sum",Device.S_sum},{"p_sum",Device.P_sum},{"p_sum_per",Device.P_sum/grid.S_rated },{"q_sum",Device.Q_sum},{"q_sum_per",Device.Q_sum/grid.S_rated },{"s_out1",Device.S_out1},{"p_out1",Device.P_out1},{"pf_out1",Device.Pf_out1}
                ,{"s_out2",Device.S_out2},{"p_out2",Device.P_out2},{"pf_out2",Device.Pf_out2},{"s_out3",Device.S_out3},{"p_out3",Device.P_out3},{"pf_out3",Device.Pf_out3}
                ,{"kwh_chg",Device.Kwh_chg},{"kwh_dischg",Device.Kwh_dischg},{"status_operation",Device.Status_operation},{"status_grid",Device.Status_grid},

            });
                Device.Ems_error = 0;
            }
            catch (Exception e)
            {
                Device.Ems_error = 1;
                Device.Ems_flag = true;
                Console.WriteLine("EPCS Error");

                Debug.Print(e.Message);
            }
        }

        private void Mongo_MBMS(IMongoDatabase db, MBMS Device, DateTime time_now)
        {
            //////{"Error", new BsonArray {new BsonDocument("Error1", PCS.Error1),new BsonDocument("Error2", PCS.Error2),new BsonDocument("Error3", PCS.Error3),new BsonDocument("Error4", PCS.Error4)}}
            //ushort[] a = new ushort[3] { Device.Error1, Device.Error2, Device.Error3 };
            try
            {
                var coll = db.GetCollection<BsonDocument>("mbms");  //指定寫入給"categories"此collection  
                coll.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset)}, { "ID", Device.Device_ID},
                    { "Read_or_not", Device.Read_or_not },

//家彰新增 
//正負不同
{"Positive_Contactor" ,new BsonArray {Device.BMS01.RealyStatus_PositiveRelay, Device.BMS02.RealyStatus_PositiveRelay, Device.BMS03.RealyStatus_PositiveRelay, Device.BMS04.RealyStatus_PositiveRelay, Device.BMS05.RealyStatus_PositiveRelay, Device.BMS06.RealyStatus_PositiveRelay } },
//{ "Negative_Contactor" ,new BsonArray {Device.BMS01.Negative_Contactor, Device.BMS02.Negative_Contactor, Device.BMS03.Negative_Contactor, Device.BMS04.Negative_Contactor } },//EMS(CATL沒有)
                    { "System_MaximumPermissibleCurrent_Charging", Device.System_MaximumPermissibleCurrent_Charging },//EMS
                    { "System_MaximumPermissibleCurrent_Discharging", Device.System_MaximumPermissibleCurrent_Discharging },//EMS
                    //BMS只能取其一(有量科技 or CATL)，沒用到必須"//"
                    #region 有量科技
                    { "System_V", Device.System_V },
                    { "System_A", Device.System_A },
                    { "System_P", Device.System_P },
                    { "System_SOC", Device.SOC },{ "System_SOH", Device.System_SOH },
                    { "RealyStatus", Device.RealyStatus },
                    { "RealyStatus_NegativeRelay", Device.RealyStatus_NegativeRelay },
                    { "RealyStatus_PositiveRelay", Device.RealyStatus_PositiveRelay },
                    { "WorkStatus", Device.WorkStatus },
                    { "Work_DischargingCharging", Device.Work_DischargingCharging },
                    { "TargetStatus", Device.TargetStatus },
                    { "Target_DischargingCharging", Device.Target_DischargingCharging },
                    { "Cell_maxV", Device.Cell_maxV },
                    { "Cell_minV", Device.Cell_minV },
                    { "Cell_maxTemp", Device.Cell_maxTemp },
                    { "Cell_minTemp", Device.Cell_minTemp },

                    {"BMS_V" ,new BsonArray {Device.BMS01.V, Device.BMS02.V, Device.BMS03.V, Device.BMS04.V, Device.BMS05.V, Device.BMS06.V } },
                    {"BMS_A" ,new BsonArray {Device.BMS01.A, Device.BMS02.A, Device.BMS03.A, Device.BMS04.A, Device.BMS05.A, Device.BMS06.A } },
                    {"BMS_SOC" ,new BsonArray {Device.BMS01.SOC, Device.BMS02.SOC, Device.BMS03.SOC, Device.BMS04.SOC, Device.BMS05.SOC, Device.BMS06.SOC } },
                    {"BMS_RealyStatus" ,new BsonArray {Device.BMS01.RealyStatus, Device.BMS02.RealyStatus, Device.BMS03.RealyStatus, Device.BMS04.RealyStatus, Device.BMS05.RealyStatus, Device.BMS06.RealyStatus } },
                    {"BMS_RealyStatus_NegativeRelay" ,new BsonArray {Device.BMS01.RealyStatus_NegativeRelay, Device.BMS02.RealyStatus_NegativeRelay, Device.BMS03.RealyStatus_NegativeRelay, Device.BMS04.RealyStatus_NegativeRelay, Device.BMS05.RealyStatus_NegativeRelay, Device.BMS06.RealyStatus_NegativeRelay } },
                    {"BMS_RealyStatus_PositiveRelay" ,new BsonArray {Device.BMS01.RealyStatus_PositiveRelay, Device.BMS02.RealyStatus_PositiveRelay, Device.BMS03.RealyStatus_PositiveRelay, Device.BMS04.RealyStatus_PositiveRelay, Device.BMS05.RealyStatus_PositiveRelay, Device.BMS06.RealyStatus_PositiveRelay } },
                    {"BMS_WorkStatus" ,new BsonArray {Device.BMS01.WorkStatus, Device.BMS02.WorkStatus, Device.BMS03.WorkStatus, Device.BMS04.WorkStatus, Device.BMS05.WorkStatus, Device.BMS06.WorkStatus } },
                    {"BMS_Work_DischargingCharging" ,new BsonArray {Device.BMS01.Work_DischargingCharging, Device.BMS02.Work_DischargingCharging, Device.BMS03.Work_DischargingCharging, Device.BMS04.Work_DischargingCharging, Device.BMS05.Work_DischargingCharging, Device.BMS06.Work_DischargingCharging } },
                    {"BMS_TargetStatus" ,new BsonArray {Device.BMS01.TargetStatus, Device.BMS02.TargetStatus, Device.BMS03.TargetStatus, Device.BMS04.TargetStatus, Device.BMS05.TargetStatus, Device.BMS06.TargetStatus } },
                    {"BMS_Target_DischargingCharging" ,new BsonArray {Device.BMS01.Target_DischargingCharging, Device.BMS02.Target_DischargingCharging, Device.BMS03.Target_DischargingCharging, Device.BMS04.Target_DischargingCharging, Device.BMS05.Target_DischargingCharging, Device.BMS06.Target_DischargingCharging } },
                    {"BMS_Cell_maxV" ,new BsonArray {Device.BMS01.Cell_maxV, Device.BMS02.Cell_maxV, Device.BMS03.Cell_maxV, Device.BMS04.Cell_maxV, Device.BMS05.Cell_maxV, Device.BMS06.Cell_maxV } },
                    {"BMS_Cell_minV" ,new BsonArray {Device.BMS01.Cell_minV, Device.BMS02.Cell_minV, Device.BMS03.Cell_minV, Device.BMS04.Cell_minV, Device.BMS05.Cell_minV, Device.BMS06.Cell_minV } },
                    {"BMS_Cell_maxTemp" ,new BsonArray {Device.BMS01.Cell_maxTemp, Device.BMS02.Cell_maxTemp, Device.BMS03.Cell_maxTemp, Device.BMS04.Cell_maxTemp, Device.BMS05.Cell_maxTemp, Device.BMS06.Cell_maxTemp } },
                    {"BMS_Cell_minTemp" ,new BsonArray {Device.BMS01.Cell_minTemp, Device.BMS02.Cell_minTemp, Device.BMS03.Cell_minTemp, Device.BMS04.Cell_minTemp, Device.BMS05.Cell_minTemp, Device.BMS06.Cell_minTemp } },
                    {"BMS_Fault_Falg_V" ,new BsonArray {Device.BMS01.Fault_Falg_V, Device.BMS02.Fault_Falg_V, Device.BMS03.Fault_Falg_V, Device.BMS04.Fault_Falg_V, Device.BMS05.Fault_Falg_V, Device.BMS06.Fault_Falg_V } },
                    {"BMS_Fault_Falg_A" ,new BsonArray {Device.BMS01.Fault_Falg_A, Device.BMS02.Fault_Falg_A, Device.BMS03.Fault_Falg_A, Device.BMS04.Fault_Falg_A, Device.BMS05.Fault_Falg_A, Device.BMS06.Fault_Falg_A } },
                    {"BMS_Fault_Falg_Temp" ,new BsonArray {Device.BMS01.Fault_Falg_Temp, Device.BMS02.Fault_Falg_Temp, Device.BMS03.Fault_Falg_Temp, Device.BMS04.Fault_Falg_Temp, Device.BMS05.Fault_Falg_Temp, Device.BMS06.Fault_Falg_Temp } },
                    {"BMS_Fault_Falg_V_LastTime" ,new BsonArray {Device.BMS01.Fault_Falg_V_LastTime, Device.BMS02.Fault_Falg_V_LastTime, Device.BMS03.Fault_Falg_V_LastTime, Device.BMS04.Fault_Falg_V_LastTime, Device.BMS05.Fault_Falg_V_LastTime, Device.BMS06.Fault_Falg_V_LastTime } },
                    {"BMS_Fault_Falg_A_LastTime" ,new BsonArray {Device.BMS01.Fault_Falg_A_LastTime, Device.BMS02.Fault_Falg_A_LastTime, Device.BMS03.Fault_Falg_A_LastTime, Device.BMS04.Fault_Falg_A_LastTime, Device.BMS05.Fault_Falg_A_LastTime, Device.BMS06.Fault_Falg_A_LastTime } },
                    {"BMS_Fault_Falg_Temp_LastTime" ,new BsonArray {Device.BMS01.Fault_Falg_Temp_LastTime, Device.BMS02.Fault_Falg_Temp_LastTime, Device.BMS03.Fault_Falg_Temp_LastTime, Device.BMS04.Fault_Falg_Temp_LastTime, Device.BMS05.Fault_Falg_Temp_LastTime, Device.BMS06.Fault_Falg_Temp_LastTime } },
                    #endregion

                    #region CATL
                    //{ "System_Status", Device.System_Status },
                    //{ "Charging_maxA", Device.Charging_maxA },
                    //{ "Discharging_maxA", Device.Discharging_maxA },
                    //{ "System_V", Device.System_V },
                    //{ "System_A", Device.System_A },
                    //{ "SOC", Device.SOC },
                    //{"Sub_RealyStatus" ,new BsonArray {Device.Sub01.RealyStatus, Device.Sub02.RealyStatus, Device.Sub03.RealyStatus, Device.Sub04.RealyStatus, Device.Sub05.RealyStatus, Device.Sub06.RealyStatus, Device.Sub07.RealyStatus, Device.Sub08.RealyStatus, Device.Sub09.RealyStatus, Device.Sub10.RealyStatus, Device.Sub11.RealyStatus, Device.Sub12.RealyStatus, Device.Sub13.RealyStatus, Device.Sub14.RealyStatus, Device.Sub15.RealyStatus, Device.Sub16.RealyStatus, Device.Sub17.RealyStatus, Device.Sub18.RealyStatus, Device.Sub19.RealyStatus, Device.Sub20.RealyStatus } },
                    //{"Sub_SOC" ,new BsonArray {Device.Sub01.SOC, Device.Sub02.SOC, Device.Sub03.SOC, Device.Sub04.SOC, Device.Sub05.SOC, Device.Sub06.SOC, Device.Sub07.SOC, Device.Sub08.SOC, Device.Sub09.SOC, Device.Sub10.SOC, Device.Sub11.SOC, Device.Sub12.SOC, Device.Sub13.SOC, Device.Sub14.SOC, Device.Sub15.SOC, Device.Sub16.SOC, Device.Sub17.SOC, Device.Sub18.SOC, Device.Sub19.SOC, Device.Sub20.SOC } },
                    //{"Sub_A" ,new BsonArray {Device.Sub01.A, Device.Sub02.A, Device.Sub03.A, Device.Sub04.A, Device.Sub05.A, Device.Sub06.A, Device.Sub07.A, Device.Sub08.A, Device.Sub09.A, Device.Sub10.A, Device.Sub11.A, Device.Sub12.A, Device.Sub13.A, Device.Sub14.A, Device.Sub15.A, Device.Sub16.A, Device.Sub17.A, Device.Sub18.A, Device.Sub19.A, Device.Sub20.A } },
                    #endregion
                 { "EMS_Error",Device.Ems_error},{"EMS_Error1",Device.Ems_error}
            });
            }
            catch (Exception ex)
            {
                Console.WriteLine("MBMS Error");
                Console.WriteLine(ex);
            }
        }
        private void Mongo_MBMS_CATL(IMongoDatabase db, MBMS_CATL Device, DateTime time_now)
        {
            //////{"Error", new BsonArray {new BsonDocument("Error1", PCS.Error1),new BsonDocument("Error2", PCS.Error2),new BsonDocument("Error3", PCS.Error3),new BsonDocument("Error4", PCS.Error4)}}
            //ushort[] a = new ushort[3] { Device.Error1, Device.Error2, Device.Error3 };
            try
            {
                var coll = db.GetCollection<BsonDocument>("mbms");  //指定寫入給"categories"此collection  
                coll.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset)}, { "ID", Device.Device_ID},
                    { "Read_or_not", Device.Read_or_not },

                    { "System_One_Key_Parallel_Switch", Device.System_One_Key_Parallel_Switch },
                    { "System_Cluster_1_is_use", Device.System_Cluster_1_is_use },
                    { "System_Cluster_2_is_use", Device.System_Cluster_2_is_use },
                    { "System_Cluster_3_is_use", Device.System_Cluster_3_is_use },
                    { "System_Cluster_4_is_use", Device.System_Cluster_4_is_use },

                    { "System_ChargingDischarging_State", Device.System_ChargingDischarging_State },
                    { "System_A", Device.System_A },//EMS
                    { "System_Spare1", Device.System_Spare1 },
                    { "System_SOC", Device.System_SOC },//EMS
                    { "System_Run_State", Device.System_Run_State },//EMS
                    { "System_V", Device.System_V },//EMS
                    
                    { "System_P", Device.System_P },//MGC

                    { "System_Insulation", Device.System_Insulation },
                    { "System_Spare2", Device.System_Spare2 },
                    { "System_Spare3", Device.System_Spare3 },
                    { "System_Spare4", Device.System_Spare4 },
                    { "System_MaximumPermissibleCurrent_Charging", Device.System_MaximumPermissibleCurrent_Charging },//EMS
                    { "System_MaximumPermissibleCurrent_Discharging", Device.System_MaximumPermissibleCurrent_Discharging },//EMS
                    { "System_SOH", Device.System_SOH },//EMS
                    { "Cell_maxV_Index_Rank", Device.Cell_maxV_Index_Rank },
                    { "Cell_maxV_Index_Module", Device.Cell_maxV_Index_Module },
                    { "Cell_maxV_Index_Cell", Device.Cell_maxV_Index_Cell },
                    { "Cell_maxV", Device.Cell_maxV },//EMS
                    { "Cell_minV_Index_Rank", Device.Cell_minV_Index_Rank },
                    { "Cell_minV_Index_Module", Device.Cell_minV_Index_Module },
                    { "Cell_minV_Index_Cell", Device.Cell_minV_Index_Cell },
                    { "Cell_minV", Device.Cell_minV },//EMS
                    { "Cell_maxTemp_Index_Rank", Device.Cell_maxTemp_Index_Rank },
                    { "Cell_maxTemp_Index_Module", Device.Cell_maxTemp_Index_Module },
                    { "Cell_maxTemp_Index_Cell", Device.Cell_maxTemp_Index_Cell },
                    { "Cell_maxTemp", Device.Cell_maxTemp },//EMS
                    { "Cell_minTemp_Index_Rank", Device.Cell_minTemp_Index_Rank },
                    { "Cell_minTemp_Index_Module", Device.Cell_minTemp_Index_Module },
                    { "Cell_minTemp_Index_Cell", Device.Cell_minTemp_Index_Cell },
                    { "Cell_minTemp", Device.Cell_minTemp },//EMS
                    
                    { "System_Error", Device.System_Error },
                    { "Ranks_Communication_Error", Device.Ranks_Communication_Error },
                    { "System_Error_Warning", Device.System_Error_Warning },
                    { "System_Error_Stop", Device.System_Error_Stop },
                    { "System_Error_Protection", Device.System_Error_Protection },

                    { "BMS_V" ,new BsonArray {Device.BMS01.V, Device.BMS02.V, Device.BMS03.V, Device.BMS04.V } },//EMS
                    { "BMS_A" ,new BsonArray {Device.BMS01.A, Device.BMS02.A, Device.BMS03.A, Device.BMS04.A } },//EMS
                    { "BMS_ChargingDischarging_State" ,new BsonArray {Device.BMS01.BMS_ChargingDischarging_State, Device.BMS02.BMS_ChargingDischarging_State, Device.BMS03.BMS_ChargingDischarging_State, Device.BMS04.BMS_ChargingDischarging_State } },
                    { "BMS_SOC" ,new BsonArray {Device.BMS01.SOC, Device.BMS02.SOC, Device.BMS03.SOC, Device.BMS04.SOC } },//EMS
                    { "BMS_SOH" ,new BsonArray {Device.BMS01.SOH, Device.BMS02.SOH, Device.BMS03.SOH, Device.BMS04.SOH } },//EMS
                    { "BMS_Cell_maxV_Index_Serial" ,new BsonArray {Device.BMS01.Cell_maxV_Index_Serial, Device.BMS02.Cell_maxV_Index_Serial, Device.BMS03.Cell_maxV_Index_Serial, Device.BMS04.Cell_maxV_Index_Serial } },
                    { "BMS_Cell_maxV" ,new BsonArray {Device.BMS01.Cell_maxV, Device.BMS02.Cell_maxV, Device.BMS03.Cell_maxV, Device.BMS04.Cell_maxV } },//EMS
                    { "BMS_Cell_minV_Index_Serial" ,new BsonArray {Device.BMS01.Cell_minV_Index_Serial, Device.BMS02.Cell_minV_Index_Serial, Device.BMS03.Cell_minV_Index_Serial, Device.BMS04.Cell_minV_Index_Serial } },
                    { "BMS_Cell_minV" ,new BsonArray {Device.BMS01.Cell_minV, Device.BMS02.Cell_minV, Device.BMS03.Cell_minV, Device.BMS04.Cell_minV } },//EMS
                    { "BMS_Cell_maxTemp_Index_Serial" ,new BsonArray {Device.BMS01.Cell_maxTemp_Index_Serial, Device.BMS02.Cell_maxTemp_Index_Serial, Device.BMS03.Cell_maxTemp_Index_Serial, Device.BMS04.Cell_maxTemp_Index_Serial } },
                    { "BMS_Cell_maxTemp" ,new BsonArray {Device.BMS01.Cell_maxTemp, Device.BMS02.Cell_maxTemp, Device.BMS03.Cell_maxTemp, Device.BMS04.Cell_maxTemp } },//EMS
                    { "BMS_Cell_minTemp_Index_Serial" ,new BsonArray {Device.BMS01.Cell_minTemp_Index_Serial, Device.BMS02.Cell_minTemp_Index_Serial, Device.BMS03.Cell_minTemp_Index_Serial, Device.BMS04.Cell_minTemp_Index_Serial } },
                    { "BMS_Cell_minTemp" ,new BsonArray {Device.BMS01.Cell_minTemp, Device.BMS02.Cell_minTemp, Device.BMS03.Cell_minTemp, Device.BMS04.Cell_minTemp } },//EMS
                    { "BMS_Cell_avgV" ,new BsonArray {Device.BMS01.Cell_avgV, Device.BMS02.Cell_avgV, Device.BMS03.Cell_avgV, Device.BMS04.Cell_avgV } },
                    { "BMS_Insulation" ,new BsonArray {Device.BMS01.Insulation, Device.BMS02.Insulation, Device.BMS03.Insulation, Device.BMS04.Insulation } },
                    { "BMS_Max_Current_Charging" ,new BsonArray {Device.BMS01.Max_Current_Charging, Device.BMS02.Max_Current_Charging, Device.BMS03.Max_Current_Charging, Device.BMS04.Max_Current_Charging } },
                    { "BMS_Max_Current_Discharging" ,new BsonArray {Device.BMS01.Max_Current_Discharging, Device.BMS02.Max_Current_Discharging, Device.BMS03.Max_Current_Discharging, Device.BMS04.Max_Current_Discharging } },
                    { "BMS_Insulation_Positive_Electrode" ,new BsonArray {Device.BMS01.Insulation_Positive_Electrode, Device.BMS02.Insulation_Positive_Electrode, Device.BMS03.Insulation_Positive_Electrode, Device.BMS04.Insulation_Positive_Electrode } },
                    { "BMS_Insulation_Negative_Electrode" ,new BsonArray {Device.BMS01.Insulation_Negative_Electrode, Device.BMS02.Insulation_Negative_Electrode, Device.BMS03.Insulation_Negative_Electrode, Device.BMS04.Insulation_Negative_Electrode } },
                    { "BMS_Cell_avgTemp" ,new BsonArray {Device.BMS01.Cell_avgTemp, Device.BMS02.Cell_avgTemp, Device.BMS03.Cell_avgTemp, Device.BMS04.Cell_avgTemp } },
                    /*
                    { "BMS01_Cell_V" ,new BsonArray {Device.BMS01.Cell_V[0], Device.BMS01.Cell_V[1], Device.BMS01.Cell_V[2], Device.BMS01.Cell_V[3], Device.BMS01.Cell_V[4], Device.BMS01.Cell_V[5], Device.BMS01.Cell_V[6], Device.BMS01.Cell_V[7], Device.BMS01.Cell_V[8], Device.BMS01.Cell_V[9], Device.BMS01.Cell_V[10], Device.BMS01.Cell_V[11], Device.BMS01.Cell_V[12], Device.BMS01.Cell_V[13], Device.BMS01.Cell_V[14], Device.BMS01.Cell_V[15], Device.BMS01.Cell_V[16], Device.BMS01.Cell_V[17], Device.BMS01.Cell_V[18], Device.BMS01.Cell_V[19], Device.BMS01.Cell_V[20], Device.BMS01.Cell_V[21], Device.BMS01.Cell_V[22], Device.BMS01.Cell_V[23], Device.BMS01.Cell_V[24], Device.BMS01.Cell_V[25], Device.BMS01.Cell_V[26], Device.BMS01.Cell_V[27], Device.BMS01.Cell_V[28], Device.BMS01.Cell_V[29], Device.BMS01.Cell_V[30], Device.BMS01.Cell_V[31], Device.BMS01.Cell_V[32], Device.BMS01.Cell_V[33], Device.BMS01.Cell_V[34], Device.BMS01.Cell_V[35], Device.BMS01.Cell_V[36], Device.BMS01.Cell_V[37], Device.BMS01.Cell_V[38], Device.BMS01.Cell_V[39], Device.BMS01.Cell_V[40], Device.BMS01.Cell_V[41], Device.BMS01.Cell_V[42], Device.BMS01.Cell_V[43], Device.BMS01.Cell_V[44], Device.BMS01.Cell_V[45], Device.BMS01.Cell_V[46], Device.BMS01.Cell_V[47], Device.BMS01.Cell_V[48], Device.BMS01.Cell_V[49], Device.BMS01.Cell_V[50], Device.BMS01.Cell_V[51], Device.BMS01.Cell_V[52], Device.BMS01.Cell_V[53], Device.BMS01.Cell_V[54], Device.BMS01.Cell_V[55], Device.BMS01.Cell_V[56], Device.BMS01.Cell_V[57], Device.BMS01.Cell_V[58], Device.BMS01.Cell_V[59], Device.BMS01.Cell_V[60], Device.BMS01.Cell_V[61], Device.BMS01.Cell_V[62], Device.BMS01.Cell_V[63], Device.BMS01.Cell_V[64], Device.BMS01.Cell_V[65], Device.BMS01.Cell_V[66], Device.BMS01.Cell_V[67], Device.BMS01.Cell_V[68], Device.BMS01.Cell_V[69], Device.BMS01.Cell_V[70], Device.BMS01.Cell_V[71], Device.BMS01.Cell_V[72], Device.BMS01.Cell_V[73], Device.BMS01.Cell_V[74], Device.BMS01.Cell_V[75], Device.BMS01.Cell_V[76], Device.BMS01.Cell_V[77], Device.BMS01.Cell_V[78], Device.BMS01.Cell_V[79], Device.BMS01.Cell_V[80], Device.BMS01.Cell_V[81], Device.BMS01.Cell_V[82], Device.BMS01.Cell_V[83], Device.BMS01.Cell_V[84], Device.BMS01.Cell_V[85], Device.BMS01.Cell_V[86], Device.BMS01.Cell_V[87], Device.BMS01.Cell_V[88], Device.BMS01.Cell_V[89], Device.BMS01.Cell_V[90], Device.BMS01.Cell_V[91], Device.BMS01.Cell_V[92], Device.BMS01.Cell_V[93], Device.BMS01.Cell_V[94], Device.BMS01.Cell_V[95], Device.BMS01.Cell_V[96], Device.BMS01.Cell_V[97], Device.BMS01.Cell_V[98], Device.BMS01.Cell_V[99]
                                                   , Device.BMS01.Cell_V[100], Device.BMS01.Cell_V[101], Device.BMS01.Cell_V[102], Device.BMS01.Cell_V[103], Device.BMS01.Cell_V[104], Device.BMS01.Cell_V[105], Device.BMS01.Cell_V[106], Device.BMS01.Cell_V[107], Device.BMS01.Cell_V[108], Device.BMS01.Cell_V[109], Device.BMS01.Cell_V[110], Device.BMS01.Cell_V[111], Device.BMS01.Cell_V[112], Device.BMS01.Cell_V[113], Device.BMS01.Cell_V[114], Device.BMS01.Cell_V[115], Device.BMS01.Cell_V[116], Device.BMS01.Cell_V[117], Device.BMS01.Cell_V[118], Device.BMS01.Cell_V[119], Device.BMS01.Cell_V[120], Device.BMS01.Cell_V[121], Device.BMS01.Cell_V[122], Device.BMS01.Cell_V[123], Device.BMS01.Cell_V[124], Device.BMS01.Cell_V[125], Device.BMS01.Cell_V[126], Device.BMS01.Cell_V[127], Device.BMS01.Cell_V[128], Device.BMS01.Cell_V[129], Device.BMS01.Cell_V[130], Device.BMS01.Cell_V[131], Device.BMS01.Cell_V[132], Device.BMS01.Cell_V[133], Device.BMS01.Cell_V[134], Device.BMS01.Cell_V[135], Device.BMS01.Cell_V[136], Device.BMS01.Cell_V[137], Device.BMS01.Cell_V[138], Device.BMS01.Cell_V[139], Device.BMS01.Cell_V[140], Device.BMS01.Cell_V[141], Device.BMS01.Cell_V[142], Device.BMS01.Cell_V[143], Device.BMS01.Cell_V[144], Device.BMS01.Cell_V[145], Device.BMS01.Cell_V[146], Device.BMS01.Cell_V[147], Device.BMS01.Cell_V[148], Device.BMS01.Cell_V[149], Device.BMS01.Cell_V[150], Device.BMS01.Cell_V[151], Device.BMS01.Cell_V[152], Device.BMS01.Cell_V[153], Device.BMS01.Cell_V[154], Device.BMS01.Cell_V[155], Device.BMS01.Cell_V[156], Device.BMS01.Cell_V[157], Device.BMS01.Cell_V[158], Device.BMS01.Cell_V[159], Device.BMS01.Cell_V[160], Device.BMS01.Cell_V[161], Device.BMS01.Cell_V[162], Device.BMS01.Cell_V[163], Device.BMS01.Cell_V[164], Device.BMS01.Cell_V[165], Device.BMS01.Cell_V[166], Device.BMS01.Cell_V[167], Device.BMS01.Cell_V[168], Device.BMS01.Cell_V[169], Device.BMS01.Cell_V[170], Device.BMS01.Cell_V[171], Device.BMS01.Cell_V[172], Device.BMS01.Cell_V[173], Device.BMS01.Cell_V[174], Device.BMS01.Cell_V[175], Device.BMS01.Cell_V[176], Device.BMS01.Cell_V[177], Device.BMS01.Cell_V[178], Device.BMS01.Cell_V[179], Device.BMS01.Cell_V[180], Device.BMS01.Cell_V[181], Device.BMS01.Cell_V[182], Device.BMS01.Cell_V[183], Device.BMS01.Cell_V[184], Device.BMS01.Cell_V[185], Device.BMS01.Cell_V[186], Device.BMS01.Cell_V[187], Device.BMS01.Cell_V[188], Device.BMS01.Cell_V[189], Device.BMS01.Cell_V[190], Device.BMS01.Cell_V[191], Device.BMS01.Cell_V[192], Device.BMS01.Cell_V[193], Device.BMS01.Cell_V[194], Device.BMS01.Cell_V[195], Device.BMS01.Cell_V[196], Device.BMS01.Cell_V[197], Device.BMS01.Cell_V[198], Device.BMS01.Cell_V[199]
                                                   , Device.BMS01.Cell_V[200], Device.BMS01.Cell_V[201], Device.BMS01.Cell_V[202], Device.BMS01.Cell_V[203], Device.BMS01.Cell_V[204], Device.BMS01.Cell_V[205], Device.BMS01.Cell_V[206], Device.BMS01.Cell_V[207], Device.BMS01.Cell_V[208], Device.BMS01.Cell_V[209], Device.BMS01.Cell_V[210], Device.BMS01.Cell_V[211], Device.BMS01.Cell_V[212], Device.BMS01.Cell_V[213], Device.BMS01.Cell_V[214], Device.BMS01.Cell_V[215], Device.BMS01.Cell_V[216], Device.BMS01.Cell_V[217], Device.BMS01.Cell_V[218], Device.BMS01.Cell_V[219], Device.BMS01.Cell_V[220], Device.BMS01.Cell_V[221], Device.BMS01.Cell_V[222], Device.BMS01.Cell_V[223], Device.BMS01.Cell_V[224], Device.BMS01.Cell_V[225], Device.BMS01.Cell_V[226], Device.BMS01.Cell_V[227], Device.BMS01.Cell_V[228], Device.BMS01.Cell_V[229], Device.BMS01.Cell_V[230], Device.BMS01.Cell_V[231], Device.BMS01.Cell_V[232], Device.BMS01.Cell_V[233], Device.BMS01.Cell_V[234], Device.BMS01.Cell_V[235], Device.BMS01.Cell_V[236], Device.BMS01.Cell_V[237]
                                                    } },
                    { "BMS02_Cell_V" ,new BsonArray {Device.BMS02.Cell_V[0], Device.BMS02.Cell_V[1], Device.BMS02.Cell_V[2], Device.BMS02.Cell_V[3], Device.BMS02.Cell_V[4], Device.BMS02.Cell_V[5], Device.BMS02.Cell_V[6], Device.BMS02.Cell_V[7], Device.BMS02.Cell_V[8], Device.BMS02.Cell_V[9], Device.BMS02.Cell_V[10], Device.BMS02.Cell_V[11], Device.BMS02.Cell_V[12], Device.BMS02.Cell_V[13], Device.BMS02.Cell_V[14], Device.BMS02.Cell_V[15], Device.BMS02.Cell_V[16], Device.BMS02.Cell_V[17], Device.BMS02.Cell_V[18], Device.BMS02.Cell_V[19], Device.BMS02.Cell_V[20], Device.BMS02.Cell_V[21], Device.BMS02.Cell_V[22], Device.BMS02.Cell_V[23], Device.BMS02.Cell_V[24], Device.BMS02.Cell_V[25], Device.BMS02.Cell_V[26], Device.BMS02.Cell_V[27], Device.BMS02.Cell_V[28], Device.BMS02.Cell_V[29], Device.BMS02.Cell_V[30], Device.BMS02.Cell_V[31], Device.BMS02.Cell_V[32], Device.BMS02.Cell_V[33], Device.BMS02.Cell_V[34], Device.BMS02.Cell_V[35], Device.BMS02.Cell_V[36], Device.BMS02.Cell_V[37], Device.BMS02.Cell_V[38], Device.BMS02.Cell_V[39], Device.BMS02.Cell_V[40], Device.BMS02.Cell_V[41], Device.BMS02.Cell_V[42], Device.BMS02.Cell_V[43], Device.BMS02.Cell_V[44], Device.BMS02.Cell_V[45], Device.BMS02.Cell_V[46], Device.BMS02.Cell_V[47], Device.BMS02.Cell_V[48], Device.BMS02.Cell_V[49], Device.BMS02.Cell_V[50], Device.BMS02.Cell_V[51], Device.BMS02.Cell_V[52], Device.BMS02.Cell_V[53], Device.BMS02.Cell_V[54], Device.BMS02.Cell_V[55], Device.BMS02.Cell_V[56], Device.BMS02.Cell_V[57], Device.BMS02.Cell_V[58], Device.BMS02.Cell_V[59], Device.BMS02.Cell_V[60], Device.BMS02.Cell_V[61], Device.BMS02.Cell_V[62], Device.BMS02.Cell_V[63], Device.BMS02.Cell_V[64], Device.BMS02.Cell_V[65], Device.BMS02.Cell_V[66], Device.BMS02.Cell_V[67], Device.BMS02.Cell_V[68], Device.BMS02.Cell_V[69], Device.BMS02.Cell_V[70], Device.BMS02.Cell_V[71], Device.BMS02.Cell_V[72], Device.BMS02.Cell_V[73], Device.BMS02.Cell_V[74], Device.BMS02.Cell_V[75], Device.BMS02.Cell_V[76], Device.BMS02.Cell_V[77], Device.BMS02.Cell_V[78], Device.BMS02.Cell_V[79], Device.BMS02.Cell_V[80], Device.BMS02.Cell_V[81], Device.BMS02.Cell_V[82], Device.BMS02.Cell_V[83], Device.BMS02.Cell_V[84], Device.BMS02.Cell_V[85], Device.BMS02.Cell_V[86], Device.BMS02.Cell_V[87], Device.BMS02.Cell_V[88], Device.BMS02.Cell_V[89], Device.BMS02.Cell_V[90], Device.BMS02.Cell_V[91], Device.BMS02.Cell_V[92], Device.BMS02.Cell_V[93], Device.BMS02.Cell_V[94], Device.BMS02.Cell_V[95], Device.BMS02.Cell_V[96], Device.BMS02.Cell_V[97], Device.BMS02.Cell_V[98], Device.BMS02.Cell_V[99]
                                                   , Device.BMS02.Cell_V[100], Device.BMS02.Cell_V[101], Device.BMS02.Cell_V[102], Device.BMS02.Cell_V[103], Device.BMS02.Cell_V[104], Device.BMS02.Cell_V[105], Device.BMS02.Cell_V[106], Device.BMS02.Cell_V[107], Device.BMS02.Cell_V[108], Device.BMS02.Cell_V[109], Device.BMS02.Cell_V[110], Device.BMS02.Cell_V[111], Device.BMS02.Cell_V[112], Device.BMS02.Cell_V[113], Device.BMS02.Cell_V[114], Device.BMS02.Cell_V[115], Device.BMS02.Cell_V[116], Device.BMS02.Cell_V[117], Device.BMS02.Cell_V[118], Device.BMS02.Cell_V[119], Device.BMS02.Cell_V[120], Device.BMS02.Cell_V[121], Device.BMS02.Cell_V[122], Device.BMS02.Cell_V[123], Device.BMS02.Cell_V[124], Device.BMS02.Cell_V[125], Device.BMS02.Cell_V[126], Device.BMS02.Cell_V[127], Device.BMS02.Cell_V[128], Device.BMS02.Cell_V[129], Device.BMS02.Cell_V[130], Device.BMS02.Cell_V[131], Device.BMS02.Cell_V[132], Device.BMS02.Cell_V[133], Device.BMS02.Cell_V[134], Device.BMS02.Cell_V[135], Device.BMS02.Cell_V[136], Device.BMS02.Cell_V[137], Device.BMS02.Cell_V[138], Device.BMS02.Cell_V[139], Device.BMS02.Cell_V[140], Device.BMS02.Cell_V[141], Device.BMS02.Cell_V[142], Device.BMS02.Cell_V[143], Device.BMS02.Cell_V[144], Device.BMS02.Cell_V[145], Device.BMS02.Cell_V[146], Device.BMS02.Cell_V[147], Device.BMS02.Cell_V[148], Device.BMS02.Cell_V[149], Device.BMS02.Cell_V[150], Device.BMS02.Cell_V[151], Device.BMS02.Cell_V[152], Device.BMS02.Cell_V[153], Device.BMS02.Cell_V[154], Device.BMS02.Cell_V[155], Device.BMS02.Cell_V[156], Device.BMS02.Cell_V[157], Device.BMS02.Cell_V[158], Device.BMS02.Cell_V[159], Device.BMS02.Cell_V[160], Device.BMS02.Cell_V[161], Device.BMS02.Cell_V[162], Device.BMS02.Cell_V[163], Device.BMS02.Cell_V[164], Device.BMS02.Cell_V[165], Device.BMS02.Cell_V[166], Device.BMS02.Cell_V[167], Device.BMS02.Cell_V[168], Device.BMS02.Cell_V[169], Device.BMS02.Cell_V[170], Device.BMS02.Cell_V[171], Device.BMS02.Cell_V[172], Device.BMS02.Cell_V[173], Device.BMS02.Cell_V[174], Device.BMS02.Cell_V[175], Device.BMS02.Cell_V[176], Device.BMS02.Cell_V[177], Device.BMS02.Cell_V[178], Device.BMS02.Cell_V[179], Device.BMS02.Cell_V[180], Device.BMS02.Cell_V[181], Device.BMS02.Cell_V[182], Device.BMS02.Cell_V[183], Device.BMS02.Cell_V[184], Device.BMS02.Cell_V[185], Device.BMS02.Cell_V[186], Device.BMS02.Cell_V[187], Device.BMS02.Cell_V[188], Device.BMS02.Cell_V[189], Device.BMS02.Cell_V[190], Device.BMS02.Cell_V[191], Device.BMS02.Cell_V[192], Device.BMS02.Cell_V[193], Device.BMS02.Cell_V[194], Device.BMS02.Cell_V[195], Device.BMS02.Cell_V[196], Device.BMS02.Cell_V[197], Device.BMS02.Cell_V[198], Device.BMS02.Cell_V[199]
                                                   , Device.BMS02.Cell_V[200], Device.BMS02.Cell_V[201], Device.BMS02.Cell_V[202], Device.BMS02.Cell_V[203], Device.BMS02.Cell_V[204], Device.BMS02.Cell_V[205], Device.BMS02.Cell_V[206], Device.BMS02.Cell_V[207], Device.BMS02.Cell_V[208], Device.BMS02.Cell_V[209], Device.BMS02.Cell_V[210], Device.BMS02.Cell_V[211], Device.BMS02.Cell_V[212], Device.BMS02.Cell_V[213], Device.BMS02.Cell_V[214], Device.BMS02.Cell_V[215], Device.BMS02.Cell_V[216], Device.BMS02.Cell_V[217], Device.BMS02.Cell_V[218], Device.BMS02.Cell_V[219], Device.BMS02.Cell_V[220], Device.BMS02.Cell_V[221], Device.BMS02.Cell_V[222], Device.BMS02.Cell_V[223], Device.BMS02.Cell_V[224], Device.BMS02.Cell_V[225], Device.BMS02.Cell_V[226], Device.BMS02.Cell_V[227], Device.BMS02.Cell_V[228], Device.BMS02.Cell_V[229], Device.BMS02.Cell_V[230], Device.BMS02.Cell_V[231], Device.BMS02.Cell_V[232], Device.BMS02.Cell_V[233], Device.BMS02.Cell_V[234], Device.BMS02.Cell_V[235], Device.BMS02.Cell_V[236], Device.BMS02.Cell_V[237]
                                                    } },
                    { "BMS03_Cell_V" ,new BsonArray {Device.BMS03.Cell_V[0], Device.BMS03.Cell_V[1], Device.BMS03.Cell_V[2], Device.BMS03.Cell_V[3], Device.BMS03.Cell_V[4], Device.BMS03.Cell_V[5], Device.BMS03.Cell_V[6], Device.BMS03.Cell_V[7], Device.BMS03.Cell_V[8], Device.BMS03.Cell_V[9], Device.BMS03.Cell_V[10], Device.BMS03.Cell_V[11], Device.BMS03.Cell_V[12], Device.BMS03.Cell_V[13], Device.BMS03.Cell_V[14], Device.BMS03.Cell_V[15], Device.BMS03.Cell_V[16], Device.BMS03.Cell_V[17], Device.BMS03.Cell_V[18], Device.BMS03.Cell_V[19], Device.BMS03.Cell_V[20], Device.BMS03.Cell_V[21], Device.BMS03.Cell_V[22], Device.BMS03.Cell_V[23], Device.BMS03.Cell_V[24], Device.BMS03.Cell_V[25], Device.BMS03.Cell_V[26], Device.BMS03.Cell_V[27], Device.BMS03.Cell_V[28], Device.BMS03.Cell_V[29], Device.BMS03.Cell_V[30], Device.BMS03.Cell_V[31], Device.BMS03.Cell_V[32], Device.BMS03.Cell_V[33], Device.BMS03.Cell_V[34], Device.BMS03.Cell_V[35], Device.BMS03.Cell_V[36], Device.BMS03.Cell_V[37], Device.BMS03.Cell_V[38], Device.BMS03.Cell_V[39], Device.BMS03.Cell_V[40], Device.BMS03.Cell_V[41], Device.BMS03.Cell_V[42], Device.BMS03.Cell_V[43], Device.BMS03.Cell_V[44], Device.BMS03.Cell_V[45], Device.BMS03.Cell_V[46], Device.BMS03.Cell_V[47], Device.BMS03.Cell_V[48], Device.BMS03.Cell_V[49], Device.BMS03.Cell_V[50], Device.BMS03.Cell_V[51], Device.BMS03.Cell_V[52], Device.BMS03.Cell_V[53], Device.BMS03.Cell_V[54], Device.BMS03.Cell_V[55], Device.BMS03.Cell_V[56], Device.BMS03.Cell_V[57], Device.BMS03.Cell_V[58], Device.BMS03.Cell_V[59], Device.BMS03.Cell_V[60], Device.BMS03.Cell_V[61], Device.BMS03.Cell_V[62], Device.BMS03.Cell_V[63], Device.BMS03.Cell_V[64], Device.BMS03.Cell_V[65], Device.BMS03.Cell_V[66], Device.BMS03.Cell_V[67], Device.BMS03.Cell_V[68], Device.BMS03.Cell_V[69], Device.BMS03.Cell_V[70], Device.BMS03.Cell_V[71], Device.BMS03.Cell_V[72], Device.BMS03.Cell_V[73], Device.BMS03.Cell_V[74], Device.BMS03.Cell_V[75], Device.BMS03.Cell_V[76], Device.BMS03.Cell_V[77], Device.BMS03.Cell_V[78], Device.BMS03.Cell_V[79], Device.BMS03.Cell_V[80], Device.BMS03.Cell_V[81], Device.BMS03.Cell_V[82], Device.BMS03.Cell_V[83], Device.BMS03.Cell_V[84], Device.BMS03.Cell_V[85], Device.BMS03.Cell_V[86], Device.BMS03.Cell_V[87], Device.BMS03.Cell_V[88], Device.BMS03.Cell_V[89], Device.BMS03.Cell_V[90], Device.BMS03.Cell_V[91], Device.BMS03.Cell_V[92], Device.BMS03.Cell_V[93], Device.BMS03.Cell_V[94], Device.BMS03.Cell_V[95], Device.BMS03.Cell_V[96], Device.BMS03.Cell_V[97], Device.BMS03.Cell_V[98], Device.BMS03.Cell_V[99]
                                                   , Device.BMS03.Cell_V[100], Device.BMS03.Cell_V[101], Device.BMS03.Cell_V[102], Device.BMS03.Cell_V[103], Device.BMS03.Cell_V[104], Device.BMS03.Cell_V[105], Device.BMS03.Cell_V[106], Device.BMS03.Cell_V[107], Device.BMS03.Cell_V[108], Device.BMS03.Cell_V[109], Device.BMS03.Cell_V[110], Device.BMS03.Cell_V[111], Device.BMS03.Cell_V[112], Device.BMS03.Cell_V[113], Device.BMS03.Cell_V[114], Device.BMS03.Cell_V[115], Device.BMS03.Cell_V[116], Device.BMS03.Cell_V[117], Device.BMS03.Cell_V[118], Device.BMS03.Cell_V[119], Device.BMS03.Cell_V[120], Device.BMS03.Cell_V[121], Device.BMS03.Cell_V[122], Device.BMS03.Cell_V[123], Device.BMS03.Cell_V[124], Device.BMS03.Cell_V[125], Device.BMS03.Cell_V[126], Device.BMS03.Cell_V[127], Device.BMS03.Cell_V[128], Device.BMS03.Cell_V[129], Device.BMS03.Cell_V[130], Device.BMS03.Cell_V[131], Device.BMS03.Cell_V[132], Device.BMS03.Cell_V[133], Device.BMS03.Cell_V[134], Device.BMS03.Cell_V[135], Device.BMS03.Cell_V[136], Device.BMS03.Cell_V[137], Device.BMS03.Cell_V[138], Device.BMS03.Cell_V[139], Device.BMS03.Cell_V[140], Device.BMS03.Cell_V[141], Device.BMS03.Cell_V[142], Device.BMS03.Cell_V[143], Device.BMS03.Cell_V[144], Device.BMS03.Cell_V[145], Device.BMS03.Cell_V[146], Device.BMS03.Cell_V[147], Device.BMS03.Cell_V[148], Device.BMS03.Cell_V[149], Device.BMS03.Cell_V[150], Device.BMS03.Cell_V[151], Device.BMS03.Cell_V[152], Device.BMS03.Cell_V[153], Device.BMS03.Cell_V[154], Device.BMS03.Cell_V[155], Device.BMS03.Cell_V[156], Device.BMS03.Cell_V[157], Device.BMS03.Cell_V[158], Device.BMS03.Cell_V[159], Device.BMS03.Cell_V[160], Device.BMS03.Cell_V[161], Device.BMS03.Cell_V[162], Device.BMS03.Cell_V[163], Device.BMS03.Cell_V[164], Device.BMS03.Cell_V[165], Device.BMS03.Cell_V[166], Device.BMS03.Cell_V[167], Device.BMS03.Cell_V[168], Device.BMS03.Cell_V[169], Device.BMS03.Cell_V[170], Device.BMS03.Cell_V[171], Device.BMS03.Cell_V[172], Device.BMS03.Cell_V[173], Device.BMS03.Cell_V[174], Device.BMS03.Cell_V[175], Device.BMS03.Cell_V[176], Device.BMS03.Cell_V[177], Device.BMS03.Cell_V[178], Device.BMS03.Cell_V[179], Device.BMS03.Cell_V[180], Device.BMS03.Cell_V[181], Device.BMS03.Cell_V[182], Device.BMS03.Cell_V[183], Device.BMS03.Cell_V[184], Device.BMS03.Cell_V[185], Device.BMS03.Cell_V[186], Device.BMS03.Cell_V[187], Device.BMS03.Cell_V[188], Device.BMS03.Cell_V[189], Device.BMS03.Cell_V[190], Device.BMS03.Cell_V[191], Device.BMS03.Cell_V[192], Device.BMS03.Cell_V[193], Device.BMS03.Cell_V[194], Device.BMS03.Cell_V[195], Device.BMS03.Cell_V[196], Device.BMS03.Cell_V[197], Device.BMS03.Cell_V[198], Device.BMS03.Cell_V[199]
                                                   , Device.BMS03.Cell_V[200], Device.BMS03.Cell_V[201], Device.BMS03.Cell_V[202], Device.BMS03.Cell_V[203], Device.BMS03.Cell_V[204], Device.BMS03.Cell_V[205], Device.BMS03.Cell_V[206], Device.BMS03.Cell_V[207], Device.BMS03.Cell_V[208], Device.BMS03.Cell_V[209], Device.BMS03.Cell_V[210], Device.BMS03.Cell_V[211], Device.BMS03.Cell_V[212], Device.BMS03.Cell_V[213], Device.BMS03.Cell_V[214], Device.BMS03.Cell_V[215], Device.BMS03.Cell_V[216], Device.BMS03.Cell_V[217], Device.BMS03.Cell_V[218], Device.BMS03.Cell_V[219], Device.BMS03.Cell_V[220], Device.BMS03.Cell_V[221], Device.BMS03.Cell_V[222], Device.BMS03.Cell_V[223], Device.BMS03.Cell_V[224], Device.BMS03.Cell_V[225], Device.BMS03.Cell_V[226], Device.BMS03.Cell_V[227], Device.BMS03.Cell_V[228], Device.BMS03.Cell_V[229], Device.BMS03.Cell_V[230], Device.BMS03.Cell_V[231], Device.BMS03.Cell_V[232], Device.BMS03.Cell_V[233], Device.BMS03.Cell_V[234], Device.BMS03.Cell_V[235], Device.BMS03.Cell_V[236], Device.BMS03.Cell_V[237]
                                                    } },
                    { "BMS04_Cell_V" ,new BsonArray {Device.BMS04.Cell_V[0], Device.BMS04.Cell_V[1], Device.BMS04.Cell_V[2], Device.BMS04.Cell_V[3], Device.BMS04.Cell_V[4], Device.BMS04.Cell_V[5], Device.BMS04.Cell_V[6], Device.BMS04.Cell_V[7], Device.BMS04.Cell_V[8], Device.BMS04.Cell_V[9], Device.BMS04.Cell_V[10], Device.BMS04.Cell_V[11], Device.BMS04.Cell_V[12], Device.BMS04.Cell_V[13], Device.BMS04.Cell_V[14], Device.BMS04.Cell_V[15], Device.BMS04.Cell_V[16], Device.BMS04.Cell_V[17], Device.BMS04.Cell_V[18], Device.BMS04.Cell_V[19], Device.BMS04.Cell_V[20], Device.BMS04.Cell_V[21], Device.BMS04.Cell_V[22], Device.BMS04.Cell_V[23], Device.BMS04.Cell_V[24], Device.BMS04.Cell_V[25], Device.BMS04.Cell_V[26], Device.BMS04.Cell_V[27], Device.BMS04.Cell_V[28], Device.BMS04.Cell_V[29], Device.BMS04.Cell_V[30], Device.BMS04.Cell_V[31], Device.BMS04.Cell_V[32], Device.BMS04.Cell_V[33], Device.BMS04.Cell_V[34], Device.BMS04.Cell_V[35], Device.BMS04.Cell_V[36], Device.BMS04.Cell_V[37], Device.BMS04.Cell_V[38], Device.BMS04.Cell_V[39], Device.BMS04.Cell_V[40], Device.BMS04.Cell_V[41], Device.BMS04.Cell_V[42], Device.BMS04.Cell_V[43], Device.BMS04.Cell_V[44], Device.BMS04.Cell_V[45], Device.BMS04.Cell_V[46], Device.BMS04.Cell_V[47], Device.BMS04.Cell_V[48], Device.BMS04.Cell_V[49], Device.BMS04.Cell_V[50], Device.BMS04.Cell_V[51], Device.BMS04.Cell_V[52], Device.BMS04.Cell_V[53], Device.BMS04.Cell_V[54], Device.BMS04.Cell_V[55], Device.BMS04.Cell_V[56], Device.BMS04.Cell_V[57], Device.BMS04.Cell_V[58], Device.BMS04.Cell_V[59], Device.BMS04.Cell_V[60], Device.BMS04.Cell_V[61], Device.BMS04.Cell_V[62], Device.BMS04.Cell_V[63], Device.BMS04.Cell_V[64], Device.BMS04.Cell_V[65], Device.BMS04.Cell_V[66], Device.BMS04.Cell_V[67], Device.BMS04.Cell_V[68], Device.BMS04.Cell_V[69], Device.BMS04.Cell_V[70], Device.BMS04.Cell_V[71], Device.BMS04.Cell_V[72], Device.BMS04.Cell_V[73], Device.BMS04.Cell_V[74], Device.BMS04.Cell_V[75], Device.BMS04.Cell_V[76], Device.BMS04.Cell_V[77], Device.BMS04.Cell_V[78], Device.BMS04.Cell_V[79], Device.BMS04.Cell_V[80], Device.BMS04.Cell_V[81], Device.BMS04.Cell_V[82], Device.BMS04.Cell_V[83], Device.BMS04.Cell_V[84], Device.BMS04.Cell_V[85], Device.BMS04.Cell_V[86], Device.BMS04.Cell_V[87], Device.BMS04.Cell_V[88], Device.BMS04.Cell_V[89], Device.BMS04.Cell_V[90], Device.BMS04.Cell_V[91], Device.BMS04.Cell_V[92], Device.BMS04.Cell_V[93], Device.BMS04.Cell_V[94], Device.BMS04.Cell_V[95], Device.BMS04.Cell_V[96], Device.BMS04.Cell_V[97], Device.BMS04.Cell_V[98], Device.BMS04.Cell_V[99]
                                                   , Device.BMS04.Cell_V[100], Device.BMS04.Cell_V[101], Device.BMS04.Cell_V[102], Device.BMS04.Cell_V[103], Device.BMS04.Cell_V[104], Device.BMS04.Cell_V[105], Device.BMS04.Cell_V[106], Device.BMS04.Cell_V[107], Device.BMS04.Cell_V[108], Device.BMS04.Cell_V[109], Device.BMS04.Cell_V[110], Device.BMS04.Cell_V[111], Device.BMS04.Cell_V[112], Device.BMS04.Cell_V[113], Device.BMS04.Cell_V[114], Device.BMS04.Cell_V[115], Device.BMS04.Cell_V[116], Device.BMS04.Cell_V[117], Device.BMS04.Cell_V[118], Device.BMS04.Cell_V[119], Device.BMS04.Cell_V[120], Device.BMS04.Cell_V[121], Device.BMS04.Cell_V[122], Device.BMS04.Cell_V[123], Device.BMS04.Cell_V[124], Device.BMS04.Cell_V[125], Device.BMS04.Cell_V[126], Device.BMS04.Cell_V[127], Device.BMS04.Cell_V[128], Device.BMS04.Cell_V[129], Device.BMS04.Cell_V[130], Device.BMS04.Cell_V[131], Device.BMS04.Cell_V[132], Device.BMS04.Cell_V[133], Device.BMS04.Cell_V[134], Device.BMS04.Cell_V[135], Device.BMS04.Cell_V[136], Device.BMS04.Cell_V[137], Device.BMS04.Cell_V[138], Device.BMS04.Cell_V[139], Device.BMS04.Cell_V[140], Device.BMS04.Cell_V[141], Device.BMS04.Cell_V[142], Device.BMS04.Cell_V[143], Device.BMS04.Cell_V[144], Device.BMS04.Cell_V[145], Device.BMS04.Cell_V[146], Device.BMS04.Cell_V[147], Device.BMS04.Cell_V[148], Device.BMS04.Cell_V[149], Device.BMS04.Cell_V[150], Device.BMS04.Cell_V[151], Device.BMS04.Cell_V[152], Device.BMS04.Cell_V[153], Device.BMS04.Cell_V[154], Device.BMS04.Cell_V[155], Device.BMS04.Cell_V[156], Device.BMS04.Cell_V[157], Device.BMS04.Cell_V[158], Device.BMS04.Cell_V[159], Device.BMS04.Cell_V[160], Device.BMS04.Cell_V[161], Device.BMS04.Cell_V[162], Device.BMS04.Cell_V[163], Device.BMS04.Cell_V[164], Device.BMS04.Cell_V[165], Device.BMS04.Cell_V[166], Device.BMS04.Cell_V[167], Device.BMS04.Cell_V[168], Device.BMS04.Cell_V[169], Device.BMS04.Cell_V[170], Device.BMS04.Cell_V[171], Device.BMS04.Cell_V[172], Device.BMS04.Cell_V[173], Device.BMS04.Cell_V[174], Device.BMS04.Cell_V[175], Device.BMS04.Cell_V[176], Device.BMS04.Cell_V[177], Device.BMS04.Cell_V[178], Device.BMS04.Cell_V[179], Device.BMS04.Cell_V[180], Device.BMS04.Cell_V[181], Device.BMS04.Cell_V[182], Device.BMS04.Cell_V[183], Device.BMS04.Cell_V[184], Device.BMS04.Cell_V[185], Device.BMS04.Cell_V[186], Device.BMS04.Cell_V[187], Device.BMS04.Cell_V[188], Device.BMS04.Cell_V[189], Device.BMS04.Cell_V[190], Device.BMS04.Cell_V[191], Device.BMS04.Cell_V[192], Device.BMS04.Cell_V[193], Device.BMS04.Cell_V[194], Device.BMS04.Cell_V[195], Device.BMS04.Cell_V[196], Device.BMS04.Cell_V[197], Device.BMS04.Cell_V[198], Device.BMS04.Cell_V[199]
                                                   , Device.BMS04.Cell_V[200], Device.BMS04.Cell_V[201], Device.BMS04.Cell_V[202], Device.BMS04.Cell_V[203], Device.BMS04.Cell_V[204], Device.BMS04.Cell_V[205], Device.BMS04.Cell_V[206], Device.BMS04.Cell_V[207], Device.BMS04.Cell_V[208], Device.BMS04.Cell_V[209], Device.BMS04.Cell_V[210], Device.BMS04.Cell_V[211], Device.BMS04.Cell_V[212], Device.BMS04.Cell_V[213], Device.BMS04.Cell_V[214], Device.BMS04.Cell_V[215], Device.BMS04.Cell_V[216], Device.BMS04.Cell_V[217], Device.BMS04.Cell_V[218], Device.BMS04.Cell_V[219], Device.BMS04.Cell_V[220], Device.BMS04.Cell_V[221], Device.BMS04.Cell_V[222], Device.BMS04.Cell_V[223], Device.BMS04.Cell_V[224], Device.BMS04.Cell_V[225], Device.BMS04.Cell_V[226], Device.BMS04.Cell_V[227], Device.BMS04.Cell_V[228], Device.BMS04.Cell_V[229], Device.BMS04.Cell_V[230], Device.BMS04.Cell_V[231], Device.BMS04.Cell_V[232], Device.BMS04.Cell_V[233], Device.BMS04.Cell_V[234], Device.BMS04.Cell_V[235], Device.BMS04.Cell_V[236], Device.BMS04.Cell_V[237]
                                                    } },

                    { "BMS01_Cell_Temp" ,new BsonArray {Device.BMS01.Cell_Temp[0], Device.BMS01.Cell_Temp[1], Device.BMS01.Cell_Temp[2], Device.BMS01.Cell_Temp[3], Device.BMS01.Cell_Temp[4], Device.BMS01.Cell_Temp[5], Device.BMS01.Cell_Temp[6], Device.BMS01.Cell_Temp[7], Device.BMS01.Cell_Temp[8], Device.BMS01.Cell_Temp[9], Device.BMS01.Cell_Temp[10], Device.BMS01.Cell_Temp[11], Device.BMS01.Cell_Temp[12], Device.BMS01.Cell_Temp[13], Device.BMS01.Cell_Temp[14], Device.BMS01.Cell_Temp[15], Device.BMS01.Cell_Temp[16], Device.BMS01.Cell_Temp[17], Device.BMS01.Cell_Temp[18], Device.BMS01.Cell_Temp[19], Device.BMS01.Cell_Temp[20], Device.BMS01.Cell_Temp[21], Device.BMS01.Cell_Temp[22], Device.BMS01.Cell_Temp[23], Device.BMS01.Cell_Temp[24], Device.BMS01.Cell_Temp[25], Device.BMS01.Cell_Temp[26], Device.BMS01.Cell_Temp[27], Device.BMS01.Cell_Temp[28], Device.BMS01.Cell_Temp[29]
                                                      , Device.BMS01.Cell_Temp[30], Device.BMS01.Cell_Temp[31], Device.BMS01.Cell_Temp[32], Device.BMS01.Cell_Temp[33], Device.BMS01.Cell_Temp[34], Device.BMS01.Cell_Temp[35], Device.BMS01.Cell_Temp[36], Device.BMS01.Cell_Temp[37], Device.BMS01.Cell_Temp[38], Device.BMS01.Cell_Temp[39], Device.BMS01.Cell_Temp[40], Device.BMS01.Cell_Temp[41], Device.BMS01.Cell_Temp[42], Device.BMS01.Cell_Temp[43], Device.BMS01.Cell_Temp[44], Device.BMS01.Cell_Temp[45], Device.BMS01.Cell_Temp[46], Device.BMS01.Cell_Temp[47], Device.BMS01.Cell_Temp[48], Device.BMS01.Cell_Temp[49], Device.BMS01.Cell_Temp[50], Device.BMS01.Cell_Temp[51], Device.BMS01.Cell_Temp[52], Device.BMS01.Cell_Temp[53], Device.BMS01.Cell_Temp[54], Device.BMS01.Cell_Temp[55], Device.BMS01.Cell_Temp[56], Device.BMS01.Cell_Temp[57], Device.BMS01.Cell_Temp[58], Device.BMS01.Cell_Temp[59]
                                                      , Device.BMS01.Cell_Temp[60], Device.BMS01.Cell_Temp[61], Device.BMS01.Cell_Temp[62], Device.BMS01.Cell_Temp[63], Device.BMS01.Cell_Temp[64], Device.BMS01.Cell_Temp[65], Device.BMS01.Cell_Temp[66], Device.BMS01.Cell_Temp[67]
                                                    } },
                    { "BMS02_Cell_Temp" ,new BsonArray {Device.BMS02.Cell_Temp[0], Device.BMS02.Cell_Temp[1], Device.BMS02.Cell_Temp[2], Device.BMS02.Cell_Temp[3], Device.BMS02.Cell_Temp[4], Device.BMS02.Cell_Temp[5], Device.BMS02.Cell_Temp[6], Device.BMS02.Cell_Temp[7], Device.BMS02.Cell_Temp[8], Device.BMS02.Cell_Temp[9], Device.BMS02.Cell_Temp[10], Device.BMS02.Cell_Temp[11], Device.BMS02.Cell_Temp[12], Device.BMS02.Cell_Temp[13], Device.BMS02.Cell_Temp[14], Device.BMS02.Cell_Temp[15], Device.BMS02.Cell_Temp[16], Device.BMS02.Cell_Temp[17], Device.BMS02.Cell_Temp[18], Device.BMS02.Cell_Temp[19], Device.BMS02.Cell_Temp[20], Device.BMS02.Cell_Temp[21], Device.BMS02.Cell_Temp[22], Device.BMS02.Cell_Temp[23], Device.BMS02.Cell_Temp[24], Device.BMS02.Cell_Temp[25], Device.BMS02.Cell_Temp[26], Device.BMS02.Cell_Temp[27], Device.BMS02.Cell_Temp[28], Device.BMS02.Cell_Temp[29]
                                                      , Device.BMS02.Cell_Temp[30], Device.BMS02.Cell_Temp[31], Device.BMS02.Cell_Temp[32], Device.BMS02.Cell_Temp[33], Device.BMS02.Cell_Temp[34], Device.BMS02.Cell_Temp[35], Device.BMS02.Cell_Temp[36], Device.BMS02.Cell_Temp[37], Device.BMS02.Cell_Temp[38], Device.BMS02.Cell_Temp[39], Device.BMS02.Cell_Temp[40], Device.BMS02.Cell_Temp[41], Device.BMS02.Cell_Temp[42], Device.BMS02.Cell_Temp[43], Device.BMS02.Cell_Temp[44], Device.BMS02.Cell_Temp[45], Device.BMS02.Cell_Temp[46], Device.BMS02.Cell_Temp[47], Device.BMS02.Cell_Temp[48], Device.BMS02.Cell_Temp[49], Device.BMS02.Cell_Temp[50], Device.BMS02.Cell_Temp[51], Device.BMS02.Cell_Temp[52], Device.BMS02.Cell_Temp[53], Device.BMS02.Cell_Temp[54], Device.BMS02.Cell_Temp[55], Device.BMS02.Cell_Temp[56], Device.BMS02.Cell_Temp[57], Device.BMS02.Cell_Temp[58], Device.BMS02.Cell_Temp[59]
                                                      , Device.BMS02.Cell_Temp[60], Device.BMS02.Cell_Temp[61], Device.BMS02.Cell_Temp[62], Device.BMS02.Cell_Temp[63], Device.BMS02.Cell_Temp[64], Device.BMS02.Cell_Temp[65], Device.BMS02.Cell_Temp[66], Device.BMS02.Cell_Temp[67]
                                                    } },
                    { "BMS03_Cell_Temp" ,new BsonArray {Device.BMS03.Cell_Temp[0], Device.BMS03.Cell_Temp[1], Device.BMS03.Cell_Temp[2], Device.BMS03.Cell_Temp[3], Device.BMS03.Cell_Temp[4], Device.BMS03.Cell_Temp[5], Device.BMS03.Cell_Temp[6], Device.BMS03.Cell_Temp[7], Device.BMS03.Cell_Temp[8], Device.BMS03.Cell_Temp[9], Device.BMS03.Cell_Temp[10], Device.BMS03.Cell_Temp[11], Device.BMS03.Cell_Temp[12], Device.BMS03.Cell_Temp[13], Device.BMS03.Cell_Temp[14], Device.BMS03.Cell_Temp[15], Device.BMS03.Cell_Temp[16], Device.BMS03.Cell_Temp[17], Device.BMS03.Cell_Temp[18], Device.BMS03.Cell_Temp[19], Device.BMS03.Cell_Temp[20], Device.BMS03.Cell_Temp[21], Device.BMS03.Cell_Temp[22], Device.BMS03.Cell_Temp[23], Device.BMS03.Cell_Temp[24], Device.BMS03.Cell_Temp[25], Device.BMS03.Cell_Temp[26], Device.BMS03.Cell_Temp[27], Device.BMS03.Cell_Temp[28], Device.BMS03.Cell_Temp[29]
                                                      , Device.BMS03.Cell_Temp[30], Device.BMS03.Cell_Temp[31], Device.BMS03.Cell_Temp[32], Device.BMS03.Cell_Temp[33], Device.BMS03.Cell_Temp[34], Device.BMS03.Cell_Temp[35], Device.BMS03.Cell_Temp[36], Device.BMS03.Cell_Temp[37], Device.BMS03.Cell_Temp[38], Device.BMS03.Cell_Temp[39], Device.BMS03.Cell_Temp[40], Device.BMS03.Cell_Temp[41], Device.BMS03.Cell_Temp[42], Device.BMS03.Cell_Temp[43], Device.BMS03.Cell_Temp[44], Device.BMS03.Cell_Temp[45], Device.BMS03.Cell_Temp[46], Device.BMS03.Cell_Temp[47], Device.BMS03.Cell_Temp[48], Device.BMS03.Cell_Temp[49], Device.BMS03.Cell_Temp[50], Device.BMS03.Cell_Temp[51], Device.BMS03.Cell_Temp[52], Device.BMS03.Cell_Temp[53], Device.BMS03.Cell_Temp[54], Device.BMS03.Cell_Temp[55], Device.BMS03.Cell_Temp[56], Device.BMS03.Cell_Temp[57], Device.BMS03.Cell_Temp[58], Device.BMS03.Cell_Temp[59]
                                                      , Device.BMS03.Cell_Temp[60], Device.BMS03.Cell_Temp[61], Device.BMS03.Cell_Temp[62], Device.BMS03.Cell_Temp[63], Device.BMS03.Cell_Temp[64], Device.BMS03.Cell_Temp[65], Device.BMS03.Cell_Temp[66], Device.BMS03.Cell_Temp[67]
                                                    } },
                    { "BMS04_Cell_Temp" ,new BsonArray {Device.BMS04.Cell_Temp[0], Device.BMS04.Cell_Temp[1], Device.BMS04.Cell_Temp[2], Device.BMS04.Cell_Temp[3], Device.BMS04.Cell_Temp[4], Device.BMS04.Cell_Temp[5], Device.BMS04.Cell_Temp[6], Device.BMS04.Cell_Temp[7], Device.BMS04.Cell_Temp[8], Device.BMS04.Cell_Temp[9], Device.BMS04.Cell_Temp[10], Device.BMS04.Cell_Temp[11], Device.BMS04.Cell_Temp[12], Device.BMS04.Cell_Temp[13], Device.BMS04.Cell_Temp[14], Device.BMS04.Cell_Temp[15], Device.BMS04.Cell_Temp[16], Device.BMS04.Cell_Temp[17], Device.BMS04.Cell_Temp[18], Device.BMS04.Cell_Temp[19], Device.BMS04.Cell_Temp[20], Device.BMS04.Cell_Temp[21], Device.BMS04.Cell_Temp[22], Device.BMS04.Cell_Temp[23], Device.BMS04.Cell_Temp[24], Device.BMS04.Cell_Temp[25], Device.BMS04.Cell_Temp[26], Device.BMS04.Cell_Temp[27], Device.BMS04.Cell_Temp[28], Device.BMS04.Cell_Temp[29]
                                                      , Device.BMS04.Cell_Temp[30], Device.BMS04.Cell_Temp[31], Device.BMS04.Cell_Temp[32], Device.BMS04.Cell_Temp[33], Device.BMS04.Cell_Temp[34], Device.BMS04.Cell_Temp[35], Device.BMS04.Cell_Temp[36], Device.BMS04.Cell_Temp[37], Device.BMS04.Cell_Temp[38], Device.BMS04.Cell_Temp[39], Device.BMS04.Cell_Temp[40], Device.BMS04.Cell_Temp[41], Device.BMS04.Cell_Temp[42], Device.BMS04.Cell_Temp[43], Device.BMS04.Cell_Temp[44], Device.BMS04.Cell_Temp[45], Device.BMS04.Cell_Temp[46], Device.BMS04.Cell_Temp[47], Device.BMS04.Cell_Temp[48], Device.BMS04.Cell_Temp[49], Device.BMS04.Cell_Temp[50], Device.BMS04.Cell_Temp[51], Device.BMS04.Cell_Temp[52], Device.BMS04.Cell_Temp[53], Device.BMS04.Cell_Temp[54], Device.BMS04.Cell_Temp[55], Device.BMS04.Cell_Temp[56], Device.BMS04.Cell_Temp[57], Device.BMS04.Cell_Temp[58], Device.BMS04.Cell_Temp[59]
                                                      , Device.BMS04.Cell_Temp[60], Device.BMS04.Cell_Temp[61], Device.BMS04.Cell_Temp[62], Device.BMS04.Cell_Temp[63], Device.BMS04.Cell_Temp[64], Device.BMS04.Cell_Temp[65], Device.BMS04.Cell_Temp[66], Device.BMS04.Cell_Temp[67]
                                                    } },
                    */
                    { "Contactors_Status" ,new BsonArray {Device.BMS01.Contactors_Status, Device.BMS02.Contactors_Status, Device.BMS03.Contactors_Status, Device.BMS04.Contactors_Status } },
                    { "Positive_Contactor" ,new BsonArray {Device.BMS01.Positive_Contactor, Device.BMS02.Positive_Contactor, Device.BMS03.Positive_Contactor, Device.BMS04.Positive_Contactor } },//EMS
                    { "Precharge_Contactor" ,new BsonArray {Device.BMS01.Precharge_Contactor, Device.BMS02.Precharge_Contactor, Device.BMS03.Precharge_Contactor, Device.BMS04.Precharge_Contactor } },//EMS
                    //{ "Negative_Contactor" ,new BsonArray {Device.BMS01.Negative_Contactor, Device.BMS02.Negative_Contactor, Device.BMS03.Negative_Contactor, Device.BMS04.Negative_Contactor } },//EMS(CATL沒有)

                    { "BMS_Run_State" ,new BsonArray {Device.BMS01.BMS_Run_State, Device.BMS02.BMS_Run_State, Device.BMS03.BMS_Run_State, Device.BMS04.BMS_Run_State } },//EMS

                    { "BMS_Error" ,new BsonArray {Device.BMS01.BMS_Error, Device.BMS02.BMS_Error, Device.BMS03.BMS_Error, Device.BMS04.BMS_Error } },
                    { "BMS_Module_Error" ,new BsonArray {Device.BMS01.Module_Error, Device.BMS02.Module_Error, Device.BMS03.Module_Error, Device.BMS04.Module_Error } },
                    { "BMS_Error_Warning" ,new BsonArray {Device.BMS01.BMS_Error_Warning, Device.BMS02.BMS_Error_Warning, Device.BMS03.BMS_Error_Warning, Device.BMS04.BMS_Error_Warning } },
                    { "BMS_Error_Stop" ,new BsonArray {Device.BMS01.BMS_Error_Stop, Device.BMS02.BMS_Error_Stop, Device.BMS03.BMS_Error_Stop, Device.BMS04.BMS_Error_Stop } },
                    { "BMS_Error_Protection" ,new BsonArray {Device.BMS01.BMS_Error_Protection, Device.BMS02.BMS_Error_Protection, Device.BMS03.BMS_Error_Protection, Device.BMS04.BMS_Error_Protection } },
                    { "BMS_Other_Error" ,new BsonArray {Device.BMS01.BMS_Other_Error, Device.BMS02.BMS_Other_Error, Device.BMS03.BMS_Other_Error, Device.BMS04.BMS_Other_Error } },

                    { "EMS_Error",Device.Ems_error},{"EMS_Error1",Device.Ems_error}
            });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mongo_MBMS_CATL Error");
                Console.WriteLine(ex);
            }
        }
        private void Mongo_Reset(IMongoDatabase db, string Slave, string error_msg, DateTime time)
        {//輸入ID還有event ，會把這個事件復歸
            //try
            //{
            //    event_log.Add(new Event_Log(time, Slave, error_msg, "null"));
            //    if (event_log.Count > 50)
            //    {
            //        event_log.RemoveAt(0);
            //    }
            //}
            //catch
            //{
            //    Console.WriteLine("Reset Error");
            //}
            try
            {
                DateTime last_event = DateTime.Now;
                string event_str;
                var sort = Builders<BsonDocument>.Sort.Descending("time");
                var coll = db.GetCollection<BsonDocument>("alarm");  //指定寫入給"categories"此collection  
                var filter = Builders<BsonDocument>.Filter.Eq("ID", Slave) & Builders<BsonDocument>.Filter.Eq("event", error_msg);

                var cursor = coll.Find(filter).Sort(sort).Limit(1).ToList();
                foreach (var event_log in cursor)
                {
                    last_event = (DateTime)event_log.GetValue("time");
                    event_str = event_log.GetValue("event").ToString();
                }
                var filter2 = Builders<BsonDocument>.Filter.Eq("ID", Slave) & Builders<BsonDocument>.Filter.Eq("event", error_msg) & Builders<BsonDocument>.Filter.Eq("time", last_event);
                var update = Builders<BsonDocument>.Update.Set("returntime", DateTime.Now)
                                                          .Set("EMS_RESET_Error", RESET_Error.EMS_Error);
                coll.UpdateOne(filter2, update);
                //coll.Find(filter).Sort(sort)
            }
            catch
            {
                Console.WriteLine("Reset Error");
            }

        }

        private void Mongo_error(IMongoDatabase db, string Slave, string error_msg, DateTime time)
        {
            //try
            //{
            //    // lv_Print(listView1, time.ToString(), Slave, "1",error_msg);
            //    event_log.Add(new Event_Log(time, Slave, error_msg, "null"));
            //    if (event_log.Count > 50)
            //    {
            //        event_log.RemoveAt(0);
            //    }
            //}
            //catch
            //{
            //    Console.WriteLine("Error Error");
            //}
            try
            {
                var coll = db.GetCollection<BsonDocument>("alarm");  //指定寫入給"categories"此collection  
                coll.InsertOne(new BsonDocument { { "time", DateTime.Now.AddHours(time_offset) },
                    { "ID", Slave },{"type","" }, { "event",error_msg },{"checktime",""},{"returntime",""},{ "level", 1.0 },{"EMS_Error",Error.EMS_Error},{"EMS_Error1",Error.EMS_Error}
                    ,{"show",1.0},{"EMS_RESET_Error",RESET_Error.EMS_Error},{"EMS_RESET_Error1",RESET_Error.EMS_Error}
                });
            }
            catch
            {
                //Console.WriteLine("MongoERROR");
                Console.WriteLine("Error Error");

                Debug.Print("Error Error");
            }
        }
        private void Mongo_EMeter_TMM70(TMM70 Device, DateTime time_now)
        {
            BsonDocument document = new BsonDocument();
            //////{"Error", new BsonArray {new BsonDocument("Error1", PCS1.Error1),new BsonDocument("Error2", PCS1.Error2),new BsonDocument("Error3", PCS1.Error3),new BsonDocument("Error4", PCS.Error4)}}
            try
            {
                var coll = ems_db.GetCollection<BsonDocument>("COM3_meter");  //指定寫入給"categories"此collection 
                BsonDocument data = new BsonDocument { { "time", time_now.AddHours(time_offset)}, { "ID", Device.Device_ID} ,
                    {"f",Device.F },{"v_avg",Device.V_avg },{"vl_avg",Device.Vl_avg },{"i_avg",Device.I_avg },{"i_n",Device.I_n },
                    {"pf_avg",Device.Pf_avg },{"v1",Device.V1 },{"v2",Device.V2 },{"v3",Device.V3 },{"v12",Device.V12 },{"v23",Device.V23 },
                    {"v31",Device.V31 },{"i1",Device.I1 },{"i2",Device.I2 },{"i3",Device.I3 },{"p1",Device.P1 },{"p2",Device.P2 },{"p3",Device.P3 },
                    {"q1",Device.Q1 },{"q2",Device.Q2 },{"q3",Device.Q3 },{"s1",Device.S1 },{"s2",Device.S2 },{"s3",Device.S3 },{"pf1",Device.Pf1 },
                    {"pf2",Device.Pf2 },{"pf3",Device.Pf3 },
                    { "kwh_in",Device.Kwh_in },{"kwh_out",Device.Kwh_out },{"kwh_total",Device.Kwh_total },{"kwh_net",Device.Kwh_net },
                    { "kvarh_in",Device.Kvarh_in },{"kvarh_out",Device.Kvarh_out },{"kvarh_total",Device.Kvarh_total },{"kvarh_net",Device.Kvarh_net },
                    { "kvarh_in",Device.Kvarh_in },{"kvarh_out",Device.Kvarh_out },{"kvarh_total",Device.Kvarh_total },{"kvarh_net",Device.Kvarh_net },
                    { "kvarh_in",Device.Kvarh_in },{"kvarh_out",Device.Kvarh_out },{"kvarh_total",Device.Kvarh_total },{"kvarh_net",Device.Kvarh_net },

                     { "THDU12",Device.Thd_u12 },{"THDU23",Device.Thd_u23 },{"THDU31",Device.Thd_u31 },{"THDUavg",Device.Thd_u_avg },
                    { "THDI1",Device.Thd_i1 },{"THDI2",Device.Thd_i2 },{"THDI3",Device.Thd_i3 },{"THDIavg",Device.Thd_i_avg },

            };
                coll.InsertOne(data);
                Device.Ems_error = 0;
                //if (data.GetValue("_id")!="")
                //Device.Ems_error = 0;
                //else { }

            }
            catch
            {
                Device.Ems_error = 1;
                Device.Ems_flag = true;
                Console.WriteLine("ETMM70 Error");
            }
        }
        #endregion

        #region 建立modbus master 
        private void set_serial(ref SerialPort serial_obj, ref ModbusSerialMaster SerialMaster, string PortName, int BaudRate)
        {
            try
            {
                serial_obj.PortName = System.Configuration.ConfigurationManager.AppSettings[PortName];
                serial_obj.BaudRate = BaudRate;
                serial_obj.DataBits = 8;
                serial_obj.Parity = Parity.None;
                serial_obj.StopBits = StopBits.One;
                serial_obj.Open();
                SerialMaster = ModbusSerialMaster.CreateRtu(serial_obj);
                Debug.Print(DateTime.Now.ToString() + " =>Open " + serial_obj.PortName + " sucessfully!");
            }
            catch
            {
                serial_obj.Close();
                Thread.Sleep(2000);
                serial_obj.Open();
                Debug.Print(DateTime.Now.ToString() + " =>Disconnect " + serial_obj.PortName);
            }
        }
        #endregion
        #region 我寫的副程式 (拿旗標寫旗標)
        private string ggetbit(int value, int bit_number)//取出16位元的幾個bit，輸入 數值  第幾個bit 
        {
            string flag;
            int Substring_bit = 15 - bit_number;
            flag = Convert.ToString(value, 2).PadLeft(16, '0');
            //Debug.Print("flag "+flag);
            //Debug.Print("return " + flag.Substring(Substring_bit, 1));
            return flag.Substring(Substring_bit, 1);
        }
        private int writeBit(int value, int bit_number, char bit_value) //輸入數值 要更改的第幾個bit  要更改的數值 回傳整個暫存器 更改好的數值  
        {
            string flag;
            int Substring_bit = 15 - bit_number;
            flag = Convert.ToString(value, 2).PadLeft(16, '0');
            var c = flag.ToCharArray();//分開成 char array
            c[Substring_bit] = bit_value; //修改
            string str = new string(c); //合併 char array to string 
            return Convert.ToInt32(str, 2);
        }
        private void ref_writeBit(ref int value, int bit_number, char bit_value) //輸入數值 要更改的第幾個bit  要更改的數值 回傳整個暫存器 更改好的數值  
        {
            string flag;
            int Substring_bit = 15 - bit_number;
            flag = Convert.ToString(value, 2).PadLeft(16, '0');
            var c = flag.ToCharArray();//分開成 char array
            c[Substring_bit] = bit_value; //修改
            string str = new string(c); //合併 char array to string 
            value = Convert.ToInt32(str, 2);
        }
        #endregion
        #region listview
        public static void lv_Print(ListView list, string message)// 輸入listview ,兩個str
        {
            String time = DateTime.Now.ToString();
            //判斷這個TextBox的物件是否在同一個執行緒上
            if (list.InvokeRequired)
            {
                Listview_Print ph = new Listview_Print(lv_Print);
                list.Invoke(ph, list, time, message);
            }
            else
            {
                String[] row = { time, message };
                ListViewItem item = new ListViewItem(row);
                //ADD ITEMS
                list.Items.Add(item);
                if (list.Items.Count > 1000)
                {
                    list.Items.RemoveAt(1);
                }
            }
        }
        public static void lv_Print(ListView list, string message1, string message2)// 輸入listview ,兩個str
        {
            //判斷這個TextBox的物件是否在同一個執行緒上
            if (list.InvokeRequired)
            {
                Listview_Print ph = new Listview_Print(lv_Print);
                list.Invoke(ph, list, message1, message2);
            }
            else
            {
                String[] row = { message1, message2 };
                ListViewItem item = new ListViewItem(row);
                //ADD ITEMS
                list.Items.Add(item);
                if (list.Items.Count > 1000)
                {
                    list.Items.RemoveAt(1);
                }
            }
        }
        private void InitialLv()
        {
            lv.View = View.Details;
            lv.GridLines = true;
            lv.LabelEdit = false;
            lv.FullRowSelect = true;
            lv.Columns.Add("message1", 150);
            lv.Columns.Add("message2", 200);
            lv.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
| System.Reflection.BindingFlags.NonPublic).SetValue(lv, true, null);
        }
        public delegate void Listview_Print(ListView list, string time, string type);//time type 沒改
        public delegate void lPrintHandler(Label label, string text);
        #endregion
        private void button3_Click(object sender, EventArgs e)
        {
            timer_read_upload.Enabled = true;
        }

        private void timer_read_upload_Tick(object sender, EventArgs e)
        {//讀取pcs資料並且上傳 
            //讀取資料
            #region Modbus 4 通訊測試
            //DateTime new_time = DateTime.Now;
            try
            {
                //last_time_dg = new_time;
                master_pcs.Transport.Retries = 0;   //don't have to do retries
                master_pcs.Transport.ReadTimeout = 500; //milliseconds
                ushort[] holdingregister_dc = master_pcs.ReadHoldingRegisters(1,bat_v_address , 2);

                Debug.Print("bat_v" + holdingregister_dc[0].ToString());
                Debug.Print("bat_i" + holdingregister_dc[1].ToString());
                PCS1.V_dc = holdingregister_dc[0];
                PCS1.I_dc = holdingregister_dc[1];
                Thread.Sleep(200);
                ushort[] holdingregister_ac = master_pcs.ReadHoldingRegisters(1, ac_v_address, 2);
                PCS1.V_out2 = holdingregister_ac[0];
                //
                ushort[] holdingregister_commend = master_pcs.ReadHoldingRegisters(1, commend_mode, 1);
                if (holdingregister_commend[0]==1)
                {
                    lv_Print(lv,"放電", (PCS1.V_dc * PCS1.I_dc).ToString() + "W");
                }
                if (holdingregister_commend[0] == 2)
                {
                    lv_Print(lv, "充電", (PCS1.V_dc* PCS1.I_dc).ToString()+"W");
                }
            }
            catch (Exception ex)
            {
                Debug.Print("modbus Exception" + ex.Message);
            }
            #endregion
            //上傳
            try
            {
                DateTime new_time = DateTime.Now;
                Mongo_PCS(ems_db,PCS1, new_time);
            }
            catch (Exception ex)
            {
                Debug.Print("modbus Exception" + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            master_pcs.WriteSingleRegister(1,commend_charge_i,10);
            master_pcs.WriteSingleRegister(1, commend_discharge_i, 10);
        }

        private void timertrickAtBeginning_Tick(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {


            
            //上傳故障 
            DateTime time_now = DateTime.Now;
            string ID = "5da46627183ced1a330f6d1f";
            //Mongo_error(local_db, ID,"error test7897", time_now);
            //Mongo_Reset(local_db, ID, "error test7897", time_now);

        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            byte slaveid = 1;
            ushort value = 10;

            ushort a = (ushort)Convert.ToInt32("0x3110", 10);
            Debug.Print("a :" + a);
            master_pcs.WriteSingleRegister(slaveid, (ushort)13313, value);//commend_charge_i
            int decValue = int.Parse(bat_v_address.ToString(), System.Globalization.NumberStyles.HexNumber)-1;
            Debug.Print("decValue" + decValue);
            ushort[] holdingregister_dc = master_pcs.ReadHoldingRegisters(1, (ushort)decValue, 2);
            //13 會寫 14 
            Debug.Print("bat_v" + holdingregister_dc[0].ToString());
            Debug.Print("bat_i" + to_2complement (holdingregister_dc[1]).ToString());
            #region pcs上傳資料
            ushort[] pcs_data = new ushort[53];
            for (ushort i = 0; i < pcs_data.Length; i++)
            {
                pcs_data[i] = i;
                if (i == 23)
                {
                    int ii = i + i + i;
                    pcs_data[i] = (ushort)ii;
                }

            }
                Debug.Print("pcs_data 製作完成 ");
                
                PCS1.Holding_register = pcs_data;
                PCS1.Put_Data1();
                PCS1.Device_ID = "5da46627183ced1a330f6d1f";
                Mongo_PCS(ems_db, PCS1, DateTime.Now);
                Debug.Print("pcs upload ");
                label1.Text = "pcs_data upload "+ DateTime.Now.ToString();
                Debug.Print(DateTime.Now.ToString());



            #region Mbms上傳資料 
            ushort[] mbms_data1 = new ushort[150];
            ushort[] mbms_data2 = new ushort[150];
            for (ushort i = 0; i < mbms_data1.Length; i++)
            {
                mbms_data1[i] = i;
                int ii = i + i;
                mbms_data2[i] = (ushort)ii;
            }
            //while (true)
            //{

            //    Debug.Print("mbms_data 製作完成 ");
            //    MBMS1.holding_register = mbms_data1;
            //    MBMS1.holding_register_2 = mbms_data2;
            //    MBMS1.holding_register[5] = 12850; //23 32 14 
            //    MBMS1.Put_Data1();
            //    MBMS1.Device_ID = "5da5a124ebb30cbaa5308210";
            //    Mongo_MBMS(local_db, MBMS1, DateTime.Now);
            //    Debug.Print("MBMS1 upload ");
            //    Thread.Sleep(1000);

            //    Debug.Print(DateTime.Now.ToString());
            //}
            #endregion

            #endregion

        }
        private ushort negative2complement(double num)
        {
            if (num < 0)
            {
                return (ushort)(65536 + num);
            }
            else
            {
                return (ushort)num;
            }
        }
        private int to_2complement(int value)
        {//把功率轉換成2的補數 
            if (value > 32768)
            {

                return value - 65536;
            }
            else
            {
                return value;
            }
            //Now value is -100
        }
        private int negative2complement(int value)
        {//把功率轉換成2的補數 
            if (value < 0)
            {
                //return value + 256;
                return value + 65536;
            }
            else
            {
                return value;
            }
            //Now value is -100
        }
    }
}
