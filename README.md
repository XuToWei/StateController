# StateController | 状态控制器

<div align="center">

**基于 FairyGUI 控制器理念的强大状态管理系统**

**支持任意状态扩展 · 可视化编辑 · 实时预览**

[![Unity Version](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.6-orange.svg)](CHANGELOG.md)

</div>

---
## 🎯 核心特性

- **🌐 通用性强** - 不仅限于UI，适用于任何需要状态切换的场景（游戏逻辑、AI、音效、关卡等）
- **🔌 高度可扩展** - 通过继承基类轻松创建自定义状态，满足各种业务需求
- **🎨 可视化编辑器** - 直观的 Inspector 界面，所见即所得的编辑体验
- **🔄 多种状态类型** - 内置布尔逻辑和可选择状态，覆盖常见使用场景
- **⚡ 性能优化** - 高效的状态切换机制，零 GC 分配，开销极小
- **🛠️ 开发者友好** - 与 [CodeBind](https://github.com/XuToWei/CodeBind) 无缝集成，自动生成访问代码
- **📦 生产就绪** - 已在多个商业项目中验证，稳定可靠

## 💡 为什么选择 StateController？

> **🌟 重要特性：StateController 不仅仅是 UI 工具！**  
> 虽然内置了丰富的 UI 扩展，但它的核心是一个**通用状态管理系统**，适用于任何需要状态切换的场景：UI界面、游戏逻辑、AI行为、关卡机制、音效系统等。

传统的状态管理往往需要编写大量重复代码，状态切换逻辑分散在各处，难以维护。StateController 提供了一套优雅的解决方案：

✨ **一键切换状态** - 无需编写复杂的条件判断逻辑  
✨ **实时预览功能** - 编辑器内即可查看效果，大幅提升开发效率  
✨ **集中管理状态** - 所有状态数据统一管理，清晰易维护  
✨ **类型安全访问** - 配合 CodeBind 实现编译时检查，杜绝运行时错误  
✨ **场景无关性** - 不仅限于UI，支持任何GameObject的状态管理

## 🎯 应用领域

StateController 是一个真正的**全场景状态管理解决方案**，可应用于：

| 领域 | 应用示例 |
|-----|---------|
| **🎨 UI 系统** | 界面切换、按钮状态、主题皮肤、多语言、动画控制 |
| **🎮 游戏玩法** | 角色状态机、武器系统、技能系统、buff系统、关卡阶段 |
| **🤖 AI 系统** | 敌人行为树、NPC状态、巡逻逻辑、决策系统 |
| **🌦️ 环境系统** | 天气切换、昼夜循环、季节变化、场景氛围 |
| **🔊 音频系统** | 背景音乐切换、音效模式、音量档位、空间音效 |
| **🎬 剧情系统** | 对话状态、章节切换、任务阶段、事件触发 |
| **⚙️ 系统设置** | 画质档位、难度选择、控制方案、游戏模式 |
| **🏗️ 关卡机制** | 机关门、传送点、陷阱状态、交互物品 |

## 📋 核心状态类型

### 1️⃣ **布尔逻辑状态** (`BaseBooleanLogicState`)

适用于需要根据多个条件组合（与/或逻辑）来响应的元素，让复杂的条件判断变得简单直观。

**典型应用场景：**

**🎨 UI 相关：**
- 根据多个游戏状态（等级、金币、道具）显示/隐藏 UI 面板
- 根据复杂条件（权限、进度、时间）启用/禁用按钮

**🎮 游戏逻辑：**
- 根据多个触发器状态开启机关门（需要同时激活3个开关）
- 敌人AI行为判断（血量低 AND 距离玩家近 = 逃跑）
- 技能释放条件检测（魔法值足够 AND 冷却完成 AND 目标在范围内）
- 成就解锁判断（完成任务A OR 任务B OR 任务C）

### 2️⃣ **可选择状态** (`BaseSelectableState`)

适用于任何需要在多个预定义状态间切换的场景，实现清晰的状态机逻辑。

**典型应用场景：**

**🎨 UI 相关：**
- 标签页导航系统（主界面、背包、设置）
- UI 主题切换（日间模式、夜间模式）
- 多语言支持（中文、英文、日文）

**🎮 游戏逻辑：**
- 角色状态机（待机、行走、奔跑、跳跃、攻击、受伤）
- 敌人AI状态（巡逻、追击、攻击、撤退）
- 武器切换系统（近战、远程、魔法）
- 天气系统（晴天、雨天、雪天、雾天）
- 关卡难度（简单、普通、困难、地狱）
- 游戏阶段控制（准备、进行中、结算）
- 音效模式（静音、低音量、标准、高音质）

## 🚀 快速开始

### 📦 安装方式

**方式一：通过 Unity Package Manager（推荐）**

1. 打开 Unity 编辑器
2. 菜单栏选择 `Window` → `Package Manager`
3. 点击左上角 `+` 按钮
4. 选择 `Add package from git URL`
5. 输入：`https://github.com/XuToWei/StateController.git`
6. 点击 `Add` 完成安装

**方式二：手动导入**

1. 下载本仓库的源代码
2. 将 `StateController` 文件夹复制到项目的 `Packages` 目录

### 💻 基础使用

**方式一：通过 GetData 访问（推荐用于链式调用）**

```csharp
using StateController;

// 获取状态控制器组件
StateControllerMono stateController = GetComponent<StateControllerMono>();

// 设置选中状态 - 通过状态名称
stateController.GetData("ButtonState").SelectedName = "Active";

// 设置选中状态 - 通过索引
stateController.GetData("TabController").SelectedIndex = 1;

// 获取当前状态名称
string currentState = stateController.GetData("TabController").SelectedName;

// 获取当前状态索引
int currentIndex = stateController.GetData("TabController").SelectedIndex;
```

**方式二：通过封装方法访问（推荐用于频繁调用）**

```csharp
// 设置状态
stateController.SetSelectedName("ButtonState", "Active");
stateController.SetSelectedIndex("TabController", 1);

// 获取状态
string stateName = stateController.GetSelectedName("TabController");
int stateIndex = stateController.GetSelectedIndex("TabController");

// 获取所有状态名称
string[] allStates = stateController.GetStateNames("TabController");
```

**游戏逻辑控制示例：**

```csharp
// 控制角色状态机
StateControllerMono characterController = character.GetComponent<StateControllerMono>();
characterController.SetSelectedName("CharacterState", "Running");

// 控制敌人AI
StateControllerMono enemyAI = enemy.GetComponent<StateControllerMono>();
enemyAI.SetSelectedName("AIState", "Chase");

// 控制关卡天气
StateControllerMono weatherController = level.GetComponent<StateControllerMono>();
weatherController.SetSelectedName("Weather", "Rainy");

// 监听状态变化
var data = stateController.GetData("CharacterState");
data.OnSelectedNameChanged += (newState) => 
{
    Debug.Log($"状态切换到: {newState}");
};
```

### 👁️ 实时预览 - 极大提升开发效率！

这是 StateController 最便捷的特性之一：

只需在 Inspector 中点击 **Apply** 按钮，即可**在编辑器中实时预览**状态变化，**无需进入播放模式**！

告别传统的"修改代码 → 运行游戏 → 查看效果 → 退出游戏"的繁琐流程，无论是调试 UI 界面、测试游戏逻辑，还是调整环境效果，都能快速迭代、高效开发。

![状态控制器演示](Images~/image.png)

## 📦 内置扩展状态

StateController 开箱即用，提供多个强大的扩展状态。虽然这些扩展主要针对 UI，但**核心框架支持任何自定义扩展**：

| 扩展状态 | 功能说明 | 使用场景 |
|---------|---------|---------|
| **StateGameObjectForActive** | 控制 GameObject 激活状态 | 显示/隐藏面板、开关特效、控制机关 |
| **StateImageForSprite** | 动态切换 Image 精灵图 | 图标状态、道具图片、状态指示器 |
| **StateImageForSpriteColor** | 根据状态修改 Image 颜色 | 按钮状态、主题颜色、状态提示 |
| **StateTextForText** | 根据状态更改文本内容 | 多语言、动态提示、状态文本 |
| **StateRectTransformForAnchoredPosition** | 实现 UI 位置动画 | 面板滑动、元素移动 |

### 自定义扩展状态

只需继承 `BaseState`、`BaseBooleanLogicState` 或 `BaseSelectableState`，即可轻松创建满足特定需求的自定义状态。

**UI 示例：音量控制**

```csharp
using UnityEngine;

namespace StateController
{
    // 定义音量数据
    [System.Serializable]
    public class VolumeData
    {
        public float volume;
    }

    public class StateAudioVolume : BaseSelectableState<VolumeData>
    {
        [SerializeField] private AudioSource audioSource;
        
        protected override void OnStateInit()
        {
            // 初始化时获取组件
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
        
        protected override void OnStateChanged(VolumeData stateData)
        {
            // 根据状态数据设置音量
            audioSource.volume = stateData.volume;
        }
    }
}
```

**游戏逻辑示例：敌人 AI 状态**

```csharp
using UnityEngine;
using UnityEngine.AI;

namespace StateController
{
    // 定义AI状态数据
    [System.Serializable]
    public class AIStateData
    {
        public float moveSpeed;
        public string animationTrigger;
    }

    public class StateEnemyAI : BaseSelectableState<AIStateData>
    {
        private NavMeshAgent m_Agent;
        private Animator m_Animator;
        
        protected override void OnStateInit()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
        }
        
        protected override void OnStateChanged(AIStateData stateData)
        {
            // 根据状态数据设置AI行为
            m_Agent.speed = stateData.moveSpeed;
            m_Animator.SetTrigger(stateData.animationTrigger);
        }
    }
}
```

**环境控制示例：天气系统**

```csharp
using UnityEngine;

namespace StateController
{
    // 定义天气数据
    [System.Serializable]
    public class WeatherData
    {
        public bool enableRain;
        public bool enableSnow;
        public float sunLightIntensity;
    }

    public class StateWeatherSystem : BaseSelectableState<WeatherData>
    {
        [SerializeField] private ParticleSystem rainEffect;
        [SerializeField] private ParticleSystem snowEffect;
        [SerializeField] private Light sunLight;
        
        protected override void OnStateInit()
        {
            // 初始化时可以做一些准备工作
        }
        
        protected override void OnStateChanged(WeatherData stateData)
        {
            // 根据状态数据切换天气效果
            rainEffect.gameObject.SetActive(stateData.enableRain);
            snowEffect.gameObject.SetActive(stateData.enableSnow);
            sunLight.intensity = stateData.sunLightIntensity;
        }
    }
}
```

## 🔧 高级功能

### CodeBind 集成（强烈推荐 ⭐）

配合 [CodeBind](https://github.com/XuToWei/CodeBind) 使用，可以实现**自动代码生成**，大幅提升开发体验！

**设置步骤：**

1. 安装 [CodeBind](https://github.com/XuToWei/CodeBind) 插件
2. 在 Unity 中添加脚本定义符号：`STATE_CONTROLLER_CODE_BIND`
   - 路径：`Edit` → `Project Settings` → `Player` → `Scripting Define Symbols`
3. 绑定代码时会自动生成状态数据访问代码

**带来的优势：**

- ✅ **类型安全** - 编译时检查，避免拼写错误
- ✅ **智能提示** - IDE 自动补全，提升编码速度
- ✅ **重构友好** - 重命名时自动更新引用
- ✅ **减少代码量** - 无需手动编写字符串索引

**使用对比：**

```csharp
// 传统方式 - 字符串索引，容易出错
stateController.GetData("ButtonState").SelectedName = "Active";

// CodeBind 生成 - 类型安全，智能提示
stateController.ButtonState.SelectedName = ButtonStateType.Active;
```

## 🎮 示例项目

安装后可在 Unity Package Manager 中导入 Demo 示例，快速体验 StateController 的强大功能。

## ⚙️ 环境要求

| 要求 | 版本/说明 |
|-----|----------|
| **Unity** | 2019.4 或更高版本 |
| **依赖插件** | [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) |

> **⚠️ 重要提示**  
> Odin Inspector 是付费插件，需要从 Unity Asset Store 单独购买。该插件用于提供强大的编辑器扩展功能，是 StateController 正常运行的必要依赖。

## 💬 社区与支持

我们重视每一位用户的反馈，欢迎通过以下方式参与交流：

- **💬 QQ 交流群**: `949482664` - 加入讨论，获取实时帮助
- **🐛 问题反馈**: [GitHub Issues](https://github.com/XuToWei/StateController/issues) - 报告 Bug 或提出功能建议

## 📄 开源许可

本项目采用 [MIT 许可证](LICENSE) - 您可以自由使用、修改和分发本项目。

## ⭐ 支持项目

如果 StateController 对您有帮助，请考虑：

- 给项目点个 ⭐ Star，让更多人发现它
- 分享给您的朋友和同事
- 在项目中使用并提供反馈

---

<div align="center">

**用 ❤️ 打造 | Made by [Xu Wei](https://github.com/XuToWei)**

感谢您使用 StateController！

</div>