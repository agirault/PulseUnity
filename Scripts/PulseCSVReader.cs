﻿/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PulseCSVReader: PulseDataSource
{
    public TextAsset CSVInput;
    public float timeElapsedAtStart = 0;

    List<string[]> CSVValues;
    int lineId;
    float startTime;

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

        startTime = Time.time;

        string[] lines = CSVInput.text.Split('\n');
        if (lines == null || lines.Length < 2) {
            return;
        }
        data.timeStampList = new FloatList(lines.Length - 1);
        CSVValues = new List<string[]>(lines.Length - 1);

        int numberOfColumns = 0;
        for (int lineId = 1; lineId < lines.Length; ++lineId) {
            var lineData = lines[lineId].Trim();
            var values = lineData.Split(',');

            if (lineId == 1) {
                numberOfColumns = values.Length;
                data.valuesTable = new List<FloatList>(numberOfColumns - 1);
                for (int columnId = 1; columnId < values.Length; ++columnId) {
                    data.valuesTable.Add(new FloatList(lines.Length - 1));
                }
            } else if (values.Length != numberOfColumns) {
                continue;
            }
            CSVValues.Add(values);
        }
    }

    void LateUpdate() {
        if (!Application.isPlaying || CSVValues == null) {
            return;
        }

        data.timeStampList.Clear();
        foreach (FloatList column in data.valuesTable) {
            column.Clear();
        }

        if (lineId >= CSVValues.Count) {
            return;
        }

        var currentTime = Time.time;
        var lineValues = CSVValues[lineId];
        string dataTimeStr = lineValues[0];
        float dataTime = float.Parse(dataTimeStr);

        while (dataTime - timeElapsedAtStart <= currentTime - startTime) {
            data.timeStampList.Add(dataTime);
            for (int columnId = 1; columnId < lineValues.Length; ++columnId) {
                string valueStr = lineValues[columnId];
                float value = float.Parse(valueStr);
                data.valuesTable[columnId - 1].Add(value);
            }
            if (++lineId >= CSVValues.Count) {
                return;
            }
            lineValues = CSVValues[lineId];
            dataTimeStr = lineValues[0];
            dataTime = float.Parse(dataTimeStr);
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
