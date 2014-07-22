using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wql
{
    [Serializable]
    public class MemberLevel
    {
        public MemberLevel()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public int id { get; set; }

        public String levelname { get; set; }

        public int favourable { get; set; }
    }
}