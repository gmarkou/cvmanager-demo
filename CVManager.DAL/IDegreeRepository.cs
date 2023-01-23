using CVManager.DAL.Models;

namespace CVManager.DAL;

public interface IDegreeRepository
{
    Task<IEnumerable<Degree>> All(bool trackChanges = false);
    Task<Degree?> Get(int id, bool trackChanges = true);
    Degree Create(Degree degree);
    Degree Update(Degree degree);
    Degree Remove(Degree degree);
}