using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using Kurisu.AkiBT;
using System.Linq;
using System.Reflection;
using Kurisu.AkiBT.Editor;
namespace Kurisu.AkiST.Editor
{
[CustomEditor(typeof(SkillTreeSO),true)]
public class SkillBehaviorSOEditor : UnityEditor.Editor
{
    const string labelText="AkiST 技能树 Version1.1";
    protected virtual string LabelText=>labelText;
    const string buttonText="打开技能树";  
    protected virtual string ButtonText=>buttonText;
    protected VisualElement myInspector;
    protected Button button;
    const string Skill_Config="技能配置";
    const string StaticSkillInfo="该技能为静态技能,你无法编辑技能树";
    private FieldResolverFactory factory=new FieldResolverFactory();
    public override VisualElement CreateInspectorGUI()
    {
            myInspector = new VisualElement();
            var bt = target as SkillTreeSO;
            var label=new Label(LabelText);
            label.style.fontSize=20;
            myInspector.Add(label);
            myInspector.styleSheets.Add((StyleSheet)Resources.Load("AkiBT/Inspector", typeof(StyleSheet)));
            if(bt.UseTree)
            {
                var field=new PropertyField(serializedObject.FindProperty("externalSkill"),"技能模板");
                myInspector.Add(field);
                field.RegisterValueChangeCallback((obj)=>{RefreshButton();});
                if(bt.SharedVariables.Count!=0)
                {var foldout=new Foldout();
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
                myInspector.Add(foldout);}
                button=new Button(()=>{SkillEditorWindow.Show(bt);});
                RefreshButton();    
                myInspector.Add(button);
            } 
            else
            {
                var readOnly=new Label(StaticSkillInfo);
                myInspector.Add(readOnly);
            }
            VisualElement inspectorFoldout = new VisualElement();
            inspectorFoldout.Add(new Label(Skill_Config));
            InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);
            var fields=target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Public)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null);
            foreach(var field in fields)
            {    
                AkiLabel newLabel= field.GetCustomAttribute<AkiLabel>();
                if(newLabel!=null)
                {
                    var pf=inspectorFoldout.Q<PropertyField>($"PropertyField:{field.Name}");
                    pf.label=newLabel.Title;
                }
            }
            foreach(var field in fields)
            {    
                AkiFolder newFolder= field.GetCustomAttribute<AkiFolder>();
                if(newFolder!=null)
                {
                    var foldout=inspectorFoldout.Q<Foldout>(newFolder.Folder);
                    var pf=inspectorFoldout.Q<PropertyField>($"PropertyField:{field.Name}");
                    int index=pf.parent.IndexOf(pf);
                    var parent=pf.parent;
                    parent.Remove(pf);
                    if(foldout==null)
                    {
                        foldout=new Foldout();
                        foldout.name=newFolder.Folder;
                        foldout.text=newFolder.Folder;
                        parent.Insert(index,foldout);
                    }
                    foldout.Add(pf);
                }
            }

            myInspector.Add(inspectorFoldout);
            return myInspector;
    }
    protected void RefreshButton()
    {
        if(button==null)return;
        var bt = target as SkillTreeSO;
        button.style.fontSize=15;
        button.style.unityFontStyleAndWeight=FontStyle.Bold;
        button.style.color=Color.white;
        if(bt.ExternalBehaviorTree==null)
        {
            button.style.backgroundColor=new StyleColor(new Color(140/255f, 160/255f, 250/255f));
            button.text=ButtonText;
        }
            
        else
        {
            button.text="根据模板修改技能树";
            button.style.backgroundColor=new StyleColor(new Color(253/255f, 163/255f, 255/255f));
        }
    }
   
    
}
}