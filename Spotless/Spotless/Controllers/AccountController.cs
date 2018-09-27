using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Spotless.Filters;
using Spotless.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Configuration;

namespace Spotless.Controllers
{
    //[Authorize]
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        private RoleRepository roleRpstry = new RoleRepository();
        private CustomerRepository cutomerRepo = new CustomerRepository();
        private DataClassesDataContext dc = new DataClassesDataContext();
        private string sectionName = "account";

        #region Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            string req = Request.ToString();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToLocal(returnUrl);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(Url.Content("~/"));
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }
        #endregion

        #region Logoff
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return RedirectToAction("login", "account");
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanCreate)
            {
                ViewBag.Permissions = permissions;
                ViewBag.Roles = new MultiSelectList(roleRpstry.GetAll(), "RoleName", "RoleName");
                ViewBag.Customers = new SelectList(cutomerRepo.GetAll(), "id", "fullName");
                return View();
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterModel model)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canCreate);
            if (hasPermissions)
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    try
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        var userEntry = dc.UserProfiles.FirstOrDefault(d => d.UserName == model.UserName);
                        var customerentity = cutomerRepo.GetBiId(Convert.ToInt32(model.customerId));
                        userEntry.customerId = customerentity.id;
                        userEntry.customername = customerentity.fullName;                        
                        dc.SubmitChanges();
                        if (model.Roles != null && model.Roles.Any())
                        Roles.AddUserToRoles(model.UserName, model.Roles);

                        return RedirectToAction("Index", new { thisid = WebSecurity.GetUserId(model.UserName) });
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ViewBag.Roles = new MultiSelectList(roleRpstry.GetAll(), "RoleName", "RoleName");
                        ViewBag.Customers = new SelectList(cutomerRepo.GetAll(), "id", "fullName");
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                        return View(model);

                    }
                }
                // If we got this far, something failed, redisplay form
                ViewBag.Roles = new MultiSelectList(roleRpstry.GetAll(), "RoleName", "RoleName");
                return View(model);
            }
            else
            {
                ViewBag.Roles = new MultiSelectList(roleRpstry.GetAll(), "RoleName", "RoleName");
                return View("Error", "You do not have permissions to access this section.");
            }

        }
        #endregion

        #region Read
        public ActionResult Index()
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanRead)
            {
                ViewBag.Permissions = permissions;
                return View();
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }
        }

        public ActionResult ReadData([DataSourceRequest] DataSourceRequest request)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canRead);
            var db = new DataClassesDataContext();
            var resultsToSearchFrom = db.UserProfiles.AsEnumerable();
            var currentRoles = Roles.GetRolesForUser(loggedUser.UserName);
            if (!currentRoles.Contains(ConfigurationManager.AppSettings["SuperAdminRoleName"]) && !currentRoles.Contains(ConfigurationManager.AppSettings["AdminRoleName"]))
            {
                resultsToSearchFrom = resultsToSearchFrom.Where(d => d.UserId == loggedUser.UserId).AsEnumerable();
            }
            if (hasPermissions)
            {
                return Json(resultsToSearchFrom.Select(x => new
                            {
                                UserId = x.UserId,
                                UserName = x.UserName,
                                customername = x.customername
                            }).ToDataSourceResult(request));
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }

        #endregion

        #region Update
        public ActionResult Update(int? id)
        {
            EPermission permissions = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
            if (permissions.CanUpdate)
            {
                ViewBag.Permissions = permissions;
                id = (id != null) ? id : loggedUser.UserId;
                var userEntry = dc.UserProfiles.FirstOrDefault(d => d.UserId == id);
                UpdateModel model = new UpdateModel();
                ViewBag.Customers = new SelectList(cutomerRepo.GetAll(), "id", "fullName",userEntry != null ? userEntry.customerId : 0);
                if (userEntry != null)
                {
                    model.Roles = Roles.GetRolesForUser(userEntry.UserName);
                }
                else
                {
                    model.Roles = new string[0];
                }
                model.UserName = userEntry.UserName;
                ViewBag.Role = Roles.GetRolesForUser(loggedUser.UserName).First();
                ViewBag.Roles = roleRpstry.GetAll();
                ViewBag.UserId = id.ToString();
                return View(model);
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int? id, UpdateModel model)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canUpdate);
            if (hasPermissions)
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    try
                    {
                        id = (id != null) ? id : loggedUser.UserId;
                        var userEntry = dc.UserProfiles.FirstOrDefault(d => d.UserId == id);
                        if (userEntry != null)
                        {
                            var customerentity = cutomerRepo.GetBiId(Convert.ToInt32(model.customerId));
                            userEntry.customerId = customerentity.id;
                            userEntry.customername = customerentity.fullName;
                            userEntry.UserName = model.UserName;
                            dc.SubmitChanges();
                        }
                        if (userEntry != null)
                        {
                            if (Roles.GetRolesForUser(userEntry.UserName).Any())
                            {
                                Roles.RemoveUserFromRoles(userEntry.UserName, Roles.GetRolesForUser(userEntry.UserName));
                            }
                            if (model.Roles != null && model.Roles.Any())
                            {
                                Roles.AddUserToRoles(userEntry.UserName, model.Roles);
                            }

                            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
                            {
                                if (WebSecurity.ChangePassword(userEntry.UserName, model.OldPassword, model.NewPassword)) { }
                                else
                                {
                                    ModelState.AddModelError("", "Old password is incorrect, password does not changed");
                                    var userEntry2 = dc.UserProfiles.FirstOrDefault(d => d.UserId == id);
                                    ViewBag.Customers = new SelectList(cutomerRepo.GetAll(), "id", "fullName", userEntry2 != null ? userEntry2.customerId : 0);
                                    if (userEntry2 != null)
                                    {
                                        model.Roles = Roles.GetRolesForUser(userEntry2.UserName);
                                    }
                                    else
                                    {
                                        model.Roles = new string[0];
                                    }
                                    EPermission permissions2 = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
                                    ViewBag.Permissions = permissions2;
                                    ViewBag.Roles = roleRpstry.GetAll();
                                    return View(model);
                                }
                            }

                        }
                        return RedirectToAction("Index", new { thisid = id });
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                        return View(model);

                    }
                }
                ModelState.AddModelError("", "Make sure you assigned a role");
                // If we got this far, something failed, redisplay form
                var userEntry1 = dc.UserProfiles.FirstOrDefault(d => d.UserId == id);
                ViewBag.Customers = new SelectList(cutomerRepo.GetAll(), "id", "fullName", userEntry1 != null ? userEntry1.customerId : 0);
                if (userEntry1 != null)
                {
                    model.Roles = Roles.GetRolesForUser(userEntry1.UserName);
                }
                else
                {
                    model.Roles = new string[0];
                }
                EPermission permissions1 = sectionRpstry.GetPermissions(sectionName, loggedUser.UserId);
                ViewBag.Permissions = permissions1;
                ViewBag.Roles = roleRpstry.GetAll();
                return View(model);
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }


        }
        #endregion

        #region Delete

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, UserProfile entry)
        {
            bool hasPermissions = sectionRpstry.GetPermission(sectionName, loggedUser.UserId, canDelete);
            var db = new DataClassesDataContext();
            if (hasPermissions)
            {
                try
                {
                    if (entry.UserName != ConfigurationManager.AppSettings["SuperAdminUserName"])
                    {
                        var roles = db.webpages_UsersInRoles.Where(d => d.UserId == entry.UserId).Select(d => d.webpages_Role.RoleName.ToString()).ToArray();
                        if (roles.Length != 0 )
                        Roles.RemoveUserFromRoles(entry.UserName, roles);
                        Membership.DeleteUser(entry.UserName, true);
                    }
                    return Json(new[] { entry }.ToDataSourceResult(request, ModelState));
                }
                catch (Exception e)
                {
                    return Json(new[] { entry }.ToDataSourceResult(request, ModelState));
                }
            }
            else
            {
                return View("Error", "You do not have permissions to access this section.");
            }

        }
        #endregion

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "account");
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {

            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
