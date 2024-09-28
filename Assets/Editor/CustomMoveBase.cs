using UnityEditor;

[CustomEditor(typeof(MoveBase))]
[CanEditMultipleObjects]
public class CustomMoveBase : Editor
{
    SerializedProperty Name;
    SerializedProperty Description;
    SerializedProperty Type;
    SerializedProperty Power;
    SerializedProperty Accuracy;
    SerializedProperty AlwaysHits;
    SerializedProperty Up;
    SerializedProperty Priority;
    SerializedProperty HighCriticalHitRate;
    SerializedProperty DoubleIfHalfOpponentHp;
    SerializedProperty ScaleOnHp;
    SerializedProperty Style;
    SerializedProperty Effects;
    SerializedProperty SecondaryEffects;
    SerializedProperty Target;

    private void OnEnable()
    {
        Name = serializedObject.FindProperty("name");
        Description = serializedObject.FindProperty("description");
        Type = serializedObject.FindProperty("type");
        Power = serializedObject.FindProperty("power");
        Accuracy = serializedObject.FindProperty("accuracy");
        AlwaysHits = serializedObject.FindProperty("alwaysHits");
        Up = serializedObject.FindProperty("up");
        Priority = serializedObject.FindProperty("priority");
        HighCriticalHitRate = serializedObject.FindProperty("highCriticalHitRate");
        DoubleIfHalfOpponentHp = serializedObject.FindProperty("doubleIfHalfOpponentHp");
        ScaleOnHp = serializedObject.FindProperty("scaleOnHp");
        Style = serializedObject.FindProperty("style");
        Effects = serializedObject.FindProperty("effects");
        SecondaryEffects = serializedObject.FindProperty("secondaryEffects");
        Target = serializedObject.FindProperty("target");
    }

    public override void OnInspectorGUI()
    {
        var myScript = (MoveBase)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(Name);
        EditorGUILayout.PropertyField(Description);
        EditorGUILayout.PropertyField(Type);
        EditorGUILayout.PropertyField(Style);
        if (myScript.Category != AttackCategory.Status)
            EditorGUILayout.PropertyField(Power);
        if (myScript.Category != AttackCategory.Status)
        {
            EditorGUILayout.PropertyField(HighCriticalHitRate);
            EditorGUILayout.PropertyField(DoubleIfHalfOpponentHp);
            EditorGUILayout.PropertyField(ScaleOnHp);
        }
        EditorGUILayout.PropertyField(AlwaysHits);
        if (!myScript.AlwaysHits)
            EditorGUILayout.PropertyField(Accuracy);
        EditorGUILayout.PropertyField(Up);
        EditorGUILayout.PropertyField(Priority);
        EditorGUILayout.PropertyField(Target);
        if (myScript.Category == AttackCategory.Status)
            EditorGUILayout.PropertyField(Effects);
        if (myScript.Category != AttackCategory.Status )
            EditorGUILayout.PropertyField(SecondaryEffects);

        serializedObject.ApplyModifiedProperties();
    }
}
