namespace CVManager.DAL;

public class UoW : IUoW
{
    private readonly DataContext _context;
    private readonly Lazy<IDegreeRepository> _degree;
    private readonly Lazy<ICvRepository> _cv;

    public UoW(DataContext context)
    {
        _context = context;
        _degree = new Lazy<IDegreeRepository>(() => new DegreeRepository(_context));
        _cv = new Lazy<ICvRepository>(() => new CvRepository(_context));
    }

    public IDegreeRepository Degree => _degree.Value;
    public ICvRepository Cv => _cv.Value;

    public Task<int> Save()
    {
        return _context.SaveChangesAsync();
    }

}