using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class LogModel
    {
        [Key] public int Id { get; set; }
        public UserModel User { get; set; }
        public DateTime When { get; set; }
        public String Data { get; set; }
    }
}
