using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sun.Models.ViewModels;
using Sun.Repository;
using System.Configuration;
using System.Security.Claims;

namespace Sun.Controllers;

public class ProfileController : Controller
{
    private readonly ProfileRepository _profileRepository;

    public ProfileController(IConfiguration configuration)
    {
        _profileRepository = new ProfileRepository(configuration);
    }
    // Страница профиля (асинхронно)
    public async Task<IActionResult> Index()
    {
        // Получаем текущего пользователя из контекста
        var username = User.Identity.Name;

        // Асинхронно получаем данные пользователя из базы данных
        var user = await _profileRepository.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Заполняем модель профиля
        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
        };

        return View(model);
    }

    // Страница редактирования профиля (асинхронно)
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var username = User.Identity.Name;
        var user = await _profileRepository.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
        };

        return View(model);
    }

    // Обработка обновления профиля (асинхронно)
    [HttpPost]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var username = User.Identity.Name;
            var user = await _profileRepository.GetUserByUsernameAsync(username);

            if (user != null)
            {
                // Обновляем данные пользователя
                user.Email = model.Email;
                user.Role = model.Role;

                // Если необходимо, обновите и другие данные пользователя

                // Сохраняем изменения в базе данных (асинхронно)
                await _profileRepository.UpdateUserAsync(user);

                // Обновляем данные в сессии
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);

                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "User not found");
        }

        return View(model);
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
        var user = await _profileRepository.GetUserByUsernameAsync(username);

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
    [ActionName("DeleteUser")]
    public async Task<IActionResult> DeleteConfirmed()
    {
        var username = User.Identity.Name;

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        // Удаляем пользователя из базы данных
        var result = await _profileRepository.DeleteUserAsync(username);

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
