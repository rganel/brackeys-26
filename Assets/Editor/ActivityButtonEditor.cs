using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ActivityButton))]
[CanEditMultipleObjects]
public class ActivityButtonEditor : ButtonEditor
{
    private ActivityButton m_activityButton;

    private SerializedProperty m_activityConfigProperty;
    private SerializedProperty m_detailsParentProperty;
    private SerializedProperty m_displayLabelProperty;
    private SerializedProperty m_detailsLabelProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_activityConfigProperty = serializedObject.FindProperty("ActivityConfig");
        m_detailsParentProperty = serializedObject.FindProperty("DetailsParent");
        m_displayLabelProperty = serializedObject.FindProperty("DisplayLabel");
        m_detailsLabelProperty = serializedObject.FindProperty("DetailsLabel");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_activityConfigProperty);
        EditorGUILayout.PropertyField(m_detailsParentProperty);
        EditorGUILayout.PropertyField(m_displayLabelProperty);
        EditorGUILayout.PropertyField(m_detailsLabelProperty);

        serializedObject.ApplyModifiedProperties();
    }
}