namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class 
    {
        Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<int> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
