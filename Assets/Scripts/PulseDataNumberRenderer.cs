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

    override internal void UpdateFromPulse(float dataTime, float dataValue) {
        // Update display at a certain frequency
        float currentTime = Time.time;
        if (frequency > 0 && currentTime < previousTime + 1 / frequency) {
            return;
        }
        previousTime = currentTime;

        string decimalCode = "F" + decimals.ToString();
        textRenderer.text = dataValue.ToString(decimalCode);
    }
}
