using Ds.Core;
using System;

namespace Ds.Data
{
    public interface IRepository
    {
        Task<IEnumerable<Supplier>> GetSuppliersAsync(string term);
        Task<IEnumerable<Item>> GetItemsAsync(string term);
        Task<int> InsertOrUpdatePurchaseOrderAndDetailsAsync(PurchaseOrder purchaseOrder);
        Task<(IEnumerable<PurchaseOrder> Data, int TotalRecords)> GetPagedPurchaseOrdersAsync(int pageIndex, int pageSize, string searchTerm, string sortColumn, string sortDirection);
        Task DeletePurchaseOrder(int purchaseOrderId);
        Task<string> GetPurchaseOrderById(int purchaseOrderId);
    }
}
