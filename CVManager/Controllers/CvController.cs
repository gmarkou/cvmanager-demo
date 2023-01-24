using System.Net.Mail;
using System.Text.RegularExpressions;
using AutoMapper;
using CVManager.DAL;
using CVManager.DAL.Models;
using CVManager.DTO;
using Microsoft.AspNetCore.Mvc;
using CVManager.Models;
using CVManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CVManager.Controllers;

[Authorize]
public class CvController : Controller
{
    private readonly ILogger<CvController> _logger;
    private readonly IUoW _uow;
    private readonly IMapper _mapper;
    private readonly IFileManager _fileManager;

    public CvController(ILogger<CvController> logger, IUoW uow, IMapper mapper, IFileManager fileManager)
    {
        _logger = logger;
        _uow = uow;
        _mapper = mapper;
        _fileManager = fileManager;
    }

    public async Task<IActionResult> Index(int id = 0)
    {
        int page = id;
        if (page < 0)
        {
            page = 0;
        }
        const int pageSize = 4;
        var cvCount = await _uow.Cv.Count();
        int maxPage = ((cvCount-1) / pageSize);
        if (maxPage < 0) maxPage = 0;
        if (page > maxPage)
        {
            page = maxPage;
        }
        var cvs = await _uow.Cv.GetPageWithDegrees(page, pageSize);
        var dto = _mapper.Map<IEnumerable<CvDetailDto>>(cvs);

        ViewBag.TotalPages = maxPage;
        ViewBag.CurrentPage = page;
        ViewBag.CvPerPage = pageSize;
        
        return View(dto);
    }

    [Authorize(Policy= "Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
        var degrees = await _uow.Degree.All();
        CvDetailWithDegreesDto dto = new CvDetailWithDegreesDto
        {
            Degrees = _mapper.Map<IEnumerable<DegreeDto>>(degrees) 
        };
        return View(dto);
    }

    private void ValidateCv(Cv cv, ModelStateDictionary modelState)
    {
        if (ModelState.GetValidationState(nameof(Cv.FirstName)) == ModelValidationState.Valid)
        {
            if (cv.FirstName.Trim() == "")
            {
                ModelState.AddModelError(nameof(Cv.FirstName), "First name could not be all spaces.");
            }            
        }
        if (ModelState.GetValidationState(nameof(Cv.LastName)) == ModelValidationState.Valid)
        {
            if (cv.LastName.Trim() == "")
            {
                ModelState.AddModelError(nameof(Cv.LastName), "Last name could not be all spaces.");
            }            
        }
        if (ModelState.GetValidationState(nameof(Cv.Email)) == ModelValidationState.Valid)
        {
            cv.Email = cv.Email.Trim();
            try
            {
                MailAddress m = new MailAddress(cv.Email);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(nameof(Cv.Email), "Email is not valid.");
            }
        }
        if (ModelState.GetValidationState(nameof(Cv.Mobile)) == ModelValidationState.Valid)
        {
            if (cv.Mobile != null)
            {
                Regex regex = new Regex(@"^[0123456789]{10}$");
                Match match = regex.Match(cv.Mobile);
                if (!match.Success)
                {
                    ModelState.AddModelError(nameof(Cv.Mobile), "Mobile is not valid (must be 10 digits)");
                }
            }            
        }
        
    }
    
    [HttpPost]
    [Authorize(Policy= "Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CvCreateUpdateDto dto)
    {
        var cv = _mapper.Map<Cv>(dto);
        
        ModelState.ClearValidationState(nameof(Cv));
        if (TryValidateModel(cv, nameof(Cv)))
        {
            ValidateCv(cv, ModelState);
        }
        if (!ModelState.IsValid)
        {
            var newDto = _mapper.Map<CvDetailWithDegreesDto>(cv);
            newDto.Degrees = _mapper.Map<IEnumerable<DegreeDto>>(await _uow.Degree.All());
            return View(newDto);
        }

        IFileManager.UploadedFile? uploadedFile = null;
        if (dto.File is { Length: > 0 })
        {
            uploadedFile = await _fileManager.UploadFile(dto.File);
        }

        cv = _uow.Cv.Create(cv);
        await _uow.Save();

        if (uploadedFile == null)
        {
            return RedirectToAction("Index");
        }
        var file = _fileManager.MoveFile(cv.Id, uploadedFile);

        cv.FileExtension = file.Extension;
        cv.FileSize = file.FileSize;

        _uow.Cv.Update(cv);
        await _uow.Save();
        return RedirectToAction("Index");
        
    }

    public async Task<IActionResult> Detail(int id)
    {
        var cv = await _uow.Cv.Get(id, false);
        if (cv == null)
        {
            return View("NotFound");
        }

        return View(_mapper.Map<CvDetailDto>(cv));
    }
    
    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Edit(int id)
    {
        var cv = await _uow.Cv.Get(id, false);
        if (cv == null)
        {
            return View("NotFound");
        }

        var dto = _mapper.Map<CvDetailWithDegreesDto>(cv);
        dto.Degrees = _mapper.Map<IEnumerable<DegreeDto>>(await _uow.Degree.All());
        return View(dto);
    }

    [HttpPost]
    [Authorize(Policy= "Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CvCreateUpdateDto dto)
    {
        var oldCv = await _uow.Cv.Get(id, false);
        if (oldCv == null)
        {
            return View("NotFound");
        }
        
        var cv = _mapper.Map<Cv>(dto);
        
        // ModelState.ClearValidationState(nameof(Cv));
        // if (TryValidateModel(cv, nameof(Cv)))
        // {
            ValidateCv(cv, ModelState);
        //}
        
        if (!ModelState.IsValid)
        {
            var newDto = _mapper.Map<CvDetailWithDegreesDto>(cv);
            newDto.Degrees = _mapper.Map<IEnumerable<DegreeDto>>(await _uow.Degree.All());
            return View(newDto);
        }

        // Κρατάμε τα δεδομένα του παλιού αρχείου
        cv.FileExtension = oldCv.FileExtension;
        cv.FileSize = oldCv.FileSize;

        if (dto.DeleteFile == "on")
        {
            //θα πρεπει να διαγράψουμε το παλιό αρχείο.
            if (oldCv.FileExtension != null)
            {
                _fileManager.RemoveFile(oldCv.Id, oldCv.FileExtension);
            }
            cv.FileExtension = null;
            cv.FileSize = null;
        }
        // Έχουμε νέο αρχείο, θα πρέπει να αντικαταστήσουμε το παλιό
        if (dto.File is { Length: > 0 })
        {
            if (oldCv.FileExtension != null)
            {
                _fileManager.RemoveFile(oldCv.Id, oldCv.FileExtension);
            }

            IFileManager.UploadedFile uploadedFile = await _fileManager.UploadFile(dto.File);
            var file = _fileManager.MoveFile(cv.Id, uploadedFile);

            cv.FileExtension = file.Extension;
            cv.FileSize = file.FileSize;
        }
        
        _uow.Cv.Update(cv);
        await _uow.Save();
        return RedirectToAction("Index");
    }

    [Authorize(Policy= "Editor")]
    public async Task<IActionResult> Delete(int id)
    {
        var cv = await _uow.Cv.Get(id, false);
        if (cv == null)
        {
            return View("NotFound");
        }
        return View(_mapper.Map<CvDetailDto>(cv));
    }
    
    public class DeleteCvDto
    {
        public int Id { get; set; }
    }

    [HttpPost]
    [Authorize(Policy= "Editor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(DeleteCvDto dto)
    {
        var cv = await _uow.Cv.Get(dto.Id);
        if (cv == null)
        {
            return View("NotFound");
        }

        if (cv.FileExtension != null)
        {
            _fileManager.RemoveFile(cv.Id, cv.FileExtension);
        }
        cv = _uow.Cv.Remove(cv);
        await _uow.Save();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Download(int id)
    {
        var cv = await _uow.Cv.Get(id);
        if ((cv == null) || (cv.FileExtension == null))
        {
            return View("NotFound");
        }

        var filename = _fileManager.GetFileName(id, cv.FileExtension);
        if (filename == null)
        {
            return View("NotFound");
        }

        try
        {
            var file = new System.IO.FileInfo(filename);
            var content = await System.IO.File.ReadAllBytesAsync(filename);
            new FileExtensionContentTypeProvider()
                .TryGetContentType(filename, out string? contentType);
            contentType ??= "application/octet-stream";

            Response.Clear();
            Response.Headers.Add("Content-Disposition", "attachment; filename=Document" + cv.FileExtension);
            Response.Headers.Add("Content-Length", file.Length.ToString());
            Response.ContentType = "application/octet-stream";
            return File(content, contentType, "Document"+cv.FileExtension);
        }
        catch (Exception e)
        {
            return View("NotFound");
        }
    }

}