using Microsoft.AspNetCore.Mvc;
using Sun.Repository;

namespace Sun.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sun.Models;
using Sun.Models.ViewModels;
using System.Threading.Tasks;

public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IConfiguration configuration)
    {
        _userRepository = new UserRepository(configuration);
    }

    //public UserController(IUserRepository userRepository)
    //{
    //    _userRepository = userRepository;
    //}

    // GET: /User/
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return View(users);
    }

    // GET: /User/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: /User/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            await _userRepository.AddUserAsync(user);
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: /User/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: /User/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _userRepository.UpdateUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // Страница для подтверждения удаления пользователя
    public async Task<IActionResult> Delete()
    {
        var username = User.Identity.Name;

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        // Получаем пользователя для подтверждения его удаления
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Отправляем данные пользователя в представление для подтверждения
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
        };

        return View(model);
    }

    // Подтверждение удаления пользователя (асинхронно)
    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed()
    {
        var username = User.Identity.Name;

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        // Удаляем пользователя из базы данных
        var result = await _userRepository.DeleteUserAsync(username);

        if (result > 0)
        {
            // После удаления пользователя, выходим из системы
            await HttpContext.SignOutAsync();

            // Перенаправляем на страницу авторизации
            return RedirectToAction("Login", "Account");
        }

        // Если не удалось удалить, возвращаем ошибку
        ModelState.AddModelError("", "Error deleting user.");
        return View();
    }

}

