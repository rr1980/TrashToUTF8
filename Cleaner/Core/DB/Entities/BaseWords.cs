using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cleaner.Core.DB.Entities
{
    public class BaseWords
    {
        public long Id { get; set; }
        //[Column(TypeName = "VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci")]
        public string Word { get; set; }
    }
}
