namespace JobsBoard.Repostry.RepostryPattern
{
    public interface RepostryPattern<TEntity>
    {
        Task<IList<TEntity>> List(); 
        Task<TEntity> Find<TId>(TId id); 

        Task Create(TEntity entity);
        Task Update<TId>(TId id, TEntity entity);
        Task Delete<TId>(TId id);

        Task<List<TEntity>> Search(string term); 
    }
}
