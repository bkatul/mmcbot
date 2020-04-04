using MMCApi.Model;
using MMCApi.Model.UserMMC;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MMCApi.Translators
{
    public static class ReceiptTranslator
    {
        public static AI_GetReceiptModel TranslateAsReceipt(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new AI_GetReceiptModel();
            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("CategoryId"))
            {
                item.CategoryId = SqlHelper.GetNullableString(reader, "CategoryId");
            }

            if (reader.IsColumnExists("AttachmentNo"))
            {
                item.AttachmentNo = SqlHelper.GetNullableString(reader, "AttachmentNo");
            }

            if (reader.IsColumnExists("BillDate"))
            {
                item.BillDate = SqlHelper.GetNullableDateTime(reader, "BillDate");
                if (item.BillDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.BillDate = DateTime.Today;
                }
            }

            if (reader.IsColumnExists("BillNumber"))
            {
                item.BillNumber = SqlHelper.GetNullableString(reader, "BillNumber");
            }

            if (reader.IsColumnExists("VendorName"))
            {
                item.VendorName = SqlHelper.GetNullableString(reader, "VendorName");
            }

            if (reader.IsColumnExists("CompanyName"))
            {
                item.CompanyName = SqlHelper.GetNullableString(reader, "CompanyName");
            }

            if (reader.IsColumnExists("VendorAddress"))
            {
                item.VendorAddress = SqlHelper.GetNullableString(reader, "VendorAddress");
            }

            if (reader.IsColumnExists("PhoneNumber"))
            {
                item.PhoneNumber = SqlHelper.GetNullableString(reader, "PhoneNumber");
            }

            if (reader.IsColumnExists("VATNo"))
            {
                item.VATNo = SqlHelper.GetNullableString(reader, "VATNo");
            }

            if (reader.IsColumnExists("ItemCode"))
            {
                item.ItemCode = SqlHelper.GetNullableString(reader, "ItemCode");
            }

            if (reader.IsColumnExists("Description"))
            {
                item.Description = SqlHelper.GetNullableString(reader, "Description");
            }

            if (reader.IsColumnExists("Quantity"))
            {
                item.Quantity = SqlHelper.GetNullableString(reader, "Quantity");
            }

            if (reader.IsColumnExists("UnitPrice"))
            {
                item.UnitPrice = SqlHelper.GetNullableDecimal(reader, "UnitPrice");
            }

            if (reader.IsColumnExists("AccountCode"))
            {
                item.AccountCode = SqlHelper.GetNullableString(reader, "AccountCode");
            }

            if (reader.IsColumnExists("Category"))
            {
                item.Category = SqlHelper.GetNullableString(reader, "Category");
            }

            if (reader.IsColumnExists("TaxType"))
            {
                item.TaxType = SqlHelper.GetNullableString(reader, "TaxType");
            }

            if (reader.IsColumnExists("TaxAmount"))
            {
                item.TaxAmount = SqlHelper.GetNullableDecimal(reader, "TaxAmount");
            }

            if (reader.IsColumnExists("TotalAmount"))
            {
                item.TotalAmount = SqlHelper.GetNullableDecimal(reader, "TotalAmount");
            }

            if (reader.IsColumnExists("Status"))
            {
                item.Status = SqlHelper.GetNullableString(reader, "Status");
            }

            if (reader.IsColumnExists("Url"))
            {
                item.Url = SqlHelper.GetNullableString(reader, "Url");
            }

            if (reader.IsColumnExists("CreatedBy"))
            {
                item.CreatedBy = SqlHelper.GetNullableString(reader, "CreatedBy");
            }

            if (reader.IsColumnExists("Subtotal"))
            {
                item.Subtotal = SqlHelper.GetNullableDecimal(reader, "Subtotal");
            }

            if (reader.IsColumnExists("MarkAsBillable"))
            {
                item.MarkAsBillable = SqlHelper.GetBoolean(reader, "MarkAsBillable");
            }

            if (reader.IsColumnExists("ClientId"))
            {
                item.ClientId = SqlHelper.GetNullableInt32(reader, "ClientId");
            }

            if (reader.IsColumnExists("Currency"))
            {
                item.Currency = SqlHelper.GetNullableString(reader, "Currency");
            }

            if (reader.IsColumnExists("TransactionNo"))
            {
                item.TransactionNo = SqlHelper.GetNullableString(reader, "TransactionNo");
            }

            if (reader.IsColumnExists("CreatedDate"))
            {
                item.CreatedDate = SqlHelper.GetNullableDateTime(reader, "CreatedDate");
                DateTime dtLocal = Convert.ToDateTime(item.CreatedDate);

                DateTime dtNew=dtLocal.ToLocalTime();
                item.CreatedDate = dtNew;
                if (item.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.CreatedDate = null;
                }
            }
            if (reader.IsColumnExists("ModifiedDate"))
            {
                item.ModifiedDate = SqlHelper.GetNullableDateTime(reader, "ModifiedDate");
                DateTime dtLocal = Convert.ToDateTime(item.ModifiedDate);

                DateTime dtNew = dtLocal.ToLocalTime();
                item.ModifiedDate = dtNew;
                if (item.ModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.ModifiedDate = null;
                }
            }

            if (reader.IsColumnExists("UploadedBy"))
            {
                item.UploadedBy = SqlHelper.GetNullableString(reader, "UploadedBy");
                StringBuilder strBuild = new StringBuilder();
                if (item.UploadedBy != null && item.UploadedBy != "")
                {
                    string str = Convert.ToString(item.UploadedBy);
                    string[] words = str.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        string strNew = UppercaseFirst(words[i]);
                        strBuild.Append(strNew);
                        strBuild.Append(' ');
                        item.UploadedBy = strBuild.ToString().TrimEnd();
                    }
                }
            }

            if (reader.IsColumnExists("Rate"))
            {
                item.Rate = SqlHelper.GetNullableDecimal(reader, "Rate");
            }
            if (reader.IsColumnExists("IsArchived"))
            {
                item.IsArchived = SqlHelper.GetBoolean(reader, "IsArchived");
            }
            return item;
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static List<AI_GetReceiptModel> TranslateAsReceiptList(this SqlDataReader reader)
        {
            var list = new List<AI_GetReceiptModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsReceipt(reader, true));
            }
            return list;
        }


        public static UpdateCategory TranslateAsReceiptGetCategory(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new UpdateCategory();
            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("CategoryId"))
            {
                item.CategoryId = SqlHelper.GetNullableString(reader, "CategoryId");
            }

            if (reader.IsColumnExists("VendorName"))
            {
                item.VendorName = SqlHelper.GetNullableString(reader, "VendorName");
            }

            return item;
        }

        public static List<UpdateCategory> TranslateAsReceiptGetCategoryList(this SqlDataReader reader)
        {
            var list = new List<UpdateCategory>();
            while (reader.Read())
            {
                list.Add(TranslateAsReceiptGetCategory(reader, true));
            }
            return list;
        }

        public static MMCReceiptModel TranslateAsReceiptForWeb(this SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                reader.Read();
            }
            var item = new MMCReceiptModel();
            if (reader.IsColumnExists("Id"))
            {
                item.Id = SqlHelper.GetNullableInt32(reader, "Id");
            }

            if (reader.IsColumnExists("CategoryId"))
            {
                item.CategoryId = SqlHelper.GetNullableString(reader, "CategoryId");
            }

            if (reader.IsColumnExists("AttachmentNo"))
            {
                item.AttachmentNo = SqlHelper.GetNullableString(reader, "AttachmentNo");
            }

            if (reader.IsColumnExists("BillDate"))
            {
                item.BillDate = SqlHelper.GetNullableDateTime(reader, "BillDate");
                if (item.BillDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.BillDate = DateTime.Today;
                }
            }

            if (reader.IsColumnExists("BillNumber"))
            {
                item.BillNumber = SqlHelper.GetNullableString(reader, "BillNumber");
            }

            if (reader.IsColumnExists("VendorName"))
            {
                item.VendorName = SqlHelper.GetNullableString(reader, "VendorName");
            }

            if (reader.IsColumnExists("CompanyName"))
            {
                item.CompanyName = SqlHelper.GetNullableString(reader, "CompanyName");
            }

            if (reader.IsColumnExists("VendorAddress"))
            {
                item.VendorAddress = SqlHelper.GetNullableString(reader, "VendorAddress");
            }

            if (reader.IsColumnExists("PhoneNumber"))
            {
                item.PhoneNumber = SqlHelper.GetNullableString(reader, "PhoneNumber");
            }

            if (reader.IsColumnExists("VATNo"))
            {
                item.VATNo = SqlHelper.GetNullableString(reader, "VATNo");
            }

            if (reader.IsColumnExists("ItemCode"))
            {
                item.ItemCode = SqlHelper.GetNullableString(reader, "ItemCode");
            }

            if (reader.IsColumnExists("Description"))
            {
                item.Description = SqlHelper.GetNullableString(reader, "Description");
            }

            if (reader.IsColumnExists("Quantity"))
            {
                item.Quantity = SqlHelper.GetNullableString(reader, "Quantity");
            }

            if (reader.IsColumnExists("UnitPrice"))
            {
                item.UnitPrice = SqlHelper.GetNullableDecimal(reader, "UnitPrice");
            }

            if (reader.IsColumnExists("AccountCode"))
            {
                item.AccountCode = SqlHelper.GetNullableString(reader, "AccountCode");
            }

            if (reader.IsColumnExists("Category"))
            {
                item.Category = SqlHelper.GetNullableString(reader, "Category");
            }

            if (reader.IsColumnExists("TaxType"))
            {
                item.TaxType = SqlHelper.GetNullableString(reader, "TaxType");
            }

            if (reader.IsColumnExists("TaxAmount"))
            {
                item.TaxAmount = SqlHelper.GetNullableDecimal(reader, "TaxAmount");
            }

            if (reader.IsColumnExists("TotalAmount"))
            {
                item.TotalAmount = SqlHelper.GetNullableDecimal(reader, "TotalAmount");
            }

            if (reader.IsColumnExists("Status"))
            {
                item.Status = SqlHelper.GetNullableString(reader, "Status");
            }

            if (reader.IsColumnExists("Url"))
            {
                item.Url = SqlHelper.GetNullableString(reader, "Url");
            }

            if (reader.IsColumnExists("CreatedBy"))
            {
                item.CreatedBy = SqlHelper.GetNullableString(reader, "CreatedBy");
            }

            if (reader.IsColumnExists("Subtotal"))
            {
                item.Subtotal = SqlHelper.GetNullableDecimal(reader, "Subtotal");
            }

            if (reader.IsColumnExists("MarkAsBillable"))
            {
                item.MarkAsBillable = SqlHelper.GetBoolean(reader, "MarkAsBillable");
            }

            if (reader.IsColumnExists("ClientId"))
            {
                item.ClientId = SqlHelper.GetNullableInt32(reader, "ClientId");
            }

            if (reader.IsColumnExists("Currency"))
            {
                item.Currency = SqlHelper.GetNullableString(reader, "Currency");
            }

            if (reader.IsColumnExists("TransactionNo"))
            {
                item.TransactionNo = SqlHelper.GetNullableString(reader, "TransactionNo");
            }

            if (reader.IsColumnExists("CreatedDate"))
            {
                item.CreatedDate = SqlHelper.GetNullableDateTime(reader, "CreatedDate");
                DateTime dtLocal = Convert.ToDateTime(item.CreatedDate);

                DateTime dtNew = dtLocal.ToLocalTime();
                item.CreatedDate = dtNew;
                if (item.CreatedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.CreatedDate = null;
                }
            }
            if (reader.IsColumnExists("ModifiedDate"))
            {
                item.ModifiedDate = SqlHelper.GetNullableDateTime(reader, "ModifiedDate");
                DateTime dtLocal = Convert.ToDateTime(item.ModifiedDate);

                DateTime dtNew = dtLocal.ToLocalTime();
                item.ModifiedDate = dtNew;
                if (item.ModifiedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                {
                    item.ModifiedDate = null;
                }
            }

            if (reader.IsColumnExists("UploadedBy"))
            {
                item.UploadedBy = SqlHelper.GetNullableString(reader, "UploadedBy");
                StringBuilder strBuild = new StringBuilder();
                if (item.UploadedBy != null && item.UploadedBy != "")
                {
                    string str = Convert.ToString(item.UploadedBy);
                    string[] words = str.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        string strNew = UppercaseFirst(words[i]);
                        strBuild.Append(strNew);
                        strBuild.Append(' ');
                        item.UploadedBy = strBuild.ToString().TrimEnd();
                    }
                }
            }

            if (reader.IsColumnExists("CurrencySymbol"))
            {
                item.CurrencySymbol = SqlHelper.GetNullableString(reader, "CurrencySymbol");
            }
            if (reader.IsColumnExists("Rate"))
            {
                item.Rate = SqlHelper.GetNullableDecimal(reader, "Rate");
            }
            return item;
        }

        public static List<MMCReceiptModel> TranslateAsReceiptListForWeb(this SqlDataReader reader)
        {
            var list = new List<MMCReceiptModel>();
            while (reader.Read())
            {
                list.Add(TranslateAsReceiptForWeb(reader, true));
            }
            return list;
        }

    }
}
