using System;

namespace MMCApi.NotificationHubs
{
    public class Notification : DeviceRegistration
    {
        public string Content { get; set; }
    }

    public class ReceiptModel
    {
        public int id { get; set; }
        public DateTime? billDate { get; set; }
        public string billNumber { get; set; }
        public string vendorName { get; set; }
        public string vendorAddress { get; set; }
        public string phoneNumber { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public decimal unitPrice { get; set; }
        public string accountCode { get; set; }
        public string taxType { get; set; }
        public decimal taxAmount { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string assignTo { get; set; }
        public decimal totalAmount { get; set; }
    }
}
