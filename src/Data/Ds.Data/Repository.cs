using Dapper;
using Ds.Core;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Ds.Data
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersAsync(string term)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                searchKeyword = term
            };
            return await connection.QueryAsync<Supplier>("GetSuppliers", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(string term)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                searchKeyword = term
            };
            return await connection.QueryAsync<Item>("GetItems", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<(IEnumerable<PurchaseOrder> Data, int TotalRecords)> GetPagedPurchaseOrdersAsync(int pageIndex, int pageSize, string searchTerm, string sortColumn, string sortDirection)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };

            var result = await connection.QueryMultipleAsync(
                "GetPagedPurchaseOrders", // Your stored procedure name
                parameters,
                commandType: CommandType.StoredProcedure);

            var data = result.Read<PurchaseOrder>().ToList();
            var totalRecords = result.Read<int>().Single();

            return (data, totalRecords);
        }
        public async Task<int> InsertOrUpdatePurchaseOrderAndDetailsAsync(PurchaseOrder purchaseOrder)
        {
            using var connection = new SqlConnection(_connectionString);
            var orderDetailsDataTable = ConvertToOrderDetailsDataTable(purchaseOrder.OrderDetails);
            var parameters = new DynamicParameters();
            parameters.Add("@PurchaseOrderId", purchaseOrder.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ReferenceId", purchaseOrder.ReferenceId, DbType.String);
            parameters.Add("@OrderNumber", purchaseOrder.OrderNumber, DbType.String);
            parameters.Add("@SupplierId", purchaseOrder.SupplierId, DbType.Int32);
            parameters.Add("@OrderDate", purchaseOrder.OrderDate, DbType.DateTime);
            parameters.Add("@ExpectedDate", purchaseOrder.ExpectedDate, DbType.DateTime);
            parameters.Add("@Remark", purchaseOrder.Remark, DbType.String);
            parameters.Add("@OrderDetailsType", orderDetailsDataTable.AsTableValuedParameter("dbo.OrderDetailType"));

            return await connection.ExecuteAsync("InsertOrUpdatePurchaseOrderAndDetails", parameters, commandType: CommandType.StoredProcedure);
        }

        private DataTable ConvertToOrderDetailsDataTable(List<OrderDetail> orderDetails)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("ItemId", typeof(int));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Rate", typeof(decimal));

            foreach (var detail in orderDetails)
            {
                dataTable.Rows.Add(detail.Id ,detail.ItemId, detail.Quantity, detail.Rate);
            }

            return dataTable;
        }

        public async Task DeletePurchaseOrder(int purchaseOrderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", purchaseOrderId, DbType.Int32);

                await connection.ExecuteAsync("DeletePurchaseOrder", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<string> GetPurchaseOrderById(int purchaseOrderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var results = await connection.QueryFirstOrDefaultAsync<string>(
                    "GetPurchaseOrderById",
                    new { PurchaseOrderId = purchaseOrderId },
                    commandType: CommandType.StoredProcedure
                );

                return results;
            }
        }
    }
}