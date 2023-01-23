using CVManager.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CVManager.DAL;

public class CvRepository: ICvRepository
{
    private readonly DataContext _context;
    
    public CvRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Cv>> All(bool trackChanges = false)
    {
        return trackChanges 
            ? await _context.Cvs!
                .OrderBy(cv => cv.LastName)
                .ThenBy(cv => cv.FirstName)
                .ToListAsync()
            : await _context.Cvs!
                .OrderBy(cv => cv.LastName)
                .ThenBy(cv => cv.FirstName)
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<int> Count()
    {
        return await _context.Cvs!.CountAsync();
    }

    public async Task<IEnumerable<Cv>> GetPageWithDegrees(int page, int pageSize, bool trackChanges = false)
    {
        return trackChanges 
            ? await _context.Cvs!
                .Include(cv => cv.Degree)
                .OrderBy(cv => cv.LastName)
                .ThenBy(cv => cv.FirstName)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync()
            : await _context.Cvs!
                .Include(cv => cv.Degree)
                .OrderBy(cv => cv.LastName)
                .ThenBy(cv => cv.FirstName)
                .Skip(page * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        
    }


    public Cv Create(Cv cv)
    {
        return _context.Cvs!.Add(cv).Entity;
    }

    public async Task<Cv?> Get(int id, bool trackChanges = true)
    {
        return trackChanges
            ? await _context.Cvs!
                .Include(cv => cv.Degree)
                .Where((cv) => cv.Id == id)
                .FirstOrDefaultAsync()
            : await _context.Cvs!
                .Include(cv => cv.Degree)
                .Where((cv) => cv.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }


    public Cv Update(Cv cv)
    {
        return _context.Cvs!.Update(cv).Entity;
    }

    public Cv Remove(Cv cv)
    {
        return _context.Cvs!.Remove(cv).Entity;
    }

    public async Task<Cv?> GetCvWithDegree(int degreeId)
    {
        return await _context.Cvs!.Where(cv => cv.DegreeId == degreeId).FirstOrDefaultAsync();
    }

}