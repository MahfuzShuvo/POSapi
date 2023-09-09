using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.QueryHelper
{
    public static class SQLContent
    {
        public static string GetDashboardInitialDataQuery(int monthNumber)
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
                    YEAR(SalesDate) = YEAR(GETDATE())
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
                    YEAR(PurchaseDate) = YEAR(GETDATE())
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
                    YEAR(CreatedDate) = YEAR(GETDATE())
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
                ExpenseTotals ET ON AD.DayNumber = ET.DayID and ET.MonthID = {0}", monthNumber);

            return sql;
        }
    }
}
