using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace MMCBoatWebApp.Models
{
    public class ReceiptModel
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Transaction No")]
        public string transactionNo { get; set; }

        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
       
        public DateTime? billDate { get; set; }

        //[Required(ErrorMessage = "Invoice Number is Required")]
        [Display(Name = "Invoice Number")]
        public string billNumber { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Vendor name is required")]
        public string vendorName { get; set; }

        [Display(Name = "Vendor Address")]
        [Required(ErrorMessage = "Vendor address is required")]
        public string vendorAddress { get; set; }

        [Display(Name = "Vendor Phone")]
        //[Required(ErrorMessage = "Vendor Phone is Required")]
        public string phoneNumber { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }
        public string quantity { get; set; }

        [Display(Name = "Unit Price")]
        public decimal unitPrice { get; set; }

        [Display(Name = "Account Code")]
        public string accountCode { get; set; }

        [Display(Name = "Tax")]
        public string taxType { get; set; }

        [Display(Name = "Tax Amount")]
        [Required(ErrorMessage = "Tax amount is required")]
        public decimal taxAmount { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Url")]
        public string url { get; set; }

        [Display(Name = "AssignTo")]
        public string assignTo { get; set; }

        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Total amount is required")]
        public decimal totalAmount { get; set; }

        [Display(Name = "CategoryId")]
        public string CategoryId { get; set; }

        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Display(Name = "categoryId")]
        public string categoryId { get; set; }

        [Display(Name = "createdBy")]
        public string createdBy { get; set; }

        [Display(Name = "Company Name")]
        public string companyName { get; set; }

        [Display(Name = "Upload Date-Time Stamp")]
        public DateTime? createdDate { get; set; }

        [Display(Name = "Processed Date-Time Stamp")]
        public DateTime? modifiedDate { get; set; }

        [Display(Name = "Uploaded By")]
        public string uploadedBy { get; set; }

        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
        //public string Status { get; set; }
        //public string CompanyName { get; set; }
    }
    public class Notification
    {
        public string Content { get; set; }
    }

    public class TempModel
    {
        [Display(Name = "message")]
        public string message { get; set; }
    }

    public class UpdateCategory
    {
        public int Id { get; set; }
      //  public string CategoryId { get; set; }
        public string categoryId { get; set; }
        public string VendorName { get; set; }
    }

    //public class ReceiptSearchModel
    //{
    //    public DateTime? FromDate { get; set; }
    //    public DateTime? ToDate { get; set; }
    //    public string Status { get; set; }
    //    public string CompanyName { get; set; }
    //}

    public class ReceiptStatusModel
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
    }

    public class CompanyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
