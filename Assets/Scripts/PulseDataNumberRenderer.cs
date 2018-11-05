using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class PulseDataNumberRenderer: PulseDataConsumer
{
    public uint decimals = 0;
    [Range(0f, 120f)]
    public float frequency = 0;

    UnityEngine.UI.Text textRenderer;
    float previousTime = 0;

    void Start() {
        textRenderer = gameObject.GetComponent<UnityEngine.UI.Text>();
    }

    void Update() {
        // Ensure there is data to add
        if (source == null ||
            source.data == null ||
            source.data.timeStampList == null ||
            timePointIndex >= source.data.timeStampList.Count ||
            dataFieldIndex >= source.data.valuesTable.Count) {
            return;
        }

        // Update display at a certain frequency
        float currentTime = Time.time;
        if (frequency > 0 && currentTime < previousTime + 1 / frequency) {
            return;
        }
        previousTime = currentTime;

        // Check all values that are past the current time
        float dataTime = source.data.timeStampList.Get(timePointIndex);
        var values = source.data.valuesTable[dataFieldIndex];
        var timeStamps = source.data.timeStampList;
        string decimalCode = "F" + decimals.ToString();
        while (currentTime >= dataTime) {
            float dataValue = values.Get(timePointIndex);
            textRenderer.text = dataValue.ToString(decimalCode);

            // Check if the next value is valid
            if (timePointIndex + 1 >= timeStamps.Count) {
                break;
            }
            dataTime = timeStamps.Get(++timePointIndex);
        }
    }
}
