using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TylerHendricks_Repo.Services;
using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Web.Filters
{
    public class CustomLoginedUserAuthentication : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string currentPagePath = context.HttpContext.Request.Path.ToString().Trim().ToLower();
            if (!string.IsNullOrEmpty(context.HttpContext.User.Identity.Name))
            {
                var userIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claims = userIdentity.Claims;
                var roleClaimType = userIdentity.RoleClaimType;
                var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList().Select(x => x.Value).ToList();
                if (roles.Contains(RoleType.Admin.ToString()))
                {
                    switch (currentPagePath)
                    {
                        case "/": 
                            context.Result = new RedirectToActionResult("PatientInfo", "Home", new { area = PortalType.AdminPortal.ToString() });
                            break;
                        case "/provider-portal":
                            context.Result = new RedirectToActionResult("PatientInfo", "Home", new { area = PortalType.AdminPortal.ToString() });
                            break;
                        case "/patient-portal":
                            context.Result = new RedirectToActionResult("PatientInfo", "Home", new { area = PortalType.AdminPortal.ToString() });
                            break;
                    }
                }
                else if (roles.Contains(RoleType.Physician.ToString()))
                {
                    switch (currentPagePath)
                    {
                        case "/":
                            context.Result = new RedirectToActionResult("PhysicianDashboard", "Home", new { area = PortalType.PhysicianPortal.ToString() });
                            break;
                        case "/patient-portal":
                            context.Result = new RedirectToActionResult("PhysicianDashboard", "Home", new { area = PortalType.PhysicianPortal.ToString() });
                            break;
                        case "/admin-portal":
                            context.Result = new RedirectToActionResult("PhysicianDashboard", "Home", new { area = PortalType.PhysicianPortal.ToString() });
                            break;
                    }
                }
                else if (roles.Contains(RoleType.Patient.ToString()))
                {
                    switch (currentPagePath)
                    {
                        case "/provider-portal":
                            context.Result = new RedirectToActionResult("Index", "Home", new { area = PortalType.PatientPortal.ToString() });
                            break;
                        case "/admin-portal":
                            context.Result = new RedirectToActionResult("Index", "Home", new { area = PortalType.PatientPortal.ToString() });
                            break;
                    }
                }
            }
        }
    }
}
