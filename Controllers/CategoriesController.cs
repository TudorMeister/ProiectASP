using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectASP.Data;
using ProiectASP.Models;
using System.Data;

namespace ProiectASP.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var categories  = db.Categories.ToList(); 

            ViewBag.Categories = categories;   
            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            Category category = db.Categories.Where(category => category.Id == id)
                                             .First();

            //SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            Category category = new Category();

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult New(Category category) 
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {
                category.CategoryName = sanitizer.Sanitize(category.CategoryName);

                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                return RedirectToAction("Index");
            } else
            {
                return View(category);
            }
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {

            Category category = db.Categories.Where(category => category.Id == id)
                                             .First();

            //article.Categ = GetAllCategories();

            //if (article.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            //{
                return View(category);
            //}

            //else
            //{
            //    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
            //    return RedirectToAction("Index");
            //}

        }


        // Se adauga articolul modificat in baza de date
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);


            if (ModelState.IsValid)
            {
                //if (article.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                //{
                    category.CategoryName = requestCategory.CategoryName;
                    TempData["message"] = "Categoria a fost modificat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
                //else
                //{
                //    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                //    return RedirectToAction("Index");
                //}
            }
            else
            {
            //    requestArticle.Categ = GetAllCategories();
                return View(requestCategory);
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Where(category => category.Id == id)
                                             .First();

            //if (category.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            //{
            if (db.Articles.Where(article => article.CategoryId == id).Count() == 0)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost stearsa";
                return RedirectToAction("Index");
            }
            //}

            else
            {
                TempData["message"] = "Categorie este folosita in articole existente";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult ArataArticole(int id)
        {
            var articles = db.Articles.Include("Category").Include("User").Where(art => art.CategoryId == id).ToList();

            int _perPage = 3;

            int totalItems = 0;

            if (articles != null)
            {
                totalItems = articles.Count();
            }

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedArticles = articles.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Articles = paginatedArticles;

            ViewBag.PaginationBaseUrl = "/Articles/Index/?page";


            return View();
        }


    }
}
