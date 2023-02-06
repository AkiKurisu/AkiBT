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
        const string labelText="AkiBT 行为树 Version1.2";
        protected virtual string LabelText=>labelText;
        const string buttonText="打开行为树";
        protected virtual string ButtonText=>buttonText;
        protected VisualElement myInspector;
        private FieldResolverFactory factory=new FieldResolverFactory();
        public override VisualElement CreateInspectorGUI()
        {
            myInspector = new VisualElement();
            var bt = target as IBehaviorTree;
            var label=new Label(LabelText);
            label.style.fontSize=20;
            myInspector.Add(label);
            myInspector.styleSheets.Add((StyleSheet)Resources.Load("AkiBT/Inspector", typeof(StyleSheet)));
            var toggle=new PropertyField(serializedObject.FindProperty("updateType"),"更新模式");
            myInspector.Add(toggle);
            var field=new PropertyField(serializedObject.FindProperty("externalBehaviorTree"),"外部行为树");
            myInspector.Add(field);
            if(bt.SharedVariables.Count!=0)
            {
                var foldout=new Foldout();
                foldout.value=false;
                foldout.text="SharedVariables";
                foreach(var variable in bt.SharedVariables)
                { 
                    var grid=new Foldout();
                    grid.text=$"{variable.GetType().Name}  :  {variable.Name}";
                    grid.value=false;
                    var content=new VisualElement();
                    content.style.flexDirection=FlexDirection.Row;
                    var valueField=factory.Create(variable.GetType().GetField("value",BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Public)).GetEditorField(bt.SharedVariables,variable);
                    content.Add(valueField);
                    var deleteButton=new Button(()=>{ 
                        bt.SharedVariables.Remove(variable);
                        foldout.Remove(grid);
                        EditorUtility.SetDirty(target);
                        EditorUtility.SetDirty(this);
                        AssetDatabase.SaveAssets();
                    });
                    deleteButton.text="Delate";
                    deleteButton.style.width=50;
                    content.Add(deleteButton);
                    grid.Add(content);
                    foldout.Add(grid);
                }
                myInspector.Add(foldout);
            }
            var button=new Button(()=>{GraphEditorWindow.Show(bt);});
            button.style.fontSize=15;
            button.style.unityFontStyleAndWeight=FontStyle.Bold;
            button.style.color=Color.white;
            if(!Application.isPlaying)
            {
                button.style.backgroundColor=new StyleColor(new Color(140/255f, 160/255f, 250/255f));
                button.text=ButtonText;
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
    [CustomEditor(typeof(BehaviorTreeSO))]
    public class BehaviorTreeSOEditor : BehaviorTreeEditor
    {
        const string labelText="AkiBT 行为树SO";
        protected override string LabelText=>labelText;
        const string buttonText="打开行为树SO";
        protected override string ButtonText=>buttonText;
    }
}