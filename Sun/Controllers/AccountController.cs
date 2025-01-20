using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Sun.Models;
using Sun.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI.Services;
using Sun.Models.ViewModels;

namespace Sun.Controllers;

public class AccountController : Controller
{
    private readonly AccountRepository _accountRepository;
    private readonly IEmailSender _emailSender;

    public AccountController(IConfiguration configuration, IEmailSender emailSender)
    {
        _accountRepository = new AccountRepository(configuration);
        _emailSender = emailSender;
    }

    //public AccountController(IUserRepository userRepository, IEmailSender emailSender)
    //{
    //    _userRepository = userRepository;
    //    _emailSender = emailSender;
    //}

    public IActionResult Register()
    {
        // Проверка, авторизован ли пользователь
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home"); // Перенаправление на главную страницу
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Хеширование пароля
            using (var sha256 = SHA256.Create())
            {
                var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

                var user = new User
                {
                    UserName = model.UserName,
                    PasswordHash = passwordHash,
                    Email = model.Email,
                    Role = "User"
                };

                var result = await _accountRepository.RegisterUserAsync(user);

                if (result > 0)
                    return RedirectToAction("Login");
                else
                    ModelState.AddModelError("", "Error during registration.");
            }
        }
        return View(model);
    }

    public IActionResult Login()
    {
        // Проверка, авторизован ли пользователь
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home"); // Перенаправление на главную страницу
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _accountRepository.LoginUserByUsernameAsync(model.UserName);

            if (user != null)
            {
                using (var sha256 = SHA256.Create())
                {
                    var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

                    if (user.PasswordHash == passwordHash)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        var identity = new ClaimsIdentity(claims, "login");
                        var principal = new ClaimsPrincipal(identity);
                        // Проверяем, выбрал ли пользователь опцию "Запомнить меня"
                        var isPersistent = model.RememberMe;

                        // Выполняем вход с учетом параметра "Запомнить меня"
                        await HttpContext.SignInAsync(principal, new AuthenticationProperties
                        {
                            IsPersistent = isPersistent,  // Запоминаем пользователя, если выбрано
                            ExpiresUtc = isPersistent ? DateTime.UtcNow.AddDays(30) : (DateTime?)null  // Устанавливаем срок действия куки (30 дней, если выбрано)
                        });

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid username or password.");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }

    // Страница для запроса сброса пароля (ввод email)
    public IActionResult RequestResetPassword()
    {
        return View();
    }

    // Асинхронное действие для обработки запроса на сброс пароля
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                // Генерация токена для сброса пароля (например, через GUID или в вашем случае через ADO.NET)
                var resetToken = Guid.NewGuid().ToString();

                // Сохранить токен в базе данных или в другом месте для последующего верифицирования
                await _accountRepository.SavePasswordResetTokenAsync(user.UserName, resetToken);

                // Создание ссылки для сброса пароля
                var resetUrl = Url.Action("ResetPassword", "Account", new { token = resetToken }, protocol: Request.Scheme);

                // Отправка письма с ссылкой для сброса пароля
                var emailSubject = "Password Reset Request";
                var emailBody = $"Please click the following link to reset your password: <a href=\"{resetUrl}\">Reset Password</a>";
                await _emailSender.SendEmailAsync(model.Email, emailSubject, emailBody);

                // Уведомление пользователя о том, что письмо отправлено
                ViewBag.Message = "An email with instructions to reset your password has been sent.";
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }
        }

        return View(model);
    }

    // Страница для сброса пароля
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("RequestResetPassword");
        }

        return View(new ResetPasswordViewModel { Token = token });
    }

    // Асинхронное действие для обработки нового пароля
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _accountRepository.GetUserByUsernameAsync(model.Token);  // Пример: Используем токен для поиска пользователя
            if (user != null && model.NewPassword == model.ConfirmPassword)
            {
                // Обновление пароля пользователя
                await _accountRepository.UpdateUserPasswordAsync(user.UserName, model.NewPassword);

                // Удаляем токен, так как пароль сброшен
                await _accountRepository.DeletePasswordResetTokenAsync(user.UserName);

                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Invalid token or passwords do not match.");
        }

        return View(model);
    }

    // Страница AccessDenied
    public IActionResult AccessDenied()
    {
        return View();  // Отображаем страницу отказа в доступе
    }
}