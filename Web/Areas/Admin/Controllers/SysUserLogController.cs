﻿using System.Linq;
using System.Web.Mvc;
using Common;
using DoddleReport;
using DoddleReport.Web;
using IServices.ISysServices;
using IServices.Infrastructure;
using Web.Helper;

namespace Web.Areas.Admin.Controllers
{
    public class SysUserLogController : Controller
    {
        private readonly ISysUserLogService _sysUserLogService;

        public SysUserLogController(ISysUserLogService sysUserLogService)
        {
            _sysUserLogService = sysUserLogService;
        }

        //
        // GET: /Platform/SysUserLog/

        public ActionResult Index(string keyword, string ordering, int pageIndex = 1)
        {
            var model =
                _sysUserLogService.GetAllEnt()
                                  .Select(
                                      a =>
                                      new
                                          {
                                              a.SysUser.SysEnterprise.EnterpriseName,
                                              a.SysUser.UserName,
                                              a.SysUser.DisplayName,
                                              a.SysControllerSysAction.SysController.SysArea.AreaDisplayName,
                                              a.SysControllerSysAction.SysController.ControllerDisplayName,
                                              a.SysControllerSysAction.SysAction.ActionDisplayName,
                                              a.RecordId,
                                              a.CreatedDate
                                          }).Search(keyword);

            if (!string.IsNullOrEmpty(ordering))
            {
                model = model.OrderBy(ordering, null);
            }

            if (!string.IsNullOrEmpty(Request["report"]))
            {
                //导出
                var reportModel = new Report(model.ToReportSource());
                return new ReportResult(reportModel);
            }

            return View(model.ToPagedList(pageIndex));
        }
    }
}