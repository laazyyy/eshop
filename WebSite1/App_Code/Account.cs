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
    public class Account{
    public string Email { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedDate { get; set; }
    public IList<string> Roles { get; set; }
}
}