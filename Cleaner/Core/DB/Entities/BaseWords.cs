using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cleaner.Core.DB.Entities
{
    public class Connections
    {
        public long Id { get; set; }

        [Column("basicwordid")]
        public long BaseWordId { get; set; }
        public virtual BaseWords BaseWord { get; set; }

        [Column("wordid")]
        public long WordId { get; set; }
        public virtual Words Word { get; set; }
    }

    public class Languages
    {
        public Languages()
        {
            Characters = new HashSet<Characters>();
            BaseWords = new HashSet<BaseWords>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string NativeName { get; set; }
        public string EnglishName { get; set; }
        public string CultureName { get; set; }
        public bool? IsInterface { get; set; }

        public virtual ICollection<Characters> Characters { get; set; }
        public virtual ICollection<BaseWords> BaseWords { get; set; }
    }

    public class Characters
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Tolerant { get; set; }

        public long LangId { get; set; }
        public virtual Languages Language { get; set; }
    }

    public class Words
    {
        public Words()
        {
            BaseWordLinks = new HashSet<Connections>();
        }

        public long Id { get; set; }
        public string Word { get; set; }

        public virtual ICollection<Connections> BaseWordLinks { get; set; }
    }

    public class BaseWords
    {
        public BaseWords()
        {
            WordLinks = new HashSet<Connections>();
        }

        public long Id { get; set; }
        public string Word { get; set; }

        public long LangId { get; set; }
        public virtual Languages Language { get; set; }

        public virtual ICollection<Connections> WordLinks { get; set; }
    }
}
