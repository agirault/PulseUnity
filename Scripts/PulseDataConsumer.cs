/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using UnityEngine;

public abstract class PulseDataConsumer : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public PulseDataSource source;
    [SerializeField, HideInInspector]
    public int dataFieldIndex;

    void LateUpdate() {
        if (!Application.isPlaying) {
            return;
        }

        // Ensure there is data to add
        if (source == null ||
            source.data == null ||
            source.data.timeStampList == null ||
            source.data.timeStampList.IsEmpty() ||
            dataFieldIndex >= source.data.valuesTable.Count) {
            return;
        }

        var dataFieldValues = source.data.valuesTable[dataFieldIndex];
        UpdateFromPulse(source.data.timeStampList, dataFieldValues);
    }

    abstract internal void UpdateFromPulse(FloatList times, FloatList values);
}
