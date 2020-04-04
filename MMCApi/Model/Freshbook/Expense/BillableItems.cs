using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model.Freshbook.Expense
{
    public class BillableItems
    {
        public billable_item billable_item { get; set; }
    }

    public class billable_item
    {
        public string name { get; set; }
        public bool billable { get; set; }
        public string description { get; set; }
        public string tax1 { get; set; }
        public string tax2 { get; set; }
        public unit_cost unit_cost { get; set; }
    } 

    public class unit_cost
    {
        public string amount { get; set; }
        public string code { get; set; }
    }
}
