﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BobBookstore.Models.ViewModels
{
    public class BookViewModel
    {
        public long BookId { get; set; }

        [Display(Name = "Title")]
        public string BookName { get; set; }

        public long ISBN { get; set; }

        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }

        public List<Book.Price> Prices { get; set; }
        public string Url { get; set; }

        [Display(Name = "$$")]
        public double MinPrice { get; set; }


        //public static void sortBy(List<BookViewModel> lst, string prop)
        //{
        //    switch (prop)
        //    {
        //        case "Name":
        //            lst.Sort((elt1, elt2) => elt1.BookName.CompareTo(elt2.BookName));
        //            break;
        //        case "Genre":
        //            lst.Sort((elt1, elt2) => elt1.GenreName.CompareTo(elt2.GenreName));
        //            break;
        //        case "Type":
        //            lst.Sort((elt1, elt2) => elt1.TypeName.CompareTo(elt2.TypeName));
        //            break;
        //        case "Price":
        //            lst.Sort((elt1, elt2) => elt1.Prices.Min().ItemPrice.CompareTo(elt2.Prices.Min().ItemPrice));
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
