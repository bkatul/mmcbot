using CoreApiAdoDemo.Translators;
using MMCApi.Model;
using MMCApi.Translators;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Repository
{
    public class FreshbookDbClient
    {
        #region Get User Detail

        //Get Freshbook detail by companyid
        public List<FreshbookModel> GetFreshbookDetailByCompanyId(string connString, string companyId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataFreshbookDetail<List<FreshbookModel>>(connString,
                "GetFreshbookDetailByCompanyId", companyId, r => r.TranslateAsFreshbookDetailList());
        }

        //Get Manager
        public List<UserModel> GetManagerByEmailId(string connString, string emailId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataManager<List<UserModel>>(connString,
                "GetManagerByEmailId", emailId, r => r.TranslateAsUsersList());
        }
        #endregion

        #region Save Freshbook Detail

        //Save Freshbook Detail
        public string SaveFreshbookDetails(FreshbookModel model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AccessToken",model.AccessToken),
                new SqlParameter("@RefreshToken",model.RefreshToken),
                new SqlParameter("@CompanyId",model.CompanyId),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "SaveFreshbookDetail", param);
            return outParam.Value.ToString();
        }

        #endregion


        #region Update Freshbook Account Id

        //UpdateFreshbookAccountId
        public string UpdateFreshbookAccountId(string account_id, int CompanyId,bool IsSelected, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AccountId",account_id),
                new SqlParameter("@CompanyId",CompanyId),
                new SqlParameter("@IsSelected",IsSelected),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateFreshbookAccountId", param);
            return outParam.Value.ToString();
        }

        #endregion

        #region Get Duplicate Receiptd

        //Get Duplicate Receipt
        public List<AI_GetReceiptModel> GetDuplicateReceipt(string connString, string vendorName,DateTime? receiptDate,decimal totalAmount,string createdBy,int companyId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForDuplicateReceipt<List<AI_GetReceiptModel>>(connString, "GetDuplicateReceipt", vendorName, receiptDate,totalAmount, createdBy, companyId,
             r => r.TranslateAsReceiptList());
        }

        #endregion
    }
}
