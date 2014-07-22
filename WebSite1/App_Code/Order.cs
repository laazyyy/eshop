using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wql
{
    [Serializable]
    public class Order
    {
        public Order()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public int id { get; set; }

        public int memberId { get; set; }

        public int cartId { get; set; }

        public String orderNo { get; set; }

        public String orderDate { get; set; }

        public int orderStatus { get; set; }
    }
}