﻿using NYB.DeviceManagementSystem.BLL;
using NYB.DeviceManagementSystem.Common;
using NYB.DeviceManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Webdiyer.WebControls.Mvc;

namespace NYB.DeviceManagementSystem.View.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult LogOn()
        {
            //new DatabaseInitHelper().InitDB();

            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string LoginName, string Pwd)
        {
            //new DatabaseInitHelper().InitDB();
            var errorInfo = "用戶名或密码错误";
            if (ModelState.IsValid)
            {
                var userBLL = new UserBLL();
                var result = userBLL.VerifyPassword(LoginName, Pwd);
                if (result.Code == 0)
                {
                    Response.Cookies.Add(new HttpCookie("CurrentProjectIDStr", result.Data.ProjectID));
                    Response.Cookies.Add(new HttpCookie("CurrentUserName", result.Data.UserName));
                    Response.Cookies.Add(new HttpCookie("CurrentUserID", result.Data.ID));
                    var roleInt = (int)(result.Data.Role);
                    Response.Cookies.Add(new HttpCookie("CurrentRole", roleInt.ToString()));
                    Response.Cookies.Add(new HttpCookie("OrderClientID", result.Data.OrderClientID));
                    //
                    if (result.Data.Role != 0)
                    {
                        FormsAuthentication.SetAuthCookie(LoginName, false);

                        errorInfo = result.Msg;

                        if (result.Data.Role == RoleType.超级管理员)
                        {
                            Response.Cookies.Add(new HttpCookie("LogoHomeUrl", Url.Action("Index", "OrderClientManager")));

                            Response.Cookies.Add(new HttpCookie("LogoFileUrl", ""));
                            Response.Cookies.Add(new HttpCookie("CompanyName", "运维管理系统"));

                            return RedirectToAction("Index", "OrderClientManager", new { _timepick = DateTime.Now.ToString("yyyyMMddhhmmssff") });
                        }
                        else
                        {
                            var companyInfo = OrderClientBLL.GetCompanyInfo(result.Data.OrderClientID);

                            if (companyInfo.Code == 0)
                            {
                                Response.Cookies.Add(new HttpCookie("LogoFileUrl", Url.Content(string.Format("~/{0}", companyInfo.Data.LogoFile))));
                                Response.Cookies.Add(new HttpCookie("CompanyName", companyInfo.Data.CompanyName));
                            }

                            if (result.Data.Role == RoleType.客户管理员)
                            {
                                Response.Cookies.Add(new HttpCookie("LogoHomeUrl", Url.Action("Index", "ProjectManager")));

                                return RedirectToAction("Index", "ProjectManager", new { _timepick = DateTime.Now.ToString("yyyyMMddhhmmssff") });
                            }
                            else
                            {
                                Response.Cookies.Add(new HttpCookie("LogoHomeUrl", Url.Action("Index", "Device")));

                                return RedirectToAction("Index", "Device", new { _timepick = DateTime.Now.ToString("yyyyMMddhhmmssff") });
                            }
                        }
                    }
                }
            }

            ModelState.AddModelError("", errorInfo);
            return View();
        }

        public ActionResult LogOff()
        {
            //避免缓存
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Expires = -1;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.CacheControl = "no-cache";

            FormsAuthentication.SignOut();

            Response.Cookies.Clear();

            // 清除session
            Session.RemoveAll();

            // 用同名cookie覆盖原cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // 用同名session cookie 覆盖原cookie
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);


            return RedirectToAction("LogOn");
        }
    }
}
