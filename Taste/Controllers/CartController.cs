﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taste.Data;
using Taste.Models;
using Taste.Models.OrderDetailsViewModel;

namespace Taste.Controllers
{
    public class CartController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart!= null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach(var list in detailCart.listCart)
            {
                list.MenuItem = _db.MenuItem.FirstOrDefault(m => m.Id == list.MenuItemId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

                if (list.MenuItem.Description.Length>100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
                
            }

            detailCart.OrderHeader.PickUpTime = DateTime.Now;

            return View(detailCart);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();
            cart.Count += 1;
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();

            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                _db.SaveChanges();

                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("CartCount", cnt);
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}