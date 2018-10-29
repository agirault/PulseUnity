using UnityEngine;
using UnityEditor;

public abstract class PulseDataConsumer : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public PulseDataSource source;
    [SerializeField, HideInInspector]
    public int dataFieldIndex;
    internal int timePointIndex;
}

[CustomEditor(typeof(PulseDataConsumer), true)]
public class PulseDataConsumerEditor : Editor
{
    SerializedProperty sourceProp;
    SerializedProperty dataFieldIndexProp;

    void OnEnable() {
        sourceProp = serializedObject.FindProperty("source");
        dataFieldIndexProp = serializedObject.FindProperty("dataFieldIndex");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Select generator
        EditorGUILayout.PropertyField(sourceProp, new GUIContent("Data source"));

        var source = sourceProp.objectReferenceValue as PulseDataSource;
        if (source == null || source.data == null || source.data.fields == null) {
            dataFieldIndexProp.intValue = 0;
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.LabelField("Error",
                                       source == null ?
                                       "Data source missing" :
                                       "Data source could not generate valid data fields");
            return;
        }

        // Select datafield
        string[] fields = source.data.fields;
        dataFieldIndexProp.intValue = Mathf.Clamp(dataFieldIndexProp.intValue,
                                                  0,
                                                  fields.Length - 1);
        dataFieldIndexProp.intValue = EditorGUILayout.Popup("Data field",
                                                            dataFieldIndexProp.intValue,
                                                            fields);

        // Show default inspector property editor
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}