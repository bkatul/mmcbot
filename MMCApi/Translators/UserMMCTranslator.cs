using MMCApi.Model.UserMMC;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Translators
{
    public static class UserMMCTranslator
    {
        public static MMCUserModel TranslateAsMMCUser(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new MMCUserModel();

            if (reader.IsColumnExists("Id"))
            {
                item.id = SqlHelper.GetNullableInt32(reader, "Id");
            }
            if (reader.IsColumnExists("CompanyId"))
            {
                item.companyId = SqlHelper.GetNullableInt32(reader, "CompanyId");
            }
            if (reader.IsColumnExists("Company"))
            {
                item.company = SqlHelper.GetNullableString(reader, "Company");
            }
            if (reader.IsColumnExists("FirstName"))
            {
                item.firstName = SqlHelper.GetNullableString(reader, "FirstName");
            }
            if (reader.IsColumnExists("LastName"))
            {
                item.lastName = SqlHelper.GetNullableString(reader, "LastName");
            }
            if (reader.IsColumnExists("EmailAddress"))
            {
                item.emailAddress = SqlHelper.GetNullableString(reader, "EmailAddress");
            }
            if (reader.IsColumnExists("CityId"))
            {
                item.cityId = SqlHelper.GetNullableInt32(reader, "CityId");
            }
            if (reader.IsColumnExists("StateId"))
            {
                item.stateId = SqlHelper.GetNullableInt32(reader, "StateId");
            }
            if (reader.IsColumnExists("CountryId"))
            {
                item.countryId = SqlHelper.GetNullableInt32(reader, "CountryId");
            }
            if (reader.IsColumnExists("Country"))
            {
                item.country = SqlHelper.GetNullableString(reader, "Country");
            }
            if (reader.IsColumnExists("IsActive"))
            {
                item.isActive = SqlHelper.GetBoolean(reader, "IsActive");
            }
            if (reader.IsColumnExists("Role"))
            {
                item.role = SqlHelper.GetNullableString(reader, "Role");
            }
            return item;
        }

        public static List<MMCUserModel> TranslateAsMMCUsersList(this SqlDataReader reader)
        {
            var list = new List<MMCUserModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsMMCUser(reader, true));
            }
            return list;
        }

        public static MMCCompanyModel TranslateAsMMCCompany(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new MMCCompanyModel();

            if (reader.IsColumnExists("Id"))
            {
                item.id = SqlHelper.GetNullableInt32(reader, "Id");
            }
            if (reader.IsColumnExists("Name"))
            {
                item.name = SqlHelper.GetNullableString(reader, "Name");
            }
            
            return item;
        }

        public static List<MMCCompanyModel> TranslateAsMMCCompanyList(this SqlDataReader reader)
        {
            var list = new List<MMCCompanyModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsMMCCompany(reader, true));
            }
            return list;
        }
    }
}
