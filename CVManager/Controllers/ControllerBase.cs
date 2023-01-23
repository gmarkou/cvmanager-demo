using Microsoft.AspNetCore.Mvc;

namespace CVManager.Controllers;


public class ControllerBase: Controller
{
    public void SetFlash(string text, string cssClass)
    {
        TempData["FlashMessage.CssClass"] = cssClass;
        TempData["FlashMessage.Text"] = text;
    }
}