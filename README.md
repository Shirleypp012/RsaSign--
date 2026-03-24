
RsaSignDemo (C# RSA 数字签名演示)

本项目是一个基于 C# (.NET) 的简单演示程序，旨在展示如何使用 .NET 内置的 System.Security.Cryptography 命名空间来实现 RSA 非对称加密算法 中的 数字签名 (Digital Signature) 和 验签 (Verification) 功能。

🚀 功能特点

密钥生成：自动生成 RSA 公钥/私钥对。
数据签名：使用私钥对指定字符串或数据进行数字签名。
签名验证：使用公钥验证签名的有效性，确保数据未被篡改且来源可信。
纯原生实现：仅依赖 .NET 标准库，无需安装第三方 NuGet 包。
控制台交互：简单的命令行界面，方便测试和查看结果。

🛠️ 环境要求

在运行此项目之前，请确保你的计算机已安装以下环境：

.NET SDK (版本 6.0, 7.0, 8.0 或更高版本)
  下载地址: https://dotnet.microsoft.com/download
  检查安装: 在终端输入 dotnet --version

📦 快速开始

克隆项目

bash
git clone https://github.com/Shirleypp012/RsaSignDemo.git
cd RsaSignDemo

运行项目

你可以选择以下两种方式之一来运行：

方式 A：直接运行源码 (推荐开发者)
如果你已安装 .NET SDK，直接在项目根目录运行：

bash
dotnet run

方式 B：运行可执行文件 (普通用户)
如果你下载的是 Release 版本或不想安装 SDK：
注意：直接双击 RsaSignDemo.exe 需要电脑预装 .NET Desktop Runtime。
如果未安装运行时，程序会提示错误并引导你下载安装。进阶：若要生成无需安装环境的独立 exe，请参考下方的“发布独立版本”章节。

预期输出

程序运行后，控制台将显示类似以下信息：

<img width="554" height="344" alt="图片" src="https://github.com/user-attachments/assets/61597cf3-56ad-4994-9429-cc185296400a" />


📂 项目结构

text
RsaSignDemo/
├── Program.cs              # 主程序入口，包含签名与验签逻辑
├── RsaSignDemo.csproj      # 项目配置文件
├── README.md               # 本说明文档
├── .gitignore              # Git 忽略文件配置
└── bin/                    # (编译输出目录，通常不上传)
└── obj/                    # (临时对象目录，通常不上传)

🔍 核心代码逻辑简述

本项目主要使用了 .NET 中的 RSA 类：

创建实例: using var rsa = RSA.Create(2048);
签名: 使用 rsa.SignData() 方法，配合 SHA256 哈希算法生成签名。
验签: 使用 rsa.VerifyData() 方法，对比原始数据和签名，返回布尔值。
(具体实现细节请参阅 Program.cs 源代码)

📤 发布独立版本 (Self-contained)

如果你希望生成一个不需要安装 .NET 环境即可在任何 Windows 电脑上运行的 .exe 文件，请使用以下命令：

bash
dotnet publish -c Release -r win-x64 --self-contained true

生成的文件位于：
bin/Release/netX.X/win-x64/publish/RsaSignDemo.exe

这个文件体积较大（约 60MB+），但包含了所有运行时依赖，可分发给任何用户直接使用。

📝 许可证

本项目采用 MIT License 开源协议。你可以自由地使用、修改和分发代码。

🤝 贡献

欢迎提交 Issue 或 Pull Request 来改进这个项目！
Created with ❤️ using Cursor & .NET

