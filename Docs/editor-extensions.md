<div align="center">

# Editor Extensions

This document contains notice for extending and customizing Editor.

</div>

## How to change font

For visual effects such as fonts, colors, layout, etc, you can change the uss style file in `BehaviorTreeSetting`.

## How to customize node

For many reason, you may want to customize node like adding a button to preview effect.

You can write an editor class to provide your node which is similar to customize `Editor` in `UnityEditor`.

```C#
[Ordered]
public class SampleResolver : INodeResolver
{
    //Create custom node
    public IBehaviorTreeNode CreateNodeInstance(Type type)
    {
        return new SampleNode();
    }
    //Identify node type
    public static bool IsAcceptable(Type behaviorType) => behaviorType == typeof(SampleClass);

    //Your custom node
    private class SampleNode : ActionNode
    {
    }
}
```

## How to customize field

Since AkiBT use GraphView as frontend which is powered by UIElement, it can not support all fields.

If you want to customize field or want to support some fields AkiBT currently not support (eg. `UnityEngine.Localization.LocalizedString`), you can write an editor class to provide your field which is similar to customize `PropertyDrawer` in `UnityEditor`.

```C#
[Ordered]
public class LocalizedStringResolver : FieldResolver<LocalizedStringField,LocalizedString>
{
    public LocalizedStringResolver(FieldInfo fieldInfo) : base(fieldInfo)
    {
    }
    protected override LocalizedStringField CreateEditorField(FieldInfo fieldInfo)
    {
        return new LocalizedStringField(fieldInfo.Name);
    }
    public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(LocalizedString);

}
public class LocalizedStringField : BaseField<LocalizedString>
{

}
```

## How to use IMGUI in graph editor

If you don't want to use ui element, you can notify the field with `WarpFieldAttribute` to let editor use IMGUI as field's frontend.

```C#
public class EventAction : Action
{
    [WarpField]
    public UnityEvent myEvent;
}
```

## How to customize graph editor

TODO