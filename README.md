# Sabatex.RadzenBlazor

> **A comprehensive solution for building modern web applications using the WebSite (SSR) + WebApp (WASM) architecture with Radzen Blazor components.**

[![NuGet](https://img.shields.io/nuget/v/Sabatex.RadzenBlazor.svg)](https://www.nuget.org/packages/Sabatex.RadzenBlazor)
[![GitHub](https://img.shields.io/github/license/sabatex/Sabatex.RadzenBlazor)](https://github.com/sabatex/Sabatex.RadzenBlazor/blob/master/LICENSE)

---

## 📖 What is Sabatex.RadzenBlazor?

**Sabatex.RadzenBlazor** is NOT just a component library — it's a complete architectural solution for building Blazor applications that combine:

- **WebSite (SSR)** — Fast server-side rendered pages for public content, SEO, and marketing (without JavaScript/WASM).
- **WebApp (WASM)** — Interactive client-side applications for complex business logic, offline support, and PWA.

**Key features:**
- ✅ Extends [Radzen.Blazor](https://blazor.radzen.com/) with CRUD data grid, API adapter, and utilities.
- ✅ Universal `BaseController` for automatic REST API generation.
- ✅ Built-in ASP.NET Core Identity integration (Google/Microsoft OAuth, Email sender).
- ✅ Minimal setup — plug-and-play NuGet packages.

---

## 🚀 Quick Start

### Prerequisites

- **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download))
- **Visual Studio 2022+** or **VS Code** with C# extension
- **SQL Server** or **PostgreSQL** (for ASP.NET Core Identity)

**Required dependencies (if not already installed):**
---

### 1. Installation

#### Install NuGet packages:

**For WASM projects:**

````````markdown
dotnet add package Sabatex.RadzenBlazor --prerelease
````````

**For Server projects:**
````````markdown
dotnet add package Sabatex.RadzenBlazor.Server --prerelease
````````

---

### 2. Server Setup (Program.cs)

#### 2.1. Add DbContext and Identity

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<AppDbContext>();
```

#### 2.2. Configure appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_db;User Id=your_user;Password=your_password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

#### 2.3. Configure services in `Program.cs`:

```csharp
builder.Services.AddSabatexRadzenBlazor();
```

#### 2.4. Configure HTTP request pipeline in `Startup.cs`:

```csharp
app.UseSabatexRadzenBlazor();
```

---

### 3. Update database

Run migrations to update the database schema:

```bash
dotnet ef database update
```

---

## 📚 Documentation

- **[Getting Started](https://github.com/sabatex/Sabatex.RadzenBlazor/wiki/Getting-Started)**
- **[API Reference](https://github.com/sabatex/Sabatex.RadzenBlazor/wiki/API-Reference)**
- **[Changelog](https://github.com/sabatex/Sabatex.RadzenBlazor/wiki/Changelog)**

---

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🔗 Links

- **GitHub:** [https://github.com/sabatex/Sabatex.RadzenBlazor](https://github.com/sabatex/Sabatex.RadzenBlazor)
- **NuGet:** [https://www.nuget.org/packages/Sabatex.RadzenBlazor](https://www.nuget.org/packages/Sabatex.RadzenBlazor)
- **Radzen Blazor (base library):** [https://blazor.radzen.com/](https://blazor.radzen.com/)
- **Demo Application:** [Demo/SabatexBlazorDemo](Demo/SabatexBlazorDemo)

---

## ⚠️ Status

> **Note:** ASP.NET Core Identity UI pages (Login, Register, etc.) are currently under development. 
> Manual Identity configuration is required (as shown in the Quick Start). 
> Check the [Demo project](Demo/SabatexBlazorDemo/Program.cs) for the latest implementation.

---

**Happy coding with Sabatex.RadzenBlazor! 🚀**


