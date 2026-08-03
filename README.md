# 防止游戏失焦工具 / Game Focus Guard

`Game Focus Guard` 是一个本地 Windows 小工具，用来降低游戏窗口失去焦点、切到后台后暂停，或者因焦点变化导致交互异常的影响。

它主要面向需要保持窗口焦点的游戏场景，例如 **《地平线6》**、多显示器游戏环境，以及容易因为误点、切窗或后台操作而中断的桌面游戏。

[English README](./README.en.md)

## 项目简介

这个项目采用一个最小化的 WinForms 桌面界面，加上一个原生 hook 组件，实现对当前目标窗口的焦点保护控制。

当前版本支持：

- 单实例运行
- 通过按钮或全局热键启用/关闭焦点保护
- 在界面内重新绑定热键
- 自动保存热键设置，并在下次启动时恢复
- 可选的主窗口始终置顶，并在下次启动时恢复
- 显示当前热键、候选窗口、目标窗口、hook 状态和启用状态

## 适用场景

本工具适合本地测试和日常使用中这些情况：

- 类似 **《地平线6》** 这样对焦点变化比较敏感的游戏
- 多显示器环境下容易误切出窗口的游戏
- 想尽量减少窗口失焦、后台暂停或输入中断影响的游戏场景

需要说明的是，实际效果仍然会因游戏本身的渲染模式、输入处理方式、反作弊或防篡改限制而有所不同。

## 快速开始

如果你使用已经打好的发布包：

1. 下载 `win-x64` 压缩包
2. 解压到任意本地目录
3. 以管理员权限运行 `FocusTool.Ui.exe`
4. 先把目标游戏窗口切到前台
5. 再使用热键或主界面的主按钮启用/关闭焦点保护

默认热键：

```text
Ctrl + Shift + Alt + T
```

修改热键的方法：

1. 点击 `改热键`
2. 按下新的组合键
3. 新热键会立即生效并自动保存
4. 在重绑过程中按 `Esc` 可以取消

如果程序已经在运行，再次启动时会直接唤醒已有实例，而不是再打开一个新的窗口。

勾选 `始终置顶` 后，工具主窗口会保持在其他普通窗口上方。该设置会自动保存。

## 从源码构建

环境要求：

- Windows
- 带 Windows 桌面支持的 .NET SDK
- Visual Studio 2022 Build Tools 或 Visual Studio 2022 Community，且已安装 C++ 工具链

构建命令：

```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj
```

说明：

- 推荐直接把 `FocusTool.Ui.csproj` 作为构建入口
- UI 项目会在构建后自动调用 `FocusTool.Hook\Build-Hook.ps1`
- 构建脚本会使用本机 Visual Studio C++ 工具链编译原生 hook DLL，并复制到 UI 输出目录

构建后运行：

```powershell
.\FocusTool.Ui\bin\Debug\net10.0-windows\FocusTool.Ui.exe
```

## 构建便捷运行包

仓库提供统一的发布脚本：

```powershell
.\Build-Release.ps1 -Version 0.4.0
```

脚本会生成自包含的 Windows x64 单文件程序，并将运行时必须独立存在的 `FocusTool.Hook.dll` 一并打包。最终压缩包只包含：

- `FocusTool.Ui.exe`
- `FocusTool.Hook.dll`
- `LICENSE`

用户无需预先安装 .NET Runtime。每个版本标签都通过 GitHub Actions 从同一份标签源码构建 Release，避免源码、标签和运行包不一致。

## 仓库结构

- `FocusTool.Ui/`
  - WinForms 桌面界面
  - 热键管理
  - 单实例控制
  - 配置持久化
- `FocusTool.Hook/`
  - 原生 hook 组件
  - 原生 DLL 构建脚本

## 当前限制

- 当前仍是一个原型版本，主要面向本地实验和实际场景验证
- 一次只处理一个目标窗口
- 暂时不提供安装器
- 不保证对所有游戏都完全兼容

## License

MIT License. See [LICENSE](./LICENSE).
