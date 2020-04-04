using MMCApi.Model;
using MMCApi.Utility;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Repository
{
    public class CompanyDbClient
    {
        #region Save Company

        //Save Company
        public string SaveCompany(UserModel model, string connString)
        {
            var outParam = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlParameter[] param = {
               new SqlParameter("@Name",model.Company),
               new SqlParameter("@AccessToken",model.AccessToken),
               new SqlParameter("@RefreshToken",model.RefreshToken),
               new SqlParameter("@Url",model.Url),
               new SqlParameter("@FreshbookAccountType",model.FreshbookAccountType),
               outParam,
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "SaveCompany", param);
            return outParam.Value.ToString();
        }
        #endregion
    }
}
