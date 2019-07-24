using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace xLingua.Entities
{
    public interface IEntity
    {
        long Id { get; set; }
    }

    public class Abbreviations : IEntity
    {
        public long Id { get; set; }

        public string Abbreviation { get; set; }
        public string Explanation { get; set; }
    }

    public class Basewordexamples : IEntity
    {
        public long Id { get; set; }

        public string Text { get; set; }
    }

    public class Feedback : IEntity
    {
        public long Id { get; set; }

        public string Text { get; set; }
        public string Comment { get; set; }
    }

    public class Universal : IEntity
    {
        public long Id { get; set; }

        public string Word { get; set; }
    }

    public class Ui_Translations : IEntity
    {
        public long Id { get; set; }

        public string Key { get; set; }
        public string Text { get; set; }
    }

    public class Statistic : IEntity
    {
        public long Id { get; set; }

        public string Keyword { get; set; }
    }

    public class Functions : IEntity
    {
        public long Id { get; set; }

        [Column("function")]
        public string Function { get; set; }
    }

    public class Grammar : IEntity
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class Connections : IEntity
    {
        public long Id { get; set; }

        [Column("basicwordid")]
        public long BaseWordId { get; set; }
        public virtual BaseWords BaseWord { get; set; }

        [Column("wordid")]
        public long WordId { get; set; }
        public virtual Words Word { get; set; }
    }

    public class LanguageTranslations : IEntity
    {
        public LanguageTranslations()
        {
        }

        public long Id { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Fromlanguage { get; set; }
        public string Tolanguage { get; set; }
        public string Fromurl { get; set; }
        public string Tourl { get; set; }

    }

    public class Languages : IEntity
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

    public class Characters : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Tolerant { get; set; }

        public long LangId { get; set; }
        public virtual Languages Language { get; set; }
    }

    public class Words : IEntity
    {
        public Words()
        {
            BaseWordLinks = new HashSet<Connections>();
        }

        public long Id { get; set; }
        public string Word { get; set; }

        public virtual ICollection<Connections> BaseWordLinks { get; set; }
    }

    public class BaseWords : IEntity
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
