﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BobBookstore.Data;
using BobBookstore.Models.Book;
using BobBookstore.Models.Carts;
using BobBookstore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BobBookstore.Controllers
{
    public class SearchController : Controller
    {
        private readonly UsedBooksContext _context;

        public SearchController(UsedBooksContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync(string SortBy, string searchString)
        {

            if (!String.IsNullOrEmpty(searchString))
            {
                var prices = from p in _context.Price
                             where p.Book.Name.Contains(searchString) ||
                            p.Book.Genre.Name.Contains(searchString) ||
                            p.Book.Type.TypeName.Contains(searchString) ||
                            p.Book.ISBN.ToString().Contains(searchString)
                             select p;

                prices = prices.OrderBy(p => p.ItemPrice);

                var books = from b in _context.Book
                            where b.Name.Contains(searchString) ||
                            b.Genre.Name.Contains(searchString) ||
                            b.Type.TypeName.Contains(searchString) ||
                            b.ISBN.ToString().Contains(searchString)
                            select new BookViewModel
                            {
                                BookId = b.Book_Id,
                                BookName = b.Name,
                                ISBN = b.ISBN,
                                GenreName = b.Genre.Name,
                                TypeName = b.Type.TypeName,
                                Prices = prices.Where(p => p.Book.Book_Id == b.Book_Id).ToList(),
                                MinPrice = prices.Where(p => p.Book.Book_Id == b.Book_Id).FirstOrDefault().ItemPrice
                            };

                // sort query
                switch (SortBy)
                {
                    case "Name":
                        books = books.OrderByDescending(b => b.BookName);
                        break;
                    case "Genre":
                        books = books.OrderBy(b => b.GenreName);
                        break;
                    case "Type":
                        books = books.OrderBy(b => b.TypeName);
                        break;
                    case "PriceAsc":
                        books = books.OrderBy(b => b.MinPrice);
                        break;
                    case "PriceDesc":
                        books = books.OrderByDescending(b => b.MinPrice);
                        break;
                    default:
                        break;
                }

                return View(await books.ToListAsync());

            }

            return View();
        }

        public async Task<IActionResult> DetailAsync(long id)
        {
            var prices = await (from p in _context.Price where p.Book.Book_Id == id
                         join c in _context.Condition
                         on p.Condition.Condition_Id equals c.Condition_Id
                         select new Price
                         {
                             Price_Id = p.Price_Id,
                             Condition = c,
                             ItemPrice = p.ItemPrice,
                             Quantity = p.Quantity
                         }).ToListAsync();

            var book = from m in _context.Book
                       where m.Book_Id == id
                       select new BookViewModel()
                       {
                           BookName = m.Name,
                           ISBN = m.ISBN,
                           GenreName = m.Genre.Name,
                           TypeName = m.Type.TypeName,
                           Url = m.Back_Url,
                           Prices = prices,
                           BookId=m.Book_Id
                       };

            return View(await book.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> AddtoCartitem(long bookid, long priceid)
        {
            var book = _context.Book.Find(bookid);
            var price = _context.Price.Find(priceid);
            var cartId = HttpContext.Request.Cookies["CartId"];
            var cart = _context.Cart.Find(Convert.ToInt32(cartId));

            var cartItem = new CartItem() { Book = book, Price = price, Cart = cart,WantToBuy=true };

            _context.Add(cartItem);
            _context.SaveChanges();
            var stringBookid = Convert.ToString(bookid);
            return RedirectToAction("Detail","Search",new { @id = bookid });
        }
        public async Task<IActionResult> AddtoWishList(long bookid, long priceid)
        {
            var book = _context.Book.Find(bookid);
            var price = _context.Price.Find(priceid);
            var cartId = HttpContext.Request.Cookies["CartId"];
            var cart = _context.Cart.Find(Convert.ToInt32(cartId));

            var cartItem = new CartItem() { Book = book, Price = price, Cart = cart};

            _context.Add(cartItem);
            _context.SaveChanges();
            var stringBookid = Convert.ToString(bookid);
            return RedirectToAction("Detail", "Search", new { @id = bookid });
        }
    }
}
