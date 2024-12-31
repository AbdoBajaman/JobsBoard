using JobsBoard.Data;
using JobsBoard.Models;
using JobsBoard.Repostry.RepostryPattern;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobsBoard.Controllers
{

    [Authorize(Roles = "مدير")]
    public class RolesController : Controller
    {
        // GET: RolesController

        private readonly JobsBoardContext _Context;

        private readonly RepostryPattern<IdentityRole> _RoleRepostry;

        public RolesController(JobsBoardContext context , RepostryPattern<IdentityRole> role)
        {
            _Context = context;
            _RoleRepostry = role;

        }


        //this make all can reach this page
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var roles = await _RoleRepostry.List(); 
            return View(roles);
        }


        // GET: RolesController/Details/5
        public ActionResult Details(string id)
        {
            var role = _RoleRepostry.Find(id).Result;
            return View(role);
        }

        // GET: RolesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole Role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _RoleRepostry.Create(Role);
               
                    
                 


                }
            }
            catch
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        // GET: RolesController/Edit/5
        public ActionResult Edit(string id)
        {

            var role = _Context.Roles
                               .Where(r => r.Id == id)
                               .Select(r => new RoleViewModel
                               {
                                   Id = r.Id,
                                   RoleName = r.Name
                               })
                               .SingleOrDefault();

            if (role == null)
            {
                return NotFound(); // Handle case where the role does not exist
            }            //return Content("Id " + role.Name);
            return View(role);
        }

        // POST: RolesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoleViewModel model)
        {
            IdentityRole? role = _Context.Roles.FirstOrDefault(x => x.Id == model.Id);

            if (role == null)
            {
                throw new Exception("Role not found"); 
            }
            try
            {
                if (ModelState.IsValid)
                {
                    role.Name = model.RoleName;

                    _RoleRepostry.Update(model.Id, role).Wait();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Concurrency error: The role was updated or deleted by another user.");
                return View(model);
            }
            return View(model);
        }
        // GET: RolesController/Delete/5
        public ActionResult Delete(RoleViewModel model)
        {
            var Role = _Context.Roles.FirstOrDefault(i => i.Id == model.Id);

         
            return View(Role);
        }

        //POST: RolesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult ConfirmDelete(IdentityRole model)
        {
            try
            {
                  _RoleRepostry.Delete(model.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle the concurrency exception
                return Content($"Concurrency error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }

            return RedirectToAction(nameof(Index));
        }


        public void SaveChanges()
        {
            _Context.SaveChanges();
        }
    }
}
