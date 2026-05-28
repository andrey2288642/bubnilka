# Исправления для проекта Бубнилка

## Ошибки в файлах клиента (для Windows)

### 1. App.razor - удалить компонент RedirectToLogin
Заменить всё содержимое на:
```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
            <NotAuthorized>
                <LayoutView Layout="@typeof(LoginLayout)">
                    <p>Требуется вход</p>
                </LayoutView>
            </NotAuthorized>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Страница не найдена.</p>
        </LayoutView>
    </NotFound>
</Router>
```

### 2. Login.razor - добавить using директивы
После @inject добавить:
```
@using Bubnilka.Shared.DTOs
@using System.ComponentModel.DataAnnotations
```

### 3. Register.razor - добавить using директивы  
После @inject добавить:
```
@using Bubnilka.Shared.DTOs
@using System.ComponentModel.DataAnnotations
```

### 4. Chats.razor - исправить синтаксис и добавить using
После @inject добавить:
```
@using Bubnilka.Shared.DTOs
@using Microsoft.AspNetCore.Components.Web
```

Исправить строку 78: заменить `"own" : "other')"` на `"own" : "other")`

### 5. MainLayout.razor - добавить using
После @inherits добавить:
```
@using Bubnilka.Client.Services
```

## Команды для запуска после исправлений:

```bash
cd Bubnilka.Server
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Во втором терминале:
```bash
cd Bubnilka.Client
dotnet run
```
