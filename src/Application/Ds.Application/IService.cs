using Ds.Application.Dto;
using Ds.Core;

namespace Ds.Application
{
    public interface ICommonService
    {
        Task<IEnumerable<DropdownDto>> GetSuppliersAsync(string term);
        Task<IEnumerable<DropdownDto>> GetItemsAsync(string term);
        Task<(IEnumerable<PurchaseOrder> Data, int TotalRecords)> GetPagedPurchaseOrdersAsync(int pageIndex, int pageSize, string searchTerm, string sortColumn, string sortDirection);
        Task<int> InsertOrUpdatePurchaseOrderAndDetailsAsync(PurchaseOrderDto purchaseOrder);
        Task DeletePurchaseOrder(int purchaseOrderId);
        Task<PurchaseOrderDto> GetPurchaseOrderById(int id);
    }

}
