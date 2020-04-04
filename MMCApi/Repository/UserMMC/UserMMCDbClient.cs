using MMCApi.Model.UserMMC;
using MMCApi.Translators;
using MMCApi.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Repository
{
    public class UserMMCDbClient
    {

        //Get User By Email Id and Password
        public List<MMCUserModel> GetMMCUserByEmailAndPassword(string EmailId, string Password, string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForLogin<List<MMCUserModel>>(connString,
                "GetMMCUserByEmailAndPassword", EmailId, Password, r => r.TranslateAsMMCUsersList());
        }

        //Get All MMC User 
        public List<MMCUserModel> GetAllMMCUser(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<MMCUserModel>>(connString,
                "GetAllMMCUser",  r => r.TranslateAsMMCUsersList());
        }

        //Get All Company 
        public List<MMCCompanyModel> GetAllCompany(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<MMCCompanyModel>>(connString,
                "GetAllCompany", r => r.TranslateAsMMCCompanyList());
        }

        #region Save User MMC

        //Create MMC User
        public string CreateMMCUser(MMCUserModel model, string connString)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {

                new SqlParameter("@RoleId",model.roleId),
                new SqlParameter("@CompanyId",1),
                new SqlParameter("@FirstName",model.firstName),
                new SqlParameter("@LastName",model.lastName),
                new SqlParameter("@EmailAddress",model.emailAddress),
                new SqlParameter("@Password",model.password),
                new SqlParameter("@CityId",1),
                new SqlParameter("@StateId",1),
                new SqlParameter("@CountryId",model.countryId),
                new SqlParameter("@IsActive",true),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "CreateMMCUser", param);
            return outParam.Value.ToString();
        }

        #endregion


        #region Get User By User Id

        //Get User By Created By
        public List<MMCUserModel> GetUserByUserId(string connString, int UserId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForUserGetByUserId<List<MMCUserModel>>(connString, "GetMMCUserById", UserId,
             r => r.TranslateAsMMCUsersList());
        }

        #endregion

    }
}
