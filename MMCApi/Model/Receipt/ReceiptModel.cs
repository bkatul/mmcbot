using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model.Receipt
{
    public class ReceiptModel
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CategoryId { get; set; }
        public string AttachmentNo { get; set; }
        public string EmailId { get; set; }
        public DateTime? BillDate { get; set; }
        public string InvoiceDate { get; set; }
        public string BillNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string VATNo { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string AccountCode { get; set; }
        public string Category { get; set; }
        public string TaxType { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
        public string Url { get; set; }
        public string ImageContent { get; set; }
        public string TransactionTime { get; set; }
        public decimal Subtotal { get; set; }
        public string ResponseMessage { get; set; }
        public int ClientId { get; set; }
        public bool MarkAsBillable { get; set; }
        public string Currency { get; set; }
    }

    public class PagingModel
    {
        public int CurrentPage { get; set; }

        public int TotalRecords { get; set; }

        public List<AI_GetReceiptModel> Data { get; set; }
    }
}
