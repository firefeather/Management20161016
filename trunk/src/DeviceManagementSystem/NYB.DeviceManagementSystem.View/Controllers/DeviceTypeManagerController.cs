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
    public class DeviceTypeManagerController : Controller
    {
        public ActionResult Index(string searchInfo, int pageIndex = 1, int pageSize = 10, string orderBy = "", bool ascending = false)
        {
            try
            {
                var deviceTypeBLL = new DeviceTypeBLL();
                int totalCount = 0;
                var list = new List<WebDeviceType>();
                var cResult = deviceTypeBLL.GetDeviceTypeList(out totalCount, this.GetCurrentProjectID(), searchInfo, pageIndex, pageSize, orderBy, ascending);
                if (cResult.Code == 0)
                {
                    list = cResult.Data;
                }
                var pageList = new PagedList<WebDeviceType>(list, pageIndex, pageSize,totalCount);
                ViewBag.SearchInfo = searchInfo;
                ViewBag.PageSize = pageSize;
                return View(pageList);
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult Details(string deviceTypeID, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Action = "Details";
            ViewBag.IsView = "True";
            try
            {
                var result = new DeviceTypeBLL().GetDeviceTypeByID(deviceTypeID);
                if (result.Code == 0)
                {
                    int totalCount;
                    var items = new MaintainItemBLL().GetMaintainItemList(out totalCount, this.GetCurrentProjectID(), "", 1, -1, "Name", true);
                    ViewBag.AllMaintainItems = items.Data;

                    return View(result.Data);
                }
            }
            catch (Exception)
            {
            }
            return View();
        }

        public ActionResult Create(string returnUrl)
        {
            int totalCount;
            var result = new MaintainItemBLL().GetMaintainItemList(out totalCount, this.GetCurrentProjectID(), "", 1, -1, "Name", true);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Action = "Create";
            ViewBag.AllMaintainItems = result.Data;
            return View(new WebDeviceType());
        }

        [HttpPost]
        public ActionResult Create(WebDeviceType webDeviceType)
        {
            try
            {
                webDeviceType.ProjectID = this.GetCurrentProjectID();
                webDeviceType.CreateUserID = this.GetCurrentUserID();
                webDeviceType.ProjectID = this.GetCurrentProjectID();

                var result = new DeviceTypeBLL().InsertDeviceType(webDeviceType);
                return JsonContentHelper.GetJsonContent(result);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(string deviceTypeID, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            try
            {
                var result = new DeviceTypeBLL().GetDeviceTypeByID(deviceTypeID);
                if (result.Code == 0)
                {
                    int totalCount;
                    var items = new MaintainItemBLL().GetMaintainItemList(out totalCount, this.GetCurrentProjectID(), "", 1, -1, "Name", true);
                    ViewBag.AllMaintainItems = items.Data;

                    return View(result.Data);
                }
            }
            catch (Exception)
            {
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(WebDeviceType webDeviceType)
        {
            try
            {
                webDeviceType.ProjectID = this.GetCurrentProjectID();
                var result = new DeviceTypeBLL().UpdateDeviceType(webDeviceType);
                return JsonContentHelper.GetJsonContent(result);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                var result = new DeviceTypeBLL().DeleteDeviceType(id);
                return JsonContentHelper.GetJsonContent(result);
            }
            catch
            {
                return View();
            }
        }
    }
}
