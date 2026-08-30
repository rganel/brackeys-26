using UI;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(EventButton))]
[CanEditMultipleObjects]
public class EventButtonEditor : ButtonEditor
{
    private EventButton m_eventButton;

    private SerializedProperty m_detailsParentProperty;
    private SerializedProperty m_eventDefinitionProperty;
    private SerializedProperty m_eventPanelProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_detailsParentProperty = serializedObject.FindProperty("DetailsParent");
        m_eventDefinitionProperty = serializedObject.FindProperty("EventDefinition");
        m_eventPanelProperty = serializedObject.FindProperty("EventPanel");
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_detailsParentProperty);
        EditorGUILayout.PropertyField(m_eventDefinitionProperty);
        EditorGUILayout.PropertyField(m_eventPanelProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
