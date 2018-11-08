/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class PulseEngineDriver: PulseDataSource
{
    public TextAsset initialStateFile;
    public PulseEngine.SerializationFormat serializationFormat;

    [Range(0.02f, 2.0f)]
    public double timeStep = 0.02;

    [HideInInspector, NonSerialized]
    public PulseEngine engine;

    float previousTime;

    readonly string[] pulseDataFields = {
        "SimTime (s)",
        "Lead3ElectricPotential (mV)",
        "HeartRate (1\u2215min)",
        "ArterialPressure (mmHg)",
        "MeanArterialPressure (mmHg)",
        "SystolicArterialPressure (mmHg)",
        "DiastolicArterialPressure (mmHg)",
        "OxygenSaturation",
        "EndTidalCarbonDioxidePressure (mmHg)",
        "RespirationRate (1\u2215min)",
        "SkinTemperature (degC)",
        "Carina-CarbonDioxide-PartialPressure (mmHg)"
    };

    void OnValidate() {
        // Round down to closest factor of 0.02. Need to use doubles due to
        // issues with floats multiplication (0.1 -> 0.0999999)
        timeStep = Math.Round(timeStep / 0.02) * 0.02;
    }

    void Awake() {
        if (data != null) return;
        data = new PulseData();
        data.fields = pulseDataFields;
        data.timeStampList = new FloatList();
        data.valuesTable = new List<FloatList>(pulseDataFields.Length);
        for (int fieldId = 0; fieldId < pulseDataFields.Length; ++fieldId) {
            data.valuesTable.Add(new FloatList());
        }
    }

    void Start () {
        if (!Application.isPlaying || initialStateFile == null) return;

        // Allocate PulseEngine with log file path
        string dateAndTimeVar = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string logFilePath = Application.persistentDataPath + "/" +
                                        gameObject.name +
                                        dateAndTimeVar + ".log";
        engine = new PulseEngine(logFilePath);

        // Initialize engine state from string
        engine.SerializeFromString(initialStateFile.text,
                                   null, // requested data currently hardcoded
                                   serializationFormat,
                                   Time.time);

        previousTime = Time.time;
    }

	void Update () {
        if (!Application.isPlaying || engine == null) return;

        // Clear PulseData container
        data.timeStampList.Clear();
        for (int j = 0; j < data.valuesTable.Count; ++j) {
            data.valuesTable[j].Clear();
        }

        // Don't advance simulation if we have waited less than the time step
        float timeElapsed = Time.time - previousTime;
        if (timeElapsed < timeStep) return;

        // Iterate over multiple time steps if needed
        var numberOfDataPointsNeeded = Math.Round(timeElapsed / timeStep);
        for (int i = 0; i < numberOfDataPointsNeeded; ++i) {
            // Increment previousTime to currentTime (factored by the time step)
            previousTime += (float)timeStep;
            data.timeStampList.Add(previousTime);

            // Advance simulation by time step
            bool success = engine.AdvanceTime_s(timeStep);
            if (!success) continue;

            // Copy result data
            IntPtr results = engine.PullData();
            int nbrOfValues = data.valuesTable.Count;
            double[] array = new double[nbrOfValues];
            Marshal.Copy(results, array, 0, nbrOfValues);
            for (int j = 0; j < nbrOfValues; ++j) {
                data.valuesTable[j].Add((float)array[j]);
            }
        }
	}
}
