using BusinessObject.Models;

namespace Repository.Interfaces
{
    public interface IContractTemplateRepository
    {
        Task<IEnumerable<ContractTemplate>> GetAllAsync();
        Task<ContractTemplate?> GetByIdAsync(Guid id);
        Task<ContractTemplate> CreateAsync(ContractTemplate template);
        Task UpdateAsync(ContractTemplate template);
        Task DeleteAsync(Guid id);

        Task AddClauseAsync(ContractClause clause);
        Task AddVariableAsync(ContractVariable variable);
    }
}
