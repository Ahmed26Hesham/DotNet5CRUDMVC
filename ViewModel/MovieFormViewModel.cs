using DotNet5CRUDMVC.Models;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNet5CRUDMVC.ViewModel
{
    // *************************************************
    // بيبقا جواه كلاسيس بتتعامل مع الفيوز لا اكثر
    // دومين موديل بيتعامل مع الداتا بيز
    public class MovieFormViewModel
    {
        // StringLength => بتستخدم مع الفيو موديل 
        [Required, StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        [Range(1,10)]
        public double Rate { get; set; }

        [Required, StringLength(2500)]
        public string StoryLine { get; set; }

        [Display(Name ="Select Poster")]
        public byte[] Poster { get; set; }

        [Display(Name ="Genre")]
        public byte GenreId { get; set; }

        public IEnumerable<Genre> Genres { get; set; }


    }
}
