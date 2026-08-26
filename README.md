# Zombie Siege

一个基于 **Unity 2022.3 LTS** 开发的第三人称 3D 塔防 Demo。玩家可以选择英雄进入不同地图，一边直接攻击丧尸，一边在固定建造点部署和升级炮塔，保护地图中央的核心区域。

> 当前项目版本：`1.0`  
> 推荐 Unity 版本：`2022.3.62f3`

## 游戏玩法

一局游戏的基本流程如下：

1. 在主菜单选择英雄；部分英雄需要使用累计获得的金币解锁。
2. 从疯狂树林、极寒雪地、熔岩旱土三个关卡中选择一个进入。
3. 操作英雄攻击不断生成的丧尸，并靠近建造点部署炮塔。
4. 击杀丧尸获得局内金币，用于建造或升级炮塔。
5. 清除全部波次即可通关；核心区域生命值归零则挑战失败。
6. 结算奖励会加入玩家总金币，并保存到本地。

## 主要特性

- 第三人称角色移动、旋转、翻滚、近战与射击攻击
- 6 名可选英雄，包含攻击力和解锁费用差异
- 3 个可游玩的关卡场景，各自拥有独立地图描述和核心生命值
- 基于 NavMesh 的丧尸寻路、波次生成和核心攻击逻辑
- 加农炮、机枪炮、魔法炮 3 条建造路线，每条可升级至 3 级
- 支持单体与范围两类炮塔攻击方式
- 主菜单、选角、选关、设置、战斗 HUD 和结算界面
- 背景音乐、音效开关及音量设置
- 基于 JSON 的角色、场景、怪物和炮塔配置
- 玩家金币、英雄解锁状态和音频设置本地持久化

## 操作说明

| 操作 | 按键 |
| --- | --- |
| 移动 | `W` `A` `S` `D` 或方向键 |
| 转向 | 鼠标水平移动 |
| 攻击 | 鼠标左键 |
| 下蹲 | 按住左 `Shift` |
| 翻滚 | `R` |
| 建造炮塔 | 靠近空建造点后按 `1` / `2` / `3` |
| 升级炮塔 | 靠近可升级炮塔后按空格键 |

## 游戏内容

### 英雄

项目当前配置了 6 名英雄。英雄数据定义在 `Assets/StreamingAssets/RoleInfo.json`，包含资源路径、攻击力、展示名称、解锁费用、攻击类型和命中特效。

### 关卡

| 关卡 | 场景文件 | 初始局内金币 | 核心生命值 |
| --- | --- | ---: | ---: |
| 疯狂树林 | `GameScene1` | 200 | 200 |
| 极寒雪地 | `GameScene2` | 200 | 100 |
| 熔岩旱土 | `GameScene3` | 200 | 100 |

### 炮塔

| 路线 | 攻击方式 | 特点 | 等级数 |
| --- | --- | --- | ---: |
| 加农炮 | 单体 | 较高单次伤害 | 3 |
| 机枪炮 | 单体 | 较短攻击间隔 | 3 |
| 魔法炮 | 范围 | 同时攻击范围内多个目标 | 3 |

炮塔的价格、攻击力、攻击范围、攻击间隔、升级目标和资源路径统一配置在 `Assets/StreamingAssets/TowerInfo.json`。

## 技术栈

- Unity `2022.3.62f3`
- C#
- Unity UI（UGUI）与 TextMesh Pro
- Unity AI Navigation / NavMesh
- Animator 动画状态机与 Animation Event
- Resources 动态资源加载
- LitJson 数据序列化
- 旧版 Unity Input Manager

## 快速开始

### 环境要求

- Unity Hub
- Unity Editor `2022.3.62f3`，或同一 `2022.3 LTS` 系列的兼容版本
- Windows 开发环境（项目当前以键盘和鼠标操作为主）
- Git LFS，用于正确拉取仓库中的大体积资源

### 获取并运行

```bash
git lfs install
git clone https://github.com/huang494824/zombie-siege.git
cd zombie-siege
```

随后在 Unity Hub 中选择“添加磁盘中的项目”，打开仓库目录。等待 Unity 完成资源导入后，打开 `Assets/Scenes/BeginScene.unity`，点击播放按钮即可运行。

项目已在 Build Settings 中按以下顺序配置场景：

1. `Assets/Scenes/BeginScene.unity`
2. `Assets/Scenes/GameScene1.unity`
3. `Assets/Scenes/GameScene2.unity`
4. `Assets/Scenes/GameScene3.unity`

如需构建可执行文件，请在 Unity 中打开 `File > Build Settings`，选择目标平台后执行构建。

## 项目结构

```text
Assets/
├─ ArtRes/                 # 配置表源文件等美术/数据资源
├─ Resources/              # 运行时动态加载的角色、怪物、炮塔、UI、音效和特效
├─ Scenes/                 # 主菜单与三个游戏关卡
├─ Scripts/
│  ├─ BeginScene/          # 主菜单、选角、选关、设置和菜单镜头逻辑
│  ├─ Data/                # 游戏数据结构与统一数据管理
│  ├─ GameScene/           # 波次、建造点、战斗对象、摄像机和战斗 UI
│  ├─ Json/                # JSON 读取、保存与 LitJson
│  ├─ UI/                  # 通用面板基类和 UI 管理器
│  ├─ GameFrameSetting.cs  # 目标帧率设置
│  └─ Main.cs              # 主菜单 UI 入口
└─ StreamingAssets/        # 可编辑的默认游戏配置 JSON
```

## 核心模块

| 模块 | 主要职责 |
| --- | --- |
| `GameDataMgr` | 加载全部配置，维护玩家、音频和当前英雄数据，提供本地保存与音效播放入口 |
| `JsonMgr` | 优先读取 `StreamingAssets` 默认配置，并将运行时数据写入 `Application.persistentDataPath` |
| `UIManager` / `BasePanel` | 按面板类型从 `Resources/UI` 动态创建界面，管理显示、淡入淡出和销毁 |
| `GameLevelMgr` | 初始化关卡、创建玩家、统计波次与存活怪物、判断胜利并清理关卡状态 |
| `MonsterPoint` | 根据场景参数按波次和时间间隔生成怪物 |
| `PlayerObject` | 处理玩家输入、动画状态、近战/射击判定和局内金币 |
| `TowerPoint` / `TowerObject` | 处理炮塔建造、升级、索敌及单体/范围攻击 |
| `MonsterObject` | 处理 NavMesh 寻路、攻击、受伤、死亡和击杀奖励 |
| `MainTowerObject` | 维护核心区域生命值并触发失败结算 |

## 数据配置

`Assets/StreamingAssets` 中的 JSON 文件负责静态游戏数据：

| 文件 | 内容 |
| --- | --- |
| `RoleInfo.json` | 英雄属性、解锁费用和资源路径 |
| `SceneInfo.json` | 关卡名称、场景名、初始金币和核心生命值 |
| `MonsterInfo.json` | 怪物攻击、生命、速度、攻击间隔和动画控制器 |
| `TowerInfo.json` | 炮塔价格、攻击参数、升级链和资源路径 |

角色、场景、怪物和炮塔配置会从 `StreamingAssets` 读取；运行后产生的 `PlayerData.json` 和 `MusicData.json` 则保存在 Unity 的 `Application.persistentDataPath` 下。

## 运行流程

```text
BeginScene
  └─ 主菜单
      └─ 选择英雄
          └─ 选择关卡
              └─ GameScene1 / GameScene2 / GameScene3
                  ├─ 玩家与炮塔共同防守
                  ├─ 清除全部波次 → 胜利结算
                  └─ 核心生命值归零 → 失败结算
```

## 开发约定

- UI 面板类名需与 `Assets/Resources/UI` 下对应预制体名称一致。
- 配置中的 `res`、`animator`、`imgRes`、`eff` 等路径均相对于 `Assets/Resources`，且不包含扩展名。
- 角色和炮塔配置目前通过 `id - 1` 访问列表，新增或调整条目时应保持 ID 从 1 开始且连续。
- 新增游戏场景后，需要同步更新 `SceneInfo.json` 和 Unity Build Settings。
- 怪物场景对象需要正确配置 NavMesh、Animator、Layer 和 Animation Event。

## 许可说明

本仓库当前未声明开源许可证。除非项目所有者另行授权，否则请勿复制、分发或用于商业用途。项目内美术、模型、动画、字体和音频资源的许可请以各自原始来源为准。
