[![GitHub release](https://img.shields.io/github/release/AkiKurisu/AkiBT.svg?style=social)](https://github.com/AkiKurisu/AkiBT/releases)
[![Star on GitHub](https://img.shields.io/github/stars/AkiKurisu/AkiBT.svg)](https://github.com/AkiKurisu/AkiBT/stargazers)
# AkiBT Version 1.4.2 Intro

***Read this document in Chinese: [中文文档](./README.md)***

AkiBT is a visual node editor derived from UniBT created by Yoshida for making behavior tree or other tree-based function. AkiKurisu extends it with more features so that you can enjoy it.

## Setup
1. Download [Release Package](https://github.com/AkiKurisu/AkiBT/releases)
2. Using git URL to download package by Unity PackageManager ```https://github.com/AkiKurisu/AkiBT.git```


## Supported Version

* Unity 2021.3 or later.


## Features
* Supports constructing behavior tree by visual node editor.
* Supports visualizing active node in runtime.
* Easily add original behaviors(Action,Conditional,Composite,Decorator).
* What you see is what you get, all fields are on the graph.

## Quick Start

   <img src="Images/demo.jpg" />

1. Add `AkiBT.BehaviorTree` component for any GameObject.  
   <img src="Images/started1.png" width="480"/>
2. `Open Graph Editor` button opens GraphView for Behavior Tree.  
   <img src="Images/started2.gif" width="480"/>
3. Add behaviors and set parameters.  
4. Finally press save button on tool bar of the editor window. (If invalid node found the color of the node become red.)  
   <img src="Images/started3.png" width="480"/>  
5. Run the unity application. you can see node status in the editor window.  
   <img src="Images/started4.png" width="480"/>
   
   * The red node means that last `Update` returned `Status.Failure`.
   * The green node means that last `Update` returned `Status.Success`.
   * The yellow node means that last `Update` returned `Status.Running`.
6. you can save the GameObject with `AkiBT.BehaviorTree` as prefab or save to ScriptableObject or save to json file.

7. Tutorial Video On BiliBili (The version in video is older and needs to be updated)
   
   [开源行为树AkiBT使用教程](https://www.bilibili.com/video/BV1Jd4y187XL/)

## How It Works

* `AkiBT.BehaviorTree` updates child nodes in `Update` timing when the UpdateType is `UpdateType.Auto`.
* If you want to update at any time, change UpdateType to `UpdateType.Manual` and call `BehaviorTree.Tick()`;
* Only `AkiBT.BehaviorTree` is the `MonoBehavior`. Each node is just a C# Serializable class.
  
  
## Extend Node

***See [API Document](./API.md)***


## Editor Function Info

1. You can select ```Copy From SO``` from the upper toolbar, or you can drag BehaviorTreeSO, BehaviorTree components, or GameObject and Json files that mount BehaviorTree into the editor to copy and paste.

    <img src="Images/DragDrop.gif" width="1920"/>

2. SharedVariable let you have access to add it in a blackboard and share value between different node.Now it supports Float,Int,Vector3,Bool,String,UnityEngine.Object and its subclass

    <img src="Images/SharedVariable.png" />

   * You can edit variable's name by double-click it and the variable will auto delate when it's name becomes empty.

    * You can edit the value of SharedVariable in the inspector.

    <img src="Images/ChangeVariableInInspector.png" width="480"/>

3. You can edit the setting in ProjectSetting where you can add mask for the node group you want to see in the SearchWindow.To be mentioned,the mask is relating to the editor you used.As default,the AkiBT editor is named with 'AkiBT',so you should edit the 'EditorName' with it. 

   <img src="Images/Setting.png" width="480"/>


4. Json Serialization best practice: Using Json serialization in the Editor will save the GUID that refers to the ``UnityEngine.Object ``(hereinafter referred to as UObject) object. The UObject object cannot be obtained when Json is deserialized at runtime. You need to load the required objects in other ways at runtime. UObject objects, for example, change all references to UObject in the behavior tree to SharedTObject and SharedObject, and obtain them from your resource loading scheme through their names at runtime, such as the resource address of Addressable or the file path of AssetBundle.


## Extend Node

***See [API Document](./API.md)***

## Extra Module

1. Runtime Update Support
   

   You can have access to runtime-updating or editing outside project by using [AkiBTDSL](https://github.com/AkiKurisu/AkiBTDSL) 

2. User Service


   The plugin currently has a new User Service (Tools/AkiBT/AkiBT User Service) built in, which provides two functions Serialize Service and Search Service

   * Serialize Service: Since AkiBT uses ScriptableObject for data storage, data loss will occur when modifying the field name of the node (this problem can be avoided by adding `FormerlySerializedAsAttribute` to the modified field). However, after modifying the name and namespace of the node, the entire node cannot be deserialized, thus losing all data of the node and subsequent nodes. After serializing to Json, you can use a text editor to modify the nodes in batches, and then re-deserialize to ScriptableObject.

      It should be noted that not all fields of ScriptableObject are serialized. Serialize Service only serializes the nodes and shared variables of the behavior tree, and the deserialization is the same.
   
   * Search Service: Select a node type to quickly find all behavior trees using the node, and combine with Serialize Service to find the corresponding Json file at the same time.