using AutoMapper;
using CVManager.DAL;
using CVManager.DAL.Models;
using CVManager.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Exception = System.Exception;

namespace CVManager.Controllers;

[Authorize]
public class DegreeController : ControllerBase
{
    private readonly IUoW _uow;
    private readonly IMapper _mapper;

    public DegreeController(IUoW uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    
    public async Task<IActionResult> Index()
    {
        var degrees = await _uow.Degree.All();
        return View(_mapper.Map<IEnumerable<DegreeDto>>(degrees));
    }

    [Authorize(Policy= "Editor")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy= "Editor")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DegreeDto degreeDto)
    {
        try
        {
            if (ModelState.GetValidationState(nameof(Cv.FirstName)) == ModelValidationState.Valid)
            {
                if (degreeDto.Title.Trim() == "")
                {
                    ModelState.AddModelError(nameof(Cv.FirstName), "Title could not be all spaces.");
                }            
            }
            if (!ModelState.IsValid)
            {
                return View(degreeDto);
            }

            _uow.Degree.Create(new Degree
            {
                Title = degreeDto.Title
            });
            await _uow.Save();
        
            SetFlash("Degree created successfully.", "flash--ok");
        }
        catch (Exception ex)
        {
            SetFlash("Something went wrong.", "flash--danger");
            return RedirectToAction("Create", "Degree");
        }
        
        return RedirectToAction("Index");
    }
    
    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Edit(int id)
    {
        var degree = await _uow.Degree.Get(id, false);
        if (degree == null)
        {
            return View("NotFound");
        }
        return View(_mapper.Map<DegreeDto>(degree));
    }

    [HttpPost]
    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Edit(DegreeDto degreeDto)
    {
        try
        {
            if (ModelState.GetValidationState(nameof(Cv.FirstName)) == ModelValidationState.Valid)
            {
                if (degreeDto.Title.Trim() == "")
                {
                    ModelState.AddModelError(nameof(Cv.FirstName), "Title could not be all spaces.");
                }            
            }
            if (!ModelState.IsValid)
            {
                return View(degreeDto);
            }
            
            _uow.Degree.Update(_mapper.Map<Degree>(degreeDto));
            await _uow.Save();
            SetFlash("Degree Updated.", "flash");
        }
        catch (Exception ex)
        {
            SetFlash("Something went wrong.", "flash--error");
        }
        return RedirectToAction("Index");
    }

    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Delete(int id)
    {
        var degree = await _uow.Degree.Get(id, false);
        if (degree == null)
        {
            return View("NotFound");
        }
        return View(_mapper.Map<DegreeDto>(degree));
    }

    public class DeleteDegreeDto
    {
        public int Id { get; set; }
    }
    
    [HttpPost]
    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Delete(DeleteDegreeDto data)
    {
        var degree = await _uow.Degree.Get(data.Id, false);
        if (degree == null)
        {
            return View("NotFound");
        }

        var cv = await _uow.Cv.GetCvWithDegree(degree.Id);
        if (cv != null)
        {
            SetFlash("Could not delete degree. It is used in a Cv.", "flash--warning");
            return RedirectToAction("Index");
        }

        try
        {
            _uow.Degree.Remove(degree);
            await _uow.Save();
        }
        catch (Exception ex)
        {
            SetFlash("Something went wrong.", "flash--error");
            return RedirectToAction("Index");
        }
        SetFlash("Degree deleted.", "flash");
        return RedirectToAction("Index");
    }

}