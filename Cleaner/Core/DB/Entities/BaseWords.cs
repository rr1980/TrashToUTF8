﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cleaner.Core.DB.Entities
{
    public class Abbreviations
    {
        public long Id { get; set; }

        public string Abbreviation { get; set; }
        public string Explanation { get; set; }
    }

    public class Basewordexamples
    {
        public long Id { get; set; }

        public string Text { get; set; }
    }

    public class Feedback
    {
        public long Id { get; set; }

        public string Text { get; set; }
        public string Comment { get; set; }
    }

    public class Universal
    {
        public long Id { get; set; }

        public string Word { get; set; }
    }

    public class Ui_Translations
    {
        public long Id { get; set; }

        public string Key { get; set; }
        public string Text { get; set; }
    }

    public class Statistic
    {
        public long Id { get; set; }

        public string Keyword { get; set; }
    }

    public class Functions
    {
        public long Id { get; set; }

        [Column("function")]
        public string Function { get; set; }
    }

    public class Grammar
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
    }

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

    public class LanguageTranslations
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
