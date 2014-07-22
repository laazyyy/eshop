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
///MemService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class MemService : System.Web.Services.WebService {

    public MemService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    // 新增注册会员
    [WebMethod]
    public bool addMember( String addMemberJsonStr )
    {
        bool result = false;

        try
        {
            //{'MemberLevel':'4','LoginName':'member4','LoginPwd':'123456','MemberName':'小四','Phone':'123456789','Address':'广东省广州市','Zip':'510000','RegDate':'2013-04-20 18:40:30','LastDate':'2013-04-23 17:40:08','LoginTimes':'1','Email':'abc@scut.com'}
            JObject jObj = JObject.Parse(@addMemberJsonStr);
            int memLevel = Convert.ToInt32(jObj["MemberLevel"]);
            String loginName = Convert.ToString(jObj["LoginName"]);
            String loginPwd = Convert.ToString(jObj["LoginPwd"]);
            String memberName = Convert.ToString(jObj["MemberName"]);
            String phone = Convert.ToString(jObj["Phone"]);
            String address = Convert.ToString(jObj["Address"]);
            String zip = Convert.ToString(jObj["Zip"]);
            String regDate = Convert.ToString(jObj["RegDate"]);
            String lastDate = Convert.ToString(jObj["LastDate"]);
            int loginTimes = Convert.ToInt32(jObj["LoginTimes"]);
            String email = Convert.ToString(jObj["Email"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("insert into member( MemberLevel, LoginName, LoginPwd, MemberName, Phone, Address, "
                    + "Zip, RegDate, LastDate, LoginTimes, EMail ) values ('" + memLevel + "','" + loginName + "','" + loginPwd + "','"
                    + memberName + "','" + phone + "','" + address + "','" + zip + "','" + regDate + "','" + lastDate + "','" + loginTimes
                    + "','" + email + "')", dbConn);
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

    // 浏览会员级别
    [WebMethod]
    public String browseMemberLevel()
    {
        String result = "";

        MySQLConnection dbConn = MysqlDataBaseConnection();
        MySQLCommand dbComm1
            = new MySQLCommand("select * from memberlevel", dbConn);
        MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

        List<MemberLevel> memberLevelList = new List<MemberLevel>();

        while (dbReader1.Read())
        {
            MemberLevel memberLevel = new MemberLevel();
            memberLevel.id = dbReader1.GetInt32(0);
            memberLevel.levelname = dbReader1.GetString(1);
            memberLevel.favourable = dbReader1.GetInt32(2);

            memberLevelList.Add( memberLevel );
        }

        dbReader1.Close();
        dbConn.Close();

        result = JsonConvert.SerializeObject(memberLevelList, Formatting.Indented);

        return result;
    }

    // 检查登录帐号是否有效
    [WebMethod]
    public bool chkLoginName(String chkLoginNameJsonStr)
    {
        bool result = true;

        try
        {
            //{'LoginName':'member4'}
            JObject jObj = JObject.Parse(chkLoginNameJsonStr);
            String loginName = Convert.ToString(jObj["LoginName"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from member where LoginName = '" + loginName + "'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            if (dbReader1.HasRows)
                result = false;

            dbReader1.Close();
            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 装载会员级别
    [WebMethod]
    public String loadMemberLevel( String loadMemberLevelJsonStr )
    {
        String result = "";

        try 
        {
            //{'MemberLevelId':'1'}
            JObject jObj = JObject.Parse(loadMemberLevelJsonStr);
            int memberLevelId = Convert.ToInt32(jObj["MemberLevelId"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from memberlevel where ID = '" + memberLevelId + "'", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            MemberLevel memberLevel = new MemberLevel();

            if(dbReader1.Read()) 
            {
                memberLevel.id = dbReader1.GetInt32(0);
                memberLevel.levelname = dbReader1.GetString(1);
                memberLevel.favourable = dbReader1.GetInt32(2);

                result = JsonConvert.SerializeObject(memberLevel, Formatting.Indented);
            }

            dbReader1.Close();
            dbConn.Close();
        }
        catch(Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 会员登录
    [WebMethod]
    public String memLogin( String memLoginJsonStr )
    {
        String result = "";

        try
        {
            //{'LoginName':'member4','LoginPwd':'123456'}
            JObject jObj = JObject.Parse(memLoginJsonStr);
            String loginName = Convert.ToString(jObj["LoginName"]);
            String loginPwd = Convert.ToString(jObj["LoginPwd"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("update member set LastDate = now(), LoginTimes = LoginTimes + 1 where LoginName = '"
                    + loginName + "' and LoginPwd = '" + loginPwd + "'", dbConn);
            MySQLDataReader dbReader2 = null;

            if (dbComm1.ExecuteNonQuery() > 0)
            {
                MySQLCommand dbComm2
                    = new MySQLCommand("select * from member where LoginName = '"
                        + loginName + "' and LoginPwd = '" + loginPwd + "'", dbConn);
                dbReader2 = dbComm2.ExecuteReaderEx();

                if (dbReader2.Read())
                {
                    Member member = new Member();
                    member.id = dbReader2.GetInt32(0);
                    member.memberlevelId = dbReader2.GetInt32(1);
                    member.loginName = dbReader2.GetString(2);
                    member.loginPwd = dbReader2.GetString(3);
                    member.memberName = dbReader2.GetString(4);
                    member.phone = dbReader2.GetString(5);
                    member.address = dbReader2.GetString(6);
                    member.zip = dbReader2.GetString(7);
                    member.regDate = dbReader2.GetString(8);
                    member.lastDate = dbReader2.GetString(9);
                    member.loginTimes = dbReader2.GetInt32(10);
                    member.email = dbReader2.GetString(11);

                    result = JsonConvert.SerializeObject(member, Formatting.Indented);
                }
            }

            if (dbReader2 != null)
                dbReader2.Close();

            dbConn.Close();
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 修改注册会员
    [WebMethod]
    public bool updateMember( String updateMemberJsonStr )
    {
        bool result = false;

        try
        {
            //{'MemberId':'11','MemberLevel':'4','LoginName':'member4','LoginPwd':'123456','MemberName':'小四','Phone':'123456789','Address':'广东省广州市','Zip':'510000','RegDate':'2013-04-20 18:40:30','LastDate':'2013-04-23 17:40:08','LoginTimes':'1','Email':'abc@scut.com'}
            JObject jObj = JObject.Parse(@updateMemberJsonStr);
            int memberId = Convert.ToInt32(jObj["MemberId"]);
            int memLevel = Convert.ToInt32(jObj["MemberLevel"]);
            String loginName = Convert.ToString(jObj["LoginName"]);
            String loginPwd = Convert.ToString(jObj["LoginPwd"]);
            String memberName = Convert.ToString(jObj["MemberName"]);
            String phone = Convert.ToString(jObj["Phone"]);
            String address = Convert.ToString(jObj["Address"]);
            String zip = Convert.ToString(jObj["Zip"]);
            String regDate = Convert.ToString(jObj["RegDate"]);
            String lastDate = Convert.ToString(jObj["LastDate"]);
            int loginTimes = Convert.ToInt32(jObj["LoginTimes"]);
            String email = Convert.ToString(jObj["Email"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("update member set Memberlevel = '" + memLevel + "' ,LoginName = '" + loginName
                    + "' ,LoginPwd = '" + loginPwd + "' ,MemberName = '" + memberName + "' ,Phone = '" + phone + "' ,Address = '"
                    + address + "' ,Zip = '" + zip + "' ,RegDate = '" + regDate + "' ,LastDate = '" + lastDate + "' ,LoginTimes = '"
                    + loginTimes + "' ,EMail = '" + email + "' where ID = '" + memberId + "'", dbConn);
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

    // 浏览注册会员
    [WebMethod]
    public String browseMember()
    {
        String result = "";

        try
        {
            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("select * from member", dbConn);
            MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

            List<Member> memberList = new List<Member>();

            while (dbReader1.Read())
            {
                Member member = new Member();
                member.id = dbReader1.GetInt32(0);
                member.memberlevelId = dbReader1.GetInt32(1);
                member.loginName = dbReader1.GetString(2);
                member.loginPwd = dbReader1.GetString(3);
                member.memberName = dbReader1.GetString(4);
                member.phone = dbReader1.GetString(5);
                member.address = dbReader1.GetString(6);
                member.zip = dbReader1.GetString(7);
                member.regDate = dbReader1.GetString(8);
                member.lastDate = dbReader1.GetString(9);
                member.loginTimes = dbReader1.GetInt32(10);
                member.email = dbReader1.GetString(11);

                memberList.Add(member);
            }

            result = JsonConvert.SerializeObject(memberList, Formatting.Indented);
        }
        catch (Exception e)
        {
            String msg = e.Message;
        }

        return result;
    }

    // 删除注册会员
    [WebMethod]
    public bool delMember( String delMemberJsonStr ) 
    {
        bool result = false;

        try
        {
            //{'MemberId':'10'}
            JObject jObj = JObject.Parse(delMemberJsonStr);
            int memberId = Convert.ToInt32(jObj["MemberId"]);

            MySQLConnection dbConn = MysqlDataBaseConnection();
            MySQLCommand dbComm1
                = new MySQLCommand("delete from member where ID = '" + memberId + "'", dbConn);
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

    // 装载注册会员
    [WebMethod]
    public String loadMember(String loadMemberJsonStr)
    {
        String result = "";

        //{'MemberId':'11'}
        JObject jObj = JObject.Parse(loadMemberJsonStr);
        int memberId = Convert.ToInt32(jObj["MemberId"]);

        MySQLConnection dbConn = MysqlDataBaseConnection();
        MySQLCommand dbComm1
            = new MySQLCommand("select * from member where ID = '" + memberId + "'", dbConn);
        MySQLDataReader dbReader1 = dbComm1.ExecuteReaderEx();

        if (dbReader1.Read())
        {
            Member member = new Member();
            member.id = dbReader1.GetInt32(0);
            member.memberlevelId = dbReader1.GetInt32(1);
            member.loginName = dbReader1.GetString(2);
            member.loginPwd = dbReader1.GetString(3);
            member.memberName = dbReader1.GetString(4);
            member.phone = dbReader1.GetString(5);
            member.address = dbReader1.GetString(6);
            member.zip = dbReader1.GetString(7);
            member.regDate = dbReader1.GetString(8);
            member.lastDate = dbReader1.GetString(9);
            member.loginTimes = dbReader1.GetInt32(10);
            member.email = dbReader1.GetString(11);

            result = JsonConvert.SerializeObject(member, Formatting.Indented);
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
