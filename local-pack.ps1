# dotnet-pack.ps1 - .NET 模板打包与安装工具（带本地 NuGet 源支持）
# 基于 GitHub Action 流程改造

param(
    [string]$OutputDir,
    [switch]$Help,
    [switch]$NoBuild,
    [string]$Configuration = "Release",
    [switch]$SkipClean,
    [switch]$SkipInstall,
    [switch]$ForceInstall,
    [switch]$ListOnly,
    [switch]$NoPrompt,
    [string]$ProjectsRoot = "src/Shared/BlazorTemplates",
    [string]$TemplateProject = "template.csproj",
    [switch]$DisableLocalSource,      # 禁用本地 NuGet 源（默认启用）
    [string]$LocalNuGetPath = "..\LocalNuget",  # 本地 NuGet 源路径
    [string]$LocalSourceName = "local" # 本地源名称
)

# ============================================
# 帮助信息
# ============================================
if ($Help) {
    @"
用法: .\dotnet-pack.ps1 [选项]

描述:
  打包 .NET 项目并可选安装模板到本地，基于 GitHub Action 流程。
  支持多项目依赖打包，默认使用本地 NuGet 源解决依赖问题。
  注意：只有主模板（template.csproj）会被安装到本地。

参数:
  -OutputDir <路径>         NuGet 包输出路径（默认：当前目录\TempNuGetPackages）
  -LocalNuGetPath <路径>    本地 NuGet 源路径（默认：..\LocalNuget）
  -LocalSourceName <名称>   本地源名称（默认：local）
  -Configuration <配置>     构建配置（默认：Release）
  -ProjectsRoot <路径>      项目根目录（默认：src/Shared/BlazorTemplates）
  -TemplateProject <文件>   主模板项目文件（默认：template.csproj）
  -NoBuild                  不重新构建项目（使用 --no-build）
  -SkipClean                跳过清理旧的 .nupkg 文件
  -SkipInstall              跳过模板安装
  -ForceInstall             强制安装模板（使用 --force）
  -ListOnly                 仅列出已安装的模板，不进行安装
  -NoPrompt                 跳过所有交互提示
  -DisableLocalSource       禁用本地 NuGet 源（默认启用）
  -Help                     显示此帮助信息

示例:
  .\dotnet-pack.ps1
  .\dotnet-pack.ps1 -OutputDir ..\Packages\ -LocalNuGetPath ..\LocalNuget
  .\dotnet-pack.ps1 -DisableLocalSource -OutputDir ..\Packages\
  .\dotnet-pack.ps1 -SkipClean -ForceInstall -NoPrompt

"@
    exit 0
}

# ============================================
# 初始化变量
# ============================================
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$CurrentDir = Get-Location
$TempDirName = "TempNuGetPackages"
$IsTempDir = $false
$UseLocalSource = -not $DisableLocalSource  # 默认启用本地源

# 解析 LocalNuGetPath 为绝对路径
if ($LocalNuGetPath) {
    # 如果是相对路径，转换为绝对路径
    if (-not [System.IO.Path]::IsPathRooted($LocalNuGetPath)) {
        $LocalNuGetPath = Join-Path $CurrentDir $LocalNuGetPath
    }
    # 确保路径存在
    if (-not (Test-Path $LocalNuGetPath)) {
        New-Item -ItemType Directory -Path $LocalNuGetPath -Force | Out-Null
    }
}

# 设置输出目录
if ([string]::IsNullOrEmpty($OutputDir)) {
    $OutputDir = Join-Path $CurrentDir $TempDirName
    $IsTempDir = $true
    Write-Host "信息: 未指定输出目录，将使用临时文件夹" -ForegroundColor Cyan
} else {
    $OutputDir = $OutputDir
    Write-Host "信息: 使用指定的输出目录" -ForegroundColor Cyan
}

# 确保输出目录以反斜杠结尾
if (-not $OutputDir.EndsWith('\')) {
    $OutputDir = $OutputDir + '\'
}

# 创建输出目录
if (-not (Test-Path $OutputDir)) {
    Write-Host "创建: 输出目录不存在，正在创建..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-Host "成功: 目录已创建" -ForegroundColor Green
}

# 如果使用本地 NuGet 源，创建并配置
if ($UseLocalSource) {
    Write-Host "配置: 设置本地 NuGet 源..." -ForegroundColor Yellow
    Write-Host "  本地源路径: $LocalNuGetPath" -ForegroundColor Gray
    
    # 创建本地 NuGet 目录
    if (-not (Test-Path $LocalNuGetPath)) {
        New-Item -ItemType Directory -Path $LocalNuGetPath -Force | Out-Null
        Write-Host "  创建: 本地源目录" -ForegroundColor Gray
    }
    
    # 检查是否已存在同名本地源，如果存在则移除
    $existingSource = & dotnet nuget list source 2>$null | Select-String "$LocalSourceName\s"
    if ($existingSource) {
        Write-Host "  移除: 已存在的本地源 ($LocalSourceName)" -ForegroundColor Gray
        & dotnet nuget remove source $LocalSourceName 2>$null
    }
    
    # 添加本地 NuGet 源
    Write-Host "  添加: 本地 NuGet 源 ($LocalNuGetPath)" -ForegroundColor Gray
    & dotnet nuget add source $LocalNuGetPath --name $LocalSourceName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  警告: 添加本地源失败，将使用默认源" -ForegroundColor Yellow
        $UseLocalSource = $false
    } else {
        Write-Host "  成功: 本地源配置完成" -ForegroundColor Green
    }
    Write-Host ""
} else {
    Write-Host "信息: 本地 NuGet 源已禁用" -ForegroundColor Cyan
    Write-Host ""
}

# ============================================
# 显示 Banner
# ============================================
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "           .NET 模板打包与安装工具 v3.5 (PowerShell)" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "当前目录: $CurrentDir" -ForegroundColor Gray
Write-Host "输出目录: $OutputDir" -ForegroundColor Gray
Write-Host "项目根目录: $ProjectsRoot" -ForegroundColor Gray
Write-Host "本地源: $(if ($UseLocalSource) { "启用 ($LocalNuGetPath)" } else { "禁用" })" -ForegroundColor Gray
Write-Host "目录类型: $(if ($IsTempDir) { '临时目录' } else { '用户指定' })" -ForegroundColor Gray
Write-Host ""

# ============================================
# 定义需要打包的项目列表（与 GitHub Action 保持一致）
# ============================================
$Projects = @(
    @{ 
        Name = "BlazorTemplate.Constraints"
        Path = "src/Shared/BlazorTemplates/BlazorTemplate.Constraints/BlazorTemplate.Constraints.csproj"
        DependsOn = @()
        OutputPrefix = "BlazorTemplate.Constraints"
        IsTemplate = $false
        Generators = @()  # 此项目依赖的生成器列表（csproj 路径）
    },
    @{ 
        Name = "BlazorTemplate.UI.Shared"
        Path = "src/Shared/BlazorTemplates/BlazorTemplate.UI.Shared/BlazorTemplate.UI.Shared.csproj"
        DependsOn = @("BlazorTemplate.Constraints")
        OutputPrefix = "BlazorTemplate.UI.Shared"
        IsTemplate = $false
        Generators = @(
            "src/Shared/BlazorTemplates/BlazorTemplate.ClientCore.Generators/BlazorTemplate.ClientCore.Generators.csproj"
        )  # 依赖的生成器
    },
    @{ 
        Name = "BlazorTemplate.AppCore"
        Path = "src/Shared/BlazorTemplates/BlazorTemplate.AppCore/BlazorTemplate.AppCore.csproj"
        DependsOn = @("BlazorTemplate.Constraints", "BlazorTemplate.UI.Shared")
        OutputPrefix = "BlazorTemplate.AppCore"
        IsTemplate = $false
        Generators = @()
    },
    @{ 
        Name = "BlazorTemplate.UI.AntBlazor"
        Path = "src/Shared/BlazorTemplates/BlazorTemplate.UI.AntBlazor/BlazorTemplate.UI.AntBlazor.csproj"
        DependsOn = @("BlazorTemplate.Constraints", "BlazorTemplate.UI.Shared", "BlazorTemplate.AppCore")
        OutputPrefix = "BlazorTemplate.UI.AntBlazor"
        IsTemplate = $false
        Generators = @()
    },
    @{ 
        Name = "BlazorTemplate.UI.FluentUI"
        Path = "src/Shared/BlazorTemplates/BlazorTemplate.UI.FluentUI/BlazorTemplate.UI.FluentUI.csproj"
        DependsOn = @("BlazorTemplate.Constraints", "BlazorTemplate.UI.Shared", "BlazorTemplate.AppCore")
        OutputPrefix = "BlazorTemplate.UI.FluentUI"
        IsTemplate = $false
        Generators = @()
    }
)

$MainTemplate = $TemplateProject

# ============================================
# 步骤 1: 清理旧的 nupkg 文件
# ============================================
if (-not $SkipClean -and -not $NoPrompt) {
    $choice = Read-Host "是否清理输出目录中的旧 .nupkg 文件? (Y/N)"
    if ($choice -eq 'Y' -or $choice -eq 'y') {
        Write-Host "清理: 正在删除旧的 .nupkg 文件..." -ForegroundColor Yellow
        Remove-Item -Path "$OutputDir*.nupkg" -Force -ErrorAction SilentlyContinue
        if ($UseLocalSource) {
            Remove-Item -Path "$LocalNuGetPath\*.nupkg" -Force -ErrorAction SilentlyContinue
        }
        Write-Host "完成: 已清理旧文件" -ForegroundColor Green
    }
} elseif (-not $SkipClean) {
    Write-Host "清理: 正在删除旧的 .nupkg 文件..." -ForegroundColor Yellow
    Remove-Item -Path "$OutputDir*.nupkg" -Force -ErrorAction SilentlyContinue
    if ($UseLocalSource) {
        Remove-Item -Path "$LocalNuGetPath\*.nupkg" -Force -ErrorAction SilentlyContinue
    }
    Write-Host "完成: 已清理旧文件" -ForegroundColor Green
}
Write-Host ""

# ============================================
# 函数: 编译生成器列表
# ============================================
function Build-Generators {
    param(
        [string[]]$GeneratorPaths,
        [string]$Configuration
    )
    
    if ($GeneratorPaths.Count -eq 0) {
        return $true
    }
    
    Write-Host "编译生成器: 共 $($GeneratorPaths.Count) 个生成器" -ForegroundColor Yellow
    
    $AllSuccess = $true
    foreach ($generatorPath in $GeneratorPaths) {
        $FullGeneratorPath = Join-Path $CurrentDir $generatorPath
        
        if (-not (Test-Path $FullGeneratorPath)) {
            Write-Host "  警告: 生成器项目文件不存在: $generatorPath" -ForegroundColor Yellow
            $AllSuccess = $false
            continue
        }
        
        $GeneratorName = Split-Path -Path $generatorPath -Leaf
        Write-Host "  编译: $GeneratorName" -ForegroundColor Gray
        Write-Host "    项目文件: $generatorPath" -ForegroundColor Gray
        
        # 构建生成器（不打包）
        $BuildCommand = "dotnet build `"$FullGeneratorPath`" -c $Configuration"
        Write-Host "    命令: $BuildCommand" -ForegroundColor Gray
        Invoke-Expression -Command $BuildCommand
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "    错误: 生成器编译失败: $GeneratorName" -ForegroundColor Red
            $AllSuccess = $false
        } else {
            Write-Host "    成功: 生成器编译完成: $GeneratorName" -ForegroundColor Green
        }
    }
    
    if ($AllSuccess) {
        Write-Host "  所有生成器编译完成" -ForegroundColor Green
    } else {
        Write-Host "  部分生成器编译失败" -ForegroundColor Red
    }
    Write-Host ""
    
    return $AllSuccess
}

# ============================================
# 函数: 打包单个项目
# ============================================
function Pack-Project {
    param(
        [string]$ProjectName,
        [string]$ProjectPath,
        [string[]]$DependsOn,
        [string]$OutputPrefix,
        [bool]$IsTemplate = $false,
        [string[]]$Generators = @()
    )
    
    $FullProjectPath = Join-Path $CurrentDir $ProjectPath
    
    if (-not (Test-Path $FullProjectPath)) {
        Write-Host "  警告: 项目文件不存在: $FullProjectPath" -ForegroundColor Yellow
        return $false
    }
    
    $ProjectType = if ($IsTemplate) { "主模板" } else { $ProjectName }
    Write-Host "打包: $ProjectType" -ForegroundColor Yellow
    Write-Host "  项目文件: $ProjectPath" -ForegroundColor Gray
    
    if ($DependsOn.Count -gt 0) {
        Write-Host "  依赖项目: $($DependsOn -join ', ')" -ForegroundColor Gray
    }
    
    if ($Generators.Count -gt 0) {
        Write-Host "  依赖生成器: $($Generators.Count) 个" -ForegroundColor Gray
        foreach ($gen in $Generators) {
            $GenName = Split-Path -Path $gen -Leaf
            Write-Host "    - $GenName" -ForegroundColor Gray
        }
    }
    
    # 构建项目
    if (-not $NoBuild) {
        $BuildCommand = "dotnet build `"$FullProjectPath`" -c $Configuration"
        if ($UseLocalSource) {
            $BuildCommand += " --source `"$LocalNuGetPath`""
        }
        Write-Host "  构建命令: $BuildCommand" -ForegroundColor Gray
        Invoke-Expression -Command $BuildCommand
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  错误: 构建失败: $ProjectType" -ForegroundColor Red
            return $false
        }
    }
    
    # 打包项目
    $PackCommand = "dotnet pack `"$FullProjectPath`" --no-build -c $Configuration -o `"$OutputDir`""
    if ($UseLocalSource) {
        $PackCommand += " --source `"$LocalNuGetPath`""
    }
    Write-Host "  打包命令: $PackCommand" -ForegroundColor Gray
    Invoke-Expression -Command $PackCommand
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  错误: 打包失败: $ProjectType" -ForegroundColor Red
        return $false
    }
    
    # 如果启用本地源，将生成的 nupkg 复制到本地源目录
    if ($UseLocalSource) {
        # 查找生成的 nupkg 文件
        $NupkgFiles = Get-ChildItem -Path "$OutputDir$OutputPrefix*.nupkg" -File -ErrorAction SilentlyContinue
        if (-not $NupkgFiles) {
            # 如果没找到，尝试通配符匹配
            $NupkgFiles = Get-ChildItem -Path "$OutputDir$ProjectName*.nupkg" -File -ErrorAction SilentlyContinue
        }
        foreach ($file in $NupkgFiles) {
            Write-Host "  复制到本地源: $($file.Name)" -ForegroundColor Gray
            Copy-Item -Path $file.FullName -Destination $LocalNuGetPath -Force
        }
    }
    
    Write-Host "  成功: 打包完成: $ProjectType" -ForegroundColor Green
    return $true
}

# ============================================
# 步骤 2: 编译所有生成器
# ============================================
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host "步骤 1/4: 编译所有生成器..." -ForegroundColor Cyan
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host ""

# 收集所有需要编译的生成器（去重）
$AllGenerators = @()
foreach ($project in $Projects) {
    if ($project.Generators.Count -gt 0) {
        $AllGenerators += $project.Generators
    }
}
# 去重
$AllGenerators = $AllGenerators | Select-Object -Unique

if ($AllGenerators.Count -gt 0 -and -not $NoBuild) {
    $GeneratorResult = Build-Generators -GeneratorPaths $AllGenerators -Configuration $Configuration
    if (-not $GeneratorResult) {
        Write-Host "错误: 生成器编译失败，请检查错误信息" -ForegroundColor Red
        if (-not $NoPrompt) {
            Read-Host "按 Enter 退出"
        }
        exit 1
    }
} elseif ($AllGenerators.Count -gt 0 -and $NoBuild) {
    Write-Host "信息: 已指定 --no-build，跳过生成器编译" -ForegroundColor Yellow
    Write-Host "  注意: 请确保生成器已经编译过，否则构建可能失败" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host "信息: 没有需要编译的生成器" -ForegroundColor Cyan
    Write-Host ""
}

# ============================================
# 步骤 3: 打包所有项目（按依赖顺序）
# ============================================
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host "步骤 2/4: 正在打包所有项目..." -ForegroundColor Cyan
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host ""

$PackedProjects = @()
$FailedProjects = @()

# 按依赖顺序打包依赖项目
foreach ($project in $Projects) {
    $result = Pack-Project -ProjectName $project.Name `
                          -ProjectPath $project.Path `
                          -DependsOn $project.DependsOn `
                          -OutputPrefix $project.OutputPrefix `
                          -IsTemplate $false `
                          -Generators $project.Generators
    
    if ($result) {
        $PackedProjects += $project.Name
    } else {
        $FailedProjects += $project.Name
        # 如果关键项目打包失败，可能影响后续项目
        if ($UseLocalSource) {
            Write-Host "  警告: $($project.Name) 打包失败，可能影响依赖它的项目" -ForegroundColor Yellow
        }
    }
    Write-Host ""
}

# 打包主模板（主模板可能也依赖生成器，但通常在项目文件中已经配置）
Write-Host "打包: 主模板" -ForegroundColor Yellow
Write-Host "  项目文件: $MainTemplate" -ForegroundColor Gray
Write-Host "  依赖项目: $($PackedProjects -join ', ')" -ForegroundColor Gray

$FullMainTemplatePath = Join-Path $CurrentDir $MainTemplate
if (-not (Test-Path $FullMainTemplatePath)) {
    Write-Host "警告: 主模板文件不存在: $FullMainTemplatePath" -ForegroundColor Yellow
} else {
    if (-not $NoBuild) {
        $BuildCommand = "dotnet build `"$FullMainTemplatePath`" -c $Configuration"
        if ($UseLocalSource) {
            $BuildCommand += " --source `"$LocalNuGetPath`""
        }
        Write-Host "  构建命令: $BuildCommand" -ForegroundColor Gray
        Invoke-Expression -Command $BuildCommand
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  错误: 构建失败: 主模板" -ForegroundColor Red
            $FailedProjects += "主模板"
        }
    }
    
    if ($LASTEXITCODE -eq 0 -or $NoBuild) {
        $PackCommand = "dotnet pack `"$FullMainTemplatePath`" --no-build -c $Configuration -o `"$OutputDir`""
        if ($UseLocalSource) {
            $PackCommand += " --source `"$LocalNuGetPath`""
        }
        Write-Host "  打包命令: $PackCommand" -ForegroundColor Gray
        Invoke-Expression -Command $PackCommand
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  错误: 打包失败: 主模板" -ForegroundColor Red
            $FailedProjects += "主模板"
        } else {
            Write-Host "  成功: 打包完成: 主模板" -ForegroundColor Green
            $PackedProjects += "主模板"
            
            # 如果启用本地源，将生成的 nupkg 复制到本地源目录
            if ($UseLocalSource) {
                $NupkgFiles = Get-ChildItem -Path "$OutputDir*.nupkg" -File | Where-Object { $_.Name -like "*Template*" }
                foreach ($file in $NupkgFiles) {
                    Write-Host "  复制到本地源: $($file.Name)" -ForegroundColor Gray
                    Copy-Item -Path $file.FullName -Destination $LocalNuGetPath -Force
                }
            }
        }
    }
}

Write-Host ""

# 显示打包结果
Write-Host "打包结果:" -ForegroundColor Cyan
Write-Host "  成功: $($PackedProjects.Count) 个项目" -ForegroundColor Green
if ($PackedProjects.Count -gt 0) {
    foreach ($proj in $PackedProjects) {
        Write-Host "    - $proj" -ForegroundColor Gray
    }
}
if ($FailedProjects.Count -gt 0) {
    Write-Host "  失败: $($FailedProjects.Count) 个项目" -ForegroundColor Red
    foreach ($proj in $FailedProjects) {
        Write-Host "    - $proj" -ForegroundColor Red
    }
}
Write-Host ""

# 如果有项目打包失败，询问是否继续
if ($FailedProjects.Count -gt 0 -and -not $NoPrompt) {
    $choice = Read-Host "有项目打包失败，是否继续安装已成功打包的模板? (Y/N)"
    if ($choice -ne 'Y' -and $choice -ne 'y') {
        Write-Host "操作已取消" -ForegroundColor Yellow
        Read-Host "按 Enter 退出"
        exit 1
    }
    Write-Host ""
}

# ============================================
# 步骤 4: 查找生成的 nupkg
# ============================================
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host "步骤 3/4: 查找生成的 NuGet 包..." -ForegroundColor Cyan
Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
Write-Host ""

$NupkgFiles = Get-ChildItem -Path "$OutputDir*.nupkg" -File

if ($NupkgFiles.Count -eq 0) {
    Write-Host "错误: 未找到任何 .nupkg 文件！" -ForegroundColor Red
    Read-Host "按 Enter 退出"
    exit 1
}

Write-Host "找到 $($NupkgFiles.Count) 个 .nupkg 文件:" -ForegroundColor Green
$Index = 1
foreach ($file in $NupkgFiles) {
    $FileSize = [math]::Round($file.Length / 1KB, 2)
    Write-Host "  $Index. $($file.Name) ($FileSize KB)" -ForegroundColor Gray
    $Index++
}
Write-Host ""

# ============================================
# 步骤 5: 安装主模板（仅安装主模板）
# ============================================
if (-not $SkipInstall) {
    Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "步骤 4/4: 安装主模板到本地" -ForegroundColor Cyan
    Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "注意: 只有主模板会被安装，其他项目作为依赖包" -ForegroundColor Yellow
    Write-Host ""
    
    # 查找主模板的 nupkg 文件
    $TemplateNupkg = Get-ChildItem -Path "$OutputDir*Template*.nupkg" -File | Select-Object -First 1
    if (-not $TemplateNupkg) {
        # 尝试其他可能的命名
        $TemplateNupkg = Get-ChildItem -Path "$OutputDir*.nupkg" -File | Where-Object { 
            $_.Name -notlike "*BlazorTemplate.*" -or $_.Name -like "*Template*" 
        } | Select-Object -First 1
    }
    
    if (-not $TemplateNupkg) {
        Write-Host "警告: 未找到主模板的 .nupkg 文件，跳过安装" -ForegroundColor Yellow
    } else {
        Write-Host "找到主模板: $($TemplateNupkg.Name)" -ForegroundColor Green
        
        if ($ListOnly) {
            # 仅列出已安装的模板
            Write-Host ""
            & dotnet new list 2>$null
            Write-Host ""
        } elseif ($ForceInstall) {
            # 强制安装（使用 --force）
            Write-Host ""
            Write-Host "强制安装: 正在使用 --force 安装主模板..." -ForegroundColor Yellow
            Write-Host ""
            
            # 先获取包名用于卸载
            $PackageName = $TemplateNupkg.BaseName -replace '\.[0-9]+\.[0-9]+\.[0-9]+.*$', ''
            Write-Host "  包名: $PackageName" -ForegroundColor Gray
            
            $InstallCommand = "dotnet new install `"$($TemplateNupkg.FullName)`" --force"
            Write-Host "  命令: $InstallCommand" -ForegroundColor Gray
            & dotnet new install $TemplateNupkg.FullName --force
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  成功: 主模板安装完成！" -ForegroundColor Green
                Write-Host ""
                & dotnet new list 2>$null
            } else {
                Write-Host "  错误: 主模板安装失败！" -ForegroundColor Red
            }
        } elseif (-not $NoPrompt) {
            # 交互式安装
            Write-Host ""
            Write-Host "请选择操作:"
            Write-Host "  Y - 安装主模板（会先卸载旧版本）"
            Write-Host "  R - 强制重新安装主模板（使用 --force，不卸载）"
            Write-Host "  L - 仅列出已安装的模板"
            Write-Host "  N - 跳过安装"
            Write-Host ""
            $choice = Read-Host "请输入选项"
            
            # 获取包名
            $PackageName = $TemplateNupkg.BaseName -replace '\.[0-9]+\.[0-9]+\.[0-9]+.*$', ''
            
            switch ($choice.ToUpper()) {
                'Y' {
                    # 先卸载再安装
                    Write-Host ""
                    Write-Host "卸载: 正在卸载旧版本模板..." -ForegroundColor Yellow
                    Write-Host "  包名: $PackageName" -ForegroundColor Gray
                    & dotnet new uninstall $PackageName 2>$null
                    Write-Host ""
                    
                    Write-Host "安装: 正在安装主模板..." -ForegroundColor Yellow
                    Write-Host "  包名: $PackageName" -ForegroundColor Gray
                    & dotnet new install $TemplateNupkg.FullName
                    
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host "  成功: 主模板安装完成！" -ForegroundColor Green
                        Write-Host ""
                        & dotnet new list 2>$null
                    } else {
                        Write-Host "  错误: 主模板安装失败！" -ForegroundColor Red
                    }
                }
                'R' {
                    # 强制安装
                    Write-Host ""
                    Write-Host "强制安装: 正在使用 --force 安装主模板..." -ForegroundColor Yellow
                    Write-Host "  包名: $PackageName" -ForegroundColor Gray
                    & dotnet new install $TemplateNupkg.FullName --force
                    
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host "  成功: 主模板强制安装完成！" -ForegroundColor Green
                        Write-Host ""
                        & dotnet new list 2>$null
                    } else {
                        Write-Host "  错误: 主模板安装失败！" -ForegroundColor Red
                    }
                }
                'L' {
                    Write-Host ""
                    & dotnet new list 2>$null
                }
                'N' {
                    Write-Host "跳过: 未安装模板。" -ForegroundColor Yellow
                }
                default {
                    Write-Host "跳过: 未选择有效选项。" -ForegroundColor Yellow
                }
            }
        } else {
            # 无交互模式，直接安装
            Write-Host ""
            Write-Host "安装: 正在安装主模板..." -ForegroundColor Yellow
            
            $PackageName = $TemplateNupkg.BaseName -replace '\.[0-9]+\.[0-9]+\.[0-9]+.*$', ''
            Write-Host "  包名: $PackageName" -ForegroundColor Gray
            & dotnet new install $TemplateNupkg.FullName --force
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  成功: 主模板安装完成！" -ForegroundColor Green
                Write-Host ""
                & dotnet new list 2>$null
            } else {
                Write-Host "  错误: 主模板安装失败！" -ForegroundColor Red
            }
        }
        Write-Host ""
    }
}

# ============================================
# 步骤 6: 清理临时文件夹
# ============================================
if ($IsTempDir -and -not $NoPrompt) {
    Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
    Write-Host "清理: 临时文件夹管理" -ForegroundColor Cyan
    Write-Host "------------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "当前使用的是临时文件夹: $OutputDir" -ForegroundColor Yellow
    Write-Host "包含 $($NupkgFiles.Count) 个 .nupkg 文件" -ForegroundColor Gray
    Write-Host ""
    
    $choice = Read-Host "是否删除临时文件夹及其内容? (Y/N)"
    
    if ($choice -eq 'Y' -or $choice -eq 'y') {
        Write-Host "删除: 正在删除临时文件夹..." -ForegroundColor Yellow
        Remove-Item -Path $OutputDir -Recurse -Force -ErrorAction SilentlyContinue
        if (Test-Path $OutputDir) {
            Write-Host "警告: 无法完全删除临时文件夹，请手动清理" -ForegroundColor Yellow
        } else {
            Write-Host "成功: 临时文件夹已删除" -ForegroundColor Green
        }
    } else {
        Write-Host "保留: 临时文件夹已保留: $OutputDir" -ForegroundColor Gray
    }
    Write-Host ""
}

# ============================================
# 完成
# ============================================
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "                   所有操作已完成！" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "打包摘要:" -ForegroundColor Cyan
Write-Host "  成功项目: $($PackedProjects.Count)" -ForegroundColor Green
Write-Host "  失败项目: $($FailedProjects.Count)" -ForegroundColor $(if ($FailedProjects.Count -gt 0) { 'Red' } else { 'Green' })
Write-Host "  输出目录: $OutputDir" -ForegroundColor Gray
if ($UseLocalSource) {
    Write-Host "  本地源: $LocalNuGetPath" -ForegroundColor Gray
}
if ($IsTempDir) {
    $TempStatus = if (Test-Path $OutputDir) { "已保留" } else { "已删除" }
    Write-Host "  临时目录: $TempStatus" -ForegroundColor Gray
}
Write-Host ""

if (-not $NoPrompt) {
    Read-Host "按 Enter 退出"
}
exit 0