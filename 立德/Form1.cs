///這個程式主要是用來快速建立一個電網控制器 ，能夠用文字編輯器設定 是否開啟，本地遠端資料庫，pcs modbus設定，多少秒讀取一次 各項變數是第幾個資料 
///
using System;
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
        string local_db_name= "solar"; //本地資料庫名稱 
        string remote_db_name= "solar";
        //遠端資料庫IP 
        public PCS PCS1 = new PCS();
        int time_offset = 8;
        #endregion
        #region MongoDB2 宣告變數 
        private MongoClient dbconn;
        public static IMongoDatabase local_db;//為了要讓其他的物件也可以使用 
        private MongoClient ems_dbconn;
        private IMongoDatabase ems_db;
        //private string mlabconn = "mongodb://localhost:27017/?wtimeoutMS=200";  //mlab提供的連線字串 
        //private string mlabconn = "mongodb://tsai_user:0000@localhost:27017";
        #endregion
        public Form1()
        {
            InitializeComponent();
            #region MongoDB3 連線建立 連線設定

            ////Local端，MongoDB連線Timeout設定
            MongoClientSettings settings = new MongoClientSettings();
            settings.WaitQueueSize = int.MaxValue;
            settings.ConnectTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.SocketTimeout = new TimeSpan(0, 0, 0, 0, 1000);
            settings.WaitQueueTimeout = new TimeSpan(0, 0, 0, 0, 0100);
            settings.Server = new MongoServerAddress("localhost");
            this.dbconn = new MongoClient(settings);
            //this.db = dbconn.GetDatabase("Tsai_Test");  //資料庫名稱 
            local_db = dbconn.GetDatabase(local_db_name);  //資料庫名稱 
                                                     //this.dbconn = new MongoClient(mlabconn);   //設立連線  
                                                     //this.db = dbconn.GetDatabase("solar");  //資料庫名稱   


            ////Server端，MongoDB連線Timeout設定
            //MongoIdentity identity = new MongoInternalIdentity("admin", "root"); //遠端資料庫帳號密碼 
            //MongoIdentityEvidence evidence = new PasswordEvidence("pc152");
            //MongoClientSettings esettings = new MongoClientSettings();
            //esettings.WaitQueueSize = int.MaxValue;
            //esettings.ConnectTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            //esettings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            //esettings.SocketTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            //esettings.WaitQueueTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            ////esettings.Server = new MongoServerAddress("140.118.172.75");
            //esettings.Server = new MongoServerAddress("119.252.117.54");
            //esettings.Credential = new MongoCredential(null, identity, evidence);
            //this.ems_dbconn = new MongoClient(esettings);
            //this.ems_db = ems_dbconn.GetDatabase(remote_db_name);  //資料庫名稱 

            #endregion

            #region pcs上傳資料
            ushort[] pcs_data = new ushort[53];
            for (ushort i = 0; i < pcs_data.Length; i++)
            {
                pcs_data[i] = i;
            }
            while (true)
            {
                Debug.Print("pcs_data 製作完成 ");
                PCS1.Holding_register = pcs_data;
                PCS1.Put_Data1();
                PCS1.Device_ID = "5da46627183ced1a330f6d1f";
                Mongo_PCS(local_db, PCS1, DateTime.Now);
                Debug.Print("pcs upload ");
                Thread.Sleep(60000);

                Debug.Print(DateTime.Now.ToString());
            }

            #endregion
        }
        #region mongodb 使用
        //讀取整個collection，並且上傳到本地端 並沒有完成 
        private void read_all_coll ()
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
        private void mongo_test(DateTime time_now,string coll_name)
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
            catch(Exception e)
            {
                Device.Ems_error = 1;
                Device.Ems_flag = true;
                Console.WriteLine("EPCS Error");

                Debug.Print(e.Message);
            }
        }
        #endregion
        #region 建立modbus master 

        #endregion
    }
}
