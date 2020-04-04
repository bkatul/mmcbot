using MMCApi.Model;
using MMCApi.Utility;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MMCApi.Translators
{
    public static class FreshbookTranslator
    {
        public static FreshbookModel TranslateAsFreshbookDetail(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                    return null;
                reader.Read();
            }
            var item = new FreshbookModel();
            if (reader.IsColumnExists("AccessToken"))
                item.AccessToken = SqlHelper.GetNullableString(reader, "AccessToken");

            if (reader.IsColumnExists("RefreshToken"))
                item.RefreshToken = SqlHelper.GetNullableString(reader, "RefreshToken");

            if (reader.IsColumnExists("Id"))
                item.CompanyId = SqlHelper.GetNullableInt32(reader, "Id");

            if (reader.IsColumnExists("Name"))
                item.Company = SqlHelper.GetNullableString(reader, "Name");

            if (reader.IsColumnExists("AccountId"))
                item.AccountId = SqlHelper.GetNullableString(reader, "AccountId");

            if (reader.IsColumnExists("IsSelected"))
                item.IsSelected = SqlHelper.GetBoolean(reader, "IsSelected");

            return item;
        }

        public static List<FreshbookModel> TranslateAsFreshbookDetailList(this SqlDataReader reader)
        {
            var list = new List<FreshbookModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsFreshbookDetail(reader, true));
            }
            return list;
        }
    }
}
