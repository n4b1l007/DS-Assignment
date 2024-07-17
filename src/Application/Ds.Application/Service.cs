using AutoMapper;
using Ds.Application.Dto;
using Ds.Core;
using Ds.Data;
using Newtonsoft.Json;
using System.IO;

namespace Ds.Application
{
    public class CommonService : ICommonService
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;

        public CommonService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DropdownDto>> GetSuppliersAsync(string term)
        {
            var data = await _repository.GetSuppliersAsync(term);
            var result = _mapper.Map<IEnumerable<DropdownDto>>(data);
            return result;
        }

        public async Task<IEnumerable<DropdownDto>> GetItemsAsync(string term)
        {
            var data = await _repository.GetItemsAsync(term);
            var result = _mapper.Map<IEnumerable<DropdownDto>>(data);
            return result;
        }
        public async Task<(IEnumerable<PurchaseOrder> Data, int TotalRecords)> GetPagedPurchaseOrdersAsync(int pageIndex, int pageSize, string searchTerm, string sortColumn, string sortDirection)
        {
            return await _repository.GetPagedPurchaseOrdersAsync(pageIndex, pageSize, searchTerm, sortColumn, sortDirection);
        }
        public Task<int> InsertOrUpdatePurchaseOrderAndDetailsAsync(PurchaseOrderDto purchaseOrderdto)
        {
            PurchaseOrder purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderdto);
            return _repository.InsertOrUpdatePurchaseOrderAndDetailsAsync(purchaseOrder);
        }

        public async Task DeletePurchaseOrder(int purchaseOrderId)
        {
            await _repository.DeletePurchaseOrder(purchaseOrderId);
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderById(int id)
        {
            var result =  await _repository.GetPurchaseOrderById(id);
            
            PurchaseOrderDto purchaseOrderDto = JsonConvert.DeserializeObject<PurchaseOrderDto>(result);
            return purchaseOrderDto;
        }
    }
}
