namespace CVManager.DAL;

public interface IUoW
{
    IDegreeRepository Degree { get; }
    ICvRepository Cv { get; }
    Task<int> Save();
}