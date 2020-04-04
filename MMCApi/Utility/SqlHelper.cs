using System;
using System.Data;
using System.Data.SqlClient;

namespace MMCApi.Utility
{
    public static class SqlHelper
    {
        public static string ExecuteProcedureReturnString(string connString, string procName,
            params SqlParameter[] paramters)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procName;
                    if (paramters != null)
                    {
                        command.Parameters.AddRange(paramters);
                    }
                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
            return result;
        }

        public static string ExecuteQueryToReturnString(string connString, string EmailId)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select Password from tbl_User Where EmailId='" + EmailId + "'";

                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
            return result;
        }

        public static string ExecuteQueryToReturnStringForCompanyId(string connString, string company)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select Id from tbl_Company Where Name='" + company + "'";

                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
            return result;
        }


        public static string AccessFreshbookDetailByCompanyId(string connString, int companyId)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select RefreshToken from tbl_Company Where Id='" + companyId + "'";

                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
            return result;
        }

        public static string ExecuteQueryToReturnStringForCount(string connString, string EmailId)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select Count(*) from tbl_User Where EmailId='" + EmailId + "'";

                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
            return result;
        }

        public static void ExecuteQueryToReturnStringForStatusUpdate(string connString, string EmailId)
        {
            string result = "";
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "Update tbl_User SET Status='Active' Where EmailId='" + EmailId + "'";

                    sqlConnection.Open();
                    var ret = command.ExecuteScalar();
                    if (ret != null)
                    {
                        result = Convert.ToString(ret);
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnData<TData>(string connString, string procName,
            Func<SqlDataReader, TData> translator,
            params SqlParameter[] parameters)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;
                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForLogin<TData>(string connString, string procName, string Email, string Password,
            Func<SqlDataReader, TData> translator,
            params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@EmailId",Email),
                        new SqlParameter("@Password",Password)
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceipt<TData>(string connString, string procName, string emailId,
          Func<SqlDataReader, TData> translator,
          params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@EmailId",emailId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceiptById<TData>(string connString, string procName, string Id,
         Func<SqlDataReader, TData> translator,
         params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@Id",Id),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceiptByCategoryId<TData>(string connString, string procName, string vendorName,
       Func<SqlDataReader, TData> translator,
       params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@Vendor",vendorName),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceiptByCompanyId<TData>(string connString, string procName, int CompanyId,
       Func<SqlDataReader, TData> translator,
       params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@CompanyId",CompanyId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceiptByAssignTo<TData>(string connString, string procName, string assignTo,
      Func<SqlDataReader, TData> translator,
      params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@AssignTo",assignTo),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForAllReceipt<TData>(string connString, string procName,
        Func<SqlDataReader, TData> translator,
        params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForMMCAllReceipt<TData>(string connString, string procName,
     Func<SqlDataReader, TData> translator,
     params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataFreshbookDetail<TData>(string connString, string procName, string companyId,
         Func<SqlDataReader, TData> translator,
         params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@CompanyId",companyId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }


        public static TData ExtecuteProcedureReturnDataManager<TData>(string connString, string procName, string emailId,
      Func<SqlDataReader, TData> translator,
      params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@EmailId",emailId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForUserGetByCreatedBy<TData>(string connString, string procName, string CreatedBy,
     Func<SqlDataReader, TData> translator,
     params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@CreatedBy",CreatedBy),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForUserGetByEmailId<TData>(string connString, string procName, string emailId,
 Func<SqlDataReader, TData> translator,
 params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@EmailId",emailId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }


        public static TData ExtecuteProcedureReturnDataForDuplicateReceipt<TData>(string connString, string procName, string vendorName, DateTime? receiptdate, decimal totalAmount, string createdBy, int companyId,
Func<SqlDataReader, TData> translator,
params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@VendorName",vendorName),
                        new SqlParameter("@BillDate",receiptdate),
                        new SqlParameter("@TotalAmount",totalAmount),
                        new SqlParameter("@CreatedBy",createdBy),
                        new SqlParameter("@CompanyId",companyId),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }


        public static TData ExtecuteProcedureReturnDataForUserGetByUserId<TData>(string connString, string procName, int Id,
  Func<SqlDataReader, TData> translator,
  params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@Id",Id),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForSearchReceipt<TData>(string connString, string procName, string statusName, string companyName, DateTime? fromDate, DateTime? toDate,
     Func<SqlDataReader, TData> translator,
     params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@Status",statusName),
                        new SqlParameter("@FromDate",fromDate),
                        new SqlParameter("@ToDate",toDate),
                        new SqlParameter("@CompanyName",companyName),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataUserGetByRole<TData>(string connString, string createdBy, string procName,
            Func<SqlDataReader, TData> translator,
            params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@CreatedBy",createdBy),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForReceiptByCompanyIdAndStatus<TData>(string connString, string procName, int CompanyId, string status, int pageno, bool isArchived,
      Func<SqlDataReader, TData> translator,
      params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;

                    SqlParameter[] parameter = {
                        new SqlParameter("@CompanyId",CompanyId),
                        new SqlParameter("@Status",status),
                        new SqlParameter("@PageNumber",pageno),
                        new SqlParameter("@IsArchived",isArchived),
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        public static TData ExtecuteProcedureReturnDataForSearchReceiptMobile<TData>(string connString, string procName, string status, string searchVal,
    Func<SqlDataReader, TData> translator,
    params SqlParameter[] param)
        {
            using (var sqlConnection = new SqlConnection(connString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = procName;


                    var outParam = new SqlParameter("@IsNumber", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    SqlParameter[] parameter = {
                        new SqlParameter("@Status",status),
                        new SqlParameter("@SearchVal",searchVal),
                        outParam
                    };

                    if (parameter != null)
                    {
                        sqlCommand.Parameters.AddRange(parameter);
                    }
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        TData elements;
                        try
                        {
                            elements = translator(reader);
                        }
                        finally
                        {
                            while (reader.NextResult())
                            { }
                        }
                        return elements;
                    }
                }
            }
        }

        ///Methods to get values of 
        ///individual columns from sql data reader
        #region Get Values from Sql Data Reader
        public static string GetNullableString(SqlDataReader reader, string colName)
        {
            return reader.IsDBNull(reader.GetOrdinal(colName)) ? null : Convert.ToString(reader[colName]);
        }

        public static int GetNullableInt32(SqlDataReader reader, string colName)
        {
            return reader.IsDBNull(reader.GetOrdinal(colName)) ? 0 : Convert.ToInt32(reader[colName]);
        }

        public static decimal GetNullableDecimal(SqlDataReader reader, string colName)
        {
            return reader.IsDBNull(reader.GetOrdinal(colName)) ? 0 : Convert.ToDecimal(reader[colName]);
        }

        public static bool GetBoolean(SqlDataReader reader, string colName)
        {
            return reader.IsDBNull(reader.GetOrdinal(colName)) ? default(bool) : Convert.ToBoolean(reader[colName]);
        }

        public static DateTime? GetNullableDateTime(SqlDataReader reader, string colName)
        {
            return reader.IsDBNull(reader.GetOrdinal(colName)) ? default(DateTime) : Convert.ToDateTime(reader[colName]);
        }

        //this method is to check wheater column exists or not in data reader
        public static bool IsColumnExists(this System.Data.IDataRecord dr, string colName)
        {
            try
            {
                return (dr.GetOrdinal(colName) >= 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
