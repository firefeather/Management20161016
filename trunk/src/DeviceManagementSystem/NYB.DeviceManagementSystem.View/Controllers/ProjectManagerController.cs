﻿using NYB.DeviceManagementSystem.BLL;
using NYB.DeviceManagementSystem.Common;
using NYB.DeviceManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace NYB.DeviceManagementSystem.View.Controllers
{
    public class ProjectManagerController : Controller
    {
        //
        // GET: /ProjectManager/

        public ActionResult Index(string searchInfo, int pageIndex = 1, int pageSize = 10, string orderBy = "", bool ascending = false)
        {
            List<WebProject> userList = new List<WebProject>();
            int totalCount = 0;
            ProjectBLL projectBLL = new ProjectBLL();
            var cResult = projectBLL.GetProjectList(out totalCount, this.GetCurrentUserID(), searchInfo, pageIndex, pageSize, orderBy, ascending);
            if (cResult.Code == 0)
            {
                userList = cResult.Data;
            }
            var pageList = new PagedList<WebProject>(userList, pageIndex, pageSize,totalCount);
            ViewBag.SearchInfo = searchInfo;
            ViewBag.PageSize = pageSize;
            if (Request.Cookies.AllKeys.Contains("ManageProjectID"))
            {
                Request.Cookies.Remove("ManageProjectID");
                Response.Cookies["ManageProjectID"].Expires = DateTime.Now.AddDays(-10);
            }


            return View(pageList);
        }

        [HttpGet]
        public ActionResult AddProject(string returnUrl)
        {
            ViewBag.Action = "Add";
            ViewBag.ReturnUrl = returnUrl;
            WebProject webProject = new WebProject();
            return View(webProject);
        }

        [HttpPost]
        public ActionResult AddProject(WebProject webProject)
        {
            ProjectBLL projectBLL = new ProjectBLL();
            webProject.CreateUserID = this.GetCurrentUserID();
            webProject.WebUser.OrderClientID = this.GetCurrentOrderClientID();
            CResult<bool> cResult = projectBLL.InsertProject(webProject);
            return JsonContentHelper.GetJsonContent(cResult);
        }

        [HttpGet]
        public ActionResult UpdateProject(string projectID, string returnUrl)
        {
            ProjectBLL projectBLL = new ProjectBLL();
            var result = projectBLL.GetProjectInfoByID(projectID);
            WebProject webProject = null;
            if (result.Code == 0)
            {
                webProject = result.Data;
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(webProject);
        }

        [HttpPost]
        public ActionResult UpdateProject(WebProject webProject, string returnUrl)
        {
            ProjectBLL projectBLL = new ProjectBLL();

            CResult<bool> cResult = projectBLL.UpdateProjectInfo(webProject);

            return JsonContentHelper.GetJsonContent(cResult);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                var result = new ProjectBLL().DeleteProject(id);
                return JsonContentHelper.GetJsonContent(result);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string userID, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var webUser = new WebUser();
            webUser.ID = userID;
            return View(webUser);
        }

        [HttpPost]
        public ActionResult ResetPasswordPost(string userID, string newPassword)
        {
            var result = new UserBLL().ResetPassword(newPassword, userID, this.GetCurrentUserID());

            return JsonContentHelper.GetJsonContent(result);
        }

        [HttpGet]
        public ActionResult ManageProject(string projectID)
        {
            Response.Cookies.Add(new HttpCookie("ManageProjectID", projectID));
            if (this.GetCurrentRole() != RoleType.客户管理员)
            {
                throw new Exception("权限不足");
            }

            return RedirectToAction("Index", "Device", new { _timepick = DateTime.Now.ToString("yyyyMMddhhmmssff") });
        }
    }
}
