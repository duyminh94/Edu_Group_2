using BlogPlatform.Data;
using BlogPlatform.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Areas.Admin.Controllers
{
    // UC26 — quản lý chuyên mục (Category) và thẻ (Tag)
    [Area("Admin")]
    [SessionAuthorize(Roles = "Admin")]
    public class TaxonomyController : Controller
    {
    }
}
