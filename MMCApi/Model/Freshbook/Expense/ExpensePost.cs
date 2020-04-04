namespace MMCApi.Model.Freshbook.Expense
{
    public class ExpensePost
    {
        public expense expense { get; set; }
    }

    public class amounts
    {
        public string amount { get; set; }
        public string code { get; set; }
    }

    public class taxAmount1
    {
        public string amount { get; set; }
        public string code { get; set; }
    }

    public class expense
    {
        public string vendor { get; set; }
        public string date { get; set; }
        public string categoryid { get; set; }
        public int staffid { get; set; }
        public string category_name { get; set; }
        public string notes { get; set; }
        public string taxName1 { get; set; }
        public int taxPercent1 { get; set; }
        public amounts amount { get; set; }
        public taxAmount1 taxAmount1 { get; set; }
        public attachment attachment { get; set; }
        public int clientid { get; set; }
        public bool include_receipt { get; set; }
    }

    public class attachment
    {
        public string jwt { get; set; }
        public string media_type { get; set; }
        public string expenseid { get; set; }
    }
}
