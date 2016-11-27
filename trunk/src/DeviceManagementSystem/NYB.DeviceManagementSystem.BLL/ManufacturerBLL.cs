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

namespace NYB.DeviceManagementSystem.BLL
{
    public class ManufacturerBLL
    {
        public CResult<List<WebManufacturer>> GetManufacturerList(out int totalCount, string projectID, string searchInfo, int pageIndex = 1, int pageSize = 10, string orderby = null, bool ascending = false)
        {
            using (DeviceMgmtEntities context = new DeviceMgmtEntities())
            {
                Expression<Func<Manufacturer, bool>> filter = t => t.ProjectID == projectID && t.IsValid == true;

                if (string.IsNullOrWhiteSpace(searchInfo) == false)
                {
                    searchInfo = searchInfo.Trim().ToUpper();
                    filter = filter.And(t => t.Name.ToUpper().Contains(searchInfo));
                }

                var temp = context.Manufacturer.Where(filter).Page(out totalCount, pageIndex, pageSize, orderby, ascending, true);

                var result = temp.Select(t => new WebManufacturer()
                {
                    ID = t.ID,
                    Name = t.Name,
                    Address = t.Address,
                    Contact = t.Contact,
                    Mobile = t.Mobile,
                    Phone = t.Phone,
                    Note = t.Note,
                    CreateDate = t.CreateDate,
                    CreateUserID = t.CreateUserID,
                    CreateUserName = t.User.Name,
                    ProjectID = t.ProjectID
                }).ToList();

                return new CResult<List<WebManufacturer>>(result);
            }
        }

        public CResult<Dictionary<string, string>> GetManufacturerDir(string projectID)
        {
            using (DeviceMgmtEntities context = new DeviceMgmtEntities())
            {
                Expression<Func<Manufacturer, bool>> filter = t => t.ProjectID == projectID && t.IsValid == true;

                var temp = context.Manufacturer.Where(filter);

                var result = temp.Select(t => new
                {
                    ID = t.ID,
                    Name = t.Name,
                }).ToList().OrderBy(t => t.Name).ToDictionary(t => t.ID, r => r.Name);

                return new CResult<Dictionary<string, string>>(result);
            }
        }

        public CResult<bool> InsertManufacturer(WebManufacturer model)
        {
            if (string.IsNullOrEmpty(model.ProjectID))
            {
                return new CResult<bool>(false, ErrorCode.ParameterError);
            }

            using (var context = new DeviceMgmtEntities())
            {
                if (context.Project.Any(t => t.IsValid && t.ID == model.ProjectID) == false)
                {
                    return new CResult<bool>(false, ErrorCode.ProjectNotExist);
                }

                var entity = new Manufacturer();
                entity.CreateDate = DateTime.Now;
                entity.CreateUserID = model.CreateUserID;
                entity.ID = Guid.NewGuid().ToString();
                entity.Name = model.Name;
                entity.IsValid = true;
                entity.Note = model.Note;
                entity.ProjectID = model.ProjectID;
                entity.Address = model.Address;
                entity.Contact = model.Contact;
                entity.Mobile = model.Mobile;
                entity.Phone = model.Phone;

                context.Manufacturer.Add(entity);

                return context.Save();
            }
        }

        public CResult<bool> UpdateManufacturer(WebManufacturer model)
        {
            if (string.IsNullOrEmpty(model.ID))
            {
                return new CResult<bool>(false, ErrorCode.ParameterError);
            }

            using (var context = new DeviceMgmtEntities())
            {
                var entity = context.Manufacturer.FirstOrDefault(t => t.ID == model.ID && t.IsValid);
                if (entity == null)
                {
                    return new CResult<bool>(false, ErrorCode.DataNoExist);
                }

                entity.Name = model.Name;
                entity.Note = model.Note;
                entity.Address = model.Address;
                entity.Contact = model.Contact;
                entity.Mobile = model.Mobile;
                entity.Phone = model.Phone;

                context.Entry(entity).State = EntityState.Modified;
                return context.Save();
            }
        }

        public CResult<WebManufacturer> GetManufacturerByID(string manufacturerID)
        {
            if (string.IsNullOrEmpty(manufacturerID))
            {
                return new CResult<WebManufacturer>(null, ErrorCode.ParameterError);
            }

            using (var context = new DeviceMgmtEntities())
            {
                var entity = context.Manufacturer.FirstOrDefault(t => t.ID == manufacturerID && t.IsValid);
                if (entity == null)
                {
                    return new CResult<WebManufacturer>(null, ErrorCode.DataNoExist);
                }

                var model = new WebManufacturer()
                {
                    ID = entity.ID,
                    Name = entity.Name,
                    Address = entity.Address,
                    Contact = entity.Contact,
                    Mobile = entity.Mobile,
                    Phone = entity.Phone,
                    Note = entity.Note,
                    CreateDate = entity.CreateDate,
                    CreateUserID = entity.CreateUserID,
                    CreateUserName = entity.User.Name,
                    ProjectID = entity.ProjectID
                };

                return new CResult<WebManufacturer>(model);
            }
        }

        public CResult<bool> DeleteManufacturer(string manufacturerID)
        {
            if (string.IsNullOrEmpty(manufacturerID))
            {
                return new CResult<bool>(false, ErrorCode.ParameterError);
            }
            using (var context = new DeviceMgmtEntities())
            {
                var entity = context.Manufacturer.FirstOrDefault(t => t.ID == manufacturerID && t.IsValid);
                if (entity == null)
                {
                    return new CResult<bool>(false, ErrorCode.DataNoExist);
                }

                if (context.Device.Any(t => t.ManufacturerID == entity.ID))
                {
                    return new CResult<bool>(false, ErrorCode.ManufactureConatinDevice);
                }

                entity.IsValid = false;

                return context.Save();
            }
        }
    }
}