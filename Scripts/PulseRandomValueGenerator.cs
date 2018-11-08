/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PulseRandomValueGenerator: PulseDataSource
{
    public float minValue = 0;
    public float maxValue = 100;
    [Range(0f, 1f)]
    public float variability = .1f;
    [Range(0f, 120f)]
    public float frequency = 0;

    [SerializeField, HideInInspector]
    float previousValue;
    float previousTime = 0;

    void Awake() {
        if (data != null) return;
        data = new PulseData();
        data.fields = new string[1] { "Random" };
        data.timeStampList = new FloatList();
        data.valuesTable = new List<FloatList>(data.fields.Length);
        data.valuesTable.Add(new FloatList());
    }

    void Start() {
        if (!Application.isPlaying) return;
        previousValue = Random.Range(minValue, maxValue);
    }

    void Update() {
        if (!Application.isPlaying) return;

        var time = Time.time;
        if (frequency > 0 && time < previousTime + 1 / frequency) {
            return;
        }
        previousTime = time;
        previousValue = ComputeRandomValue();

        data.timeStampList.Clear();
        data.valuesTable[0].Clear();

        data.timeStampList.Add(previousTime);
        data.valuesTable[0].Add(previousValue);
    }

    float ComputeRandomValue() {
        var val = Random.Range(minValue, maxValue);
        val = variability * val + (1 - variability) * previousValue;
        previousValue = val;
        return val;
    }
}
