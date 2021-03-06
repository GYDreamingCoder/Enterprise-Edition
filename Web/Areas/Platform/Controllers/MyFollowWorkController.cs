﻿using System;
using System.Linq;
using System.Web.Mvc;
using Common;
using IServices.Infrastructure;
using IServices.ISysServices;
using IServices.IUserServices;
using Models.UserModels;

namespace Web.Areas.Platform.Controllers
{
    public class MyFollowWorkController : Controller
    {
        // 我关注的项目
        // GET: /Platform/MyFollowWork/

        private readonly IProjectInfoService _iProjectInfoService;
        private readonly IProjectUserService _iProjectUserService;
        private readonly IUserInfo _iUserInfo;
        private readonly IUnitOfWork _unitOfWork;

        public MyFollowWorkController(IProjectInfoService iProjectInfoService, IUnitOfWork unitOfWork,
            IUserInfo iUserInfo, IProjectUserService iProjectUserService)
        {
            _iProjectInfoService = iProjectInfoService;
            _unitOfWork = unitOfWork;
            _iUserInfo = iUserInfo;
            _iProjectUserService = iProjectUserService;
        }

        // 公司其他公开项目
        // GET: /Platform/AllWork/

        public ActionResult Index(string keyword, bool? finish, int pageIndex = 1)
        {
            ViewBag.UserId = _iUserInfo.UserId;
            IQueryable<ProjectInfo> model =
                _iProjectInfoService.GetAll(
                    a => a.Public && a.ProjectUsers.Any(b => b.SysUserId == _iUserInfo.UserId && b.Follow));

            //为了显示子项目准备
            ViewBag.ProjectInfo = model;

            if (!string.IsNullOrEmpty(keyword))
            {
                model =
                    model.Where(
                        a =>
                            a.ProjectName.Contains(keyword) || a.ProjectObjective.Contains(keyword) ||
                            a.Tag.Contains(keyword) || a.ProjectUsers.Any(b => b.SysUser.UserName == keyword));
            }
            else
            {
                //筛选主项目
                model = model.Where(a => a.LastProjectInfoId == null);
            }

            ViewBag.CountAll = model.Count();
            ViewBag.unfinish = model.Count(a => !a.Finish);
            ViewBag.finish = model.Count(a => a.Finish);

            if (finish.HasValue)
            {
                model = model.Where(a => a.Finish == finish);
            }

            return View(model.ToPagedList(pageIndex));
        }

        //修改关注
        public ActionResult Edit(Guid id)
        {
            _iProjectUserService.Follow(id, _iUserInfo.UserId);
            _unitOfWork.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            ViewBag.UserId = _iUserInfo.UserId;
            ProjectInfo item = _iProjectInfoService.GetById(id);
            return View(item);
        }
    }
}