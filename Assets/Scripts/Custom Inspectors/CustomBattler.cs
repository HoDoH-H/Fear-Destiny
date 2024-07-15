using System.Xml.Linq;
using UnityEditor;

[CustomEditor(typeof(Anigma))]
[CanEditMultipleObjects]
public class CustomBattler : Editor
{
    SerializedProperty Base;
    SerializedProperty Level;
    SerializedProperty Loadout;

    private void OnEnable()
    {
        Base = serializedObject.FindProperty("_base");
        Level = serializedObject.FindProperty("level");
        Loadout = serializedObject.FindProperty("loadout");
    }

    public override void OnInspectorGUI()
    {
        var myScript = (BattlerBase)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(Base);
        EditorGUILayout.PropertyField(Level);
        

        serializedObject.ApplyModifiedProperties();
    }
}
