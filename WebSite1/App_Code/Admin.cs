using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///MenuItem 的摘要说明
/// </summary>
/// 
namespace wangxu {
[Serializable]
public class Admin
{
	public Admin()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
    public int id { get; set; }

    public int adminType { get; set; }

    public String adminName { get; set; }

    public String loginName { get; set; }

    public String loginPwd { get; set; }
}

}