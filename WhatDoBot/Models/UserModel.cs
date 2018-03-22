using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public String UserId { get; set; }
        public bool Confirmed { get; set; }
    }
}
