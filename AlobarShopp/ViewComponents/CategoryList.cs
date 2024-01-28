using AlobarShopp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AlobarShopp.ViewComponents
{
    public class CategoryList : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CategoryList(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var category = _db.Categories.ToList();
            return View(category);
        }
    }
}