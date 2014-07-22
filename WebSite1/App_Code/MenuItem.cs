using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///MenuItem 的摘要说明
/// </summary>
/// 
namespace wangxu { 
public class MenuItem
{
	public MenuItem()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
    private String name;
    private String description;
    private String price;
    private String icon;

    public String getName()
    {
        return name;
    }

    public void setName(String name)
    {
        this.name = name;
    }

    public String getDescription()
    {
        return description;
    }

    public void setDescription(String description)
    {
        this.description = description;
    }

    public String getPrice()
    {
        return price;
    }

    public void setPrice(String price)
    {
        this.price = price;
    }

    public String getIcon()
    {
        return icon;
    }

    public void setIcon(String icon)
    {
        this.icon = icon;
    }	
	

}

}