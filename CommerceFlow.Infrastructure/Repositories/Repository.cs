namespace CommerceFlow.Infrastructure.Repositories
{
    public abstract class Repository(CommerceFlowDbContext dbContext) : IRepository
    {
        public ICommitScope CommitScope => dbContext;
        public IQueryable<TEntity> Set<TEntity>() where TEntity : Entity
        {
            var query = dbContext.Set<TEntity>();
            return query;
        }
    }
}
