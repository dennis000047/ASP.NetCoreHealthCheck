# ASP.NetCoreHealthCheck
使用ASP.NetCore6 Web API

此程式版本 ASP.NET CORE 6 WEB API & MSSql

使用HealthCheck前應裝套件:

=> nuget套件管理主控台 最新版即可

AspNetCore.HealthChecks.UI: 
AspNetCore.HealthChecks.UI.Client
AspNetCore.HealthChecks.UI.InMemory.Storage
AspNetCore.HealthChecks.Uris
AspNetCore.HealthChecks.SqlServer

基本User SQL程式碼在文件夾SqlScripts中

套件重點設定文件位址:

=> appsetting.json 和 Program.cs


測試時網址為
https://{Your-API-URL}/healthcheck  <=此路由為json方式呈現 範例圖為資料夾內成果(3)
https://{Your-API-URL}/healthchecks-ui <=此路由為UI呈現方式，需安裝上列套件，範例圖為成果(1)(2)

結果的UI中，Description處內容，在個人實作的IHealthCheck介面內自定義。

HealthCheck UI自動檢查的時間在appsetting內EvaluationTimeInSeconds自定義。
