using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class ContractTemplateRepository : IContractTemplateRepository
    {
        private readonly AppDbContext _context;
        public ContractTemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContractTemplate>> GetAllAsync()
        {
            return await _context.ContractTemplates
                .Include(t => t.Clauses)
                .Include(t => t.Variables)
                .ToListAsync();
        }

        public async Task<ContractTemplate?> GetByIdAsync(Guid id)
        {
            return await _context.ContractTemplates
                .Include(t => t.Clauses)
                .Include(t => t.Variables)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ContractTemplate> CreateAsync(ContractTemplate template)
        {
            _context.ContractTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task UpdateAsync(ContractTemplate template)
        {
            _context.ContractTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.ContractTemplates.FindAsync(id);
            if (entity != null)
            {
                _context.ContractTemplates.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddClauseAsync(ContractClause clause)
        {
            _context.ContractClauses.Add(clause);
            await _context.SaveChangesAsync();
        }

        public async Task AddVariableAsync(ContractVariable variable)
        {
            _context.ContractVariables.Add(variable);
            await _context.SaveChangesAsync();
        }
    }
}
