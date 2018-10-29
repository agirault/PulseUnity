using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PulseVitalRenderer))]
public class PulseVitalRandomInputGenerator : MonoBehaviour {

    public float startValue;
    [Range(0f, 1f)]
    public float variability = .1f;
    [Range(0f, 120f)]
    public float frequency = 0;

    [SerializeField, HideInInspector]
    PulseVitalRenderer lineGraphRenderer;
    float previousValue;
    float previousTime = 0;

    void Reset() {
        lineGraphRenderer = gameObject.GetComponent<PulseVitalRenderer>();
    }

    void Start() {
        previousValue = startValue;
    }

    void OnValidate() {
        if (!lineGraphRenderer) return;

        startValue = Mathf.Clamp(startValue,
                                 lineGraphRenderer.yMin,
                                 lineGraphRenderer.yMax);
    }

    void Update() {
        if (!Application.isPlaying) return;

        var time = Time.time;
        if (frequency > 0 && time < previousTime + 1 / frequency) {
            return;
        }
        previousTime = time;

        lineGraphRenderer.AddPoint(time, previousValue);
        previousValue = ComputeRandomValue();
    }

    float ComputeRandomValue() {
        var val = Random.Range(lineGraphRenderer.yMin, lineGraphRenderer.yMax);
        val = variability * val + (1 - variability) * previousValue;
        previousValue = val;
        return val;
    }
}
