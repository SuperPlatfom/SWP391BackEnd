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

        Task<List<ContractClause>> GetClausesByTemplateIdAsync(Guid templateId);
        Task<ContractClause?> GetClauseByIdAsync(Guid clauseId);
        void UpdateClause(ContractClause clause);
        void DeleteClause(ContractClause clause);
        Task<List<ContractVariable>> GetVariablesByTemplateIdAsync(Guid templateId);
        Task<ContractVariable?> GetVariableByIdAsync(Guid variableId);
        void UpdateVariable(ContractVariable variable);
        void DeleteVariable(ContractVariable variable);
        Task<int> SaveChangesAsync();
    }
}
