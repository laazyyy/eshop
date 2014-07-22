using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using wangxut;
using wangxu;
using MySQLDriverCS;
using System.Text;
using Newtonsoft.Json;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    public Service () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }
    
    [WebMethod]
    public String getPizzaShackMenuItems()
    {
        MenuItem[] items = new MenuItem[20];
        MenuItem item1 = new MenuItem();
        item1.setName("Chicken Parmesan");
        item1.setDescription("Grilled chicken, fresh tomatoes, feta and mozzarella cheese");
        item1.setPrice("10");
        item1.setIcon("/images/1.png");
        items[0] = item1;

        MenuItem item2 = new MenuItem();
        item2.setName("Spicy Italian");
        item2.setDescription("Pepperoni and a double portion of spicy Italian sausage");
        item2.setPrice("10");
        item2.setIcon("/images/2.png");
        items[1] = item2;

        MenuItem item3 = new MenuItem();
        item3.setName("Garden Fresh");
        item3.setDescription("Slices onions and green peppers, gourmet " +
                "mushrooms, black olives and ripe Roma tomatoes");
        item3.setPrice("10");
        item3.setIcon("/images/3.png");
        items[2] = item3;

        MenuItem item4 = new MenuItem();
        item4.setName("Tuscan Six Cheese");
        item4.setDescription("Six cheese blend of mozzarella, Parmesan, Romano, Asiago and Fontina");
        item4.setPrice("10");
        item4.setIcon("/images/4.png");
        items[3] = item4;

        MenuItem item5 = new MenuItem();
        item5.setName("Spinach Alfredo");
        item5.setDescription("Rich and creamy blend of spinach and garlic Parmesan with Alfredo sauce");
        item5.setPrice("10");
        item5.setIcon("/images/5.png");
        items[4] = item5;

        MenuItem item6 = new MenuItem();
        item6.setName("BBQ Chicken Bacon");
        item6.setDescription("Grilled white chicken, hickory-smoked bacon and fresh sliced onions in barbeque sauce");
        item6.setPrice("10");
        item6.setIcon("/images/6.png");
        items[5] = item6;

        MenuItem item7 = new MenuItem();
        item7.setName("Hawaiian BBQ Chicken");
        item7.setDescription("Grilled white chicken, hickory-smoked bacon, barbeque sauce topped with sweet pine-apple");
        item7.setPrice("34");
        item7.setIcon("/images/7.png");
        items[6] = item7;

        MenuItem item8 = new MenuItem();
        item8.setName("Grilled Chicken Club");
        item8.setDescription("Grilled white chicken, hickory-smoked bacon and fresh sliced onions topped with Roma tomatoes");
        item8.setPrice("440");
        item8.setIcon("/images/8.png");
        items[7] = item8;

        MenuItem item9 = new MenuItem();
        item9.setName("Double Bacon 6Cheese");
        item9.setDescription("Hickory-smoked bacon, Julienne cut Canadian bacon, Parmesan, " +
                "mozzarella, Romano, Asiago and and Fontina cheese");
        item9.setPrice("103");
        item9.setIcon("/images/9.png");
        items[8] = item9;

        MenuItem item10 = new MenuItem();
        item10.setName("Chilly Chicken Cordon Bleu");
        item10.setDescription("Spinash Alfredo sauce topped with grilled chicken, ham, onions and " +
                "mozzarella");
        item10.setPrice("103");
        item10.setIcon("/images/10.png");
        items[9] = item10;
        String s1 = null;
        
        for (int i = 0; i < 9; i++ )
        {
            s1 += items[i].getName() + " " + items[i].getDescription() + " ";
        }
        return s1;
    }

    [WebMethod]
    public bool adminLogin(String loginName, String loginPwd)
    {
        MySQLConnection DBConn = MysqlDataBaseConnection();
        
        MySQLCommand DBComm;
        DBComm = new MySQLCommand("select LoginName, LoginPwd from admin", DBConn);

        MySQLDataReader DBReader = DBComm.ExecuteReaderEx();
        // 显示数据
        try
        {
            while (DBReader.Read())
            {
                if (loginName.Equals(DBReader.GetString(0)) && loginPwd.Equals(DBReader.GetString(1)))
                {
                    return true;
                }
            }
        }
        finally
        {
            DBReader.Close();
            DBConn.Close();
        } 

        return false;
    }
    
    [WebMethod]
    public bool addAdmin(String adminJsonString) 
    {

        //{'ID':'5','AdminType':'5','AdminName':'wangxu','LoginName':'wangxu','LoginPwd':'wangxu'}

        JObject js = JObject.Parse(@adminJsonString);
        int ID = Convert.ToInt32(js["ID"]);
        int AdminType = Convert.ToInt32(js["AdminType"]);
        String AdminName = (String)js["AdminName"];
        String LoginName = (String)js["LoginName"];
        String LoginPwd = (String)js["LoginPwd"];
        
        MySQLConnection DBConn = MysqlDataBaseConnection();
        MySQLCommand DBComm;
        StringBuilder sb = new StringBuilder();
        //if (ID!=null && AdminType != null && AdminName != null && LoginName != null && LoginPwd != null)
        //{
        sb.Append("INSERT INTO admin SET AdminType=").Append(AdminType).Append(",").Append(" AdminName=").Append("'").Append(AdminName).Append("'").Append(",").Append(" LoginName=").Append("'").Append(LoginName).Append("'").Append(",").Append(" LoginPwd=").Append("'").Append(LoginPwd).Append("'");
        //return sb.ToString();
        DBComm = new MySQLCommand(sb.ToString(), DBConn);
        if (DBComm.ExecuteNonQuery() > 0)
        {
            return true;
        }
        return false;
        
        //else
        //{
        //    return false;
        //}
    }

    [WebMethod]
    public string browseAdmin()
    {
        MySQLConnection DBConn = MysqlDataBaseConnection();
        MySQLCommand DBComm;
        
        DBComm = new MySQLCommand("select ID,AdminType,AdminName,LoginName,LoginPwd from admin", DBConn);
        MySQLDataReader DBReader = DBComm.ExecuteReaderEx();
        List<Admin> adminList = new List<Admin>();
        try
        {
            while (DBReader.Read())
            {
                int ID = DBReader.GetInt32(0);
                int AdminType = DBReader.GetInt32(1);
                String AdminName = DBReader.GetString(2);
                String LoginName = DBReader.GetString(3);
                String LoginPwd = DBReader.GetString(4);
                Admin admin = new Admin();
                admin.id = ID;
                admin.adminType = AdminType;
                admin.adminName = AdminName;
                admin.loginName = LoginName;
                admin.loginPwd = LoginPwd;
                adminList.Add(admin);
            }
        }
        finally
        {
            DBReader.Close();
            DBConn.Close();
        }


        String result = "";
        try
        {
            result = JsonConvert.SerializeObject(adminList, Formatting.Indented);
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }
        return result;
    }

    [WebMethod]
    public bool delAdmin(int ID)
    {
        MySQLConnection DBConn = MysqlDataBaseConnection();
        MySQLCommand DBComm;
        StringBuilder sb = new StringBuilder();
        sb.Append("DELETE FROM admin  WHERE ID = ").Append("'").Append(ID).Append("'");
        DBComm = new MySQLCommand(sb.ToString(), DBConn);
        if (DBComm.ExecuteNonQuery() > 0)
        {
            return true;
        }
        return false;
    }
    
    [WebMethod]
    public String loadAdmin(int id)
    {
        MySQLConnection DBConn = MysqlDataBaseConnection();
        MySQLCommand DBComm;
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT * FROM admin  WHERE ID = ").Append("'").Append(id).Append("'");
        DBComm = new MySQLCommand(sb.ToString(), DBConn);
        MySQLDataReader DBReader = DBComm.ExecuteReaderEx();
        List<Admin> adminList = new List<Admin>();
        try
        {
            while (DBReader.Read())
            {
                int ID = DBReader.GetInt32(0);
                int AdminType = DBReader.GetInt32(1);
                String AdminName = DBReader.GetString(2);
                String LoginName = DBReader.GetString(3);
                String LoginPwd = DBReader.GetString(4);
                Admin admin = new Admin();
                admin.id = ID;
                admin.adminType = AdminType;
                admin.adminName = AdminName;
                admin.loginName = LoginName;
                admin.loginPwd = LoginPwd;
                adminList.Add(admin);
            }
        }
        finally
        {
            DBReader.Close();
            DBConn.Close();
        }
        return JsonConvert.SerializeObject(adminList, Formatting.Indented);
    }

    [WebMethod]
    public bool updateAdmin(String adminJsonString)
    {
        MySQLConnection DBConn = MysqlDataBaseConnection();
        MySQLCommand DBComm;

        JObject js = JObject.Parse(@adminJsonString);
        int ID = Convert.ToInt32(js["ID"]);
        int AdminType = Convert.ToInt32(js["AdminType"]);
        String AdminName = (String)js["AdminName"];
        String LoginName = (String)js["LoginName"];
        String LoginPwd = (String)js["LoginPwd"];
        StringBuilder sb1 = new StringBuilder();
        sb1.Append("UPDATE admin SET AdminType = ").Append(AdminType).Append(",").Append(" AdminName=").Append("'").Append(AdminName).Append("'").Append(",").Append(" LoginName=").Append("'").Append(LoginName).Append("'").Append(",").Append(" LoginPwd=").Append("'").Append(LoginPwd).Append("'").Append(" WHERE ID = ").Append("'").Append(ID).Append("'");
        DBComm = new MySQLCommand(sb1.ToString(), DBConn);
        if (DBComm.ExecuteNonQuery() > 0)
        {
            return true;
        }
        return false;
    }
    
    [WebMethod]
    public string jsonTest()
    {
        Account account = new Account
        {
        Email = "james@example.com",
        Active = true,
        CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
        Roles = new List<string>
        {
         "User",
         "Admin"
         }
        };
        return JsonConvert.SerializeObject(account, Formatting.Indented);
    }

    public MySQLConnection MysqlDataBaseConnection()
    {
        MySQLConnection DBConn;
        DBConn = new MySQLConnection(new MySQLConnectionString("10.1.35.230", "db_eshop", "root", "", 3306).AsString);

        try
        {
            DBConn.Open();
        }
        catch (Exception e)
        {
            String str = e.Message;
        }
        // 执行查询语句
        MySQLCommand setformat = new MySQLCommand("set names gb2312", DBConn);
        setformat.ExecuteNonQuery();
        setformat.Dispose();
        return DBConn;
    }

    [WebMethod]
    public String browseMemberLevel(){
        String ml = null;
        MySQLConnection DBConn;
        DBConn = new MySQLConnection(new MySQLConnectionString("10.1.35.230", "db_eshop", "root", "", 3306).AsString);

        DBConn.Open();
        // 执行查询语句
        MySQLCommand setformat = new MySQLCommand("set names gb2312", DBConn);
        setformat.ExecuteNonQuery();
        setformat.Dispose();

        MySQLCommand DBComm;
        
        DBComm = new MySQLCommand("select ID, LevelName, Favourable from memberlevel as a order by a.id", DBConn);
        // 读取数据


        MySQLDataReader DBReader = DBComm.ExecuteReaderEx();
        // 显示数据
        try
        {
            while (DBReader.Read())
            {
                ml += "ID = " + DBReader.GetString(0) + " LevelName = " + DBReader.GetString(1) + " Favourable = " + DBReader.GetString(2) + "        ";
            }
            Console.Write(ml);
        }
        finally
        {
            DBReader.Close();
            DBConn.Close();
        } 
        return ml;
    }
    
    [WebMethod(Description = "需要 Access Token 认证")]
    public String getJeeShopHotProducts(String AccessToken)
    {
        String ml = null;
        MySQLConnection DBConn;
        DBConn = new MySQLConnection(new MySQLConnectionString("10.1.35.233", "jeeshop1_0", "root", "123456", 3306).AsString);


        DBConn.Open();
        // 执行查询语句
        MySQLCommand setformat = new MySQLCommand("set names gb2312", DBConn);
        setformat.ExecuteNonQuery();
        setformat.Dispose();

        MySQLCommand DBComm;

        DBComm = new MySQLCommand("select t.id ,t.catalogID ,t.name,t.picture,t.price,t.nowPrice from t_product t where 1=1 and status=2", DBConn);
        // 读取数据


        MySQLDataReader DBReader = DBComm.ExecuteReaderEx();
        // 显示数据
        try
        {
            while (DBReader.Read())
            {
                ml += "id = " + DBReader.GetString(0)
                   + " catalogID = " + DBReader.GetString(1)
                   + " name = " + DBReader.GetString(2)
                   + " picture = " + DBReader.GetString(3)
                   + " Price = " + DBReader.GetString(4)
                   + " NowPrice = " + DBReader.GetString(5);
            }
            Console.Write(ml);
        }
        finally
        {
            DBReader.Close();
            DBConn.Close();
        }
        return ml;
    }

}