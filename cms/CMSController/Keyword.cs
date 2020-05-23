using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSController
{
    public class Keyword
    {
        public Keyword(string value)
        {
            Value = value;
        }

        [Key]
        public int Id
        {
            get; set;
        }

        public string Value { get; set; }

        public static implicit operator string(Keyword keyword)
        {
            return keyword.Value;
        }

        public static explicit operator Keyword(string value)
        {
            var castValue = new Keyword(value);
            return castValue;
        }
    }
}
