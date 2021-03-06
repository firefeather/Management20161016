﻿using NYB.DeviceManagementSystem.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYB.DeviceManagementSystem.ViewModel
{
    public class WebUser : ViewModelBase
    {
        public string OrderClientID { get; set; }

        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _userName = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名必填")]
        [Display(Name = "姓名")]
        [StringLength(20, ErrorMessage = "最大长度为20")]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _pwd = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码必填")]
        [Display(Name = "密码")]
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

        private string _olmPwd = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码必填")]
        [Display(Name = "密码")]
        public string OldPwd
        {
            get { return _olmPwd; }
            set { _olmPwd = value; }
        }

        private string _confirmPwd = string.Empty;
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "密码必填")]
        [Compare("Pwd", ErrorMessage = "密码必须一致")]
        [Display(Name = "确认密码")]
        public string ConfirmPwd
        {
            get { return _confirmPwd; }
            set { _confirmPwd = value; }
        }

        private string _loginName = string.Empty;

        /// <summary>
        /// 登录名
        /// </summary>
        [Required(ErrorMessage = "登录名必填")]
        [Display(Name = "登录名")]
        [StringLength(20, ErrorMessage = "最大长度为20")]
        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        private string _address = string.Empty;
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100, ErrorMessage = "最大长度为100")]
        [Display(Name = "地址")]
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [StringLength(20, ErrorMessage = "最大长度为20")]
        [Display(Name = "手机")]
        public string Moblie { get; set; }

        private string _telPhone = string.Empty;

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        [StringLength(20, ErrorMessage = "最大长度为20")]
        public string TelPhone
        {
            get { return _telPhone; }
            set { _telPhone = value; }
        }

        private string _email = string.Empty;
        /// <summary>
        /// 电子邮件
        /// </summary>
        [Display(Name = "电子邮件")]
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "电子邮件格式不正确")]
        [StringLength(50, ErrorMessage = "最大长度为50")]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public RoleType Role { get; set; }
    }
}
