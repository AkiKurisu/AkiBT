[![GitHub release](https://img.shields.io/github/release/AkiKurisu/AkiBT.svg?style=social)](https://github.com/AkiKurisu/AkiBT/releases)
[![Star on GitHub](https://img.shields.io/github/stars/AkiKurisu/AkiBT.svg)](https://github.com/AkiKurisu/AkiBT/stargazers)
# 行为树 AkiBT Version 1.4.1 简介 Intro

AkiBT是一款由[爱姬kurisu](https://space.bilibili.com/20472331)优化并拓展的行为树结点编辑器.  
行为树编辑器衍生自[UniBT](https://github.com/yoshidan/UniBT),原作者[Yoshida](https://github.com/yoshidan/).

AkiBT is a visual node editor derived from UniBT created by Yoshida for making behavior tree or other tree-based function. AkiKurisu extends it with more features so that you can enjoy it.

## 安装 Setup
1. Download [Release Package](https://github.com/AkiKurisu/AkiBT/releases)
2. Using git URL to download package by Unity PackageManager ```https://github.com/AkiKurisu/AkiBT.git```


## 支持的版本 Supported Version

* Unity 2021.3 or later.


## 特点 Features
* 支持使用可视化节点编辑器构造行为树
* Supports constructing behavior tree by visual node editor.
* 支持运行时可视化结点状态
* Supports visualizing active node in runtime.
* 非常便于拓展和自定义新的行为
* Easily add original behaviors(Action,Conditional,Composite,Decorator).

## 使用方式 How To Use 

Modified from [UniBT documentation](https://github.com/yoshidan/UniBT)

   <img src="Images/demo.jpg" />

1. Add `AkiBT.BehaviorTree` component for any GameObject.  
   <img src="Images/started1.png" width="480"/>
2. `Open Graph Editor` button opens GraphView for Behavior Tree.  
   <img src="Images/started2.jpg" width="480"/>
3. Add behaviors and set parameters.  
4. Finally press save button on tool bar of the editor window. (If invalid node found the color of the node become red.)  
   <img src="Images/started3.gif" width="480"/>  
5. Run the unity application. you can see node status in the editor window.  
   <img src="Images/started4.jpg" width="480"/>
   
   * The red node means that last `Update` returned Status.Failure`.
   * The green node means that last `Update` returned `Status.Success`.
   * The yellow node means that last `Update` returned `Status.Running`.
6. you can save the GameObject with `AkiBT.BehaviorTree` as prefab or save to ScriptableObject or save to json file.

7. Tutorial Video On BiliBili 
   
   [开源行为树AkiBT使用教程](https://www.bilibili.com/video/BV1Jd4y187XL/)

## 工作原理 How It Works

* `AkiBT.BehaviorTree` updates child nodes in `Update` timing when the UpdateType is `UpdateType.Auto`.
* If you want to update at any time, change UpdateType to `UpdateType.Manual` and call `BehaviorTree.Tick()`;
* Only `AkiBT.BehaviorTree` is the `MonoBehavior`. Each node is just a C# Serializable class.
  
  
## 拓展结点 Extend Node

***See [API Document](./API.md)***


## 编辑器功能说明 Editor Function Info

1. 你可以从上侧工具栏选择```Copy From SO```，也可以将BehaviorTreeSO、BehaviorTree组件或挂载BehaviorTree的GameObject、Json文件拖拽进编辑器中进行复制粘贴。
   
   You can select ```Copy From SO``` from the upper toolbar, or you can drag BehaviorTreeSO, BehaviorTree components, or GameObject and Json files that mount BehaviorTree into the editor to copy and paste.

    <img src="Images/DragDrop.gif" width="1920"/>

2. 共享变量SharedVariable可以在黑板中添加,变量目前支持Float、Int、Vector3、Bool、String、UnityEngine.Object以及其继承子类型
   
    SharedVariable let you have access to add it in a blackboard and share value between different node.Now it supports Float,Int,Vector3,Bool,String,UnityEngine.Object and its subclass

    <img src="Images/SharedVariable.png" />

   * 修改共享变量名称的方式为双击变量,为空时自动删除
    
        You can edit variable's name by double-click it and the variable will auto delate when it's name becomes empty.

    * Inspector中也可以进行共享变量的修改和删除功能
   
      You can edit the value of SharedVariable in the inspector.

    <img src="Images/ChangeVariableInInspector.png" width="480"/>

3. 你可以在ProjectSetting中设置AkiBT编辑器或者其余继承自AkiBT的编辑器的搜索遮罩。你可以设置工作流中需要的Group类型（Group特性相关见上文）,没有添加Group特性的结点不会被过滤。

    You can edit the setting in ProjectSetting where you can add mask for the node group you want to see in the SearchWindow.To be mentioned,the mask is relating to the editor you used.As default,the AkiBT editor is named with 'AkiBT',so you should edit the 'EditorName' with it. 

   <img src="Images/Setting.png" width="480"/>

4. Json序列化最佳实践：在Editor中使用Json序列化会保存引用``UnityEngine.Object``（以下简称UObject）对象的GUID，Runtime时Json反序列时无法获取UObject对象，你需要在Runtime通过别的方式加载所需要的UObject对象，例如将行为树中对UObject的引用全部改为SharedTObject和SharedObject，在Runtime通过其名称从你的资源加载方案中获取，例如Addressable的资源地址或AssetBundle的文件路径。

  Json Serialization best practice: Using Json serialization in the Editor will save the GUID that refers to the ``UnityEngine.Object ``(hereinafter referred to as UObject) object. The UObject object cannot be obtained when Json is deserialized at runtime. You need to load the required objects in other ways at runtime. UObject objects, for example, change all references to UObject in the behavior tree to SharedTObject and SharedObject, and obtain them from your resource loading scheme through their names at runtime, such as the resource address of Addressable or the file path of AssetBundle.

## 拓展功能 Extra Module

1. 运行时更新 Runtime Update Support
   
   你可以使用[AkiBTDSL](https://github.com/AkiKurisu/AkiBTDSL)实现运行时或在项目外编辑行为树

   You can have access to runtime-updating or editing outside project by using [AkiBTDSL](https://github.com/AkiKurisu/AkiBTDSL) 

2. 开发便捷服务 User Service

    插件目前内置了新的User Service(Tools/AkiBT/AkiBT User Service), 提供了两个功能Serialize Service和Search Service

    * Serialize Service:由于AkiBT使用ScriptableObject进行数据存储,在修改结点的字段名称时会导致数据的丢失（该问题可以通过在修改字段上添加`FormerlySerializedAsAttribute`进行避免）。而对于结点的名称、命名空间进行修改后也会导致整个结点无法被反序列化，从而丢失该结点以及之后结点的所有数据。序列化为Json后，你可以使用文本编辑器批量对结点进行修改，再重新反序列化为ScriptableObject。

        需要注意的是, 并非ScriptableObject所有的字段都被序列化, Serialize Service只对行为树的结点和共享变量进行序列化,反序列化同理。
    
    * Search Service:选择结点类型快速找到使用该结点的所有行为树, 结合Serialize Service可以同时找到对应的Json文件。

    The plugin currently has a new User Service (Tools/AkiBT/AkiBT User Service) built in, which provides two functions Serialize Service and Search Service

     * Serialize Service: Since AkiBT uses ScriptableObject for data storage, data loss will occur when modifying the field name of the node (this problem can be avoided by adding `FormerlySerializedAsAttribute` to the modified field). However, after modifying the name and namespace of the node, the entire node cannot be deserialized, thus losing all data of the node and subsequent nodes. After serializing to Json, you can use a text editor to modify the nodes in batches, and then re-deserialize to ScriptableObject.

         It should be noted that not all fields of ScriptableObject are serialized. Serialize Service only serializes the nodes and shared variables of the behavior tree, and the deserialization is the same.
    
     * Search Service: Select a node type to quickly find all behavior trees using the node, and combine with Serialize Service to find the corresponding Json file at the same time.