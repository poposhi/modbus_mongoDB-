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
        #endregion
        #region MongoDB2 宣告變數 
        private MongoClient dbconn;
        public static IMongoDatabase db;//為了要讓其他的物件也可以使用 
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
            db = dbconn.GetDatabase(local_db_name);  //資料庫名稱 
                                                     //this.dbconn = new MongoClient(mlabconn);   //設立連線  
                                                     //this.db = dbconn.GetDatabase("solar");  //資料庫名稱   


            ////Server端，MongoDB連線Timeout設定
            MongoIdentity identity = new MongoInternalIdentity("admin", "root"); //遠端資料庫帳號密碼 
            MongoIdentityEvidence evidence = new PasswordEvidence("pc152");
            MongoClientSettings esettings = new MongoClientSettings();
            esettings.WaitQueueSize = int.MaxValue;
            esettings.ConnectTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            esettings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            esettings.SocketTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            esettings.WaitQueueTimeout = new TimeSpan(0, 0, 0, 0, 2000);
            //esettings.Server = new MongoServerAddress("140.118.172.75");
            esettings.Server = new MongoServerAddress("119.252.117.54");
            esettings.Credential = new MongoCredential(null, identity, evidence);
            this.ems_dbconn = new MongoClient(esettings);
            this.ems_db = ems_dbconn.GetDatabase(remote_db_name);  //資料庫名稱 

            #endregion

            read_all_coll();
        }
        #region mongodb 使用
        //讀取整個collection，並且上傳到本地端 
        private void read_all_coll ()
        {
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
            var coll_local = db.GetCollection<BsonDocument>("equipment");  //指定寫入給"categories"此collection  
            coll_local.InsertOne();
        }
        //測試是否可以上傳資料 
        private void mongo_test(DateTime time_now,string coll_name)
        {
            int time_offset = 8;
            var coll = db.GetCollection<BsonDocument>(coll_name);  //指定寫入給"categories"此collection  
            coll.InsertOne(new BsonDocument { { "time", time_now.AddHours(time_offset) } });
        }
        #endregion
        #region 建立modbus master 

        #endregion
    }
}
