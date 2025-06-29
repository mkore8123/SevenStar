
# .NET Razor Components / Blazor `@` 指示詞最完整手冊

這份文件包含所有常見 Razor 組件 `@` 指示詞，提供詳細說明、用法及程式碼範例，方便做為開發索引手冊。

---

## 🚀 `@page`
### 用途
設定此組件對應的 URL 路由。
```razor
@page "/"
@page "/product/{id:int}"
```
- `"/"` 代表首頁。
- `"/product/{id:int}"` 支援路由參數並強制型別為 int。

---

## 🚀 `@inject`
### 用途
從 DI 容器注入服務。
```razor
@inject HttpClient Http
```
等同於
```csharp
[Inject] public HttpClient Http { get; set; }
```

---

## 🚀 `@attribute`
### 用途
將 Attribute 加到 Razor 組件的類別上。
```razor
@attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]
@attribute [StreamRendering(true)]
@attribute [OutputCache(Duration = 60)]
@attribute [MyAudit(Category="User")]
```
可用任何合法的 C# Attribute，包括位置參數與命名參數。

---

## 🚀 `@namespace`
### 用途
覆寫 Razor 組件預設的命名空間。
```razor
@namespace MyApp.Features.Dashboard
```

---

## 🚀 `@inherits`
### 用途
讓此組件繼承指定基底類別。
```razor
@inherits MyBaseComponent
```

---

## 🚀 `@implements`
### 用途
讓此 Razor 組件實作介面。
```razor
@implements IDisposable
```

---

## 🚀 `@using`
### 用途
匯入命名空間。
```razor
@using System.Text.Json
@using MyApp.Shared.Models
```

---

## 🚀 `@layout`
### 用途
指定此 Razor 組件所使用的 Layout。
```razor
@layout MainLayout
```

---

## 🚀 `@rendermode`
### 用途
在 .NET 8+ Razor Components 決定互動執行模式。
```razor
@rendermode InteractiveServer
@rendermode InteractiveWebAssembly
@rendermode Static
@rendermode InteractiveAuto
```
| 模式                     | 功能 |
|--------------------------|------|
| `InteractiveServer`      | 使用 SignalR 在伺服器執行互動 |
| `InteractiveWebAssembly` | 在瀏覽器 (WASM) 執行互動 |
| `Static`                 | 只輸出靜態 HTML |
| `InteractiveAuto`        | 自動決定最佳模式 |

---

## 🚀 `@typeparam`
### 用途
定義泛型 Razor 組件型別參數。
```razor
@typeparam TItem
```

---

## 🚀 `@code`
### 用途
定義 Razor 組件的 C# 成員欄位與方法。
```razor
@code {
    private int count = 0;
    private void Increment() => count++;
}
```

---

## 🚀 `@bind`
### 用途
建立雙向繫結。
```razor
<input @bind="name" />
<MyInput @bind-Value="value" />
```
可使用 `@bind-Value:event="oninput"` 指定觸發事件。

---

## 🚀 `@ref`
### 用途
取得組件或 DOM 參考。
```razor
<InputText @ref="myInputRef" />
@code {
    private InputText? myInputRef;
}
```

---

## 🚀 `@key`
### 用途
在重新渲染時提供差異化追蹤。
```razor
@foreach (var item in items)
{
    <div @key="item.Id">@item.Name</div>
}
```

---

## 🚀 `@section`
### 用途
在 Layout 中插入可覆寫的內容區塊。
```razor
@section Scripts {
    <script src="myscript.js"></script>
}
```

---

## 🚀 `@* ... *@`
### 用途
Razor 註解，不會輸出到 HTML。
```razor
@* 這是一段 Razor 註解 *@
```

---

## 🚀 `@if`, `@foreach`, `@switch`
### 用途
在 Razor 中使用 C# 流程控制。
```razor
@if (isOk)
{
    <p>狀態正常</p>
}

@foreach (var u in users)
{
    <li>@u.Name</li>
}
```

---

## 🚀 `@onclick` 與事件處理
### 用途
處理 Blazor 組件事件。
```razor
<button @onclick="HandleClick">Click me</button>

@code {
    private void HandleClick() => Console.WriteLine("Clicked!");
}
```

---

# ✅ 小結索引
| 指示詞        | 說明 |
|---------------|------|
| `@page`       | 路由 URL |
| `@inject`     | 注入 DI 服務 |
| `@attribute`  | 加上 C# Attribute |
| `@namespace`  | 改組件命名空間 |
| `@inherits`   | 繼承基底類別 |
| `@implements` | 實作介面 |
| `@using`      | 匯入命名空間 |
| `@layout`     | 指定版面 |
| `@rendermode` | 決定互動模式 |
| `@typeparam`  | 泛型 Razor 組件 |
| `@code`       | 定義 C# 成員與方法 |
| `@bind`       | 雙向繫結 |
| `@ref`        | 元件或元素參考 |
| `@key`        | 指定追蹤鍵值 |
| `@section`    | Layout 區塊 |
| `@* *@`       | Razor 註解 |
| `@if`, `@foreach`, `@onclick` | Razor 程式邏輯與事件 |

---

📚 這就是目前最完整的 Razor Components `@` 指示詞大全，適合作為 .NET 8+ 開發參考手冊。
