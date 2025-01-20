using Microsoft.AspNetCore.Mvc;
using Sun.Models;
using Sun.Repository;

namespace Sun.Controllers;

public class RoleController : Controller
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IConfiguration configuration)
    {
        _roleRepository = new RoleRepository(configuration);
    }

    //public RoleController(IRoleRepository roleRepository)
    //{
    //    _roleRepository = roleRepository;
    //}

    // GET: /Role/Index
    public async Task<IActionResult> Index()
    {
        var roles = await _roleRepository.GetAllRolesAsync();
        return View(roles);
    }

    // GET: /Role/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Role/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Role role)
    {
        if (ModelState.IsValid)
        {
            await _roleRepository.AddRoleAsync(role);
            return RedirectToAction(nameof(Index));
        }
        return View(role);
    }

    // GET: /Role/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: /Role/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Role role)
    {
        if (id != role.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _roleRepository.UpdateRoleAsync(role);
            return RedirectToAction(nameof(Index));
        }

        return View(role);
    }

    // GET: /Role/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: /Role/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _roleRepository.DeleteRoleAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // POST: /Role/AssignRoleToUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
    {
        await _roleRepository.AssignRoleToUserAsync(userId, roleId);
        return RedirectToAction("Index", "User");  // Можно перенаправить на страницу пользователей
    }

    // POST: /Role/RemoveRoleFromUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
    {
        await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
        return RedirectToAction("Index", "User");  // Можно перенаправить на страницу пользователей
    }
}
