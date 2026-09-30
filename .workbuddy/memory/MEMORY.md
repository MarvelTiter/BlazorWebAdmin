# BlazorWebAdmin 长期记忆

> 细则与完整踩坑集见同目录 `AOT-DETAIL.md`；本文件只留高频口径。

## 定位
`dotnet new` 模板仓库（id `mtblazor`）；`master`=web host、`master.wpf`=wpf host；可选参数在
`src/.template.config/template.json`。`src/Shared/Project.*`、`AppCore/Constraints/UI.Shared`、`*Generator`
等目录只剩 bin/obj 空壳，源码已并入 `BlazorTemplate.ClientCore`；版本统一在
`src/Shared/Props/projects.versions.props`。

## AOT 分级（唯一口径）
**判据：看有没有 `MakeGeneric*`，不是看有没有 `Compile()`。**
- **R1 动态代码生成**（`Expression.Compile` / `Reflection.Emit`）→ 不致命，解释器始终在，只是慢一档。
  `Expression<T>.Compile()` **不带** `[RequiresDynamicCode]`；带的是基类 `LambdaExpression.Compile()` 与
  非泛型 `Expression.Lambda(...)` ⇒ IL3050 只看静态类型是不是 `LambdaExpression`。
- **R2 运行时泛型实例化**（`MakeGenericMethod` / `MakeGenericType`，实参含**值类型**）→ **唯一真硬阻塞**，
  报 **IL3050 + IL2060**。修法：方法组委托 + `Expression.Invoke`
  （`static readonly Func<IEnumerable<TValue>, List<TValue>> toList = Enumerable.ToList;`），
  实例化点写死在本类型内。已落地 `UI/Builders/SelectComponentBuilder.cs`。
- **R3 字符串反射**（`GetMethod("X")` / `Expression.Property(e,"X")` / `GetCustomAttribute<T>()`）→ 不崩但
  **静默失败**，用 DAM / `[DynamicDependency]` / 源生成器标注即可。

## 源生成器（要点）
基类留反射 fallback + `virtual` 钩子 → 生成器在**派生类所在程序集**发 `partial override` → 生成器工程
netstandard2.0 **不得引用宿主类型**（名字用字符串常量）→ 业务页面加 `partial`。生成器**只看当前编译单元**
⇒ ClientCore / BlazorAdmin.Client / BlazorAdmin 三处 csproj 各挂一次（`OutputItemType=Analyzer` +
`ReferenceOutputAssembly=false`）。**看不到 Razor 语法树** ⇒ `@page` / `@attribute` 永久盲区，反射兜底。
产物：头部**必须显式 `#nullable enable`**（否则每个 `string?` 报 CS8669）；字面量走
`SymbolDisplay.FormatLiteral(v, quote: true)`；hintName 用稳定哈希（FNV-1a）；多源合流**按全名去重**；
语法取最保守形式（显式 `new Xxx(...)`，不用 `[...]`、不用 `new()`）。
**类型擦除优先**：查表层保持 `object` / `Array`，泛型只出现在实参可静态写死处（拿 `IXxx<TEnum>` 只能
`MakeGenericType` = R2）；推论「能靠通用类 + 元数据做的，不要逐符号生成类」。生成器模板里写死的 `using`
不走 IDE 重命名，改目录后必须查模板字面量。

## 高频 AOT 注解
`OpenComponent<TComponent>()` 要求 DAM **`All`**（`UI/Builders` 靠 `AddMultipleAttributes` 按字符串名匹配
`[Parameter]`）；`ConfigureOptions<T>` 同。DAM 只能加在**泛型参数位置**，且仅封闭具体类型才标；可被消费方
任意扩展的（`ModelPage<TModel>`、`AddTab<T>`）改局部 `[UnconditionalSuppressMessage("Trimming","IL2091")]`。
`Insert/Update/Delete<T>`、`InnerJoin<TJoin>` 的 DAM 缺口在 **LightORM 仓库**侧补。

## 项目约定：读同步 / 写异步
渲染路径上的取数（表格格式化、Builder、条件构造器、下拉）一律**同步只读缓存**、不可 `await`；
`Lookup/` 的读取接口（`ILookupProvider` 三成员、`ILookupService` 两成员）因此**只有同步形式**。
加载由 **provider 自驱**（`BaseLookupProvider` 构造期起 `RefreshInterval` 轮询循环，整体替换缓存指针）
⇒ 读侧无锁、不见半成品；代价是宿主没有可 await 的预热点、也无法强制刷新（细节见 `AOT-DETAIL.md`
「Lookup 加载路径」，含 `new Task(...)` 冷任务踩坑）。

## 验证纪律（要点）
Bash 工具 PATH 失效（`ls`/`grep` 全 not found），管道会静默吞掉 dotnet 输出 ⇒ 构建一律
`dotnet build … *> .workbuddy/tmp/x.log 2>&1` 再 Read/Grep，shell 操作用 PowerShell。**必须**
`--no-incremental`（否则 obj 旧产物给「假绿」）；导出生成产物还须加 `-p:EmitCompilerGeneratedFiles=true`。
`obj\GFEnum\**\*_razor.g.cs` 已停更，**不能**用来判断改没改。`ClientCore.csproj` 已写
`<IsAotCompatible>true</IsAotCompatible>`，**不要**再当命令行全局属性传（会触发 NETSDK1210）。
