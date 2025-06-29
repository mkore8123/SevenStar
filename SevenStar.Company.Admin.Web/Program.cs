using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SevenStar.Company.Admin.Web;

// 建立 WebAssemblyHostBuilder，會自動載入 appsettings.json 與環境設定
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 註冊根組件 <App>，並把它掛到 wwwroot/index.html 裡的 <div id="app"></div>
builder.RootComponents.Add<App>("#app");
// 註冊 <HeadOutlet>，讓組件可以用 <PageTitle> 或 <HeadContent> 自動把 <title> 與 <meta> 注入 <head>
builder.RootComponents.Add<HeadOutlet>("head::after");

// 註冊 HttpClient 到 DI 容器，讓組件可以 @inject HttpClient。
// 預設 BaseAddress 是 index.html 所在位址，用於呼叫相同主機上的 API 或靜態檔案。
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 建立並啟動 Blazor WebAssembly 應用程式
// 這會在瀏覽器下載 .dll 與 WASM runtime，並開始執行 <App>
await builder.Build().RunAsync();
