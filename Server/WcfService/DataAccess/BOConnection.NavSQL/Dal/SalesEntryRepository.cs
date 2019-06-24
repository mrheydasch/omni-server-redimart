﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using LSOmni.Common.Util;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSOmni.DataAccess.BOConnection.NavSQL.Dal
{
    public class SalesEntryRepository : BaseRepository
    {
        private string sql = string.Empty;
        private string sqlSearch = string.Empty;
        private string documentId = string.Empty;
        private string externalId = string.Empty;
        private string externalIdString = string.Empty;
        private string cac = string.Empty;
        private string cac2 = string.Empty;


        public SalesEntryRepository(BOConfiguration config, Version navVersion) : base(config, navVersion)
        {
            documentId = (navVersion > new Version("13.5")) ? "Document ID" : "Document Id";
            externalId = (navVersion > new Version("13.5")) ? "External ID" : "Web Trans_ GUID";
            externalIdString = (navVersion > new Version("13.5")) ? "''" : "NULL";
            cac = (navVersion > new Version("13.5")) ? "Click and Collect Order" : "ClickAndCollectOrder";
            cac2 = (navVersion > new Version("13.5")) ? "Click and Collect Order" : "Click And Collect Order";

            sql = "(" +
                "SELECT mt.[Document No_] AS [Document ID], MAX(mt.[Store No_]) AS [Store No_], MAX(mt.[Date]) AS [Date]," +
                "MAX(mt.[Member Card No_]) AS [Member Card No_], 1 AS [Posted], 0 AS [PayCount]," +
                externalIdString + " AS [External ID],'' AS [Sales Order No_], 0 AS [Click and Collect Order]," +
                "MAX(mt.[Transaction No_]) AS [Transaction No_], SUM(mt.[Quantity]) AS [Quantity]," +
                "SUM(mt.[Net Amount]) AS [Net Amount],SUM(mt.[Gross Amount]) AS [Gross Amount],SUM(mt.[Discount Amount]) AS [Discount Amount]," +
                "MAX(st.[Name]) AS [Name],MAX(mt.[POS Terminal No_]) AS [POS Terminal No_], 1 AS [Transaction]" +
                "FROM[" + navCompanyName + "Member Sales Entry] mt " +
                "JOIN[" + navCompanyName + "Store] st on st.[No_] = mt.[Store No_] " +
                "WHERE [Transaction No_] != 0" +
                "GROUP BY [Document No_]" +
                "UNION " +
                "SELECT mt.[" + documentId + "],mt.[Store No_],mt.[Document DateTime],mt.[Member Card No_], 0 AS Posted," +
                "(SELECT COUNT(*) FROM[" + navCompanyName + "Customer Order Payment] cop WHERE cop.[" + documentId + "] = mt.[" + documentId + "]) AS PayCount" +
                " , mt.[" + externalId + "],mt.[Sales Order No_],mt.[" + cac2 + "]," +
                "'' AS [Transaction No_], 0 AS [Quantity],0 AS [Net Amount],0 AS [Gross Amount],0 AS [Discount Amount]," +
                "(SELECT [Name] FROM [" + navCompanyName + "Store] WHERE [No_] = mt.[Store No_]) AS [Name],'' AS [POS Terminal No_], 0 AS [Transaction]" +
                "FROM[" + navCompanyName + "Customer Order Header] mt " +
                "UNION " +
                "SELECT mt.[" + documentId + "], mt.[Store No_], mt.[Document DateTime], mt.[Member Card No_],1 AS Posted," +
                "(SELECT COUNT(*) FROM [" + navCompanyName + "Posted Customer Order Payment] cop WHERE cop.[" + documentId + "]= mt.[" + documentId + "]) AS PayCount" +
                " , mt.[" + externalId + "],mt.[Sales Order No_],mt.[" + cac + "]," +
                "'' AS [Transaction No_], 0 AS [Quantity],0 AS [Net Amount],0 AS [Gross Amount],0 AS [Discount Amount]," +
                "(SELECT [Name] FROM [" + navCompanyName + "Store] WHERE [No_] = mt.[Store No_]) AS [Name],'' AS [POS Terminal No_], 0 AS [Transaction]" +
                "FROM[" + navCompanyName + "Posted Customer Order Header] mt " +
                "WHERE (SELECT COUNT(*) FROM [" + navCompanyName + "Member Sales Entry] WHERE [Document No_] = [Receipt No_]) = 0" +
                ") AS SalesEntries";

            sqlSearch = "(" +
                "SELECT DISTINCT mt.[Document No_] AS [Document ID], MAX(mt.[Store No_]) AS [Store No_], MAX(mt.[Date]) AS [Date]," +
                "MAX(mt.[Member Card No_]) AS [Member Card No_], 1 AS[Posted], 0 AS[PayCount]," +
                "'' AS[External ID],'' AS[Sales Order No_], 0 AS[Click and Collect Order]," +
                "MAX(mt.[Transaction No_]) AS[Transaction No_], SUM(mt.[Quantity]) AS[Quantity]," +
                "SUM(mt.[Net Amount]) AS[Net Amount],SUM(mt.[Gross Amount]) AS[Gross Amount],SUM(mt.[Discount Amount]) AS[Discount Amount]," +
                "MAX(st.[Name]) AS[Name],MAX(mt.[POS Terminal No_]) AS[POS Terminal No_], 1 AS[Transaction] " +
                "FROM[" + navCompanyName + "Member Sales Entry] mt " +
                "JOIN[" + navCompanyName + "Store] st on st.[No_] = mt.[Store No_] " +
                "INNER JOIN[" + navCompanyName + "Item] i on mt.[Item No_] = i.[No_] " +
                "WHERE[Transaction No_] != 0 AND UPPER(i.Description) LIKE UPPER(@search) " +
                "GROUP BY[Document No_] " +
                "UNION " +
                "SELECT DISTINCT mt.[" + documentId + "],mt.[Store No_],mt.[Document DateTime],mt.[Member Card No_], 0 AS Posted," +
                "(SELECT COUNT(*) FROM[" + navCompanyName + "Customer Order Payment] cop WHERE cop.[" + documentId + "] = mt.[" + documentId + "]) AS PayCount" +
                ", mt.[" + externalId + "],mt.[Sales Order No_],mt.[" + cac2 + "]," +
                "'' AS[Transaction No_], 0 AS[Quantity],0 AS[Net Amount],0 AS[Gross Amount],0 AS[Discount Amount]," +
                "(SELECT[Name] FROM [" + navCompanyName + "Store] WHERE[No_] = mt.[Store No_]) AS[Name],'' AS[POS Terminal No_], 0 AS[Transaction] " +
                "FROM[" + navCompanyName + "Customer Order Header] mt " +
                "INNER JOIN[" + navCompanyName + "Customer Order Line] ol on ol.[" + documentId + "] = mt.[" + documentId + "] " +
                "WHERE UPPER([Item Description]) LIKE UPPER(@search) " +
                "UNION " +
                "SELECT mt.[" + documentId + "], mt.[Store No_], mt.[Document DateTime], mt.[Member Card No_],1 AS Posted," +
                "(SELECT COUNT(*) FROM [" + navCompanyName + "Posted Customer Order Payment] cop WHERE cop.[" + documentId + "]= mt.[" + documentId + "]) AS PayCount" +
                ", mt.[" + externalId + "],mt.[Sales Order No_],mt.[" + cac + "]," +
                "'' AS[Transaction No_], 0 AS[Quantity],0 AS[Net Amount],0 AS[Gross Amount],0 AS[Discount Amount]," +
                "(SELECT[Name] FROM [" + navCompanyName + "Store] WHERE[No_] = mt.[Store No_]) AS[Name],'' AS[POS Terminal No_], 0 AS[Transaction] " +
                "FROM[" + navCompanyName + "Posted Customer Order Header] mt " +
                "INNER JOIN[" + navCompanyName + "Posted Customer Order Line] ol on ol.[" + documentId + "] = mt.[" + documentId + "] " +
                "WHERE (SELECT COUNT(*) FROM [" + navCompanyName + "Member Sales Entry] WHERE[Document No_] = [Receipt No_]) = 0 " +
                "AND UPPER([Item Description]) LIKE UPPER(@search) " +
                ") AS SalesEntries ";
        }

        public List<SalesEntry> SalesEntriesByCardId(string cardId, int maxNrOfEntries)
        {
            List<SalesEntry> list = new List<SalesEntry>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = "SELECT " + (maxNrOfEntries > 0 ? "TOP " + maxNrOfEntries : "") + " * FROM" + sql + " WHERE [Member Card No_]=@id ORDER BY [Date] DESC";
                    command.Parameters.AddWithValue("@id", cardId);
                    TraceSqlCommand(command);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (SQLHelper.GetBool(reader["Transaction"]))
                                list.Add(TransactionToSalesEntry(reader, false));
                            else
                                list.Add(OrderToSalesEntry(reader, false));
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return list;
        }


        public SalesEntry SalesEntryGetById(string entryId)
        {
            SalesEntry entry = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT mt.[Transaction No_], MAX(mt.[Member Card No_]) AS [Member Card No_],SUM(mt.[Quantity]) AS [Quantity]," +
                        "SUM(mt.[Net Amount]) AS [Net Amount],SUM(mt.[Gross Amount]) AS[Gross Amount],SUM(mt.[Discount Amount]) AS[Discount Amount],MAX(mt.[Date]) AS [Date]," +
                        "MAX(mt.[Store No_]) AS [Store No_], MAX(st.[Name]) AS [Name],MAX(mt.[POS Terminal No_]) AS[POS Terminal No_],MAX(mt.[Document No_]) AS [Document ID] " +
                        "FROM [" + navCompanyName + "Member Sales Entry] mt " +
                        "JOIN [" + navCompanyName + "Store] st on st.[No_] = mt.[Store No_] " +
                        "WHERE [Document No_]=@id " +
                        "GROUP BY [Transaction No_] ";

                    command.Parameters.AddWithValue("@id", entryId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entry = TransactionToSalesEntry(reader, true);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return entry;
        }

        public List<SalesEntry> SalesEntrySearch(string search, string cardId, int maxNumberOfTransactions, bool includeLines)
        {
            List<SalesEntry> list = new List<SalesEntry>();
            if (string.IsNullOrWhiteSpace(search))
                return list;

            SQLHelper.CheckForSQLInjection(search);

            char[] sep = new char[] { ' ' };
            string[] searchitems = search.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            string searchWords = string.Empty;
            foreach (string si in searchitems)
            {
                searchWords += string.Format("%{0}", si);
            }
            searchWords += "%";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " + (maxNumberOfTransactions > 0 ? "TOP " + maxNumberOfTransactions + " * FROM " : "* FROM ") + sqlSearch+
                        " WHERE [Member Card No_]=@cardId ORDER BY [Date] DESC ";

                    command.Parameters.AddWithValue("@cardId", cardId);
                    command.Parameters.AddWithValue("@search", searchWords);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (SQLHelper.GetBool(reader["Transaction"]))
                                list.Add(TransactionToSalesEntry(reader, includeLines));
                            else
                                list.Add(OrderToSalesEntry(reader, includeLines));
                        }
                    }
                    connection.Close();
                }
            }
            return list;
        }

        private void OrderLinesGetTotals(string orderId, out int itemCount, out decimal totalAmount, out decimal totalNetAmount, out decimal totalDiscount)
        {
            itemCount = 0;
            totalAmount = 0;
            totalNetAmount = 0;
            totalDiscount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string select = "SELECT [" + documentId + "], SUM([Quantity]) AS Cnt, SUM([Discount Amount]) AS Disc, SUM([Net Amount]) AS NAmt, SUM([Amount]) AS Amt";

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM (" + select +
                                            " FROM [" + navCompanyName + "Customer Order Line] GROUP BY [" + documentId + "] " +
                                            "UNION " + select +
                                            " FROM [" + navCompanyName + "Posted Customer Order Line] GROUP BY [" + documentId + "] " +
                                            ") AS OrderTotals WHERE [" + documentId + "]=@id";
                    command.Parameters.AddWithValue("@id", orderId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemCount = SQLHelper.GetInt32(reader["Cnt"]);
                            totalAmount = SQLHelper.GetDecimal(reader, "Amt");
                            totalNetAmount = SQLHelper.GetDecimal(reader, "NAmt");
                            totalDiscount = SQLHelper.GetDecimal(reader, "Disc");
                        }
                    }
                }
                connection.Close();
            }
        }

        public void OrderPointsGetTotal(string orderId, out decimal rewarded, out decimal used)
        {
            rewarded = 0;
            used = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    //Awarded points are linked to Sales Invoice Id
                    command.CommandText = "SELECT [No_] FROM [" + navCompanyName + "Sales Invoice Header] Where [External Document No_]=@id";
                    command.Parameters.AddWithValue("@id", orderId);
                    TraceSqlCommand(command);
                    logger.Info(config.LSKey.Key, "ORDERID: " + orderId);
                    connection.Open();
                    string salesId = (string)command.ExecuteScalar();

                    if (string.IsNullOrEmpty(salesId))
                    {
                        //Get rewarded points with SalesInvoice id
                        command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=0";
                        TraceSqlCommand(command);
                        var res1 = command.ExecuteScalar();
                        rewarded = res1 == null ? 0 : (decimal)res1;

                        command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=1";
                        TraceSqlCommand(command);
                        res1 = command.ExecuteScalar();
                        used = res1 == null ? 0 : -(decimal)res1;
                        return;
                    }

                    //Get Used points with orderId
                    command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id";
                    TraceSqlCommand(command);
                    var res = command.ExecuteScalar();
                    used = res == null ? 0 : -(decimal)res; //use '-' to convert to positive number

                    //Get rewarded points with SalesInvoice id
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", salesId);
                    TraceSqlCommand(command);
                    res = command.ExecuteScalar();
                    rewarded = res == null ? 0 : (decimal)res;
                }
                connection.Close();
            }
        }

        private SalesEntry TransactionToSalesEntry(SqlDataReader reader, bool includeLines)
        {
            SalesEntry entry = new SalesEntry()
            {
                Id = SQLHelper.GetString(reader["Document ID"]),
                IdType = DocumentIdType.Receipt,
                LineItemCount = (int)SQLHelper.GetDecimal(reader, "Quantity", true),
                TotalNetAmount = SQLHelper.GetDecimal(reader, "Net Amount", true),
                TotalAmount = SQLHelper.GetDecimal(reader, "Gross Amount", true),
                TotalDiscount = SQLHelper.GetDecimal(reader, "Discount Amount", false),
                DocumentRegTime = SQLHelper.GetDateTime(reader["Date"]),
                StoreId = SQLHelper.GetString(reader["Store No_"]),
                CardId = SQLHelper.GetString(reader["Member Card No_"]),
                ClickAndCollectOrder = false,
                AnonymousOrder = false,
                Status = SalesEntryStatus.Complete,
                PaymentStatus = PaymentStatus.Posted,
                Posted = true,
                ShippingStatus = ShippingStatus.ShippigNotRequired,
                TerminalId = SQLHelper.GetString(reader["POS Terminal No_"]),
                StoreName = SQLHelper.GetString(reader["Name"]),
            };

            SalesEntryPointsGetTotal(entry.Id, out decimal rewarded, out decimal used);
            entry.PointsRewarded = rewarded;
            entry.PointsUsedInOrder = used;

            if (includeLines)
            {
                entry.Lines = TransSalesEntryLinesGet(entry.Id);
                entry.Payments = TransSalesEntryPaymentGet(SQLHelper.GetString(reader["Transaction No_"]), entry.StoreId, SQLHelper.GetString(reader["POS Terminal No_"]), "");
            }
            return entry;
        }

        private SalesEntry OrderToSalesEntry(SqlDataReader reader, bool includeLines)
        {
            SalesEntry salesEntry = new SalesEntry
            {
                Id = SQLHelper.GetString(reader["Document ID"]),
                StoreId = SQLHelper.GetString(reader["Store No_"]),
                DocumentRegTime = SQLHelper.GetDateTime(reader["Date"]),
                IdType = DocumentIdType.Order,
                CardId = SQLHelper.GetString(reader["Member Card No_"]),
                Status = SalesEntryStatus.Created,
                StoreName = SQLHelper.GetString(reader["Name"])
            };

            salesEntry.Posted = SQLHelper.GetBool(reader["Posted"]);
            salesEntry.ClickAndCollectOrder = SQLHelper.GetBool(reader["Click And Collect Order"]);
            salesEntry.AnonymousOrder = string.IsNullOrEmpty(salesEntry.CardId);

            salesEntry.ExternalId = SQLHelper.GetString(reader["External ID"]);

            int copay = SQLHelper.GetInt32(reader["PayCount"]);
            salesEntry.PaymentStatus = (copay > 0) ? PaymentStatus.PreApproved : PaymentStatus.Approved;
            salesEntry.ShippingStatus = (salesEntry.ClickAndCollectOrder) ? ShippingStatus.ShippigNotRequired : ShippingStatus.NotYetShipped;

            OrderLinesGetTotals(salesEntry.Id, out int cnt, out decimal amt, out decimal namt, out decimal disc);
            salesEntry.LineItemCount = cnt;
            salesEntry.TotalAmount = amt;
            salesEntry.TotalNetAmount = namt;
            salesEntry.TotalDiscount = disc;

            if (salesEntry.Posted)
            {
                string sorderNo = SQLHelper.GetString(reader["Sales Order No_"]);
                salesEntry.Status = SalesEntryStatus.Complete;

                if (string.IsNullOrEmpty(sorderNo) == false)
                {
                    // we just use the data from the sales order for posted orders
                    int status = SaleOrderGetStatus(sorderNo);
                    if (status == 0)
                    {
                        salesEntry.Status = SalesEntryStatus.Pending;
                        salesEntry.ShippingStatus = ShippingStatus.NotYetShipped;
                    }
                    else
                    {
                        salesEntry.Status = SalesEntryStatus.Complete;
                    }
                }

                SalesEntryPointsGetTotal(salesEntry.Id, out decimal rewarded, out decimal used);
                salesEntry.PointsRewarded = rewarded;
                salesEntry.PointsUsedInOrder = used;
            }

            return salesEntry;
        }

        public List<SalesEntryLine> TransSalesEntryLinesGet(string receiptId)
        {
            string sqlsalelinecol = "ml.[Transaction No_],ml.[Store No_],ml.[POS Terminal No_],ml.[Item No_],ml.[Variant Code],ml.[Unit of Measure]," +
                             "ml.[Quantity],ml.[Price],ml.[Net Price],ml.[Net Amount],ml.[Discount Amount],ml.[VAT Amount],ml.[Refund Qty_],ml.[Line No_], i.[Description]";
            string sqlsalelinefrom = " FROM [" + navCompanyName + "Trans_ Sales Entry] ml " +
                "INNER JOIN  [" + navCompanyName + "Item] i on ml.[Item No_] = i.[No_]";

            List<SalesEntryLine> list = new List<SalesEntryLine>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " + sqlsalelinecol + sqlsalelinefrom +
                                          " WHERE ml.[Receipt No_]=@id ";
                    command.Parameters.AddWithValue("@id", receiptId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(TransToSalesEntryLine(reader));
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return list;
        }

        public List<SalesEntryPayment> TransSalesEntryPaymentGet(string transId, string storeId, string terminalId, string culture)
        {
            string sqltendercol = "ml.[Store No_],ml.[POS Terminal No_],ml.[Transaction No_],ml.[Line No_],ml.[Tender Type],ml.[Amount Tendered],ml.[Currency Code],ml.[Amount in Currency],t.[Description]";
            string sqltenderfrom = " FROM [" + navCompanyName + "Trans_ Payment Entry] ml LEFT OUTER JOIN [" + navCompanyName + "Tender Type] t ON t.[Code]=ml.[Tender Type] AND t.[Store No_]=ml.[Store No_]";

            List<SalesEntryPayment> list = new List<SalesEntryPayment>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT " + sqltendercol + sqltenderfrom +
                                          " WHERE ml.[Transaction No_]=@id AND ml.[Store No_]=@Sid AND ml.[POS Terminal No_]=@Tid ORDER BY ml.[Line No_]";
                    command.Parameters.AddWithValue("@id", transId);
                    command.Parameters.AddWithValue("@Sid", storeId);
                    command.Parameters.AddWithValue("@Tid", terminalId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(TransToSalesEntryPayment(reader, culture));
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return list;
        }

        public void SalesEntryPointsGetTotal(string entryId, out decimal rewarded, out decimal used)
        {
            rewarded = 0;
            used = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    //Awarded points are linked to Sales Invoice Id
                    command.CommandText = "SELECT [No_] FROM [" + navCompanyName + "Sales Invoice Header] Where [External Document No_]=@id";
                    command.Parameters.AddWithValue("@id", entryId);
                    TraceSqlCommand(command);
                    logger.Info(config.LSKey.Key, "ORDERID: " + entryId);
                    connection.Open();
                    string salesId = (string)command.ExecuteScalar();

                    //Use salesinvoice id to get Rewarded points for customer orders
                    if (!string.IsNullOrEmpty(salesId))
                        entryId = salesId;

                    //Get Used points with entryId (receiptId/orderId)
                    command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=1"; //Entry type = 1 is redemption
                    TraceSqlCommand(command);
                    var res = command.ExecuteScalar();
                    used = res == null ? 0 : -(decimal)res; //use '-' to convert to positive number

                    //Get rewarded points
                    command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=0";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", entryId);
                    TraceSqlCommand(command);
                    res = command.ExecuteScalar();
                    rewarded = res == null ? 0 : (decimal)res;
                }
                connection.Close();
            }
        }

        private int SaleOrderGetStatus(string id)
        {
            int status = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Status] FROM [" + navCompanyName + "Sales Header] WHERE [No_]=@id";
                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    var value = command.ExecuteScalar();
                    if (value != null)
                        status = (int)value;
                }
                connection.Close();
            }
            return status;
        }

        private SalesEntryLine TransToSalesEntryLine(SqlDataReader reader)
        {
            SalesEntryLine line = new SalesEntryLine()
            {
                LineNumber = Convert.ToInt32(SQLHelper.GetInt32(reader["Line No_"])),
                VariantId = SQLHelper.GetString(reader["Variant Code"]),
                UomId = SQLHelper.GetString(reader["Unit of Measure"]),
                Quantity = SQLHelper.GetDecimal(reader, "Quantity", true),
                LineType = LineType.Item,
                ItemId = SQLHelper.GetString(reader["Item No_"]),
                NetPrice = SQLHelper.GetDecimal(reader, "Net Price"),
                Price = SQLHelper.GetDecimal(reader, "Price"),
                DiscountAmount = SQLHelper.GetDecimal(reader, "Discount Amount", true),
                NetAmount = SQLHelper.GetDecimal(reader, "Net Amount", true),
                TaxAmount = SQLHelper.GetDecimal(reader, "VAT Amount", true),
                ItemDescription = SQLHelper.GetString(reader["Description"]),
            };

            line.Amount = line.NetAmount + line.TaxAmount;

            return line;
        }

        private SalesEntryPayment TransToSalesEntryPayment(SqlDataReader reader, string culture)
        {
            SalesEntryPayment pay = new SalesEntryPayment()
            {
                LineNumber = ConvertTo.SafeInt(SQLHelper.GetString(reader["Line No_"])),
                TenderType = SQLHelper.GetString(reader["Tender Type"]),
                Amount = SQLHelper.GetDecimal(reader, "Amount Tendered", false),
            };
            return pay;
        }
        public string FormatAmountToString(decimal amount, string culture)
        {
            return FormatAmount(amount, culture);
        }
    }
}