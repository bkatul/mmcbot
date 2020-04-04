using MMCApi.Model;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Repository.Receipt
{
    public class ReceiptDbClient
    {
        public string ArchivedOrUnarchivedReceipt(AI_GetReceiptModel model, string connString)
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
                new SqlParameter("@AccountCode",model.AccountCode),
                new SqlParameter("@Category",model.Category),
                new SqlParameter("@TaxType",model.TaxType),
                new SqlParameter("@TaxAmount",model.TaxAmount),
                new SqlParameter("@TotalAmount",model.TotalAmount),
                new SqlParameter("@Status",model.Status),
                new SqlParameter("@ClientId",model.ClientId),
                new SqlParameter("@MarkAsBillable",model.MarkAsBillable),
                new SqlParameter("@Currency",model.Currency),
                new SqlParameter("@Subtotal",model.Subtotal),
                new SqlParameter("@CategoryId",model.CategoryId),
                new SqlParameter("@IsArchived",model.IsArchived),
                new SqlParameter("@ModifiedDate",DateTime.UtcNow),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "ArchiveOrUnArchiveReceipts", param);
            return outParam.Value.ToString();

        }
    }
}
