using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        FieldResolverFactory fieldResolverFactory=new FieldResolverFactory();
        public override VisualElement CreateInspectorGUI()
        {
            
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement myInspector = new VisualElement();
            var bt = target as BehaviorTree;
            // Add a simple label
            var label=new Label("AkiBT 行为树 Version1.0");
            label.style.fontSize=20;
            myInspector.Add(label);
            myInspector.styleSheets.Add((StyleSheet)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/AkiBT/Inspector.uss", typeof(StyleSheet)));
            var toggle=new PropertyField();
            toggle.bindingPath="updateType";
            toggle.label="更新模式";
            myInspector.Add(toggle);
            var field=new PropertyField();
            field.bindingPath="externalBehaviorTree";
            field.label="外部行为树";
            myInspector.Add(field);
            if(bt.SharedVariables.Count!=0)
            {var foldout=new Foldout();
            foldout.value=false;
            foldout.text="SharedVariables";
            foreach(var variable in bt.SharedVariables)
            {
                var valueLabel=new Label($"Value  :  {variable.GetValue()}");
                var grid=new Foldout();
                grid.text=$"{variable.GetType().Name}  :  {variable.Name}";
                grid.style.flexDirection=FlexDirection.Column;
                grid.value=false;
                grid.Add(valueLabel);
                foldout.Add(grid);
            }
            myInspector.Add(foldout);}
            var button=new Button(()=>{GraphEditorWindow.Show(bt);});
            
            button.style.fontSize=15;
            button.style.unityFontStyleAndWeight=FontStyle.Bold;
            button.style.color=Color.white;
            if(!Application.isPlaying)
            {
                button.style.backgroundColor=new StyleColor(new Color(140/255f, 160/255f, 250/255f));
                button.text="打开行为树";
            }
                
            else
            {
                button.text="打开行为树(运行中)";
                button.style.backgroundColor=new StyleColor(new Color(253/255f, 163/255f, 255/255f));
            }
                
            myInspector.Add(button);
            return myInspector;
        }
       
    }

}