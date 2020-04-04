using MMCApi.Model;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CoreApiAdoDemo.Translators
{
    public static class UserTranslator
    {
        public static UserModel TranslateAsUser(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new UserModel();

            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("AssignTo"))
            {
                item.AssignTo = SqlHelper.GetNullableString(reader, "AssignTo");
            }

            if (reader.IsColumnExists("CreatedBy"))
            {
                item.CreatedBy = SqlHelper.GetNullableString(reader, "CreatedBy");
            }

            if (reader.IsColumnExists("FirstName"))
            {
                string name = SqlHelper.GetNullableString(reader, "FirstName");
                string fName = UppercaseFirst(name);
                item.FirstName = fName;
             //   item.FirstName = SqlHelper.GetNullableString(reader, "FirstName");
            }

            if (reader.IsColumnExists("LastName"))
            {
                string name = SqlHelper.GetNullableString(reader, "LastName");
                string lName = UppercaseFirst(name);
                item.LastName = lName;
                //item.LastName = SqlHelper.GetNullableString(reader, "LastName");
            }

            if (reader.IsColumnExists("EmailId"))
            {
                item.EmailId = SqlHelper.GetNullableString(reader, "EmailId");
            }

            if (reader.IsColumnExists("Company"))
            {
                item.Company = SqlHelper.GetNullableString(reader, "Company");
            }

            if (reader.IsColumnExists("Password"))
            {
                item.Password = SqlHelper.GetNullableString(reader, "Password");
            }

            if (reader.IsColumnExists("Role"))
            {
                item.Role = SqlHelper.GetNullableString(reader, "Role");
            }

            if (reader.IsColumnExists("RegistrationId"))
            {
                item.RegistrationId = SqlHelper.GetNullableString(reader, "RegistrationId");
            }

            if (reader.IsColumnExists("Tag"))
            {
                item.Tag = SqlHelper.GetNullableString(reader, "Tag");
            }

            if (reader.IsColumnExists("CompanyId"))
            {
                item.CompanyId = SqlHelper.GetNullableInt32(reader, "CompanyId");
            }

            return item;
        }

        public static List<UserModel> TranslateAsUsersList(this SqlDataReader reader)
        {
            var list = new List<UserModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsUser(reader, true));
            }
            return list;
        }

        public static RoleModel TranslateAsRole(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new RoleModel();
            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("Role"))
            {
                item.Role = SqlHelper.GetNullableString(reader, "Role");
            }

            return item;
        }

        public static List<RoleModel> TranslateAsRoleList(this SqlDataReader reader)
        {
            var list = new List<RoleModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsRole(reader, true));
            }
            return list;
        }

        public static UserGetModel TranslateAsGetUser(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new UserGetModel();

            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("RoleId"))
            {
                item.RoleId = SqlHelper.GetNullableInt32(reader, "RoleId");
            }

            if (reader.IsColumnExists("AssignTo"))
            {
                item.AssignTo = SqlHelper.GetNullableString(reader, "AssignTo");
            }

            if (reader.IsColumnExists("CreatedBy"))
            {
                item.CreatedBy = SqlHelper.GetNullableString(reader, "CreatedBy");
            }

            if (reader.IsColumnExists("FirstName"))
            {
                string name= SqlHelper.GetNullableString(reader, "FirstName");
                string fName=UppercaseFirst(name);
                item.FirstName = fName;
            }

            if (reader.IsColumnExists("LastName"))
            {
                string name = SqlHelper.GetNullableString(reader, "LastName");
                string lName = UppercaseFirst(name);
                item.LastName = lName;
            }

            if (reader.IsColumnExists("EmailId"))
            {
                item.EmailId = SqlHelper.GetNullableString(reader, "EmailId");
            }

            if (reader.IsColumnExists("Company"))
            {
                item.Company = SqlHelper.GetNullableString(reader, "Company");
            }

            if (reader.IsColumnExists("Password"))
            {
                item.Password = SqlHelper.GetNullableString(reader, "Password");
            }

            if (reader.IsColumnExists("Role"))
            {
                item.Role = SqlHelper.GetNullableString(reader, "Role");
            }

            if (reader.IsColumnExists("RegistrationId"))
            {
                item.RegistrationId = SqlHelper.GetNullableString(reader, "RegistrationId");
            }

            if (reader.IsColumnExists("Tag"))
            {
                item.Tag = SqlHelper.GetNullableString(reader, "Tag");
            }

            if (reader.IsColumnExists("CompanyId"))
            {
                item.CompanyId = SqlHelper.GetNullableInt32(reader, "CompanyId");
            }

            if (reader.IsColumnExists("CreatedDate"))
            {
                item.CreatedDate = SqlHelper.GetNullableDateTime(reader, "CreatedDate");
            }

            if (reader.IsColumnExists("Address"))
            {
                item.Address = SqlHelper.GetNullableString(reader, "Address");
            }
            if (reader.IsColumnExists("CountryId"))
            {
                item.CountryId = SqlHelper.GetNullableInt32(reader, "CountryId");
            }
            return item;
        }

        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static List<UserGetModel> TranslateAsGetUsersList(this SqlDataReader reader)
        {
            var list = new List<UserGetModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsGetUser(reader, true));
            }
            return list;
        }

        public static CountriesModel TranslateAsGetCountries(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new CountriesModel();

            if (reader.IsColumnExists("CountryID"))
            {
                item.CountryID = SqlHelper.GetNullableInt32(reader, "CountryID");
            }

            if (reader.IsColumnExists("CountryName"))
            {
                item.CountryName = SqlHelper.GetNullableString(reader, "CountryName");
            }

            if (reader.IsColumnExists("ISO3"))
            {
                item.ISO3 = SqlHelper.GetNullableString(reader, "ISO3");
            }

            return item;
        }

        public static List<CountriesModel> TranslateAsGetCountriesList(this SqlDataReader reader)
        {
            var list = new List<CountriesModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsGetCountries(reader, true));
            }
            return list;
        }

        public static MMCRoleModel TranslateAsMMCRole(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new MMCRoleModel();
            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("Name"))
            {
                item.Name = SqlHelper.GetNullableString(reader, "Name");
            }

            return item;
        }

        public static List<MMCRoleModel> TranslateAsMMCRoleList(this SqlDataReader reader)
        {
            var list = new List<MMCRoleModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsMMCRole(reader, true));
            }
            return list;
        }

        public static CurrencyModel TranslateAsCurrency(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new CurrencyModel();
            if (reader.IsColumnExists("code"))
            {
                item.code = SqlHelper.GetNullableString(reader, "code");
            }

            if (reader.IsColumnExists("symbol"))
            {
                item.symbol = SqlHelper.GetNullableString(reader, "symbol");
            }

            return item;
        }

        public static List<CurrencyModel> TranslateAsCurrencyList(this SqlDataReader reader)
        {
            var list = new List<CurrencyModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsCurrency(reader, true));
            }
            return list;
        }
    }
}
