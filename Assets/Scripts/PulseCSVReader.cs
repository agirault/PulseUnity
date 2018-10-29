using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PulseCSVReader: PulseDataSource
{
    public TextAsset CSVInput;

    void Awake() {
        if (data != null) return;
        data = new PulseData();
        ComputeHeaders();
    }

    void OnValidate() {
        if (Application.isPlaying) return;
        ComputeHeaders();
    }

    void Start() {
        if (!Application.isPlaying || CSVInput == null) {
            return;
        }

        string[] lines = CSVInput.text.Split('\n');
        if (lines == null || lines.Length < 2) {
            return;
        }
        data.timeStampList = new FloatList(lines.Length - 1);

        int numberOfColumns = 0;
        for (int lineId = 1; lineId < lines.Length; ++lineId) {
            var lineData = lines[lineId].Trim();
            var values = lineData.Split(',');

            if (lineId == 1) {
                numberOfColumns = values.Length;
            } else if (values.Length != numberOfColumns) {
                continue;
            }

            string timeStr = values[0];
            float time = float.Parse(timeStr);
            data.timeStampList.Add(time);

            for (int columnId = 1; columnId <= data.fields.Length; ++columnId) {
                if (lineId == 1) {
                    data.valuesTable.Add(new FloatList(lines.Length - 1));
                }
                string valueStr = values[columnId];
                float value = float.Parse(valueStr);
                data.valuesTable[columnId - 1].Add(value);
            }
        }
    }

    void ComputeHeaders() {
        if (CSVInput == null) {
            data.fields = null;
            return;
        }

        string[] lines = CSVInput.text.Split('\n');
        if (lines == null || lines.Length <= 0) {
            data.fields = null;
            return;
        }

        string firstLineData = lines[0].Trim();
        data.fields = firstLineData.Split(',');

        // Remove "Time(s)" column
        var list = data.fields.ToList();
        list.RemoveAt(0);
        data.fields = list.ToArray();

        // Fix backslash in EditorGUILayout.Popup
        for (uint headerId = 0; headerId < data.fields.Length; ++headerId) {
            string header = data.fields[headerId];
            data.fields[headerId] = header.Replace("(", " (").Replace("/", "\u2215");
        }

        // Make space for values for each field
        data.valuesTable = new List<FloatList>(data.fields.Length);
    }
}
