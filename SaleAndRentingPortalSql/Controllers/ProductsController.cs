using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;
using SaleAndRentingPortalSql.Data;
using SaleAndRentingPortalSql.Models;
using SaleAndRentingPortalSql.Models.DatabaseModels;
using SaleAndRentingPortalSql.Models.ProductViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaleAndRentingPortalSql.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        private readonly ILogger _logger;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProductsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index(Productsearchmodel model, string searchString)
        {
            if (model == null)
            {
                model = new Productsearchmodel();
            }
            var SearchProducts = from m in _context.Product
                                 select m;

            if (!String.IsNullOrEmpty(model.searchString) && String.IsNullOrEmpty(searchString))
            {
                SearchProducts = SearchProducts.Where(s =>
                    s.Name.ToLower().Contains(model.searchString.ToLower()) ||
                    s.Description.ToLower().Contains(model.searchString.ToLower()) ||
                    s.Id.ToLower().Contains(model.searchString.ToLower()));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                SearchProducts = SearchProducts.Where(s =>
                    s.Name.ToLower().Contains(searchString.ToLower()) ||
                    s.Description.ToLower().Contains(searchString.ToLower()) ||
                    s.Id.ToLower().Contains(searchString.ToLower()));

                model.searchString = searchString;
            }

            if (model.minprice != null)
            {
                SearchProducts = SearchProducts.Where(s => s.Price > model.minprice);
            }

            if (model.maxprice != null)
            {
                SearchProducts = SearchProducts.Where(s => s.Price < model.maxprice);
            }
            if (model.isstock)
            {
                SearchProducts = SearchProducts.Where(s => s.NoOfItems > 0);
            }


            foreach (var product in SearchProducts)
            {
                model.products.Add(new Product(product.Id, product.Price, product.Name, product.Description,
                    product.NoOfItems));
            }


            return View(model);
        }


        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(string Myid)
        {
            if (Myid == null)
            {
                return NotFound();
            }


            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == Myid);
            if (product == null)
            {
                return NotFound();
            }

            Product viewProduct = new Product(product.Id, product.Price, product.Name, product.Description,
                product.NoOfItems);


            foreach (var dbSpecse in _context.Specs.Where(i => i.ProductId == Myid))
            {
                viewProduct.Specs.Add(new Specs() { Text = dbSpecse.SpecName, Value = dbSpecse.Value });
            }

            foreach (var productCategory in _context.ProductCategory.Where(i => i.ProductId == Myid))
            {
                viewProduct.Categories.Add(_context.Category.FirstOrDefault(i => i.Id == productCategory.CategoryId)
                    .Category);
            }
            viewProduct.pictures = new List<Picture>();

            string storageConnectionString =
                "StorageConnection";
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer blobContainer =
                blobClient.GetContainerReference("productid-" + Myid);

            if (await blobContainer.ExistsAsync())
            {
                var blobs = await blobContainer.ListBlobsSegmentedAsync(string.Empty, false, BlobListingDetails.None,
                    int.MaxValue, null, null, null);
                foreach (var blob in blobs.Results)
                {
                    Picture picture = new Picture();
                    picture.Path = blob.Uri.ToString();
                    viewProduct.pictures.Add(picture);
                }
            }


            return View(viewProduct);
        }

        private string HTMLENCODE()
        {
            throw new NotImplementedException();
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            Product product = new Product();
            product.Specs.Add(new Specs());
            product.Categories.Add("");
            product.pictures = new List<Picture>();
            product.pictures.Add(new Picture());
            return View(product);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( /*[Bind("Price,Id,Name,Description,NumberOfItemsInStock")]*/
            Product product)
        {
            switch (product.status)
            {
                case 1:
                    if (product.Categories.Count != product.Categories.Distinct().Count())
                    {
                        ModelState.AddModelError("Categories", "Category findes allerede");
                    }
                    if (product.Categories.LastOrDefault() != null)
                    {
                        product.Categories.Add("");
                    }
                    return View(product);
                case 2:
                    if (product.Specs.LastOrDefault().Value != null && product.Specs.LastOrDefault().Text != null)
                    {
                        var dubtest = from s in product.Specs select new { text = s.Text, value = s.Value };

                        if (dubtest.Count() != dubtest.Distinct().Count())
                        {
                            ModelState.AddModelError("Specs", "Specification findes allerede");
                        }
                        if (product.Specs.LastOrDefault().Value != null && product.Specs.LastOrDefault().Text != null)
                        {
                            product.Specs.Add(new Specs());
                        }

                    }
                    return View(product);
                default:
                    if (ModelState.IsValid)
                    {
                        string Productid;
                        if (_context.Product.FirstOrDefault(i => i.Id != null) != null)
                        {
                            var createdtime = _context.Product.Max(t => t.Created);
                            string Id = Guid.NewGuid().ToString();
                            while (_context.Product.FirstOrDefault(i => i.Id == Id) != null)
                            {
                                Id = Guid.NewGuid().ToString();
                            }

                            Productid = Id;
                            product.Id = Productid;
                            _context.Product.Add(new DbProductc(Productid, product.Price, product.Name, DateTime.Now,
                                product.Description, product.NoOfItems));
                        }
                        else
                        {
                            Productid = Guid.NewGuid().ToString();
                            _context.Product.Add(new DbProductc(Productid, product.Price, product.Name, DateTime.Now,
                                product.Description, product.NoOfItems));
                        }

                        foreach (var category in product.Categories)
                        {
                            if (category == null)
                                continue;
                            if (_context.Category.FirstOrDefault(i => i.Category == category.ToLower()) == null)
                            {
                                if (_context.Category.FirstOrDefault(i => i.Id != null) != null)
                                {
                                    var createdtime = _context.Category.Max(t => t.Created);
                                    string id = Guid.NewGuid().ToString();
                                    while (_context.Category.FirstOrDefault(i => i.Id == id) != null)
                                    {
                                        id = Guid.NewGuid().ToString();
                                    }
                                    _context.Category.Add(new DbCategory(id, category.ToLower(), DateTime.Now));
                                    _context.ProductCategory.Add(new DbProductCategory(Productid, id));
                                }
                                else
                                {
                                    string id = Guid.NewGuid().ToString();
                                    _context.Category.Add(new DbCategory(id, category.ToLower(), DateTime.Now));
                                    _context.ProductCategory.Add(new DbProductCategory(Productid, id));
                                }
                            }
                            else
                            {
                                var cat = _context.Category.FirstOrDefault(i => i.Category == category.ToLower());

                                _context.ProductCategory.Add(new DbProductCategory(Productid, cat.Id));
                            }
                        }
                        foreach (var spec in product.Specs)
                        {
                            if (spec.Text == null || spec.Value == null)
                                continue;
                            if (_context.Specs.FirstOrDefault(i => i.Id != null) != null)
                            {
                                var createdtime = _context.Specs.Max(t => t.Created);
                                string id = Guid.NewGuid().ToString();
                                while (_context.Specs.FirstOrDefault(i => i.Id == id) != null)
                                {
                                    id = Guid.NewGuid().ToString();
                                }
                                _context.Specs.Add(new DbSpecs(spec.Text, id, spec.Value, Productid, DateTime.Now));
                            }
                            else
                            {
                                string id = Guid.NewGuid().ToString();
                                _context.Specs.Add(new DbSpecs(spec.Text, id, spec.Value, Productid, DateTime.Now));
                            }
                        }

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string Myid)
        {
            if (Myid == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == Myid);

            if (product == null)
            {
                return NotFound();
            }

            Product viewProduct = new Product(product.Id, product.Price, product.Name, product.Description,
                product.NoOfItems);


            foreach (var dbSpecse in _context.Specs.Where(i => i.ProductId == Myid))
            {
                viewProduct.Specs.Add(new Specs() { Text = dbSpecse.SpecName, Value = dbSpecse.Value });
            }

            foreach (var productCategory in _context.ProductCategory.Where(i => i.ProductId == Myid))
            {
                viewProduct.Categories.Add(_context.Category.FirstOrDefault(i => i.Id == productCategory.CategoryId)
                    .Category);
            }
            viewProduct.pictures = new List<Picture>();
            viewProduct.Specs.Add(new Specs());
            viewProduct.Categories.Add("");

            string storageConnectionString =
"StorageConnection";
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer blobContainer =
                blobClient.GetContainerReference("productid-" + Myid);

            if (await blobContainer.ExistsAsync())
            {
                var blobs = await blobContainer.ListBlobsSegmentedAsync(string.Empty, false, BlobListingDetails.None,
                    int.MaxValue, null, null, null);
                foreach (var blob in blobs.Results)
                {
                    Picture picture = new Picture();
                    picture.Path = blob.Uri.ToString();
                    viewProduct.pictures.Add(picture);
                }
            }
            viewProduct.pictures.Add(new Picture());


            return View(viewProduct);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            switch (product.status)
            {
                case 1:
                    if (product.Categories.LastOrDefault() != null)
                    {
                        if (product.Categories.Count != product.Categories.Distinct().Count())
                        {
                            ModelState.AddModelError("Categories", "Category findes allerede");
                        }
                        else
                        {
                            product.Categories.Add("");
                        }
                    }
                    return View(product);
                case 2:
                    var dubtest = from s in product.Specs select new { text = s.Text, value = s.Value };
                    if (product.Specs.LastOrDefault().Value != null && product.Specs.LastOrDefault().Text != null)
                    {
                        if (dubtest.Count() != dubtest.Distinct().Count())
                        {
                            ModelState.AddModelError("Specs", "Specification findes allerede");
                        }
                        else
                        {
                            product.Specs.Add(new Specs());
                        }
                    }

                    return View(product);


                case 3:
                    {
                        if (product.pictures.LastOrDefault() == null || product.pictures.LastOrDefault().File == null ||
                            (!product.pictures.LastOrDefault().File.FileName.ToLower().EndsWith(".png") &&
                             !product.pictures.LastOrDefault().File.FileName.ToLower().EndsWith(".jpg") &&
                             !product.pictures.LastOrDefault().File.FileName.ToLower().EndsWith(".jpeg") &&
                             !product.pictures.LastOrDefault().File.FileName.ToLower().EndsWith(".gif")))
                        {
                            ModelState.AddModelError("pictures", "Uglydig filtype du kan kun uploade png,jpeg,jpg,gif");
                        }
                        else if (product.pictures.LastOrDefault().File.Length > 5242880)
                        {
                            ModelState.AddModelError("pictures", "Filen må ikke være større end 5Mb");
                        }


                        string storageConnectionString =
                            "StorageConnection";
                        CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                        CloudBlobClient blobClient = account.CreateCloudBlobClient();
                        CloudBlobContainer blobContainer =
                            blobClient.GetContainerReference("productid-" + product.Id);
                        if (ModelState.IsValid)
                        {
                            await blobContainer.CreateIfNotExistsAsync();
                            TransferManager.Configurations.ParallelOperations = 64;
                            BlobContainerPermissions permissions = await blobContainer.GetPermissionsAsync();
                            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                            await blobContainer.SetPermissionsAsync(permissions);

                            // string sourcePath = "path\\to\\test.txt";
                            CloudBlockBlob destBlob =
                                blobContainer.GetBlockBlobReference(product.pictures.LastOrDefault().File.FileName);


                            // Setup the transfer context and track the upoload progress
                            // Upload a local blob
                            if (!await destBlob.ExistsAsync())
                            {
                                using (Stream s1 = product.pictures.LastOrDefault().File.OpenReadStream())
                                {
                                    var task = TransferManager.UploadAsync(s1, destBlob);
                                    task.Wait();
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("pictures", "Billedet er allerede uploaded");
                            }
                        }


                        product.pictures = new List<Picture>();

                        if (await blobContainer.ExistsAsync())
                        {
                            var blobs = await blobContainer.ListBlobsSegmentedAsync(string.Empty, false,
                                BlobListingDetails.None, int.MaxValue, null, null, null);
                            foreach (var blob in blobs.Results)
                            {
                                Picture picture = new Picture();
                                picture.Path = blob.Uri.ToString();
                                product.pictures.Add(picture);
                            }
                        }

                        product.pictures.Add(new Picture());

                        return View(product);
                    }

                default:

                    try
                    {
                        if (_context.Product.FirstOrDefault(i => i.Id == product.Id) == null)
                        {
                            return NotFound(product);
                        }

                        if (ModelState.IsValid)
                        {
                            var dbproduct = _context.Product.FirstOrDefault(i => i.Id == product.Id);

                            dbproduct.Description = product.Description;
                            dbproduct.Name = product.Name;
                            dbproduct.Price = product.Price;
                            dbproduct.NoOfItems = product.NoOfItems;
                            _context.Product.Update(dbproduct);


                            if (product.Categories == null)
                            {
                                product.Categories = new List<string>();
                            }
                            var categories = from PC in _context.ProductCategory
                                             where PC.ProductId == product.Id
                                             join CA in _context.Category on PC.CategoryId equals CA.Id
                                             select CA;

                            foreach (var category in categories)
                            {
                                if (product.Categories.Contains(category.Category) || category == null)
                                {
                                    continue;
                                }
                                _context.ProductCategory.Remove(
                                    _context.ProductCategory.FirstOrDefault(i => i.CategoryId == category.Id));
                            }
                            var categories2 = from PC in _context.ProductCategory
                                              where PC.ProductId == product.Id
                                              join CA in _context.Category on PC.CategoryId equals CA.Id
                                              select CA.Category;
                            foreach (var category in product.Categories)
                            {
                                if (categories2.Contains(category) || category == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (_context.Category.FirstOrDefault(i => i.Category == category.ToLower()) == null)
                                    {
                                        if (_context.Category.FirstOrDefault(i => i.Id != null) != null)
                                        {
                                            var createdtime = _context.Category.Max(t => t.Created);

                                            string Id = Guid.NewGuid().ToString();
                                            while (_context.Category.FirstOrDefault(i => i.Id == Id) != null)
                                            {
                                                Id = Guid.NewGuid().ToString();
                                            }
                                            _context.Category.Add(new DbCategory(Id, category.ToLower(),
                                                DateTime.Now));
                                            _context.ProductCategory.Add(new DbProductCategory(product.Id, Id));
                                        }
                                        else
                                        {
                                            string Id = Guid.NewGuid().ToString();
                                            _context.Category.Add(new DbCategory(Id, category.ToLower(),
                                                DateTime.Now));
                                            _context.ProductCategory.Add(new DbProductCategory(product.Id, Id));
                                        }
                                    }
                                    else
                                    {
                                        var cat = _context.Category.FirstOrDefault(
                                            i => i.Category == category.ToLower());

                                        _context.ProductCategory.Add(new DbProductCategory(product.Id, cat.Id));
                                    }
                                }
                            }
                            if (product.Specs == null)
                            {
                                product.Specs = new List<Specs>();
                            }

                            var specs = from s in _context.Specs
                                        where s.ProductId == product.Id
                                        select new SelectListItem() { Text = s.SpecName, Value = s.Value };
                            foreach (var spec in specs)
                            {
                                if (spec == null ||
                                    product.Specs.Where(i => i.Value == spec.Value && i.Text == spec.Text)
                                        .LastOrDefault() != null)
                                {
                                    continue;
                                }
                                else
                                {
                                    _context.Remove(_context.Specs.FirstOrDefault(i =>
                                        i.SpecName == spec.Text && i.Value == spec.Value));
                                }
                            }
                            foreach (var spec in product.Specs)
                            {
                                if (spec == null || specs.Where(i => i.Value == spec.Value && i.Text == spec.Text)
                                        .LastOrDefault() != null)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (spec.Text == null || spec.Value == null)
                                        continue;
                                    if (_context.Specs.FirstOrDefault(i => i.Id != null) != null)
                                    {
                                        string Id = Guid.NewGuid().ToString();
                                        while (_context.Specs.FirstOrDefault(i => i.Id == Id) != null)
                                        {
                                            Id = Guid.NewGuid().ToString();
                                        }
                                        _context.Specs.Add(new DbSpecs(spec.Text, Id, spec.Value, product.Id,
                                            DateTime.Now));
                                    }
                                    else
                                    {
                                        string Id = Guid.NewGuid().ToString();
                                        _context.Specs.Add(new DbSpecs(spec.Text, Id, spec.Value, product.Id,
                                            DateTime.Now));
                                    }
                                }
                            }


                            _context.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
            }
        }

        private bool ProductExists(string id)
        {
            throw new NotImplementedException();
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string Myid, bool? saveChangesError = false)
        {
            if (Myid == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == Myid);
            if (product == null)
            {
                return NotFound();
            }


            Product viewProduct = new Product(product.Id, product.Price, product.Name, product.Description,
                product.NoOfItems);


            foreach (var dbSpecse in _context.Specs.Where(i => i.ProductId == Myid))
            {
                viewProduct.Specs.Add(new Specs() { Text = dbSpecse.SpecName, Value = dbSpecse.Value });
            }

            foreach (var productCategory in _context.ProductCategory.Where(i => i.ProductId == Myid))
            {
                viewProduct.Categories.Add(_context.Category.FirstOrDefault(i => i.Id == productCategory.CategoryId)
                    .Category);
            }
            viewProduct.pictures = new List<Picture>();

            string storageConnectionString =
                "StorageConnection";
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer blobContainer =
                blobClient.GetContainerReference("productid-" + Myid);

            if (await blobContainer.ExistsAsync())
            {
                var blobs = await blobContainer.ListBlobsSegmentedAsync(string.Empty, false, BlobListingDetails.None,
                    int.MaxValue, null, null, null);
                foreach (var blob in blobs.Results)
                {
                    Picture picture = new Picture();
                    picture.Path = blob.Uri.ToString();
                    viewProduct.pictures.Add(picture);
                }
            }


            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(viewProduct);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Product
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var productcategories = _context.ProductCategory.Where(i => i.ProductId == id);
                foreach (var productcategory in productcategories)
                {
                    _context.ProductCategory.Remove(productcategory);
                }

                var Specs = _context.Specs.Where(i => i.ProductId == id);
                foreach (var spec in Specs)
                {
                    _context.Specs.Remove(spec);
                }

                _context.Product.Remove(product);


                string storageConnectionString =
                    "StorageConnection";
                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient blobClient = account.CreateCloudBlobClient();
                CloudBlobContainer blobContainer =
                    blobClient.GetContainerReference("productid-" + id);


                if (await blobContainer.ExistsAsync())
                    await blobContainer.DeleteAsync();

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Error in delete " + ex.Message);
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { Myid = id, saveChangesError = true });
            }

            catch (Exception ex)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { Myid = id, saveChangesError = true });
            }


            //private bool ProductExists(string id)
            //{
            //    return _context.Product.Any(e => e.Id == id);
            //}
        }





    }
}