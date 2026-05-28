# 🗨️ Бубнилка — Веб-мессенджер

**Бубнилка** — это современный веб-мессенджер с дизайном в бирюзовых тонах, похожий на Telegram. Поддерживает регистрацию/вход по email, чаты 1-на-1, сообщения в реальном времени через SignalR и отображение онлайн-статуса пользователей.

## 🎨 Цветовая схема

- **Основной цвет**: #00B4B4 (бирюзовый)
- **Тёмный фон**: #1A2B2B
- **Светлый фон**: #E0F7F7
- **Акцент**: #00D4D4

## 🚀 Технологии

- **Backend**: ASP.NET Core 8, SignalR, Entity Framework Core, SQLite
- **Frontend**: Blazor WebAssembly
- **Аутентификация**: JWT токены
- **Хранение данных**: BCrypt для паролей, LocalStorage на клиенте

## 📁 Структура проекта

```
Bubnilka/
├── Bubnilka.sln                    # Решение Visual Studio
├── Bubnilka.Shared/                # Общие модели и DTO
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Chat.cs
│   │   └── Message.cs
│   └── DTOs/
│       ├── RegisterRequest.cs
│       ├── LoginRequest.cs
│       ├── AuthResponse.cs
│       └── CreateChatRequest.cs
├── Bubnilka.Server/                # Серверная часть (API + SignalR)
│   ├── Program.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── ChatsController.cs
│   ├── Hubs/
│   │   └── ChatHub.cs
│   ├── Services/
│   │   └── TokenService.cs
│   └── appsettings.json
└── Bubnilka.Client/                # Клиентская часть (Blazor WASM)
    ├── Program.cs
    ├── App.razor
    ├── Shared/
    │   ├── MainLayout.razor
    │   └── LoginLayout.razor
    ├── Pages/
    │   ├── Login.razor
    │   ├── Register.razor
    │   └── Chats.razor
    ├── Services/
    │   ├── AuthService.cs
    │   ├── ChatService.cs
    │   └── SignalRService.cs
    └── wwwroot/css/app.css
```

## 🔧 Установка и запуск

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Шаг 1: Клонирование репозитория

```bash
git clone https://github.com/ВАШ_ЛОГИН/Bubnilka.git
cd Bubnilka
```

### Шаг 2: Создание миграции базы данных

```bash
cd Bubnilka.Server
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ..
```

### Шаг 3: Запуск сервера

Откройте первый терминал:

```bash
dotnet run --project Bubnilka.Server
```

Сервер запустится на `https://localhost:7001`

### Шаг 4: Запуск клиента

Откройте второй терминал:

```bash
dotnet run --project Bubnilka.Client
```

Клиент запустится на `https://localhost:5001`

### Шаг 5: Открытие в браузере

Перейдите по адресу: **https://localhost:5001**

## 📝 Функционал

### Аутентификация
- ✅ Регистрация нового пользователя (email + пароль + имя)
- ✅ Вход по email и паролю
- ✅ JWT-токены для авторизации
- ✅ Хранение токена в LocalStorage

### Чаты
- ✅ Создание чата 1-на-1 с любым пользователем
- ✅ Список всех чатов с последним сообщением
- ✅ Отображение онлайн-статуса собеседника
- ✅ История сообщений

### Сообщения
- ✅ Отправка сообщений в реальном времени (SignalR)
- ✅ Получение сообщений мгновенно
- ✅ Индикация прочтения
- ✅ Время отправки

## 🌐 Публикация на GitHub

```bash
# Перейти в папку с проектом
cd Bubnilka

# Инициализировать Git-репозиторий
git init

# Добавить .gitignore
echo "bin/
obj/
.qwen/
*.db
*.sqlite3" > .gitignore

# Добавить все файлы в Git
git add .

# Сделать первый коммит
git commit -m "Initial commit: Бубнилка мессенджер"

# Подключить удалённый репозиторий (ЗАМЕНИ НА СВОЙ)
git remote add origin https://github.com/ВАШ_ЛОГИН/Bubnilka.git

# Отправить на GitHub
git push -u origin main
```

## 🔒 Безопасность

- Пароли хешируются с помощью BCrypt
- JWT-токены с сроком действия 7 дней
- HTTPS по умолчанию
- CORS настроен для локальной разработки

## 🤝 Вклад

Проект открыт для улучшений! Feel free to fork и создавать pull requests.

## 📄 Лицензия

MIT License — используйте свободно!

---

**Разработано с 💚 для сообщества .NET**
