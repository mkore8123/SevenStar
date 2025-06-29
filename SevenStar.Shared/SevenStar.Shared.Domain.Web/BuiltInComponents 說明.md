
# .NET 10 Razor Components / Blazor 內建元件完整總覽

本文件詳細列出所有 .NET 10（基於 .NET 8+）官方支援的 Razor Components / Blazor 內建元件，包含用途、參數與範例。

---

## 🚀 Routing 與 SEO 元件

### `<Routes />`
| 主要參數 | 說明 |
|----------|------|
| (無)     | 自動依據 `@page` 掃描 URL 路由對應 Razor 組件 |

**範例：**
```html
<body>
    <Routes />
</body>
```

---

### `<HeadOutlet />`
| 主要參數 | 說明 |
|----------|------|
| (無)     | 將 `HeadContent` 中的內容注入 `<head>` |

**範例：**
```html
<head>
    <HeadOutlet />
</head>

<!-- 在 Razor 組件中 -->
<HeadContent>
    <title>我的頁面</title>
    <meta name="description" content="說明" />
</HeadContent>
```

---

### `<ImportMap />`
| 主要參數 | 說明 |
|----------|------|
| (無)     | 自動產生 importmap 以供 ESM 載入 |
```html
<head>
    <ImportMap />
</head>
```

---

## 📝 Forms 與驗證元件

### `<EditForm>`
| 主要參數        | 說明 |
|-----------------|------|
| `Model`         | 雙向綁定物件 |
| `EditContext`   | 自訂驗證上下文 |
| `OnValidSubmit` | 驗證成功時呼叫 |
| `OnInvalidSubmit` | 驗證失敗時呼叫 |

```razor
<EditForm Model="@user" OnValidSubmit="HandleSubmit">
    <InputText @bind-Value="user.Name" />
</EditForm>
```

---

### `<InputText>`, `<InputNumber>`, `<InputDate>`, `<InputSelect>`, `<InputCheckbox>`
| 主要參數 | 說明 |
|----------|------|
| `@bind-Value` | 雙向綁定屬性 |
| `ValueExpression` | 用於驗證 |
| `ValueChanged` | 自訂變更事件 |

```razor
<InputText @bind-Value="user.Name" class="form-control" />
<InputNumber @bind-Value="user.Age" />
<InputDate @bind-Value="user.BirthDate" />
<InputCheckbox @bind-Value="user.IsActive" />
<InputSelect @bind-Value="user.Level">
    <option value="1">初級</option>
    <option value="2">高級</option>
</InputSelect>
```

---

### `<InputFile>`
| 主要參數 | 說明 |
|----------|------|
| `OnChange` | 檔案選擇變更事件 |
| `multiple` | 是否允許多檔 |
| `accept`   | 檔案 MIME |

```razor
<InputFile OnChange="HandleFile" multiple accept="image/png" />
```

---

### `<InputRadio>`
| 主要參數 | 說明 |
|----------|------|
| `@bind-Value` | 雙向綁定欄位 |
| `Value`       | 單選值 |

```razor
<InputRadio @bind-Value="user.Gender" Value="Male" />
<InputRadio @bind-Value="user.Gender" Value="Female" />
```

---

### `<ValidationSummary>`
顯示所有欄位驗證錯誤。
```razor
<ValidationSummary />
```

---

### `<ValidationMessage>`
| 主要參數 | 說明 |
|----------|------|
| `For`    | 指定 lambda 取得欄位 |

```razor
<ValidationMessage For="@(() => user.Name)" />
```

---

## ⚙️ Interactive / 動態元件

### `<DynamicComponent>`
| 主要參數   | 說明 |
|------------|------|
| `Type`     | 要渲染的 Razor Component 型別 |
| `Parameters` | 傳入屬性 |

```razor
<DynamicComponent Type="@dynamicType" Parameters="@parameters" />

@code {
    private Type dynamicType = typeof(MyComponent);
    private Dictionary<string, object> parameters = new() {
        ["Title"] = "Hello"
    };
}
```

---

### `<FocusOnNavigate>`
| 主要參數 | 說明 |
|----------|------|
| `Selector` | CSS Selector |
| `Delay`    | 延遲毫秒數 |

```razor
<FocusOnNavigate Selector="input" Delay="100" />
<input placeholder="自動聚焦" />
```

---

# ✅ 最終總覽表

| 分類            | 元件 |
|-----------------|------|
| Routing / SEO   | `<Routes />`, `<HeadOutlet />`, `<ImportMap />` |
| Forms           | `<EditForm>`, `<InputText>`, `<InputNumber>`, `<InputDate>`, `<InputSelect>`, `<InputCheckbox>`, `<InputRadio>`, `<InputFile>`, `<ValidationSummary>`, `<ValidationMessage>` |
| Interactive     | `<DynamicComponent>`, `<FocusOnNavigate>` |

---

✅ 這是針對 .NET 10 Razor Components 最完整官方內建元件、參數與範例文件，隨時可當索引查詢。
