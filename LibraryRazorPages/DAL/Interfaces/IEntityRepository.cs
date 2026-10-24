namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class 
    {
        Task<TEntity?> GetAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<int> CreateAsync(TEntity entity, CancellationToken ct = default);
        Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
