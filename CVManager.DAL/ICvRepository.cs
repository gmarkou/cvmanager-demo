using CVManager.DAL.Models;

namespace CVManager.DAL;

public interface ICvRepository
{
    Task<IEnumerable<Cv>> All(bool trackChanges = false);

    Task<int> Count();
    Task<IEnumerable<Cv>> GetPageWithDegrees(int page, int pageSize, bool trackChanges = false);
    Task<Cv?> Get(int id, bool trackChanges = true);
    
    
    Cv Create(Cv cv);
    Cv Update(Cv cv);
    Cv Remove(Cv cv);

    Task<Cv?> GetCvWithDegree(int degreeId);

}