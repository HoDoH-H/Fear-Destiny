using UnityEditor;

[CustomEditor(typeof(BattlerBase))]
[CanEditMultipleObjects]
public class CustomBattlerBase : Editor
{
    SerializedProperty IsPlayer;
    SerializedProperty IsHuman;
    SerializedProperty IsAnigma;
    SerializedProperty Name;
    SerializedProperty Description;
    SerializedProperty FrontSprite;
    SerializedProperty BackSprite;
    SerializedProperty MaxHp;
    SerializedProperty MaxMp;
    SerializedProperty Attack;
    SerializedProperty Defense;
    SerializedProperty SpAttack;
    SerializedProperty SpDefense;
    SerializedProperty Speed;
    SerializedProperty ExpYield;
    SerializedProperty CatchRate;
    SerializedProperty GrowthRate;
    SerializedProperty Type1;
    SerializedProperty Type2;
    SerializedProperty LearnableMoves;
    SerializedProperty LearnableByItems;
    SerializedProperty EMaxHp;
    SerializedProperty EAttack;
    SerializedProperty EDefense;
    SerializedProperty ESpAttack;
    SerializedProperty ESpDefense;
    SerializedProperty ESpeed;
    SerializedProperty IMaxHp;
    SerializedProperty IAttack;
    SerializedProperty IDefense;
    SerializedProperty ISpAttack;
    SerializedProperty ISpDefense;
    SerializedProperty ISpeed;
    SerializedProperty Morleniss;

    private void OnEnable()
    {
        IsPlayer = serializedObject.FindProperty("isPlayer");
        IsHuman = serializedObject.FindProperty("isHuman");
        IsAnigma = serializedObject.FindProperty("isAnigma");
        Name = serializedObject.FindProperty("name");
        Description = serializedObject.FindProperty("description");
        FrontSprite = serializedObject.FindProperty("frontSprite");
        BackSprite = serializedObject.FindProperty("backSprite");
        MaxHp = serializedObject.FindProperty("maxHp");
        MaxMp = serializedObject.FindProperty("maxMp");
        Attack = serializedObject.FindProperty("attack");
        Defense = serializedObject.FindProperty("defense");
        SpAttack = serializedObject.FindProperty("spAttack");
        SpDefense = serializedObject.FindProperty("spDefense");
        Speed = serializedObject.FindProperty("speed");
        ExpYield = serializedObject.FindProperty("expYield");
        CatchRate = serializedObject.FindProperty("catchRate");
        GrowthRate = serializedObject.FindProperty("growthRate");
        Type1 = serializedObject.FindProperty("type1");
        Type2 = serializedObject.FindProperty("type2");
        LearnableMoves = serializedObject.FindProperty("learnableMoves");
        LearnableByItems = serializedObject.FindProperty("learnableByItems");
        EMaxHp = serializedObject.FindProperty("eMaxHp");
        EAttack = serializedObject.FindProperty("eAttack");
        EDefense = serializedObject.FindProperty("eDefense");
        ESpAttack = serializedObject.FindProperty("eSpAttack");
        ESpDefense = serializedObject.FindProperty("eSpDefense");
        ESpeed = serializedObject.FindProperty("eSpeed");
        IMaxHp = serializedObject.FindProperty("iMaxHp");
        IAttack = serializedObject.FindProperty("iAttack");
        IDefense = serializedObject.FindProperty("iDefense");
        ISpAttack = serializedObject.FindProperty("iSpAttack");
        ISpDefense = serializedObject.FindProperty("iSpDefense");
        ISpeed = serializedObject.FindProperty("iSpeed");
        Morleniss = serializedObject.FindProperty("morleniss");
    }

    public override void OnInspectorGUI()
    {
        var myScript = (BattlerBase)target;
        serializedObject.Update();

        #region Common Properties
        if (!myScript.IsAnigma && !myScript.IsHuman)
            EditorGUILayout.PropertyField(IsPlayer);
        if (!myScript.IsPlayer)
        {
            if (!myScript.IsAnigma)
                EditorGUILayout.PropertyField(IsHuman);
            if (!myScript.IsHuman)
                EditorGUILayout.PropertyField(IsAnigma);
        }
        EditorGUILayout.PropertyField(Name);
        EditorGUILayout.PropertyField(Description);
        if (!myScript.IsPlayer)
            EditorGUILayout.PropertyField(FrontSprite);
        EditorGUILayout.PropertyField(BackSprite);
        if (myScript.IsAnigma || myScript.IsPlayer)
        {
            EditorGUILayout.PropertyField(Type1);
            EditorGUILayout.PropertyField(Type2);
        }
        EditorGUILayout.PropertyField(MaxHp);
        if (myScript.IsHuman || myScript.IsPlayer)
            EditorGUILayout.PropertyField(MaxMp);
        EditorGUILayout.PropertyField(Attack);
        EditorGUILayout.PropertyField(Defense);
        EditorGUILayout.PropertyField(SpAttack);
        EditorGUILayout.PropertyField(SpDefense);
        EditorGUILayout.PropertyField(Speed);
        if (!myScript.IsPlayer)
            EditorGUILayout.PropertyField(ExpYield);
        if (myScript.IsAnigma || myScript.IsPlayer)
            EditorGUILayout.PropertyField(CatchRate);
        if (!myScript.IsHuman)
            EditorGUILayout.PropertyField(GrowthRate);
        EditorGUILayout.PropertyField(LearnableMoves);
        EditorGUILayout.PropertyField(LearnableByItems);
        if (myScript.IsPlayer || myScript.IsAnigma)
            EditorGUILayout.PropertyField(Morleniss);
        EditorGUILayout.PropertyField(EMaxHp);
        EditorGUILayout.PropertyField(EAttack);
        EditorGUILayout.PropertyField(EDefense);
        EditorGUILayout.PropertyField(ESpAttack);
        EditorGUILayout.PropertyField(ESpDefense);
        EditorGUILayout.PropertyField(ESpeed);
        #endregion

        if (myScript.IsAnigma || myScript.IsPlayer)
        {
            EditorGUILayout.PropertyField(IMaxHp);
            EditorGUILayout.PropertyField(IAttack);
            EditorGUILayout.PropertyField(IDefense);
            EditorGUILayout.PropertyField(ISpAttack);
            EditorGUILayout.PropertyField(ISpDefense);
            EditorGUILayout.PropertyField(ISpeed);
        }
            
        serializedObject.ApplyModifiedProperties();
    }
}
