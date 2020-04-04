using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MMCApi.Model
{
    public class AI_ReceiptModel
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
        public decimal Rate { get; set; }
    }

    public class AI_GetReceiptModel
    {
        [Key]
        public int Id { get; set; }
        public string AttachmentNo { get; set; }
        public string CategoryId { get; set; }
        public string TransactionNo { get; set; }
        public DateTime? BillDate { get; set; }
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
        public string Url { get; set; }
        public string TransactionTime { get; set; }
        public decimal Subtotal { get; set; }
        public string CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public int ClientId { get; set; }
        public bool MarkAsBillable { get; set; }
        public string Currency { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UploadedBy { get; set; }
        public decimal Rate { get; set; }
        public bool IsArchived { get; set; }
    }

    public class CognitiveServiceRequest
    {
        public string url { get; set; }
    }

    public class ImageResponse
    {
        public image image { get; set; }

    }
    public class image
    {
        [DataMember(Name = "filename")]
        public string filename { get; set; }

        [DataMember(Name = "public_id")]
        public string public_id { get; set; }

        [DataMember(Name = "jwt")]
        public string jwt { get; set; }

        [DataMember(Name = "media_type")]
        public string media_type { get; set; }
    }

    public class cognitiveResponse
    {
        public string status { get; set; }
    }

    public class UpdateCategory
    {
        public int Id { get; set; }
        public string CategoryId { get; set; }
        public string VendorName { get; set; }
    }


    public class AI_ReceiptInsertModel
    {
        [Key]
        public int id { get; set; }
        public int companyId { get; set; }
        public string categoryId { get; set; }
        public string attachmentNo { get; set; }
        public string emailId { get; set; }
        public DateTime? billDate { get; set; }
        public string invoiceDate { get; set; }
        public string billNumber { get; set; }
        public string vendorName { get; set; }
        public string vendorAddress { get; set; }
        public string phoneNumber { get; set; }
        public string vatNo { get; set; }
        public string itemCode { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public decimal unitPrice { get; set; }
        public string accountCode { get; set; }
        public string category { get; set; }
        public string taxType { get; set; }
        public decimal taxAmount { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; }
        public string createdBy { get; set; }
        public string assignTo { get; set; }
        public string url { get; set; }
        public string imageContent { get; set; }
        public string transactionTime { get; set; }
        public decimal subtotal { get; set; }
        public string responseMessage { get; set; }
        public int clientId { get; set; }
        public bool markAsBillable { get; set; }
        public string currency { get; set; }
    }

    public class CategoryModel
    {
        public int categoryid { get; set; }
        public string category { get; set; }
    }
}
