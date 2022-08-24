using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Web.Claim;

namespace TylerHendricks_Web.Areas.ViewComponents
{
    public class PhysicianRecordViewComponent:ViewComponent
    {
        private readonly IRepositoryCollection _repositoryCollection;
        private readonly IUserService _userService;
        private readonly IUsers _userManager;
        public PhysicianRecordViewComponent(IRepositoryCollection repositoryCollection, IUserService userService, IUsers userManager)
        {
            _repositoryCollection = repositoryCollection;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int RecordType, string RecordText, string Class)
        {
            ViewBag.RecordCount = 0;
            ViewBag.RecordType = RecordType;
            ViewBag.RecordHeading = RecordText;
            ViewBag.Class = Class;
            var StateId = await _userManager.GetStateId(_userService.GetUserId()).ConfigureAwait(true);
            var timeZoneConfig = new TimeZoneConfig() { OffSet = -330, IsDayLightSaving = false };
            if (RecordType != 8)
            {
                var records = await _repositoryCollection.Physician.GetInformationModels(RecordType, StateId, timeZoneConfig, "", "", "", 10, 0, 0).ConfigureAwait(true);
                if (records != null)
                {
                    if (records.Count > 0)
                    {
                        ViewBag.RecordCount = records[0].TotalRows;
                    }
                    else
                    {
                        ViewBag.RecordCount = records.Count;
                    }
                }
            }
            else
            {
                var records = await _repositoryCollection.Physician.GetNotify().ConfigureAwait(true);
                if (records != null)
                {
                    ViewBag.RecordCount = records.Count;
                }
            }
            return View();
        }

        public IViewComponentResult PatientChartTab()
        {
            return View();
        }
    }
}
