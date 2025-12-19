# Sabatex.RadzenBlazor — Project Context for Copilot

> **Мета:** Технічний опис проєкту для ефективної роботи з GitHub Copilot.  
> **Аудиторія:** Copilot AI, розробники бібліотеки.  
> **Оновлено:** грудня 19, 2025

---

## 🎯 Мета проєкту

**Це НЕ просто бібліотека компонентів — це комплексне рішення** для побудови застосунків за концепцією **WebSite (SSR) + WebApp (WASM)**.

### Що це означає:
- **WebSite (SSR)** — швидкі серверні сторінки (як у Razor Pages) для публічного контенту, SEO, швидкого завантаження.
- **WebApp (WASM)** — інтерактивні клієнтські застосунки для складної логіки, офлайн роботи, PWA.
- **Одне рішення** — обидва підходи в одному проєкті без необхідності створювати окремі застосунки.

### Розширення Radzen.Blazor:
**Sabatex.RadzenBlazor** (WASM) розширює функціонал [Radzen.Blazor](https://blazor.radzen.com/) для WebAssembly сценаріїв.  
**Sabatex.RadzenBlazor.Server** (SSR) — **НЕ використовує Radzen.Blazor**, це власні SSR компоненти та сторінки для серверної частини.

---

## 🏗️ Архітектура проєкту

### ⚠️ КРИТИЧНО ВАЖЛИВО: Дві незалежні бібліотеки, які працюють разом

**`Sabatex.RadzenBlazor`** (WASM) та **`Sabatex.RadzenBlazor.Server`** (SSR) — **технічно незалежні** бібліотеки:
- **НЕ мають `<ProjectReference>` одна на одну** — не бачать одна одну на рівні коду.
- **Зв'язані тільки через `Sabatex.Core`** — спільні інтерфейси, базові класи, утиліти.
- **Функціонально працюють разом** — не можна використовувати одну без іншої у реальному проєкті.

**Чому потрібні разом:**
- `Sabatex.RadzenBlazor.Server` надає компоненти для монтування WASM клієнтів (`WasmClientRoute`), PWA headers (`SabatexPWAHeaderContent`), маршрутизацію.
- `Sabatex.RadzenBlazor` надає WASM компоненти, які монтуються через серверну частину.
- Без серверної частини WASM компоненти не можуть бути змонтовані в застосунку.

### Основні бібліотеки:

1. **Sabatex.RadzenBlazor** — Razor Class Library (RCL) для **WASM** компонентів (клієнтська сторона).
   - **Розширює Radzen.Blazor** для WebAssembly сценаріїв.
   - Містить компоненти, що працюють на клієнті (у браузері).
   - **Залежить від:** `Radzen.Blazor` + `Sabatex.Core`.
   - **НЕ має ProjectReference** на `Sabatex.RadzenBlazor.Server`.

2. **Sabatex.RadzenBlazor.Server** — Razor Class Library для **Server-Side** (SSR) компонентів.
   - **НЕ використовує Radzen.Blazor** — це власні SSR компоненти та сторінки.
   - Містить компоненти/сторінки, що **завжди рендеряться тільки на сервері**.
   - Містить утиліти для SSR, PWA, маршрутизації, монтування WASM клієнтів.
   - **Залежить від:** `Sabatex.Core` + ASP.NET Core Identity, EF Core, MailKit, тощо.
   - **НЕ має ProjectReference** на `Sabatex.RadzenBlazor`.

3. **Sabatex.Core** — Спільна бібліотека з інтерфейсами, базовими класами, утилітами.
   - **Єдиний технічний зв'язок** між WASM та Server бібліотеками.
   - Містить спільні інтерфейси, базові класи, енуми, константи.

### Демо-проєкти:
4. **Demo\SabatexBlazorDemo** — Демо-застосунок (SSR + WASM host).
   - Використовує **обидві** бібліотеки: `Sabatex.RadzenBlazor` + `Sabatex.RadzenBlazor.Server`.
5. **Demo\SabatexBlazorDemo.WASMClientA** — WASM клієнт без аутентифікації (RCL).
6. **Demo\SabatexBlazorDemo.WASMClientB** — WASM клієнт з аутентифікацією (RCL).
7. **Demo\SabatexBlazorDemo.Shared** — Спільні моделі та API контракти.
8. **Demo\SabatexBlazorDemo.WASMApp** — Точка входу для WASМ (один застосунок типу `Microsoft.NET.Sdk.BlazorWebAssembly`).

### Технології:
- **.NET 10**
- **Blazor WebAssembly + SSR (Server-Side Rendering)**
- **Radzen.Blazor** (базова UI бібліотека) — **тільки для WASM** (`Sabatex.RadzenBlazor`)
- **C# 14.0**
- **Entity Framework Core** (для Sabatex.RadzenBlazor.Server)
- **ASP.NET Core Identity** (для Sabatex.RadzenBlazor.Server)
- **Sabatex.Core** (спільні інтерфейси та утиліти)

---

## 📦 Структура бібліотеки `Sabatex.RadzenBlazor` (WASM)

**Призначення:** Компоненти та утиліти для Blazor WebAssembly (клієнтська сторона) — **WebApp**.

**Залежності:**
- `Radzen.Blazor` ✅ (розширює Radzen компоненти)
- `Sabatex.Core` ✅ (спільні інтерфейси)
- **НЕ залежить від** `Sabatex.RadzenBlazor.Server` ❌ (технічно незалежні)

### Компоненти (.razor):
- **`WASMLoadProgress.razor`** — Індикатор завантаження WASM (з CSS `WASMLoadProgress.razor.css`).
  - Показується під час завантаження WASM runtime.
  - CSS: використовує `--blazor-load-percentage` для прогресу.
- **`SabatexPageHeader.razor`** — Заголовок сторінки з підпискою на `SabatexBlazorAppState.PageHeader`.
- _(Додати інші компоненти, коли будуть додані)_

### Сервіси (.cs):
- **`SabatexBlazorAppState.cs`** — Централізований стан застосунку (поточна реалізація):
  - `PageHeader` (string) + `OnHeaderChange` (event Action<string>)
  - `ContentAvaliableHeight` (double) + `OnContentAvaliableHeightChange` (event Action<double>)
  - **Видимість:** `public` (планується зробити `internal`).
  - **Lifetime:** Scoped (один екземпляр на WASM сесію).
  - **Проблема:** Потенційні memory leaks при неправильному використанні підписок.

### Утиліти (планується):
- `ServiceCollectionExtensions.cs` — Extension method для реєстрації DI (`AddSabatexRadzenBlazor()`).
- `SabatexComponentBase.cs` — Базовий клас для компонентів бібліотеки з автоматичною відпискою.
- `SabatexBlazorAppStateExtensions.cs` — Extension methods для безпечних підписок (internal).
- `ISabatexAppService.cs` + `SabatexAppService.cs` — Публічний API для зміни стану (якщо потрібно).

---

## 📦 Структура бібліотеки `Sabatex.RadzenBlazor.Server` (SSR)

**Призначення:** Компоненти та сторінки для Blazor Server-Side Rendering — **WebSite**.

**⚠️ ВАЖЛИВО:** **НЕ використовує Radzen.Blazor** — це власні SSR компоненти/сторінки, що завжди рендеряться тільки на сервері.

**Залежності:**
- `Sabatex.Core` ✅ (спільні інтерфейси)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` ✅ — Identity для аутентифікації.
- `Microsoft.AspNetCore.Authentication.Google` ✅ — OAuth Google.
- `Microsoft.AspNetCore.Authentication.MicrosoftAccount` ✅ — OAuth Microsoft.
- `MailKit` ✅ — Відправка email.
- `Microsoft.EntityFrameworkCore.DynamicLinq` ✅ — Динамічні LINQ запити.
- `System.CommandLine` ✅ — CLI утиліти.
- **НЕ залежить від** `Radzen.Blazor` ❌.
- **НЕ залежить від** `Sabatex.RadzenBlazor` ❌ (технічно незалежні).

### Компоненти (існуючі, згадані в демо):
- **`SabatexPWAHeaderContent`** — Інжектує PWA headers для окремих WASM клієнтів.
- **`WasmClientRoute`** — Компонент для монтування WASМ RCL клієнтів (критично для роботи WASМ).
- **`SabatexRoutes`** — Роутинг helper для SSR/WASM.
- **`ResourcePreloader`** — Прелоадер ресурсів (оптимізація завантаження).
- **`ImportMap`** — Import map helper (для модульних JavaScript залежностей).
- **`RadzenTheme`** — Wrapper для Radzen тем (для WASM клієнтів, не для SSR сторінок).

### Сторінки Identity (в процесі перенесення):
- **⚠️ В РОЗРОБЦІ:** Повний перенос сторінок ASP.NET Core Identity на сервер ще не завершений.
- **Можливі конфлікти** з існуючими реалізаціями Identity в проєктах.

**Статус:**
- **В розробці** — буде вирішено пізніше.
- **Не блокує** поточну роботу з `WASMLoadProgress` та `SabatexBlazorAppState`.

### Утиліти (існуючі):
- **Extension method `HttpContext.IsRouteWASMClient()`** — Визначає, чи запит для WASM клієнта.
- _(Додати інші утиліти після аналізу)_

### Що планується:
- `ServiceCollectionExtensions.cs` — Extension method для реєстрації DI (`AddSabatexRadzenBlazorServer()`).

---

## 🔧 Ключові принципи архітектури

### 1. Концепція WebSite + WebApp (КРИТИЧНО)
- **WebSite (SSR)** — швидкі серверні сторінки для публічного контенту, SEO, маркетингу.
  - Рендериться на сервері → чистий HTML → без JavaScript/WASM.
  - Компоненти з `Sabatex.RadzenBlazor.Server`.
- **WebApp (WASM)** — інтерактивні клієнтські застосунки для бізнес-логіки, складних UI, офлайн роботи.
  - Рендериться на клієнті → повна інтерактивність → PWA.
  - Компоненти з `Sabatex.RadzenBlazor` (розширення Radzen.Blazor).
- **Одне рішення:** Обидва підходи в одному ASP.NET Core проєкті без необхідності створювати окремі застосунки.

### 2. Дві технічно незалежні бібліотеки, функціонально працюють разом
- **`Sabatex.RadzenBlazor`** (WASM) — клієнтські компоненти, розширення Radzen.Blazor.
  - **НЕ має ProjectReference** на `Sabatex.RadzenBlazor.Server`.
- **`Sabatex.RadzenBlazor.Server`** (SSR) — серверні компоненти/сторінки, БЕЗ Radzen.Blazor.
  - **НЕ має ProjectReference** на `Sabatex.RadzenBlazor`.
- **`Sabatex.Core`** — спільні інтерфейси, базові класи, утиліти — єдиний технічний зв'язок.
- **Функціонально не можна використовувати одну без іншої** — серверна частина потрібна для монтування WASM клієнтів.

### 3. Мінімізація змін для впровадження (поточна мета)
- **Проблема:** Щоразу при створенні нового проєкту потрібно копіювати відомі напрацювання → багато помилок.
- **Рішення:** Винести всі відомі напрацювання в бібліотеку (`Sabatex.RadzenBlazor` + `Sabatex.RadzenBlazor.Server`) → мінімальна кількість змін для старту нового проєкту.
- **Приклад:** Просто підключити NuGet пакети, викликати `AddSabatexRadzenBlazor()` + `AddSabatexRadzenBlazorServer()` → готово.

### 4. Один WASM застосунок (`WASMApp`), кілька віртуальних клієнтів (RCL) — WebApp
- `WASMApp` (`Microsoft.NET.Sdk.BlazorWebAssembly`) — точка входу.
- `WASMClientA` і `WASMClientB` — Razor Class Libraries (RCL), не окремі виконувані проєкти.
- Маршрутизація (Router) — всередині RCL клієнтів (`ClientARoutes`, `ClientBRoutes`).
- PWA — окрема реєстрація per client через `SabatexPWAHeaderContent`.

### 5. Обмеження використання `SabatexBlazorAppState`
- **Мета:** Захистити від витоків пам'яті через неправильні підписки.
- **Стратегія:** Зробити `internal`, використовувати тільки всередині компонентів бібліотеки.
- **Публічний API:** Через `ISabatexAppService` (якщо розробникам потрібно змінювати стан).
- **Підписки:** Тільки через extension methods з автоматичною відпискою.

### 6. `App.razor` — статичний на сервері
- Не має lifecycle на клієнті.
- `OnAfterRenderAsync` у `App.razor` **не спрацює** для WASM.
- Контроль завершення завантаження — через `ClientARoutes.OnAfterRenderAsync` або JS callback.

---

## 🚀 Демо-застосунок (`SabatexBlazorDemo`)

### Структура:
- **`App.razor`** — Root host (статичний HTML на сервері).
  - Містить `<WASMLoadProgress />` для WASМ клієнтів (з `Sabatex.RadzenBlazor`).
  - Містить `<WasmClientRoute PrefixRoute="/SabatexBlazorDemo_WASMClientA">` для монтування RCL клієнта A (з `Sabatex.RadzenBlazor.Server`).
  - Містить `<WasmClientRoute PrefixRoute="/SabatexBlazorDemo_WASMClientB">` для монтування RCL клієнта B (з `Sabatex.RadzenBlazor.Server`).
  - Вибір SSR vs WASM через `HttpContext.IsRouteWASMClient()`.
- **`SSRRoutes.razor`** — Серверний Router для SSR сторінок.
  - Сканує `typeof(Program).Assembly` + `typeof(Sabatex.RadzenBlazor._Imports).Assembly`.
- **`Pages\Home.razor`** — SSR сторінка з описом проєкту, архітектури (SVG діаграма), режимів рендерингу.

### WASM клієнти (віртуальні, RCL) — WebApp:
- **WASMClientA (RCL):**
  - `ClientARoutes.razor` — клієнтський Router (виконується на клієнті).
  - `Layout\MainLayout.razor` — Radzen Layout (RadzenComponents, RadzenLayout, RadzenHeader, RadzenSidebar, RadzenBody).
  - `Pages\...` — сторінки без аутентифікації (Home, EnumDropDownDemo, Weather/List).
  - **Використовує:** `Sabatex.RadzenBlazor` + `Radzen.Blazor`.
- **WASMClientB (RCL):**
  - `ClientBRoutes.razor` — клієнтський Router.
  - `Pages\Auth.razor` — сторінка з `[Authorize]`.
  - `Pages\People\...` — CRUD для People (з аутентифікацією).
  - **PWA:** Окрема реєстрація service-worker через `SabatexPWAHeaderContent`.
  - **Використовує:** `Sabatex.RadzenBlazor` + `Radzen.Blazor` + authentication.

### WASMApp:
- **Один застосунок** типу `Microsoft.NET.Sdk.BlazorWebAssembly`.
- **Ініціалізує** DI, завантажує RCL клієнти (ClientA, ClientB), запускає `blazor.web.js`.
- **Без маршрутизації та лайоута** — маршрутизація в RCL клієнтах.
- **Посилання на:** `WASMClientA.csproj`, `WASMClientB.csproj`.

---

## 🐛 Відомі проблеми

### 1. `WASMLoadProgress` не ховається після завантаження при перезавантаженні (F5)
**Проблема:**
- `OnAfterRenderAsync` у `App.razor` не спрацьовує (App.razor — статичний на сервері).
- `OnAfterRenderAsync` у `WASMLoadProgress` не викликається стабільно при перезавантаженні.

**Рішення (планується):**
- **Варіант 1:** Додати `IsWASMLoadingComplete` до `SabatexBlazorAppState`, викликати `AppState.CompleteWASMLoading()` з `ClientARoutes.OnAfterRenderAsync`.
- **Варіант 2:** JS callback `Blazor.start().then(() => { hideLoader(); })` у `App.razor`.
- **Комбінований:** Використовувати обидва методи (C# lifecycle + JS fallback).

### 2. Memory leaks через підписки на події
**Проблема:**
- Компоненти підписуються на `AppState.OnHeaderChange`, але не відписуються при `Dispose()`.
- При навігації між сторінками (`Home` → `People`) старі підписки залишаються активними → витік пам'яті.

**Рішення (планується):**
- Створити `SabatexComponentBase` з автоматичною відпискою (`RegisterSubscription` + `Dispose`).
- Створити internal extension methods `SubscribeToPageHeaderChange(...)` тощо.
- Зробити `SabatexBlazorAppState` `internal`.
- Створити публічний `ISabatexAppService` для зміни стану (якщо потрібно).

### 3. Перенос Identity сторінок на сервер не завершений
**Проблема:**
- Повний перенос сторінок ASP.NET Core Identity (Login, Register, ForgotPassword, тощо) на `Sabatex.RadzenBlazor.Server` ще не завершений.
- Можливі конфлікти з існуючими реалізаціями Identity в проєктах.

**Статус:**
- **В розробці** — буде вирішено пізніше.
- **Не блокує** поточну роботу з `WASMLoadProgress` та `SabatexBlazorAppState`.

---

## 📝 TODO / Roadmap

### Sabatex.RadzenBlazor (WASM):
- [ ] Додати `IsWASMLoadingComplete` (bool) + `OnWASMLoadingStateChange` (event) до `SabatexBlazorAppState`.
- [ ] Додати метод `CompleteWASMLoading()` до `SabatexBlazorAppState`.
- [ ] Створити `SabatexComponentBase` для базових компонентів бібліотеки.
- [ ] Створити internal extension methods для безпечних підписок (`SabatexBlazorAppStateExtensions.cs`).
- [ ] Зробити `SabatexBlazorAppState` `internal`.
- [ ] Створити `ISabatexAppService` (публічний API) + `SabatexAppService` (internal реалізація).
- [ ] Оновити `WASMLoadProgress.razor` з безпечною підпискою (успадкувати `SabatexComponentBase`).
- [ ] Додати JS fallback у `App.razor` для `WASMLoadProgress` (`Blazor.start().then(...)`).
- [ ] Створити `ServiceCollectionExtensions.cs` для реєстрації DI (`AddSabatexRadzenBlazor()`).

### Sabatex.RadzenBlazor.Server (SSR):
- [ ] Документувати існуючі компоненти (`SabatexPWAHeaderContent`, `WasmClientRoute`, `ResourcePreloader`, `ImportMap`, `RadzenTheme`).
- [ ] Створити `ServiceCollectionExtensions.cs` для реєстрації DI (`AddSabatexRadzenBlazorServer()`).
- [ ] Завершити перенос сторінок Identity на сервер (Login, Register, ForgotPassword, тощо).

### Sabatex.Core:
- [ ] Документувати спільні інтерфейси та базові класи.

---

## 📚 Корисні посилання

- **GitHub:** https://github.com/sabatex/Sabatex.RadzenBlazor
- **Radzen.Blazor (для WASM):** https://blazor.radzen.com/
- **Blazor Lifecycle:** https://learn.microsoft.com/aspnet/core/blazor/components/lifecycle
- **Blazor Render Modes:** https://learn.microsoft.com/aspnet/core/blazor/components/render-modes

---

## 🔍 Швидкі факти для Copilot

### ⚠️ КРИТИЧНО ВАЖЛИВО:
- **Це НЕ просто бібліотека компонентів** — це комплексне рішення для побудови застосунків за концепцією **WebSite (SSR) + WebApp (WASM)**.
- **Дві технічно незалежні бібліотеки** (не бачать одна одну на рівні коду), **функціонально працюють разом**.
- **`Sabatex.RadzenBlazor.Server` НЕ використовує Radzen.Blazor** — це власні SSR компоненти/сторінки.
- **Sabatex.Core** — єдиний технічний зв'язок між WASM та Server бібліотеками.
- **Мета зараз:** Мінімізувати зміни для впровадження — винести відомі напрацювання в бібліотеку.

### Що вже існує (НЕ пропонувати створити):
#### Sabatex.RadzenBlazor (WASM):
- `SabatexBlazorAppState.cs` ✅
- `WASMLoadProgress.razor` ✅
- `WASMLoadProgress.razor.css` ✅
- `SabatexPageHeader.razor` ✅

#### Sabatex.RadzenBlazor.Server (SSR):
- `SabatexPWAHeaderContent` ✅ (згадано в App.razor)
- `WasmClientRoute` ✅ (згадано в App.razor)
- `ResourcePreloader` ✅ (згадано в App.razor)
- `ImportMap` ✅ (згадано в App.razor)
- `RadzenTheme` ✅ (згадано в App.razor)
- Extension method `HttpContext.IsRouteWASMClient()` ✅

#### Демо:
- `App.razor` ✅
- `SSRRoutes.razor` ✅
- `Home.razor` (з SVG діаграмою архітектури) ✅
- `ClientARoutes.razor` (WASMClientA) ✅
- `ClientBRoutes.razor` (WASMClientB) ✅
- `MainLayout.razor` (WASMClientA, WASMClientB) ✅

### Що НЕ існує (можна створити):
#### Sabatex.RadzenBlazor (WASM):
- `ServiceCollectionExtensions.cs` ❌
- `SabatexComponentBase.cs` ❌
- `SabatexBlazorAppStateExtensions.cs` ❌
- `ISabatexAppService.cs` + `SabatexAppService.cs` ❌

#### Sabatex.RadzenBlazor.Server (SSR):
- `ServiceCollectionExtensions.cs` ❌ (ймовірно)

### Загальні правила:
- **Завжди перевіряти** наявність файлів перед створенням (запитувати у розробника).
- **Використовувати українську мову** для всіх відповідей.
- **Пріоритет:** Blazor WebAssembly + SSR (не Razor Pages, не MVC).
- **Не змінювати файли без явної інструкції** користувача; перед внесенням правок з'ясувати повні абсолютні шляхи до файлів.
- **Цільова платформа:** .NET 10.
- **Radzen.Blazor** — використовується **тільки** у `Sabatex.RadzenBlazor` (WASM), **НЕ** у `Sabatex.RadzenBlazor.Server`.
- **Sabatex.Core** — спільна бібліотека, єдиний технічний зв'язок між WASM та Server бібліотеками.

### ⌨️ Обробка помилок розкладки клавіатури:
**Проблема:** Користувач може випадково не перемкнути розкладку клавіатури при введенні тексту.

**Правило:** Якщо текст виглядає як **білиберда** (неправильна розкладка), спробувати транскодувати його:
- **Українська → Латиниця:** `фдв` → `add`, `туче` → `test`, `былыберду` → `білиберда`
- **Латиниця → Українська:** `add` → `фдв`, `test` → `туче`

**Приклад:**
- Користувач пише: `"давай зупинимось. Ти бачишь весь мый проект?"`
- Copilot розуміє: Користувач має на увазі `"давай зупинимось. Ти бачишь весь мій проєкт?"`
- Copilot **не коментує помилку**, а продовжує відповідати нормально українською.

**Ще приклад (латиниця → українська):**
- Користувач пише: `"ghbdtn, ghbv rkfdbfnehe"`
- Copilot розуміє: `"привіт, прийм клавіатуру"` → `"привіт, приймай клавіатуру"` (можливо "прийми")
- Copilot відповідає: "Розумію, продовжую роботу..."

**Карта транскодації українська ↔ латиниця (QWERTY):**

| Українська | Латиниця | Українська | Латиниця |
|------------|----------|------------|----------|
| й | q | н | y |
| ц | w | г | u |
| у | e | ш | i |
| к | r | щ | o |
| е | t | з | p |
| н | y | х | [ |
| г | u | ї | ] |
| ш | i | ф | a |
| щ | o | і | s |
| з | p | в | d |
| х | [ | а | f |
| ї | ] | п | g |
| ф | a | р | h |
| і | s | о | j |
| в | d | л | k |
| а | f | д | l |
| п | g | ж | ; |
| р | h | є | ' |
| о | j | я | z |
| л | k | ч | x |
| д | l | с | c |
| ж | ; | м | v |
| є | ' | и | b |
| я | z | т | n |
| ч | x | ь | m |
| с | c | б | , |
| м | v | ю | . |
| и | b | . | / |
| т | n | ґ | ` |
| ь | m | | |

**Дія Copilot:**
1. Якщо текст складається з **послідовності символів, яка не має сенсу** українською або англійською, спробувати транскодувати за таблицею.
2. Якщо після транскодації текст стає **осмисленим українським** або **англійським**, використати транскодовану версію.
3. **Не коментувати** помилку користувача, не писати "ви помилилися з розкладкою" — просто продовжити роботу з транскодованим текстом.
4. Якщо транскодація **не допомагає** (текст залишається білибердою) — **попросити користувача уточнити** запит.

**Приклад обробки:**
- Користувач: `"былыберду"` (українська розкладка, хоче написати англійською `"білиберда"`)
- Copilot транскодує: `"білиберда"` (українською) ✅
- Copilot відповідає: "Розумію, йдеться про білиберду (незрозумілий текст)..."

- Користувач: `"cthutq hjp,hsytr"` (англійська розкладка, хоче написати українською `"середній розбірник"`)
- Copilot транскодує: `"середній розбірник"` (українською) ✅
- Copilot відповідає: "Розумію, йдеться про середній розбірник..."

**Коли НЕ транскодувати:**
- Якщо текст **осмислений** українською або англійською (навіть з граматичними помилками) → не транскодувати.
- Приклад: `"давай зупинимось"` — це **нормальна українська** (хоч і з опечаткою "зупинимось" замість "зупинимося"), не транскодувати.

---

_Цей файл оновлюється вручну при змінах в архітектурі проєкту. Copilot використовує його для контексту._