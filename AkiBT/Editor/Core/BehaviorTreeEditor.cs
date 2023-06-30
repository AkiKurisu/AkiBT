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
        const string LabelText="AkiBT 行为树 <size=12>Version1.3.0</size>";
        const string ButtonText="打开行为树";
        protected VisualElement myInspector;
        private FieldResolverFactory factory=FieldResolverFactory.Instance;
        public override VisualElement CreateInspectorGUI()
        {
            myInspector = new VisualElement();
            var bt = target as IBehaviorTree;
            var label=new Label(LabelText);
            label.style.fontSize=20;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            var toggle=new PropertyField(serializedObject.FindProperty("updateType"),"更新模式");
            myInspector.Add(toggle);
            var field=new PropertyField(serializedObject.FindProperty("externalBehaviorTree"),"外部行为树");
            myInspector.Add(field);
            var foldout=BehaviorTreeEditorUtility.DrawSharedVariable(bt,factory,target,this);
            if(foldout!=null)myInspector.Add(foldout);
            var button=BehaviorTreeEditorUtility.GetButton(()=>{GraphEditorWindow.Show(bt);});
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
    public class BehaviorTreeSOEditor : UnityEditor.Editor
    {
        const string LabelText="AkiBT 行为树SO <size=12>Version1.3.0</size>";
        const string ButtonText="打开行为树SO";
        protected VisualElement myInspector;
        private FieldResolverFactory factory=new FieldResolverFactory();
        public override VisualElement CreateInspectorGUI()
        {
            myInspector = new VisualElement();
            var bt = target as IBehaviorTree;
            var label=new Label(LabelText);
            label.style.fontSize=20;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            myInspector.Add(new Label("行为树描述"));
            var description=new PropertyField(serializedObject.FindProperty("Description"),string.Empty);
            myInspector.Add(description);
            var foldout=BehaviorTreeEditorUtility.DrawSharedVariable(bt,factory,target,this);
            if(foldout!=null)myInspector.Add(foldout);
            if(!Application.isPlaying)
            { 
                var button=BehaviorTreeEditorUtility.GetButton(()=>{GraphEditorWindow.Show(bt);});
                button.style.backgroundColor=new StyleColor(new Color(140/255f, 160/255f, 250/255f));
                button.text=ButtonText;
                myInspector.Add(button);   
            }
            return myInspector;
        
    }
    }
    internal class BehaviorTreeEditorUtility
    {
        internal static Button GetButton(System.Action clickEvent)
        {
            var button=new Button(clickEvent);
            button.style.fontSize=15;
            button.style.unityFontStyleAndWeight=FontStyle.Bold;
            button.style.color=Color.white;
            return button;
        }
        internal static Foldout DrawSharedVariable(IBehaviorTree bt,FieldResolverFactory factory,Object target,UnityEditor.Editor editor)
        {
            if(bt.SharedVariables.Count==0)return null;
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
                content.style.justifyContent=Justify.SpaceBetween;
                var valueField=factory.Create(variable.GetType().GetField("value",BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Public)).GetEditorField(bt.SharedVariables,variable);
                valueField.style.width=Length.Percent(70f);
                content.Add(valueField);
                var deleteButton=new Button(()=>{ 
                    bt.SharedVariables.Remove(variable);
                    foldout.Remove(grid);
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(editor);
                    AssetDatabase.SaveAssets();
                });
                deleteButton.text="Delate";
                deleteButton.style.width=Length.Percent(20f);
                content.Add(deleteButton);
                grid.Add(content);
                foldout.Add(grid);   
            }
            return foldout;
        }
    }
}