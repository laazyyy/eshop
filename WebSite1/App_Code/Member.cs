using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wql
{
    [Serializable]
    public class Member
    {
        public Member()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public int id { get; set; }

        public int memberlevelId { get; set; }

        public String loginName { get; set; }

        public String loginPwd { get; set; }

        public String memberName { get; set; }

        public String phone { get; set; }

        public String address { get; set; }

        public String zip { get; set; }

        public String regDate { get; set; }

        public String lastDate { get; set; }

        public int loginTimes { get; set; }

        public String email { get; set; }

        //public HashSet<String> orders { get; set; } //订单集合

    }
}