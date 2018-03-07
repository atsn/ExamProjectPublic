using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaleAndRentingPortalSql.Data;
using SaleAndRentingPortalSql.Models;
using SaleAndRentingPortalSql.Models.DatabaseModels;
using SaleAndRentingPortalSql.Models.ProductViewModels;
using SaleAndRentingPortalSql.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Controllers
{
    [Authorize]
    public class OrdresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        private readonly ILogger _logger;


        public OrdresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProductsController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        // GET: Ordres
   
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Ordre.ToListAsync());
            }
            else
            {
                return View(await _context.Ordre.Where(i => i.UserId == _context.Users.FirstOrDefault(z => z.UserName == User.Identity.Name).Id).ToListAsync());
            }
        }

        public async Task<IActionResult> OrdreDetail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            OrdreProductViewModel2 model = new OrdreProductViewModel2();


            var ordre = _context.Ordre.FirstOrDefault(i => i.Orderid == id);
            if (!User.IsInRole("Admin") &&
                ordre.UserId != _context.Users.FirstOrDefault(z => z.UserName == User.Identity.Name).Id)
            {
                return RedirectToAction("Index");
            }

            model.Ordre = ordre;

            model.Products = new List<Product>();

            foreach (var product in _context.OrderProduct.Where(i => i.OrderId == id))
            {

                model.Products.Add(new Product(_context.Product.FirstOrDefault(i => i.Id == product.ProductId)));
            }


            return View(model);


        }
        // GET: Ordres/Details/5
        public async Task<IActionResult> Details(string id, bool haspassworderror = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (haspassworderror)
            {
                ModelState.AddModelError("Password", "Kodeord er forkert");
            }
            OrdreProductViewModel model = new OrdreProductViewModel();


            var ordre = _context.Ordre.FirstOrDefault(i => i.Orderid == id);


            model.Ordre = new OrdreViewModel(Encrypter.Encrypt(ordre.Orderid), ordre.Price, ordre.OrderDate, ordre.OrderTime, ordre.KortholderNavn, ordre.Address, ordre.Zipcode, ordre.Email);

            model.Products = new List<Product>();

            foreach (var product in _context.OrderProduct.Where(i => i.OrderId == id))
            {

                model.Products.Add(new Product(_context.Product.FirstOrDefault(i => i.Id == product.ProductId)));
            }


            return View(model);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string Token, string Password)
        {
            if (Token == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(Password))
            {
                return RedirectToAction("Details", new { id = Encrypter.Decrypt(Token), haspassworderror = true });
            }

            var user = await _userManager.CheckPasswordAsync(_context.Users.FirstOrDefault(i => i.UserName == User.Identity.Name), Password);

            if (!user)
            {
                return RedirectToAction("Details", new { id = Encrypter.Decrypt(Token), haspassworderror = true });
            }

            var ordre = _context.Ordre.FirstOrDefault(i => i.Orderid == Encrypter.Decrypt(Token));
            ordre.Status = "Payment Reshived";
            _context.Ordre.Update(ordre);

            string body =
                @"<br/>Du vil modtage dine vare inde for få hverdage<br/><table><thead><tr><th>Pris</th><th>Productnavn</th></tr></thead><tbody>";

            foreach (var product in _context.OrderProduct.Where(i => i.OrderId == Encrypter.Decrypt(Token)))
            {
                var products = _context.Product.FirstOrDefault(i => i.Id == product.ProductId);
                products.NoOfItems--;
                if (products.NoOfItems < 0)
                {
                    ModelState.AddModelError("Email", "Der gik noget galt under ordren. prøv veligst igen, og vis det statid ikke virker så kontakt venligst en administator");
                    RedirectToAction("Details", new { id = Encrypter.Decrypt(Token) });
                }
                _context.Update(products);
                body = body + @"<tr><td>" + products.Price + @" DKK &nbsp</td><td>" + products.Name + @"</td></tr>";

            }
            body = body + "</tbody></table><br/><br>Total pris: " + ordre.Price + "DKK";
            _context.SaveChanges();
            EmailSender.SendEmail("Kvitering for køb", body, "Kvitering", ordre.Email);


            return RedirectToAction("Confirmation");


        }

        
        public IActionResult Confirmation()
        {
            return View();
        }

        private bool DbOrdreExists(string id)
        {
            return _context.Ordre.Any(e => e.Orderid == id);
        }

 
        public async Task<IActionResult> OrderTjeck()
        {
            DbOrdre ordre = new DbOrdre();
            Random random = new Random();
            ordre.Orderid = DateTime.Now.ToString("yyMMddhhmmssfffff") + random.Next(10000, 100000);
            while (_context.Ordre.FirstOrDefault(i => i.Orderid == ordre.Orderid) != null)
            {
                ordre.Orderid = DateTime.Now.ToString("yyMMddhhmmssfffff") + random.Next(10000, 100000);
            }

            var products = new List<Product>();
            var s = Request.Cookies.Keys;
            var c = Request.Cookies["Shoppingcart"];
            var Items = c.Split("-|-");


            int price = 0;
            for (int i = 1; i < Items.Length; i++)
            {

                products.Add(new Product(_context.Product.FirstOrDefault(z => z.Id.Equals(Items[i]))));

                if (products.Count(z => z.Id == products.LastOrDefault().Id) > products.LastOrDefault().NoOfItems)
                {
                    if (ModelState["Name"] == null || ModelState["Name"].Errors.Where(z => z.ErrorMessage == "Vi har ikke så mange " + products.LastOrDefault().Name + " som du ønsker") == null)
                    {
                        ModelState.AddModelError("Name", "Vi har ikke så mange " + products.LastOrDefault().Name + " som du ønsker");

                    }
                }
                price = price + products.LastOrDefault().Price;

                string Id = Guid.NewGuid().ToString();

                while (_context.OrderProduct.FirstOrDefault(z => z.Id == Id) != null)
                {
                    Id = Guid.NewGuid().ToString();
                }
                _context.OrderProduct.Add(new DbOrdreProduct(Id, ordre.Orderid, products.LastOrDefault().Id));

            }


            if (!ModelState.IsValid)
            {

                return View("ShoppingCart", products);
            }


            ordre.Price = price;
            var user = _context.ApplicationUser.FirstOrDefault(i => i.UserName == User.Identity.Name);
            ordre.UserId = user.Id;

            ordre.OrderDate = DateTime.Now.ToString("yyMMdd");
            ordre.OrderTime = DateTime.Now.ToString("hhmmss");
            ordre.Status = "Ikke Gennemført";
            _context.Ordre.Add(ordre);

            _context.SaveChanges();

            return View(new OrdreViewModel(Encrypter.Encrypt(ordre.Orderid), ordre.Price, ordre.OrderDate, ordre.OrderTime, user.FirstName + " " + user.LastName, user.Address, user.Zipcode, user.Email));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ShoppingCart()
        {
            var products = new List<Product>();
            var s = Request.Cookies.Keys;
            var c = Request.Cookies["Shoppingcart"];
            var Items = c.Split("-|-");


            for (int i = 1; i < Items.Length; i++)
            {
                products.Add(new Product(_context.Product.FirstOrDefault(z => z.Id.Equals(Items[i]))));
            }
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderTjeck(OrdreViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.ZipCodes.FirstOrDefault(i => i.ZipCode == model.Zipcode) == null)

            {
                ModelState.AddModelError("ZipCode", "Postnummer findes ikke");

                return View(model);

            }

            var ordre = _context.Ordre.FirstOrDefault(i => i.Orderid == Encrypter.Decrypt(model.Token));

            ordre.KortholderNavn = model.KortholderNavn;

            ordre.Address = model.Address;

            ordre.Zipcode = model.Zipcode;

            ordre.Email = model.Email;

            _context.Ordre.Update(ordre);

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = ordre.Orderid });

        }

    }
}
