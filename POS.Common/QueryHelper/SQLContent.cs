using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.QueryHelper
{
    public static class SQLContent
    {
        public static string GetDashboardInitialDataQuery(int monthNumber, int branchID)
        {
            string sql = string.Format(@"WITH AllDays AS (
                SELECT 1 AS DayNumber
                UNION ALL
                SELECT DayNumber + 1
                FROM AllDays
                WHERE DayNumber < DAY(EOMONTH(DATEFROMPARTS(YEAR(GETDATE()), {0}, 1)))
            ),
            DayTotals AS (
                SELECT
                    DAY(SalesDate) AS DayID, MONTH(SalesDate) AS MonthID,
                    ISNULL(SUM(TotalSalesPrice), 0) AS TotalSales
                FROM
                    Sales
                WHERE
                    YEAR(SalesDate) = YEAR(GETDATE()) and BranchID = {1}
                GROUP BY
                    DAY(SalesDate), MONTH(SalesDate)
            ),
            PurchaseTotals AS (
                SELECT
                    DAY(PurchaseDate) AS DayID, MONTH(PurchaseDate) AS MonthID,
                    ISNULL(SUM(TotalPurchasePrice), 0) AS TotalPurchases
                FROM
                    Purchase
                WHERE
                    YEAR(PurchaseDate) = YEAR(GETDATE()) and BranchID = {1}
                GROUP BY
                    DAY(PurchaseDate), MONTH(PurchaseDate)
            ),
            ExpenseTotals AS (
                SELECT
                    DAY(CreatedDate) AS DayID, MONTH(CreatedDate) AS MonthID,
                    ISNULL(SUM(CASE WHEN PurchaseID = 0 THEN Amount ELSE 0 END), 0) AS TotalExpenses
                FROM
                    Expense
                WHERE
                    YEAR(CreatedDate) = YEAR(GETDATE()) and BranchID = {1}
                GROUP BY
                    DAY(CreatedDate), MONTH(CreatedDate)
            )
            SELECT
                AD.DayNumber,
                --LEFT(DATENAME(DAY, DATEFROMPARTS(YEAR(GETDATE()), AD.DayNumber, 1)), 3) AS MonthName,
                ISNULL(DT.TotalSales, 0) AS TotalSales,
                ISNULL(PT.TotalPurchases, 0) AS TotalPurchases,
                ISNULL(ET.TotalExpenses, 0) AS TotalExpenses
            FROM
                AllDays AD
            LEFT JOIN
                DayTotals DT ON AD.DayNumber = DT.DayID and DT.MonthID = {0}
            LEFT JOIN
                PurchaseTotals PT ON AD.DayNumber = PT.DayID and PT.MonthID = {0}
            LEFT JOIN
                ExpenseTotals ET ON AD.DayNumber = ET.DayID and ET.MonthID = {0}", monthNumber, branchID);

            return sql;
        }

        public static string GetAllProductByBranchID(int branchID)
        {
            string sql = string.Format(@"SELECT
                p.SKU, 
                p.ProductName, 
                p.Description, 
                p.Slug, 
                p.Image, 
                p.PurchasePrice, 
                p.FinalPrice, 
                ISNULL(SUM(bpm.Quantity), 0) Qty,
                p.MinQty,
                p.Status,
                p.CategoryID,
                p.BrandID,
                ISNULL(c.CategoryName, '') CategoryName, 
                ISNULL(b.BrandName, '') BrandName, 
                ISNULL(u.DisplayName, ISNULL(u.UnitName, '')) UnitName
            FROM dbo.Product AS p
            left JOIN dbo.Category AS c ON c.CategoryID = p.CategoryID 
            left JOIN dbo.Brand AS b ON b.BrandID = p.BrandID 
            left JOIN dbo.Unit AS u ON u.UnitID = p.Unit
            left JOIN BranchProductMapping AS bpm ON bpm.ProductID = p.ProductID and bpm.BranchID={0}
            Group by
                p.SKU, 
                p.ProductName, 
                p.Description, 
                p.Slug, 
                p.Image, 
                p.PurchasePrice, 
                p.FinalPrice, 
                p.MinQty,
                p.Status,
                p.CategoryID,
                p.BrandID,
                ISNULL(c.CategoryName, ''), 
                ISNULL(b.BrandName, ''), 
                ISNULL(u.DisplayName, ISNULL(u.UnitName, ''))", branchID);

            return sql;
        }

        public static string GetAccountBalanceExpenseByBranchID(int branchID)
        {
            string sql = string.Format(@"SELECT a.AccountID, 
                   a.AccountTitle,
	               acs.TotalSales,
	               acs.TotalDeposit,
                   (COALESCE(acs.TotalBalance, 0) - COALESCE(e.TotalExpense, 0)) AS CurrentBalance,
                   COALESCE(e.TotalExpense, 0) AS Expense, 
                   a.AccountNumber, 
                   a.Description, 
                   a.Status 
            FROM Account a 
            LEFT JOIN (
                SELECT AccountID, 
                        SUM(Amount) AS TotalExpense
                FROM Expense where BranchID={0}
                GROUP BY AccountID
            ) e ON a.AccountID = e.AccountID
            LEFT JOIN (
                SELECT AccountID, 
                       SUM(CASE WHEN IsDeposit = 0 THEN InBalance ELSE 0 END) AS TotalSales,
		               SUM(CASE WHEN IsDeposit = 1 THEN InBalance ELSE 0 END) AS TotalDeposit,
		               SUM(InBalance) AS TotalBalance
                FROM AccountStatement where BranchID={0}
                GROUP BY AccountID
            ) acs ON a.AccountID = acs.AccountID", branchID);

            return sql;
        }
    }
}
