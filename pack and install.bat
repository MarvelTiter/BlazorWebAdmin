@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion

REM ============================================
REM dotnet-pack.bat - .NET 模板打包与安装工具
REM ============================================

set "SCRIPT_DIR=%~dp0"
set "CURRENT_DIR=%CD%"
set "IS_TEMP_DIR=0"
set "TEMP_DIR_NAME=TempNuGetPackages"
set "CONFIGURATION=Release"
set "NO_BUILD=--no-build"

REM ============================================
REM 解析参数
REM ============================================
set "OUTPUT_ARG=%~1"

if "%OUTPUT_ARG%"=="" (
    set "OUTPUT_DIR=%CURRENT_DIR%\%TEMP_DIR_NAME%\"
    set "IS_TEMP_DIR=1"
    echo 信息: 未指定输出目录，将使用临时文件夹 
) else (
    if /i "%OUTPUT_ARG%"=="-h" goto :show_help
    if /i "%OUTPUT_ARG%"=="--help" goto :show_help
    
    set "FIRST_CHAR=%OUTPUT_ARG:~0,1%"
    if "!FIRST_CHAR!"=="-" (
        echo 错误: 无效的参数: %OUTPUT_ARG% 
        echo 请使用 -h 查看帮助信息 
        pause
        exit /b 1
    )
    
    set "OUTPUT_DIR=%OUTPUT_ARG%"
    set "IS_TEMP_DIR=0"
    echo 信息: 使用指定的输出目录 
)

set "OUTPUT_DIR=%OUTPUT_DIR:"=%"
if not "%OUTPUT_DIR:~-1%"=="\" (
    set "OUTPUT_DIR=%OUTPUT_DIR%\"
)

call :get_absolute_path "%OUTPUT_DIR%" OUTPUT_DIR_ABS
set "OUTPUT_DIR=%OUTPUT_DIR_ABS%"

REM ============================================
REM 显示 Banner
REM ============================================
echo.
echo ============================================================ 
echo           .NET 模板打包与安装工具 v2.9 
echo ============================================================ 
echo.
echo 当前目录: %CURRENT_DIR% 
echo 输出目录: %OUTPUT_DIR% 
if "%IS_TEMP_DIR%"=="1" (
    echo 目录类型: 临时目录 
) else (
    echo 目录类型: 用户指定 
)
echo.

REM ============================================
REM 创建输出目录
REM ============================================
if not exist "%OUTPUT_DIR%" (
    echo 创建: 输出目录不存在，正在创建... 
    echo 命令: mkdir "%OUTPUT_DIR%"
    mkdir "%OUTPUT_DIR%" 2>nul
    if errorlevel 1 (
        echo 错误: 无法创建目录: %OUTPUT_DIR% 
        pause
        exit /b 1
    )
    echo 成功: 目录已创建 
    echo.
) else (
    echo 信息: 输出目录已存在 
    echo.
)

REM ============================================
REM 查找项目文件
REM ============================================
set "CSPROJ_FILE="
for %%f in (*.csproj) do (
    set "CSPROJ_FILE=%%f"
    goto :found_csproj
)

:found_csproj
if "%CSPROJ_FILE%"=="" (
    echo 错误: 在当前目录未找到 .csproj 文件！ 
    echo 请确保在项目目录中运行此脚本。 
    echo.
    pause
    exit /b 1
)

echo 项目: 找到项目文件: %CSPROJ_FILE% 
echo.

for %%f in ("%CSPROJ_FILE%") do set "PROJECT_NAME=%%~nf"

REM 从 csproj 文件中读取 PackageId
set "PACKAGE_ID="
for /f "usebackq tokens=*" %%i in (`findstr /i "<PackageId>" "%CSPROJ_FILE%" 2^>nul`) do (
    set "LINE=%%i"
    set "LINE=!LINE:*<PackageId>=!"
    for /f "delims=<" %%a in ("!LINE!") do set "PACKAGE_ID=%%a"
    goto :found_package_id
)
:found_package_id

REM 如果找不到 PackageId，从文件名提取包名（去掉版本号）
if "%PACKAGE_ID%"=="" (
    echo 信息: 未在 csproj 中找到 PackageId，从文件名提取...
    set "PACKAGE_ID="
    for %%f in ("%OUTPUT_DIR%*.nupkg") do (
        set "NUPKG_BASENAME=%%~nf"
        call :extract_package_name "!NUPKG_BASENAME!"
        set "PACKAGE_ID=!NUPKG_NAME!"
        goto :found_package_from_file
    )
)
:found_package_from_file

if "%PACKAGE_ID%"=="" (
    echo 错误: 无法获取包 ID 
    pause
    exit /b 1
)

echo 包 ID: %PACKAGE_ID% 
echo.

REM ============================================
REM 步骤 1: 清理旧的 nupkg 文件
REM ============================================
:ask_clean
choice /C YN /M "是否清理输出目录中的旧 .nupkg 文件"

if errorlevel 2 (
    echo.
    echo 跳过: 保留现有文件 
    echo.
) else (
    echo.
    echo 清理: 正在删除旧的 .nupkg 文件... 
    echo 命令: del /q "%OUTPUT_DIR%*.nupkg"
    del /q "%OUTPUT_DIR%*.nupkg" 2>nul
    echo 完成: 已清理旧文件 
    echo.
)

REM ============================================
REM 步骤 2: 打包
REM ============================================
echo ------------------------------------------------------------ 
echo 步骤 1/3: 正在打包项目... 
echo ------------------------------------------------------------ 
echo.

set "PACK_COMMAND=dotnet pack "%CSPROJ_FILE%" --output %OUTPUT_DIR% --configuration %CONFIGURATION% %NO_BUILD%"
echo 命令: !PACK_COMMAND!
echo.

!PACK_COMMAND!

if errorlevel 1 (
    echo.
    echo 错误: 打包失败！请检查错误信息。 
    echo.
    pause
    exit /b %errorlevel%
)

echo.
echo 成功: 打包完成！ 
echo.

REM ============================================
REM 步骤 3: 查找生成的 nupkg
REM ============================================
echo ------------------------------------------------------------ 
echo 步骤 2/3: 查找生成的 NuGet 包... 
echo ------------------------------------------------------------ 
echo.

set "NUPKG_COUNT=0"
for %%f in ("%OUTPUT_DIR%*.nupkg") do (
    set /a NUPKG_COUNT+=1
)

if %NUPKG_COUNT% equ 0 (
    echo 错误: 未找到任何 .nupkg 文件！ 
    echo.
    pause
    exit /b 1
)

echo 找到 %NUPKG_COUNT% 个 .nupkg 文件: 
set "INDEX=1"
for %%f in ("%OUTPUT_DIR%*.nupkg") do (
    echo   %INDEX%. %%~nxf 
    set /a INDEX+=1
)
echo.

REM ============================================
REM 步骤 4: 安装模板
REM ============================================
:ask_install
echo ------------------------------------------------------------ 
echo 步骤 3/3: 安装模板到本地 
echo ------------------------------------------------------------ 
echo.
echo 请选择操作: 
echo   Y - 安装模板（会先卸载旧版本） 
echo   R - 强制重新安装（使用 --force，不卸载） 
echo   L - 仅列出已安装的模板 
echo   N - 跳过安装 
echo.

choice /C YRLN /M "请输入选项"

if errorlevel 4 (
    echo.
    echo 跳过: 未安装模板。 
) else if errorlevel 3 (
    call :list_templates
    echo.
    goto :ask_install
) else if errorlevel 2 (
    call :force_install_template
) else if errorlevel 1 (
    call :install_template
)

echo.

REM ============================================
REM 步骤 5: 清理临时文件夹
REM ============================================
if "%IS_TEMP_DIR%"=="1" (
    :ask_delete_temp
    echo ------------------------------------------------------------ 
    echo 清理: 临时文件夹管理 
    echo ------------------------------------------------------------ 
    echo.
    echo 当前使用的是临时文件夹: %OUTPUT_DIR% 
    echo 包含 %NUPKG_COUNT% 个 .nupkg 文件 
    echo.
    choice /C YN /M "是否删除临时文件夹及其内容"
    
    if errorlevel 2 (
        echo.
        echo 保留: 临时文件夹已保留: %OUTPUT_DIR% 
    ) else (
        echo.
        echo 删除: 正在删除临时文件夹... 
        echo 命令: rmdir /s /q "%OUTPUT_DIR%"
        cd /d "%CURRENT_DIR%" 2>nul
        rmdir /s /q "%OUTPUT_DIR%" 2>nul
        if errorlevel 1 (
            echo 警告: 无法完全删除临时文件夹，请手动清理 
        ) else (
            echo 成功: 临时文件夹已删除 
        )
    )
    echo.
)

REM ============================================
REM 完成
REM ============================================
echo ============================================================ 
echo                   所有操作已完成！ 
echo ============================================================ 
echo.
echo 项目名称: %PROJECT_NAME% 
echo 包 ID:    %PACKAGE_ID% 
echo 输出目录: %OUTPUT_DIR% 
if "%IS_TEMP_DIR%"=="1" (
    if errorlevel 2 (
        echo 临时目录: 已保留 
    ) else (
        echo 临时目录: 已删除 
    )
)
echo.
pause
exit /b 0

REM ============================================
REM 子程序: 获取绝对路径
REM ============================================
:get_absolute_path
setlocal
set "REL_PATH=%~1"
set "REL_PATH=%REL_PATH:\=/%"
if "%REL_PATH:~0,1%"=="/" (
    endlocal & set "%~2=%REL_PATH:/=\%"
    exit /b 0
)
if "%REL_PATH:~1,2%"==":\" (
    endlocal & set "%~2=%REL_PATH%"
    exit /b 0
)
if "%REL_PATH:~0,2%"=="\\" (
    endlocal & set "%~2=%REL_PATH%"
    exit /b 0
)
pushd "%REL_PATH%" 2>nul
if errorlevel 1 (
    mkdir "%REL_PATH%" 2>nul
    pushd "%REL_PATH%" 2>nul
)
set "ABS_PATH=%CD%"
popd
endlocal & set "%~2=%ABS_PATH%\"
exit /b 0

REM ============================================
REM 子程序: 提取包名（去掉末尾的版本号）
REM ============================================
:extract_package_name
set "FULLNAME=%~1"

REM 从末尾开始，去掉版本号（数字和点组成的部分）
set "TEMP_NAME=!FULLNAME!"
:loop
set "LAST_CHAR=!TEMP_NAME:~-1!"
echo !LAST_CHAR! | findstr /r "[0-9.]" >nul
if errorlevel 1 (
    set "NUPKG_NAME=!TEMP_NAME!"
    exit /b 0
)
set "TEMP_NAME=!TEMP_NAME:~0,-1!"
if "!TEMP_NAME!"=="" (
    set "NUPKG_NAME=!FULLNAME!"
    exit /b 0
)
goto :loop

REM ============================================
REM 子程序: 安装模板（先卸载再安装）
REM ============================================
:install_template
echo.
echo 卸载: 正在卸载旧版本模板... 
set "UNINSTALL_COMMAND=dotnet new uninstall %PACKAGE_ID%"
echo 命令: !UNINSTALL_COMMAND!
!UNINSTALL_COMMAND! 2>nul

if errorlevel 1 (
    echo 信息: 没有找到旧版本模板或卸载失败 
) else (
    echo 成功: 旧版本已卸载 
)
echo.

echo 安装: 正在安装新模板... 
echo.

set "INSTALL_SUCCESS=0"
for %%f in ("%OUTPUT_DIR%*.nupkg") do (
    echo   安装: %%~nxf 
    set "INSTALL_COMMAND=dotnet new install "%%f""
    echo   命令: !INSTALL_COMMAND!
    !INSTALL_COMMAND!
    
    if errorlevel 1 (
        echo   警告: 安装失败: %%~nxf 
    ) else (
        set "INSTALL_SUCCESS=1"
        echo   成功: 安装完成: %%~nxf 
    )
    echo.
)

if %INSTALL_SUCCESS% equ 1 (
    echo 成功: 模板安装完成！ 
    echo.
    call :list_templates
) else (
    echo 错误: 所有模板安装均失败！ 
)

goto :eof

REM ============================================
REM 子程序: 强制安装模板（不卸载，直接 --force）
REM ============================================
:force_install_template
echo.
echo 强制安装: 正在使用 --force 安装新模板（不卸载旧版本）... 
echo.

set "INSTALL_SUCCESS=0"
for %%f in ("%OUTPUT_DIR%*.nupkg") do (
    echo   安装: %%~nxf 
    set "INSTALL_COMMAND=dotnet new install "%%f" --force"
    echo   命令: !INSTALL_COMMAND!
    !INSTALL_COMMAND!
    
    if errorlevel 1 (
        echo   警告: 安装失败: %%~nxf 
    ) else (
        set "INSTALL_SUCCESS=1"
        echo   成功: 安装完成: %%~nxf 
    )
    echo.
)

if %INSTALL_SUCCESS% equ 1 (
    echo 成功: 模板强制安装完成！ 
    echo.
    call :list_templates
) else (
    echo 错误: 所有模板安装均失败！ 
)

goto :eof

REM ============================================
REM 子程序: 列出已安装模板
REM ============================================
:list_templates
echo.
echo 当前已安装的模板列表: 
echo ------------------------------------------------------------ 
set "LIST_COMMAND=dotnet new list %PACKAGE_ID%"
echo 命令: !LIST_COMMAND!
!LIST_COMMAND! 2>nul

if %errorlevel% equ 1 (
    echo   (未找到匹配的模板) 
)
echo ------------------------------------------------------------ 
goto :eof

REM ============================================
REM 帮助信息
REM ============================================
:show_help
echo.
echo 用法: %~nx0 [输出目录] 
echo.
echo 描述: 
echo   打包 .NET 项目并可选安装模板到本地。 
echo   如果不指定输出目录，会在当前目录创建临时文件夹。 
echo.
echo 参数: 
echo   输出目录     NuGet 包输出路径（可选） 
echo.
echo 交互流程: 
echo   1. 可选择是否清理旧的 .nupkg 文件 
echo   2. 执行 dotnet pack 打包 
echo   3. 可选择安装模板到本地 
echo   4. 如果是临时文件夹，可选择是否删除 
echo.
echo 示例: 
echo   %~nx0                     使用临时目录 
echo   %~nx0 ..\..\LocalNuget\   指定输出目录 
echo   %~nx0 C:\MyPackages\      指定绝对路径 
echo.
echo 选项: 
echo   -h, --help   显示此帮助信息 
echo.
pause
exit /b 0
