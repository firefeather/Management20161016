﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NYB.DeviceManagementSystem.Common.Helper;
using NYB.DeviceManagementSystem.Common.Logger;
using NYB.DeviceManagementSystem.Common;
using NYB.DeviceManagementSystem.ViewModel;
using NYB.DeviceManagementSystem.DAL;
using System.Linq.Expressions;
using System.Data;
using System.Web.Security;
using System.Reflection;

namespace NYB.DeviceManagementSystem.BLL
{
    public class ProjectBLL
    {
        public CResult<List<WebProject>> GetProjectList(out int totalCount, string userID, string searchInfo, int pageIndex = 1, int pageSize = 10, string orderby = null, bool ascending = false)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().ToString());

            using (DeviceMgmtEntities context = new DeviceMgmtEntities())
            {
                Expression<Func<Project, bool>> filter = t => t.IsValid && t.CreateUserID == userID;

                if (string.IsNullOrWhiteSpace(searchInfo) == false)
                {
                    searchInfo = searchInfo.Trim().ToUpper();
                    filter = filter.And(t => t.Name.ToUpper().Contains(searchInfo));
                }

                if (string.IsNullOrEmpty(orderby))
                {
                    orderby = "CreateDate";
                    ascending = false;
                }

                var temp = context.Project.Where(filter).Page(out totalCount, pageIndex, pageSize, orderby, ascending, true);

                var result = temp.Select(t => new WebProject()
                {
                    CreateDate = t.CreateDate,
                    ID = t.ID,
                    Note = t.Note,
                    Name = t.Name,
                }).ToList();

                var projectIDs = result.Select(t => t.ID).ToList();
                var users = context.User.Where(t => t.Role == (int)RoleType.项目管理员 && projectIDs.Contains(t.ProjectID)).Select(user => new WebUser
                {
                    ID = user.UserID,
                    Address = user.Address,
                    Email = user.Email,
                    LoginName = user.LoginName,
                    TelPhone = user.Telephone,
                    Moblie = user.Moblie,
                    UserName = user.Name,
                    ProjectID = user.ProjectID
                }).ToList();

                foreach (var user in users)
                {
                    var project = result.FirstOrDefault(t => t.ID == user.ProjectID);
                    project.WebUser = user;
                }

                LogHelper.Info("result", result);

                return new CResult<List<WebProject>>(result);
            }
        }

        public CResult<bool> InsertProject(WebProject webProject)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().ToString());
            LogHelper.Info("webProject", webProject);

            using (var context = new DeviceMgmtEntities())
            {
                if (context.Project.Any(t => t.CreateUserID == webProject.CreateUserID && t.Name.ToUpper() == webProject.Name.ToUpper() && t.IsValid))
                {
                    return new CResult<bool>(false, ErrorCode.ProjectNameIsExist);
                }

                var project = new Project();
                project.CreateDate = DateTime.Now;
                project.CreateUserID = webProject.CreateUserID;
                project.ID = Guid.NewGuid().ToString();
                project.Name = webProject.Name;
                project.Note = webProject.Note;
                project.IsValid = true;

                var webUser = webProject.WebUser;
                webUser.CreateUserID = webProject.CreateUserID;

                var userLoginName = webUser.LoginName.ToUpper();
                if (context.User.Any(t => t.LoginName.ToUpper() == userLoginName))
                {
                    return new CResult<bool>(false, ErrorCode.LoginNameIsExist);
                }

                var entity = new User()
                {
                    UserID = Guid.NewGuid().ToString(),
                    Password = webUser.Pwd,
                    LoginName = webUser.LoginName,
                    Name = webUser.UserName,
                    ProjectID = project.ID,
                    Address = webUser.Address,
                    Telephone = webUser.TelPhone,
                    CreateDate = DateTime.Now,
                    CreateUserID = webUser.CreateUserID,
                    Email = webUser.Email,
                    IsValid = true,
                    Role = (int)RoleType.项目管理员,
                    OrderClientID = webUser.OrderClientID
                };
                context.Project.Add(project);
                context.User.Add(entity);

                return context.Save();
            }
        }

        public CResult<bool> UpdateProjectInfo(WebProject webProject)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().ToString());
            LogHelper.Info("webProject", webProject);

            using (var context = new DeviceMgmtEntities())
            {
                var project = context.Project.FirstOrDefault(t => t.ID == webProject.ID);
                if (project == null)
                {
                    return new CResult<bool>(false, ErrorCode.DataNoExist);
                }

                if (context.Project.Any(t => t.CreateUserID == project.CreateUserID && t.Name.ToUpper() == webProject.Name.ToUpper() && t.IsValid && t.ID != webProject.ID))
                {
                    return new CResult<bool>(false, ErrorCode.ProjectNameIsExist);
                }

                project.Name = webProject.Name;
                project.Note = webProject.Note;

                var user = context.User.FirstOrDefault(t => t.Role == (int)RoleType.项目管理员 && t.ProjectID == webProject.ID);
                if (user == null)
                {
                    return new CResult<bool>(false, ErrorCode.DataNoExist);
                }

                var webUser = webProject.WebUser;
                user.Address = webUser.Address;
                user.Email = webUser.Email;
                user.Name = webUser.UserName;
                user.Telephone = webUser.TelPhone;
                user.Moblie = webUser.Moblie;

                context.Entry(project).State = EntityState.Modified;
                return context.Save();
            }
        }

        public CResult<WebProject> GetProjectInfoByID(string projectID)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().ToString());
            LogHelper.Info("projectID", projectID);

            using (var context = new DeviceMgmtEntities())
            {
                var project = context.Project.FirstOrDefault(t => t.ID == projectID);
                if (project == null)
                {
                    return new CResult<WebProject>(null, ErrorCode.DataNoExist);
                }

                var webProject = new WebProject()
                {
                    CreateDate = project.CreateDate,
                    ID = project.ID,
                    Note = project.Note,
                    Name = project.Name,
                };

                var user = context.User.FirstOrDefault(t => t.Role == (int)RoleType.项目管理员 && t.ProjectID == webProject.ID);
                var webUser = new WebUser
                {
                    ID = user.UserID,
                    Address = user.Address,
                    Email = user.Email,
                    LoginName = user.LoginName,
                    TelPhone = user.Telephone,
                    Moblie = user.Moblie,
                    UserName = user.Name,
                    OrderClientID = user.OrderClientID
                };

                webProject.WebUser = webUser;

                LogHelper.Info("result", webProject);

                return new CResult<WebProject>(webProject);
            }
        }

        public CResult<bool> DeleteProject(string projectID)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().ToString());
            LogHelper.Info("projectID", projectID);

            using (var context = new DeviceMgmtEntities())
            {
                var project = context.Project.FirstOrDefault(t => t.ID == projectID);
                if (project == null)
                {
                    return new CResult<bool>(false, ErrorCode.DataNoExist);
                }

                project.IsValid = false;

                var users = context.User.Where(t => t.IsValid && t.ProjectID == projectID).ToList();
                foreach (var user in users)
                {
                    user.IsValid = false;
                }

                return context.Save();
            }
        }
    }
}
