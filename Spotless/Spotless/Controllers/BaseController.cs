using Spotless.Models;
using Spotless.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Spotless.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        internal SectionRepository sectionRpstry = new SectionRepository();
        internal UserProfile loggedUser = null;
        internal string canRead = "can-read";
        internal string canCreate = "can-add";
        internal string canUpdate = "can-edit";
        internal string canDelete = "can-delete";
        internal string canPublish = "can-publish";
        internal string canSort = "can-sort";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dc = new DataClassesDataContext();
                loggedUser = dc.UserProfiles.FirstOrDefault(d => d.UserName.ToLower() == User.Identity.Name.ToLower());
                ViewBag.MenuSections = sectionRpstry.GetMenuSections(loggedUser.UserId);
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
