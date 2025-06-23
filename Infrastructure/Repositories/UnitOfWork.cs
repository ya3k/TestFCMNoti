using Domain.Interfaces.Repositories;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Common;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EPDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private readonly ICurrentUser _currentUser;

        public UnitOfWork(EPDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
            _currentUser = currentUser;

        }
        public async Task SaveChangesAsync()
        {
            await ApplyAuditInfoAsync();
            await _context.SaveChangesAsync();
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repoInstance = new GenericRepository<T>(_context);
                _repositories[type] = repoInstance;
            }

            return (IGenericRepository<T>)_repositories[type];
        }
        public void Dispose() => _context.Dispose();

        private async Task ApplyAuditInfoAsync()
        {
            var userId = await _currentUser.GetCurrentUserIdAsync();
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable auditableEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedBy = userId;
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditableEntity.LastModifiedBy = userId;
                        auditableEntity.LastModified = DateTime.UtcNow;
                    }
                }

                if (entry.State == EntityState.Deleted && entry.Entity is IDeletable deletableEntity)
                {
                    deletableEntity.IsDeleted = true;
                    deletableEntity.DeletedBy = userId;
                    deletableEntity.DeletedOn = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }
        }

    }
}
