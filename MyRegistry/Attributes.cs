using System;
using System.Collections.Generic;
using System.Text;

namespace MyLibrary
{
    public class RequiredKeyNameAttribute : Attribute
    {
        public RequiredKeyNameAttribute()
        { }
        
        public string KeyName { get; set; }
    }
}
