using MMCApi.Model;
using MMCApi.Model.UserMMC;
using MMCApi.Translators;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Repository
{
    public class AI_ReceiptDbClient
    {
        //Get Receipt Result
        public List<AI_GetReceiptModel> GetReceiptResult(string connString, string emailId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceipt<List<AI_GetReceiptModel>>(connString, "GetReceiptResult", emailId,
             r => r.TranslateAsReceiptList());
        }

        //Get All Receipt
        public List<AI_GetReceiptModel> GetAllReceipt(string connString, string emailId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForAllReceipt<List<AI_GetReceiptModel>>(connString, "GetAllReceipt",
             r => r.TranslateAsReceiptList());
        }

        //Get All Receipt
        public List<MMCReceiptModel> GetAllMMCReceipt(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForMMCAllReceipt<List<MMCReceiptModel>>(connString, "GetAllReceipt",
             r => r.TranslateAsReceiptListForWeb());
        }

        //Get All Receipt By CompanyId
        public List<AI_GetReceiptModel> GetReceiptByCompanyId(string connString, int companyId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceiptByCompanyId<List<AI_GetReceiptModel>>(connString, "GetReceiptByCompanyId", companyId,
             r => r.TranslateAsReceiptList());
        }


        //Get All Receipt
        public List<AI_GetReceiptModel> GetReceiptById(string connString, string id)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceiptById<List<AI_GetReceiptModel>>(connString, "GetReceiptById", id,
             r => r.TranslateAsReceiptList());
        }

        //Get All Receipt
        public List<UpdateCategory> GetCategoryIdByVendor(string connString, string vendorName)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceiptByCategoryId<List<UpdateCategory>>(connString, "GetCategoryIdByVendor", vendorName,
             r => r.TranslateAsReceiptGetCategoryList());
        }

        //Get All Receipt By Assign to
        public List<AI_GetReceiptModel> GetReceiptByAssignTo(string connString, string assignTo)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceiptByAssignTo<List<AI_GetReceiptModel>>(connString, "GetReceiptByAssignTo", assignTo,
             r => r.TranslateAsReceiptList());
        }

        //Insert Receipt
        public string InsertReceipt(AI_ReceiptModel model, string connString)
        {
            string TransactionNo = DateTime.Now.Ticks.ToString().Substring(0, 10);
            var outParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@CompanyId",model.CompanyId),
                new SqlParameter("@TransactionNo",TransactionNo),
                new SqlParameter("@EmailId",model.EmailId),
                new SqlParameter("@AttachmentNo",model.AttachmentNo),
                new SqlParameter("@BillDate",model.BillDate),
                new SqlParameter("@BillNumber",model.BillNumber),
                new SqlParameter("@VendorName",model.VendorName),
                new SqlParameter("@VendorAddress",model.VendorAddress),
                new SqlParameter("@PhoneNumber",model.PhoneNumber),
                new SqlParameter("@VATNo",model.VATNo),
                new SqlParameter("@ItemCode",model.ItemCode),
                new SqlParameter("@Description",model.Description),
                new SqlParameter("@Quantity",model.Quantity),
                new SqlParameter("@UnitPrice",model.UnitPrice),
                new SqlParameter("@AccountCode",model.AccountCode),
                new SqlParameter("@CategoryId",model.CategoryId),
                new SqlParameter("@TaxType",model.TaxType),
                new SqlParameter("@TaxAmount",model.TaxAmount),
                new SqlParameter("@TotalAmount",model.TotalAmount),
                new SqlParameter("@Status","Pending for MMC Review"),
                new SqlParameter("@CreatedBy",model.CreatedBy),
                new SqlParameter("@AssignTo",model.AssignTo),
                new SqlParameter("@Url",model.Url),
                new SqlParameter("@ImageContent",model.ImageContent),
                new SqlParameter("@TransactionTime",model.TransactionTime),
                new SqlParameter("@Subtotal",model.Subtotal),
                new SqlParameter("@CreatedDate",DateTime.UtcNow),
                new SqlParameter("@Currency",model.Currency),
                new SqlParameter("@MarkAsBillable",false),
                new SqlParameter("@Rate",model.Rate),
                new SqlParameter("@IsArchived",false),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "InsertAnalyzeReceipt", param);
            return outParam.Value.ToString();
        }

        //Uodate Receipt
        public string UpdateReceipt(AI_ReceiptModel model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@BillDate",model.BillDate),
                new SqlParameter("@BillNumber",model.BillNumber),
                new SqlParameter("@VendorName",model.VendorName),
                new SqlParameter("@VendorAddress",model.VendorAddress),
                new SqlParameter("@PhoneNumber",model.PhoneNumber),
                new SqlParameter("@Description",model.Description),
                new SqlParameter("@Quantity",model.Quantity),
                new SqlParameter("@UnitPrice",model.UnitPrice),
                new SqlParameter("@AccountCode",0),
                new SqlParameter("@TaxType",model.TaxType),
                new SqlParameter("@TaxAmount",model.TaxAmount),
                new SqlParameter("@TotalAmount",model.TotalAmount),
                new SqlParameter("@Status","Ready to Publish"),
                new SqlParameter("@ClientId",model.ClientId),
                new SqlParameter("@MarkAsBillable",model.MarkAsBillable),
                new SqlParameter("@Currency",model.Currency),
                new SqlParameter("@Subtotal",model.Subtotal),
                new SqlParameter("@CategoryId",model.CategoryId),
                new SqlParameter("@ModifiedDate",DateTime.UtcNow),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateAnalyzeReceipt", param);
            return outParam.Value.ToString();

        }


        //Uodate Receipt
        public string UpdateReceiptFromWeb(AI_ReceiptModel model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@BillDate",model.BillDate),
                new SqlParameter("@BillNumber",model.BillNumber),
                new SqlParameter("@VendorName",model.VendorName),
                new SqlParameter("@VendorAddress",model.VendorAddress),
                new SqlParameter("@PhoneNumber",model.PhoneNumber),
                new SqlParameter("@Description",model.Description),
                new SqlParameter("@Quantity",model.Quantity),
                new SqlParameter("@UnitPrice",model.UnitPrice),
                new SqlParameter("@AccountCode",0),
                new SqlParameter("@TaxType",model.TaxType),
                new SqlParameter("@TaxAmount",model.TaxAmount),
                new SqlParameter("@TotalAmount",model.TotalAmount),
                new SqlParameter("@Status","Ready to Publish"),
                new SqlParameter("@ClientId",model.ClientId),
                new SqlParameter("@MarkAsBillable",model.MarkAsBillable),
                new SqlParameter("@Subtotal",model.Subtotal),
                new SqlParameter("@CategoryId",model.CategoryId),
                new SqlParameter("@ModifiedDate",DateTime.UtcNow),

                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateAnalyzeReceiptForWeb", param);
            return outParam.Value.ToString();

        }

        //Update CategoryId
        public string UpdateCategoryIdByVendor(UpdateCategory model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@Vendor",model.VendorName),
                new SqlParameter("@CategoryId",model.CategoryId),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateCategoryIdByVendor", param);
            return outParam.Value.ToString();

        }

        //Update Receipt Status
        public string UpdateReceiptStatus(AI_ReceiptModel model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@BillDate",model.BillDate),
                new SqlParameter("@BillNumber",model.BillNumber),
                new SqlParameter("@VendorName",model.VendorName),
                new SqlParameter("@Description",model.Description),
                new SqlParameter("@Quantity",model.Quantity),
                new SqlParameter("@UnitPrice",model.UnitPrice),
                new SqlParameter("@TaxType",model.TaxType),
                new SqlParameter("@TaxAmount",model.TaxAmount),
                new SqlParameter("@TotalAmount",model.TotalAmount),
                new SqlParameter("@AssignTo",model.AssignTo),
                new SqlParameter("@Status","Ready to Publish"),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateReceiptStatus", param);
            return outParam.Value.ToString();

        }

        //delete receipt
        public string DeleteReceiptById(string connString, int Id)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",Id),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "DeleteReceiptById", param);
            return outParam.Value.ToString();

        }

        //Update Receipt Status
        public string UpdateReceiptPushedToFreshbook(string connString, int Id)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",Id),
                new SqlParameter("@Status","Pushed to freshbook"),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateReceiptPushedToFreshbook", param);
            return outParam.Value.ToString();

        }

        //Update Category and Client Id 
        public string UpdateReceiptCategoryIdClientId(string connString, int Id, string categoryId)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@Id",Id),
                new SqlParameter("@CategoryId",categoryId),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateReceiptCategoryIdClientId", param);
            return outParam.Value.ToString();

        }

        //Search Receipt
        public List<MMCReceiptModel> SearchReceiptData(string connString, string statusName, string companyName, DateTime? frmDate, DateTime? todDate)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForSearchReceipt<List<MMCReceiptModel>>(connString, "SearchReceiptData_New", statusName, companyName, frmDate, todDate,
             r => r.TranslateAsReceiptListForWeb());
        }

        //Get All Receipt By CompanyId and Status
        public List<AI_GetReceiptModel> GetAllReceiptByCompanyIdAndStatus(string connString, int companyId, string status, int pageno, bool isArchived)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForReceiptByCompanyIdAndStatus<List<AI_GetReceiptModel>>(connString, "GetAllReceiptByCompanyIdAndStatus", companyId, status, pageno, isArchived,
             r => r.TranslateAsReceiptList());
        }

        public List<MMCReceiptModel> SearchReceiptDataMobile(string connString, string status, string searchVal)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForSearchReceiptMobile<List<MMCReceiptModel>>(connString, "SearchReceiptDataMobile", status, searchVal,
             r => r.TranslateAsReceiptListForWeb());
        }
    }
}
