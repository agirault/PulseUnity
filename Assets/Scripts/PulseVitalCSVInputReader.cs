using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PulseVitalCSVInputReader : MonoBehaviour {

    public TextAsset CSVInput;
    public int timeFieldIndex = 0;
    public int dataFieldIndex = 0;

    List<float> timeStampList;
    List<float> dataList;
    int dataPointIndex = 0;
    PulseVitalRenderer lineGraphRenderer;

    void Start() {
        string fileData = CSVInput.text;
        string[] lines = fileData.Split('\n');
        if (lines == null || lines.Length < 2) {
            return;
        }

        timeStampList = new List<float>(lines.Length - 1);
        dataList = new List<float>(lines.Length - 1);

        int numberOfColumns = 0;
        for (int lineId = 1; lineId < lines.Length; ++lineId) {
            var lineData = lines[lineId].Trim();
            var values = lineData.Split(',');

            if (lineId == 1) {
                numberOfColumns = values.Length;
            } else if (values.Length != numberOfColumns) {
                continue;
            }

            string timeStr = values[timeFieldIndex];
            float time = float.Parse(timeStr);
            timeStampList.Add(time);

            string dataStr = values[dataFieldIndex];
            float data = float.Parse(dataStr);
            dataList.Add(data);
        }

        lineGraphRenderer = gameObject.GetComponent<PulseVitalRenderer>();
    }
	
    void Update() {
        if (timeStampList == null || dataPointIndex >= timeStampList.Count) {
            return;
        }

        float currentTime = Time.time;
        float dataTime = timeStampList[dataPointIndex];
        while (currentTime >= dataTime) {
            float dataValue = dataList[dataPointIndex];
            lineGraphRenderer.AddPoint(dataTime, dataValue);
            dataTime = timeStampList[++dataPointIndex];
        }
    }
}

[CustomEditor(typeof(PulseVitalCSVInputReader)), CanEditMultipleObjects]
public class PulseVitalCSVInputEditor : Editor {

    PulseVitalCSVInputReader reader;
    SerializedProperty CSVInput;
    SerializedProperty timeFieldIndex;
    SerializedProperty dataFieldIndex;

    void OnEnable() {
        reader = target as PulseVitalCSVInputReader;
        CSVInput = serializedObject.FindProperty("CSVInput");
        timeFieldIndex = serializedObject.FindProperty("timeFieldIndex");
        dataFieldIndex = serializedObject.FindProperty("dataFieldIndex");
    }

    override public void OnInspectorGUI() {
        //DrawDefaultInspector();
        serializedObject.Update();

        EditorGUILayout.PropertyField(CSVInput, new GUIContent("CSV Input"));
        var headers = HeadersFromCSV();
        if (headers == null) {
            return;
        }

        timeFieldIndex.intValue = EditorGUILayout.Popup("Time field",
                                                        timeFieldIndex.intValue,
                                                        headers);

        dataFieldIndex.intValue = EditorGUILayout.Popup("Data field",
                                                        dataFieldIndex.intValue,
                                                        headers);
        serializedObject.ApplyModifiedProperties();
    }

    string[] HeadersFromCSV() {
        if (CSVInput == null) {
            return null;
        }

        var csv = CSVInput.objectReferenceValue as TextAsset;
        if (csv == null) {
            return null;
        }

        string[] lines = csv.text.Split('\n');
        if (lines == null || lines.Length == 0) {
            return null;
        }

        string firstLineData = lines[0].Trim();
        string[] headers = firstLineData.Split(',');

        // Fix backslash in EditorGUILayout.Popup
        for (uint headerId = 0; headerId < headers.Length; ++headerId) {
            string header = headers[headerId];
            headers[headerId] = header.Replace("(", " (").Replace("/", "\u2215");
        }

        return headers;
    }
}
