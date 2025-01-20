1. ### Add 404 page

2. ### Change hesh password
```cs
    // Хэширование пароля
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    // Проверка пароля
    private bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }
    ```