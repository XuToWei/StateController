# Changelog

## [1.1.0] - 2026-06-16
- 控制器链接（Link）由一对一升级为一对多，单个状态可同时联动多个目标控制器
- 自动迁移旧的一对一 Link 数据，升级后不丢失已有链接
- 重构编辑器 Link UI，每个状态下方竖排展示多条链接，支持增删
- 编辑器统一 Data/State 配色标识（Data 蓝青、State 橙黄），Link 行补充 Data:/State: 文字标签，便于区分
- 精简编辑器布局：移除独立的 Add Data 框，Data 的新增/重命名共用一个输入框（Add/Rename 双按钮），State 新增并入列表上方，整体更紧凑
- State Names 列表去掉上下移动按钮，改为直接拖拽排序；拖动时自动同步 LinkData、各子 State 数据顺序及被 Apply 状态的索引
- State 改名时该行的 link 编辑器内容保持显示不变（此前进入改名态会隐藏该行的 link 下拉）
- 重构：状态的 link 与各子 State 的值改为「按状态名」存储（不再按下标），拖拽改序无需任何顺序同步逻辑，彻底移除 EditorTrySyncStateOrder/快照/映射；旧资产（含 Demo）需在编辑器内重设
- 重构：StateControllerData 的 m_StateNames 与 m_StateLinkDatas 合并为单个 m_States（List<StateControllerState>，状态名与其 link 列表合为一条数据）；LinkData 更名为 StateControllerStateLink；序列化结构变更，旧资产需重新保存
- 重构：BaseBooleanLogicState 的 m_StateDataNames1/2 与原 m_StateDatas1/2 合并为 m_StateValues1/2（List<StateValue<bool>>，状态名与布尔值合为一条数据），移除并列的状态名列表；序列化结构变更，旧资产需重新保存
- 重构：BaseSelectableState<T> 的 m_StateDataNames 与原 m_StateDatas 合并为 m_StateValues（List<StateValue<T>>，状态名与值合为一条数据），移除并列的状态名列表；因泛型序列化要求，最低 Unity 版本提升至 2020.1；序列化结构变更，旧资产需重新保存

## [1.0.7] - 2025-01-22
- 重构 StateControllerData 状态切换逻辑，提取公共方法
- 优化 BaseBooleanLogicState 编辑器代码，减少重复
- 完善 README 文档，添加详细使用说明和 API 示例

## [1.0.6] - 2024-12-11
- 优化编辑器布局
- 添加复制粘贴功能

## [1.0.5] - 2024-7-9
- 命名优化

## [1.0.4] - 2024-6-5
- 添加State改名功能

## [1.0.3] - 2024-3-27
- 添加SelectableState换行显示的DrawSetting
- 添加StateImageColor
- 修复删除State编辑器报错

## [1.0.2] - 2024-3-20
- 添加控制器链接

## [1.0.1] - 2024-3-15
- 优化代码.
- 支持了CodeBind.
- 添加Demo.

## [1.0.0] - 2024-3-13