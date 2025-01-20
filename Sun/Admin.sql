CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Role VARCHAR(50),
    CONSTRAINT Unique_UserName UNIQUE(UserName)
);

-- Таблица ролей
CREATE TABLE Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
);

-- Таблица связи пользователей с ролями (связь многие ко многим)
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);
