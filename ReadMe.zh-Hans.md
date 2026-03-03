# MsixInstaller

| [English](ReadMe.md) | 简体中文 |

GitHub Action 用于创建 MSIX 安装程序的包装器。该工具可以将 MSIX 包嵌入到独立的 .NET Framework 可执行文件中，便于分发和部署。

## 功能特性

- 📦 自动将 MSIX 包嵌入到 .NET Framework 4.8 可执行文件中
- 🎨 基于 WPF-UI 构建的现代化用户界面
- 🔄 支持自动主题跟随系统
- 📝 自动生成安装程序校验和
- 🏗️ 可配置的输出架构 (x86/x64/ARM/ARM64/Any)
- 🔍 从 MSIX 清单自动提取版本和架构信息

## 使用方式

### 作为 GitHub Action 使用

在您的工作流文件中添加以下步骤：

```yaml
- name: Create MSIX Installer
  uses: frg2089/MsixInstaller@main
  with:
    msix: 'path/to/your/app.msix'
    name: 'MyAppInstaller'
    arch: 'x64'  # 可选: neutral|x86|x64|arm|arm64
    version: '1.0.0.0'  # 可选: 默认从 MSIX 清单读取
```

### 输出参数

该 Action 会输出以下信息：

- `path`: 安装程序的完整路径
- `algorithm`: 校验和算法 (如 SHA256)
- `checksum`: 安装程序的校验值

示例：

```yaml
- name: Upload Installer
  uses: actions/upload-artifact@v4
  with:
    name: installer
    path: ${{ steps.create_installer.outputs.path }}
```

## 本地构建

### 前置要求

- .NET SDK 9.0.300 或更高版本
- .NET Framework 4.8

### 构建步骤

```powershell
# 构建项目
dotnet build

# 发布到指定架构
dotnet publish --configuration Release --arch x64 --artifacts-path artifacts
```

## 技术栈

- **框架**: .NET Framework 4.8 + WPF
- **UI 库**: WPF-UI 4.0.3
- **MVVM**: CommunityToolkit.Mvvm 8.4.0
- **资源嵌入**: Costura.Fody 6.0.0
- **语言**: C# 12

## 项目结构

```
MsixInstaller/
├── action.yml          # GitHub Action 配置
├── scripts/
│   └── Pack-MsixInstaller.ps1  # 打包脚本
├── src/
│   └── MsixInstaller/  # WPF 应用程序源码
│       ├── MainWindow.xaml    # 主窗口
│       ├── Program.cs         # 入口点
│       └── Resources/         # 资源文件
└── artifacts/          # 构建输出目录
```

## 许可证

本项目采用开源许可证，详见 [LICENSE](LICENSE) 文件。

## 作者

frg2089
