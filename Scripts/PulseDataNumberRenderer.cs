/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class PulseDataNumberRenderer: PulseDataConsumer
{
    public string prefix = "";
    public string suffix = "";
    public float multiplier = 1;
    public uint decimals = 0;
    [Range(0f, 120f)]
    public float frequency = 0;

    UnityEngine.UI.Text textRenderer;
    float previousTime = 0;

    void Start() {
        textRenderer = gameObject.GetComponent<UnityEngine.UI.Text>();
    }

    override internal void UpdateFromPulse(FloatList times, FloatList values) {
        // Update display at a certain frequency
        float currentTime = Time.time;
        if (frequency > 0 && currentTime < previousTime + 1 / frequency) {
            return;
        }
        previousTime = currentTime;

        int lastIndex = values.Count - 1;
        float dataValue = values.Get(lastIndex);
        dataValue *= multiplier;
        string decimalCode = "F" + decimals.ToString();
        string dataString = dataValue.ToString(decimalCode);
        textRenderer.text = prefix + dataString + suffix;
    }
}
