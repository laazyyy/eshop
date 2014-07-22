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
///CartService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class CartService : System.Web.Services.WebService {

    public CartService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    [WebMethod]
    public bool addCart(String addCartJsonString)
    {
        bool result = false;

        try
        {
            //{'MemberId':'3','MemberFavourable':'95','MerchandiseId':'4','Price':'44.00','SPrice':'33.00','Special':'0','Numbers':'3'}
            JObject jObj = JObject.Parse(@addCartJsonString);
            int memberId = Convert.ToInt32(jObj["MemberId"]);
            int memberFavourable = Convert.ToInt32(jObj["MemberFavourable"]);
            int merchandiseId = Convert.ToInt32(jObj["MerchandiseId"]);
            double Price = Convert.ToDouble(jObj["Price"]);
            double SPrice = Convert.ToDouble(jObj["SPrice"]);
            int Special = Convert.ToInt32(jObj["Special"]); //是否特价
            int Numbers = Convert.ToInt32(jObj["Numbers"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select ID from cart where Member = '" + memberId + "' and CartStatus = '0'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            // 会员没有使用过购物车,创建购物车
            if (!dbReader1.HasRows)
            {
                MySQLCommand dbComm2
                    = new MySQLCommand("insert into cart(Member, Money, CartStatus) values('" + memberId + "','" + Price * Numbers
                        + "','0')", dbConn);
                dbComm2.ExecuteNonQuery();

                dbReader1 = dbComm1.ExecuteReaderEx();
            }

            int cartId = 0;

            if (dbReader1.Read())
            {
                // 获取购物车ID 
                cartId = dbReader1.GetInt32(0);

                MySQLCommand dbComm3
                    = new MySQLCommand("select ID, Number from cartselectedmer where Cart = '" + cartId + "' and Merchandise = '"
                        + merchandiseId + "' order by ID desc", dbConn);
                MySQLDataReader dbReader3 = dbComm3.ExecuteReaderEx();

                double newPrice = Special == 1 ? SPrice : Price * memberFavourable / 100;
                double newMoney = 0;

                // 商品已加入购物车列表，修改数量、价格
                if (dbReader3.Read())
                {
                    int cartselectedmerId = dbReader3.GetInt32(0);
                    int newNumber = dbReader3.GetInt32(1) + Numbers;
                   
                    newMoney = newPrice * newNumber;

                    MySQLCommand dbComm4 =
                         new MySQLCommand("update cartselectedmer set Number = '" + newNumber
                            + "', Price = '" + newPrice + "', Money = '" + newMoney + "' where ID = '" + cartselectedmerId + "'", dbConn);
                    dbComm4.ExecuteNonQuery();
                }
                // 商品未加入购物车列表，添加相应字段
                else
                {
                    newMoney = newPrice * Numbers;

                    MySQLCommand dbComm5
                        = new MySQLCommand("insert into cartselectedmer(Cart, Merchandise, Number, Price, Money) values('"
                            + cartId + "','" + merchandiseId + "','" + Numbers + "','" + newPrice + "','" + newMoney + "')", dbConn);
                    dbComm5.ExecuteNonQuery();
                }

                // 更新购物车总价格
                MySQLCommand dbComm6
                = new MySQLCommand("update cart set Money = Money + '"
                    + Numbers * newPrice + "' where ID = '" + cartId + "'", dbConn);
                            dbComm6.ExecuteNonQuery();

                dbReader3.Close();
            }

            dbReader1.Close();
            dbConn.Close();
            result = true;
        }
        catch (Exception e)
        {
            string msg = e.Message;
        }

        return result;
    }

    // 浏览购物车
    [WebMethod]
    public String browseCart(String browseCartJsonString)
    {
        //{'MemberId':'3'}
        JObject jObj = JObject.Parse(@browseCartJsonString);
        int memberId = Convert.ToInt32(jObj["MemberId"]);

        MySQLConnection dbConn = MysqlDataBaseConnection();
        MySQLCommand dbComm1 
            = new MySQLCommand("select ID from cart where Member = '" + memberId + "' and CartStatus = '0'", dbConn);
        MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

        String cartStr = "";

        if (dbReader1.Read())
        {
            int cartId = dbReader1.GetInt32(0);

            MySQLCommand dbComm2
                = new MySQLCommand("select ID, Cart, Merchandise, Number, Price, Money from cartselectedmer where Cart = '" 
                    + cartId + "'", dbConn);
            MySQLDataReader dbReader2 = dbComm2.ExecuteReaderEx();

            List<Cartselectedmer> cartselectedmerList = new List<Cartselectedmer>();
            
            while (dbReader2.Read())
            {
                Cartselectedmer cartselectedmerTmp = new Cartselectedmer();
                cartselectedmerTmp.id = dbReader2.GetInt32(0);
                cartselectedmerTmp.cart = dbReader2.GetInt32(1);
                cartselectedmerTmp.merchandise = dbReader2.GetInt32(2);
                cartselectedmerTmp.number = dbReader2.GetInt32(3);
                cartselectedmerTmp.price = dbReader2.GetDouble(4);
                cartselectedmerTmp.money = dbReader2.GetDouble(5);
                cartselectedmerList.Add(cartselectedmerTmp);
            }

            dbReader2.Close();
            
            cartStr = JsonConvert.SerializeObject(cartselectedmerList, Formatting.Indented);
        }

        dbReader1.Close();
        dbConn.Close();

        return cartStr;
    }

    // 清空购物车
    [WebMethod]
    public bool clearCart( String clearCartJsonString )
    {
        bool result = false;

        //{'MemberId':'3'}
        try
        {
            JObject jObj = JObject.Parse(@clearCartJsonString);
            int memberId = Convert.ToInt32(jObj["MemberId"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select ID from cart where Member = '" + memberId + "' and CartStatus = '0'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            if (dbReader1.Read())
            {
                int cartId = dbReader1.GetInt32(0);
                MySQLCommand dbComm2
                = new MySQLCommand("delete from cartselectedmer where Cart = '"
                    + cartId + "'", dbConn);

               dbComm2.ExecuteNonQuery();

               MySQLCommand dbComm3
               = new MySQLCommand("update cart set Money = '0' where ID = '"
                   + cartId + "'", dbConn);

               dbComm3.ExecuteNonQuery();
            }

            dbReader1.Close();
            dbConn.Close();

            result = true;
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 删除购物车选项
    [WebMethod]
    public bool delCart(String delCartJsonString)
    {
        bool result = false;

        try
        {
            //{'CartselectedmerId':'?'}
            JObject jObj = JObject.Parse(@delCartJsonString);
            int cartselectedmerId = Convert.ToInt32(jObj["CartselectedmerId"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            // 获取购物车选项对应的购物车ID和价格
            MySQLCommand dbComm1
                = new MySQLCommand("select Cart, Money from cartselectedmer where ID = '" + cartselectedmerId + "'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            if (dbReader1.Read())
            {
                int cartId = dbReader1.GetInt32(0);
                double money = dbReader1.GetDouble(1);

                // 删除购物车选项
                MySQLCommand dbComm2
                    = new MySQLCommand("delete from cartselectedmer where ID = '" + cartselectedmerId + "'", dbConn);
                dbComm2.ExecuteNonQuery();

                MySQLCommand dbComm3
                    = new MySQLCommand("update cart set Money = Money - '" + money + "' where ID = '" + cartId + "'", dbConn);
                if (dbComm3.ExecuteNonQuery() > 0)
                {
                    result = true;
                }
            }

            dbReader1.Close();
            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 加载购物车
    [WebMethod]
    public String loadCart(String loadCartJsonString)
    {
        String result = "";

        //{'MemberId':'?'}
        JObject jObj = JObject.Parse(@loadCartJsonString);
        int memberId = Convert.ToInt32(jObj["MemberId"]);

        MySQLConnection dbConn = MysqlDataBaseConnection();
        MySQLCommand dbComm1
            = new MySQLCommand("select ID, Money from cart where Member = '" + memberId + "' and CartStatus = '0'", dbConn);
        MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();
    
        if (dbReader1.Read())
        {
            Cart cart = new Cart();
            cart.id = dbReader1.GetInt32(0);
            cart.memberId = memberId;
            cart.money = dbReader1.GetDouble(1);
            cart.cartStatus = 0;

            MySQLCommand dbComm2
                = new MySQLCommand("select * from cartselectedmer where Cart = '" + cart.id + "'", dbConn);
            MySQLDataReader dbReader2 = dbComm2.ExecuteReaderEx();

            List<Cartselectedmer> cartselectedmerList = new List<Cartselectedmer>();
            while (dbReader2.Read())
            {
                Cartselectedmer cartselectedmer = new Cartselectedmer();
                cartselectedmer.id = dbReader2.GetInt32(0);
                cartselectedmer.cart = dbReader2.GetInt32(1);
                cartselectedmer.merchandise = dbReader2.GetInt32(2);
                cartselectedmer.number = dbReader2.GetInt32(3);
                cartselectedmer.price = dbReader2.GetDouble(4);
                cartselectedmer.money = dbReader2.GetDouble(5);

                cartselectedmerList.Add(cartselectedmer);
            }

            cart.merchandises = cartselectedmerList;

            result = JsonConvert.SerializeObject(cart, Formatting.Indented);

            dbReader1.Close();
            dbReader2.Close();
            dbConn.Close();
        }

        return result;
    }

    [WebMethod]
    public bool modiCart( String  modiCartJsonString )
    {
        bool result = false;

        try
        {
            //{'CartselectedmerId':'?','Number':'?'}
            JObject jObj = JObject.Parse(@modiCartJsonString);
            int cartselectedmerId = Convert.ToInt32(jObj["CartselectedmerId"]);
            int number = Convert.ToInt32(jObj["Number"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select Cart, Number, Price from cartselectedmer where ID = '" + cartselectedmerId + "'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            if (dbReader1.Read())
            {
                int cartId = dbReader1.GetInt32(0);
                int oldNumber = dbReader1.GetInt32(1);
                double price = dbReader1.GetDouble(2);

                MySQLCommand dbComm2
                = new MySQLCommand("update cartselectedmer set Number = '" + number +
                    "', Money = '" + price * number + "' where ID = '" + cartselectedmerId + "'", dbConn);
                dbComm2.ExecuteNonQuery();

                MySQLCommand dbComm3
                = new MySQLCommand("update cart set Money = Money - '" + (oldNumber - number) * price +
                    "' where ID = '" + cartId + "'", dbConn);
                dbComm3.ExecuteNonQuery();

                result = true;
            }

            dbReader1.Close();
            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }


    [WebMethod]
    public bool updateCart(String updateCartJsonString)
    {
        bool result = false;

        try
        {
            //{'CartId':'?','MemberId':'?','Money':'?','CartStatus':'?'}
            JObject jObj = JObject.Parse(updateCartJsonString);
            int cartId = Convert.ToInt32(jObj["CartId"]);
            int memberId = Convert.ToInt32(jObj["MemberId"]);
            double money = Convert.ToDouble(jObj["Money"]);
            int cartStatus = Convert.ToInt32(jObj["CartStatus"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("update cart set Money = '" + money + "', CartStatus = '" 
                    + cartStatus + "' where ID = '" + cartId + "'", dbConn);

            if (dbComm1.ExecuteNonQuery() > 0)
            {
                result = true;
            }

            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

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


