using CVManager.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CVManager.DAL;

public class DegreeRepository: IDegreeRepository
{
    private readonly DataContext _context;
    
    public DegreeRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Degree>> All(bool trackChanges = false)
    {
        return trackChanges 
            ? await _context.Degrees!.OrderBy(d => d.Title).ToListAsync()
            : await _context.Degrees!.OrderBy(d => d.Title).AsNoTracking().ToListAsync();
    }

    public async Task<Degree?> Get(int id, bool trackChanges = true)
    {
        return trackChanges
            ? await _context.Degrees!.Where((d) => d.Id == id).FirstOrDefaultAsync()
            : await _context.Degrees!.Where((d) => d.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }

    public Degree Create(Degree degree)
    {
        return _context.Degrees!.Add(degree).Entity;
    }

    public Degree Update(Degree degree)
    {
        return _context.Degrees!.Update(degree).Entity;
    }

    public Degree Remove(Degree degree)
    {
        return _context.Degrees!.Remove(degree).Entity;
    }


}