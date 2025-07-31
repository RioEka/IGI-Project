using UnityEditor;
using UnityEngine;

using IGI.Enemy;

[CustomPropertyDrawer(typeof(TransitionState))]
public class AIStateDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var transitionProp = property.FindPropertyRelative("transition");
        var nextIndexProp = property.FindPropertyRelative("nextStateIndex");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect r1 = new(position.x, position.y, position.width, lineHeight);
        EditorGUI.PropertyField(r1, transitionProp);

        float r2Y = r1.yMax + spacing;
        Rect r2 = new(position.x, r2Y, position.width, lineHeight);

        var brain = property.serializedObject.targetObject as EnemyBrain;
        string[] stateNames = GetStateNames(brain);

        nextIndexProp.intValue = EditorGUI.Popup(r2, "Next State", nextIndexProp.intValue, stateNames);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        return lineHeight * 2 + spacing;
    }

    private string[] GetStateNames(EnemyBrain brain)
    {
        if (brain == null || brain.StateVariants == null)
            return new[] { "None" };

        string[] names = new string[brain.StateVariants.Length];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = brain.StateVariants[i]?.baseState != null
                ? brain.StateVariants[i].baseState.name
                : $"State {i}";
        }

        return names;
    }
}