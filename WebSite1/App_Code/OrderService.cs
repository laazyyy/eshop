using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using MySQLDriverCS;
using System.Text;
using wql;
using Newtonsoft.Json;

/// <summary>
///OrderService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class OrderService : System.Web.Services.WebService {

    public OrderService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    // 新增订单
    [WebMethod]
    public bool addOrder(String addOrderJsonStr) 
    {
        bool result = false;

        try
        {
            //{'MemberId':'8','CartId':'8','OrderNo':'1240299319330','OrderDate':'2014-7-22 11:30:30','OrderStatus':'2'}
            JObject jObj = JObject.Parse(@addOrderJsonStr);
            int memberId = Convert.ToInt32(jObj["MemberId"]);
            int cartId = Convert.ToInt32(jObj["CartId"]);
            String orderNo = Convert.ToString(jObj["OrderNo"]);
            String orderDate = Convert.ToString(jObj["OrderDate"]);
            int orderStatus = Convert.ToInt32(jObj["OrderStatus"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("insert into orders( Member, Cart, OrderNO, OrderDate, OrderStatus ) values ('"
                     + memberId + "','" + cartId + "','" + orderNo + "','" + orderDate + "','" + orderStatus + "')", dbConn);
            if (dbComm1.ExecuteNonQuery() > 0)
                result = true;

            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

	// 浏览某会员的所有订单
    [WebMethod]
	public String browseMemOrder(String browseMemOrderJsonStr)
    {
        String result = "";
        MySQLConnection dbConn = null;
        MySQLDataReader dbReader1 = null;

        try
        {
            //{'MemberId':'8'}
            JObject jObj = JObject.Parse(@browseMemOrderJsonStr);
            int memberId = Convert.ToInt32(jObj["MemberId"]);

            dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from orders where Member = '" + memberId + "'", dbConn);
            dbReader1 = dbComm1.ExecuteReaderEx();

            List<Order> orderList = new List<Order>();

            while (dbReader1.Read())
            {
                Order order = new Order();

                order.id = dbReader1.GetInt32(0);
                order.memberId = dbReader1.GetInt32(1);
                order.cartId = dbReader1.GetInt32(2);
                order.orderNo = dbReader1.GetString(3);
                order.orderDate = dbReader1.GetString(4);
                order.orderStatus = dbReader1.GetInt32(5);

                orderList.Add(order);
            }

            result = JsonConvert.SerializeObject(orderList, Formatting.Indented);

        }
        catch (Exception e)
        {
            String msg = e.Message;
        }
        finally
        {
            if(dbReader1 != null)
                dbReader1.Close();
            
            if(dbConn != null)
                dbConn.Close();
        }

        return result;
    }

	// 浏览所有订单 
    [WebMethod]
	public String browseOrder()
    {
        String result = "";
        MySQLConnection dbConn = null;
        MySQLDataReader dbReader1 = null;

        try
        {
            dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from orders", dbConn);
            dbReader1 = dbComm1.ExecuteReaderEx();

            List<Order> orderList = new List<Order>();

            while (dbReader1.Read())
            {
                Order order = new Order();

                order.id = dbReader1.GetInt32(0);
                order.memberId = dbReader1.GetInt32(1);
                order.cartId = dbReader1.GetInt32(2);
                order.orderNo = dbReader1.GetString(3);
                order.orderDate = dbReader1.GetString(4);
                order.orderStatus = dbReader1.GetInt32(5);

                orderList.Add(order);
            }

            result = JsonConvert.SerializeObject(orderList, Formatting.Indented);

        }
        catch (Exception e)
        {
            String msg = e.Message;
        }
        finally
        {
            if (dbReader1 != null)
                dbReader1.Close();

            if (dbConn != null)
                dbConn.Close();
        }

        return result;
    }

	// 浏览某订单的所有商品记录 
    [WebMethod]
	public String browseOrderMer(String browseOrderMerJsonStr)
    {
        String result = "";
        MySQLConnection dbConn = null;
        MySQLDataReader dbReader1 = null;

        try
        {
            //{'CartId':'8'}
            JObject jObj = JObject.Parse(@browseOrderMerJsonStr);
            int cartId = Convert.ToInt32(jObj["CartId"]);

            dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from cartselectedmer where Cart = '" + cartId + "'", dbConn);
            dbReader1 = dbComm1.ExecuteReaderEx();

            List<Cartselectedmer> cartselectedmerList = new List<Cartselectedmer>();

            while (dbReader1.Read())
            {
                Cartselectedmer cartselectedmer = new Cartselectedmer();
                cartselectedmer.id = dbReader1.GetInt32(0);
                cartselectedmer.cart = dbReader1.GetInt32(1);
                cartselectedmer.merchandise = dbReader1.GetInt32(2);
                cartselectedmer.number = dbReader1.GetInt32(3);
                cartselectedmer.price = dbReader1.GetDouble(4);
                cartselectedmer.money = dbReader1.GetDouble(5);

                cartselectedmerList.Add(cartselectedmer);
            }

            result = JsonConvert.SerializeObject(cartselectedmerList, Formatting.Indented);
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }
        finally
        {
            if (dbReader1 != null)
                dbReader1.Close();

            if (dbConn != null)
                dbConn.Close();
        }

        return result;
    }
	
	// 删除订单
    [WebMethod]
	public bool delOrder(String delOrderJsonStr)
    {
        bool result = false;
        MySQLConnection dbConn = null;

        try
        {
            //{'OrderId':'6'}
            JObject jObj = JObject.Parse(@delOrderJsonStr);
            int orderId = Convert.ToInt32(jObj["OrderId"]);

            dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("delete from orders where ID = '" + orderId + "'", dbConn);

            if (dbComm1.ExecuteNonQuery() > 0)
                result = true;
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }
        finally
        {
            if (dbConn != null)
                dbConn.Close();
        }

        return result;
    }

	// 装载订单
    [WebMethod]
	public String loadOrder(String loadOrderJsonStr)
    {
        String result = "";
        MySQLConnection dbConn = null;
        MySQLDataReader dbReader1 = null;

        //{'OrderId':'6'}
        JObject jObj = JObject.Parse(@loadOrderJsonStr);
        int orderId = Convert.ToInt32(jObj["OrderId"]);

        dbConn = new MySQLConnection();
        MySQLCommand dbComm1
                = new MySQLCommand("select * orders where ID = '" + orderId + "'", dbConn);
        dbReader1 = dbComm1.ExecuteReaderEx();

        if (dbReader1.Read())
        {
            Order order = new Order();
            order.id = dbReader1.GetInt32(0);
        }

        return result;
    }

	// 修改订单
    [WebMethod]
    public bool updateOrder(String updateOrderJsonStr) 
    {
        bool result = false;

        return result;
    }

    public MySQLConnection MysqlDataBaseConnection()
    {
        MySQLConnection DBConn;
        DBConn = new MySQLConnection(new MySQLConnectionString("localhost", "db_eshop", "root", "123456", 3306).AsString);

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
}
