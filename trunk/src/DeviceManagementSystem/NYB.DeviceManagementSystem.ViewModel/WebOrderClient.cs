﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYB.DeviceManagementSystem.ViewModel
{
    public class WebOrderClient
    {
        WebUser _webUser = new WebUser();

        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }


        private string _name = string.Empty;

        [Required(ErrorMessage = "客户名称必填")]
        [StringLength(30, ErrorMessage = "最大长度为30")]
        [Display(Name = "客户名称")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _note = string.Empty;

        [StringLength(500, ErrorMessage = "最大长度为500")]
        [Display(Name = "备注")]
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public WebUser WebUser
        {
            get
            {
                return _webUser;
            }
            set
            {
                _webUser = value;
            }
        }

        public DateTime CreateDate { get; set; }

        public string CreateUserID { get; set; }

        public string LogoFile { get; set; }

        [StringLength(2000, ErrorMessage = "最大长度为2000")]
        [Display(Name = "公司简介")]
        public string CompanyDescribe { get; set; }

        [StringLength(2000, ErrorMessage = "最大长度为2000")]
        [Display(Name = "联系我们")]
        public string CompanyContact { get; set; }

        [StringLength(20, ErrorMessage = "最大长度为20")]
        [Display(Name = "系统名称")]
        public string CompanyName { get; set; }
    }
}
