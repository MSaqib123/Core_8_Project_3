﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        //public double OrderTotal { get; set; }  Moving to Detail Table this properity

        public OrderHeader OrderHeader { get; set; }

    }
}
