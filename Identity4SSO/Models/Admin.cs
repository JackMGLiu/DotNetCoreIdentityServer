﻿using System;

namespace Identity4SSO.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Remark { get; set; }
    }
}