using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wql
{
    [Serializable]
    public class Cart
    {
        public Cart()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public int id { get; set; }

        public int memberId { get; set; }

        public Double money { get; set; }

        public int cartStatus { get; set; }

        public List<Cartselectedmer> merchandises { get; set; }  //商品集合
    }
}