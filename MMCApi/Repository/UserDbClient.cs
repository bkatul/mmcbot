using CoreApiAdoDemo.Translators;
using MMCApi.Model;
using MMCApi.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Repository
{
    public class UseDbClient
    {
        #region Get User Detail

        //Get User By Email Id and Password
        public List<UserModel> GetUserByEmailPassword(string connString, string EmailId, string Password)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForLogin<List<UserModel>>(connString,
                "GetUserByEmailPassword", EmailId, Password, r => r.TranslateAsUsersList());
        }

        //Get User By Email Id
        public string GetUserByEmail(string connString, string EmailId)
        {
            return SqlHelper.ExecuteQueryToReturnString(connString, EmailId);
        }

        //Get User By Email Id
        public string GetUserByEmailForUserCount(string connString, string EmailId)
        {
            return SqlHelper.ExecuteQueryToReturnStringForCount(connString, EmailId);
        }


        //AccessFreshbookDetailByCompanyId
        public string AccessFreshbookDetailByCompanyId(string connString, int companyId)
        {
            return SqlHelper.AccessFreshbookDetailByCompanyId(connString, companyId);
        }

        public string GetCompanyIdByCompany(string connString, string companyid)
        {
            return SqlHelper.ExecuteQueryToReturnStringForCompanyId(connString, companyid);
        }

        #endregion

        #region Save and Update User 

        //Save User Detail
        public string SaveUser(UserModel model, string connString)
        {
            var outParam = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AssignTo",model.AssignTo),
                new SqlParameter("@CreatedBy",model.CreatedBy),
                new SqlParameter("@RegistrationId",model.RegistrationId),
                new SqlParameter("@Tag",model.Tag),
                new SqlParameter("@CompanyId",model.CompanyId),
                new SqlParameter("@Company",model.Company),
                new SqlParameter("@FirstName",model.FirstName),
                new SqlParameter("@LastName",model.LastName),
                new SqlParameter("@EmailId",model.EmailId),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@RoleId",1),
                new SqlParameter("@Role","Owner"),
                new SqlParameter("@IsRegisteredByFreshbook",model.IsRegisteredByFreshbook),
                new SqlParameter("@Status","Active"),
                new SqlParameter("@EmailInAddress",model.EmailInAddress),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "SaveUser", param);
            return outParam.Value.ToString();
        }

        //Update User Detail
        public string SaveUserDetail(UserDetailModel model, string connString)
        {
            var outParam = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AssignTo",model.AssignTo),
                new SqlParameter("@CreatedBy",model.CreatedBy),
                new SqlParameter("@FirstName",model.FirstName),
                new SqlParameter("@LastName",model.LastName),
                new SqlParameter("@EmailId",model.EmailId),
                new SqlParameter("@Position",model.Position),
                new SqlParameter("@Address",model.Address),
                new SqlParameter("@City",model.City),
                new SqlParameter("@State",model.State),
                new SqlParameter("@ZIP",model.ZIP),
                new SqlParameter("@Country",model.Country),
                new SqlParameter("@PhoneNumber",model.PhoneNumber),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "SaveUserDetail", param);
            return outParam.Value.ToString();
        }

        // Invite User
        public string InviteUser(InvitationModel model, string connString)
        {
            var outParam = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AssignTo",model.AssignTo),
                new SqlParameter("@CreatedBy",model.CreatedBy),
                new SqlParameter("@CompanyId",model.CompanyId),
                new SqlParameter("@Company",model.Company),
                new SqlParameter("@RegistrationId",model.RegistrationId),
                new SqlParameter("@Tag",model.Tag),
                new SqlParameter("@FirstName",model.FirstName),
                new SqlParameter("@LastName",model.LastName),
                new SqlParameter("@EmailId",model.EmailId),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@RoleId",model.RoleId),
                new SqlParameter("@Role",model.Role),
                new SqlParameter("@IsRegisteredByFreshbook",false),
                new SqlParameter("@Status","InActive"),
                 new SqlParameter("@EmailInAddress",model.EmailInAddress),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "SaveUser", param);
            return outParam.Value.ToString();
        }

        //Update status
        public void UpdateUserStatusByEmail(string connString, string EmailId)
        {
            SqlHelper.ExecuteQueryToReturnStringForStatusUpdate(connString, EmailId);
        }

        //Update User Profile
        public string UpdateUserProfile(string connString, string EmailId, string Address, string Country)
        {
            var outParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {

                new SqlParameter("@EmailId",EmailId),
                new SqlParameter("@Address",Address),
                new SqlParameter("@Country",Country),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateUserProfile", param);
            return outParam.Value.ToString();
        }

        //Update User On Login
        public string UpdateUserOnLogin(string connString, string emailId, string registrationId, string tag)
        {
            var outParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@EmailId",emailId),
                new SqlParameter("@RegistrationId",registrationId),
                new SqlParameter("@Tag",tag),

                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateUserDetailOnLogin", param);
            return outParam.Value.ToString();
        }

        public string UpdateTokenInDatabase(string connString, string access_token, string refresh_token, int companyId)
        {
            var outParam = new SqlParameter("@returnId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@AccessToken",access_token),
                new SqlParameter("@RefreshToken",refresh_token),
                new SqlParameter("@Id",companyId),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "UpdateTokenInDatabase", param);
            return outParam.Value.ToString();
        }

        #endregion

        #region Forgot Password

        //Forgot Password
        public string ForgotPassword(string emailid, string password, string connString)
        {
            var outParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@EmailId",emailid),
                new SqlParameter("@Password",password),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "ForgotPassword", param);
            return outParam.Value.ToString();
        }

        #endregion

        #region Change Password

        //Change Password
        public string ChangePassword(string emailId, string password, string connString)
        {
            var outParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] param = {
                new SqlParameter("@EmailId",emailId),
                new SqlParameter("@Password",password),
                outParam
            };
            SqlHelper.ExecuteProcedureReturnString(connString, "ChangePassword", param);
            return outParam.Value.ToString();
        }

        #endregion

        #region Get Role
        public List<RoleModel> GetAllRole(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<RoleModel>>(connString,
                "GetAllRole", r => r.TranslateAsRoleList());
        }
        public List<MMCRoleModel> GetAllRoleMMC(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<MMCRoleModel>>(connString,
                "GetAllRoleMMC", r => r.TranslateAsMMCRoleList());
        }

        public List<UserModel> GetUserByRole(string connString,string createdBy)
        {
            return SqlHelper.ExtecuteProcedureReturnDataUserGetByRole<List<UserModel>>(connString, createdBy,
                "GetUserByRole", r => r.TranslateAsUsersList());
        }
        #endregion

        #region Get All Currency

        public List<CurrencyModel> GetAllCurrency(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<CurrencyModel>>(connString,
                "GetAllCurrency", r => r.TranslateAsCurrencyList());
        }

        #endregion

        #region Get User By Created By

        //Get User By Created By
        public List<UserGetModel> GetUserByCreatedBy(string connString, string CreatedBy)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForUserGetByCreatedBy<List<UserGetModel>>(connString, "GetUserByCreatedBy", CreatedBy,
             r => r.TranslateAsGetUsersList());
        }

        #endregion

        #region GetUserByEmailId

        //GetUserByEmailId
        public List<UserGetModel> GetUserByEmailId(string connString, string emailid)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForUserGetByEmailId<List<UserGetModel>>(connString, "GetUserByEmailId", emailid,
             r => r.TranslateAsGetUsersList());
        }

        #endregion

        #region Get User By User Id

        //Get User By Created By
        public List<UserGetModel> GetUserByUserId(string connString, int UserId)
        {
            return SqlHelper.ExtecuteProcedureReturnDataForUserGetByUserId<List<UserGetModel>>(connString, "GetUserById", UserId,
             r => r.TranslateAsGetUsersList());
        }

        #endregion

        #region Get All Countries

        public List<CountriesModel> GetAllCountries(string connString)
        {
            return SqlHelper.ExtecuteProcedureReturnData<List<CountriesModel>>(connString,
                "GetAllCountries", r => r.TranslateAsGetCountriesList());
        }
        #endregion



    }
}
