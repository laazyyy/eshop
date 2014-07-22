using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wql
{
    /// <summary>
    /// 购物车选购的商品
    /// </summary>
    [Serializable]
    public class Cartselectedmer
    {
        public Cartselectedmer()
        {
            //null
        }

        public int id { set; get; }

        public int cart { set; get; }

        public int merchandise { set; get; }

        public int number { set; get; }

        public Double price { set; get; }

        public Double money { set; get; }
    }
}