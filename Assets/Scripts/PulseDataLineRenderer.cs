using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class PulseDataLineRenderer: PulseDataConsumer
{
    [Range(.1f, 10f)]
    public float thickness = 1f;
    public Color color = Color.red;

    public bool traceInitialLine = true;
    public float yMin = 0f;
    public float yMax = 100f;
    public float xRange = 10f;

    float previousTime = 0f;
    LineRenderer lineRenderer;
    RectTransform T;

    // Monobehavior methods

    void Awake() {
        Init();
        if (!Application.isPlaying) {
            UpdateInitialLine();
        }
    }

    void OnEnable() {
        lineRenderer.enabled = true;
    }

    void OnDisable() {
        lineRenderer.enabled = false;
    }

    void OnValidate() {
        if (yMax < yMin) {
            yMax = yMin + .1f;
        }
        if (xRange <= 0) {
            xRange = .1f;
        }

        UpdateProperties();

        if (!Application.isPlaying) {
            UpdateInitialLine();
        }
    }

    override internal void UpdateFromPulse(float dataTime, float dataValue) {
        AddPoint(dataTime, dataValue);
    }

    // Custom methods

    void Init() {
        T = (RectTransform)this.transform;

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (!lineRenderer) {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.hideFlags = HideFlags.HideInInspector;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.allowOcclusionWhenDynamic = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.sortingOrder = 1;
        lineRenderer.positionCount = 0;

        // Initial updates
        UpdateProperties();
    }

    void UpdateInitialLine() {
        if (!lineRenderer) return;

        // Remove any points
        lineRenderer.positionCount = 0;

        // To start with a flat line
        var mean = (yMax + yMin) / 2;
        AddPoint(0, mean);
    }

    void UpdateProperties() {
        if (!lineRenderer) return;

        lineRenderer.widthMultiplier = thickness / 100 * T.rect.height;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void AddPoint(float t, float val){
        // Add a point on the left
        lineRenderer.positionCount++;

        // Compute dx to shift previous points
        var dt = t - previousTime;
        previousTime = t;
        float dx = DeltaTimeToDeltaX(dt);

        // Move previous position to the left
        for (int i = lineRenderer.positionCount - 1; i > 0; --i) {
            var pos = lineRenderer.GetPosition(i - 1);
            pos.x = pos.x - dx;
            lineRenderer.SetPosition(i, pos);
        }

        // Put new point on far right (far right, id = 0)
        float x = (1 - T.pivot.x) * T.rect.width;
        float y = ValueToY(Mathf.Clamp(val, yMin, yMax));
        lineRenderer.SetPosition(0, new Vector3(x, y));

        // Check if points out of bounds need to be removed
        float originX = -T.pivot.x * T.rect.width;
        for (int i = lineRenderer.positionCount - 1; i > 0; --i) {
            // If point not out of bounds, break
            var pos1 = lineRenderer.GetPosition(i);
            if (pos1.x >= originX) {
                break;
            }

            // If next point in bound, move current point to the intersection
            // of the line with the left bound of the canvas
            var pos2 = lineRenderer.GetPosition(i - 1);
            if (pos2.x > originX)
            {
                // Calculate slop-intercept between those two points
                var m = (pos2.y - pos1.y) / (pos2.x - pos1.x);
                var b = pos1.y - m * pos1.x;

                // Place last point on the far left
                pos1.x = originX;
                pos1.y = m * originX + b;
                lineRenderer.SetPosition(i, pos1);
                break;
            }

            // If next point out of bounds too, decrease and go to next
            lineRenderer.positionCount--;
        }

        // Add point of far left if we only have one point on far right
        if (lineRenderer.positionCount == 1 && traceInitialLine) {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(1, new Vector3(originX, y));
        }
    }

    float DeltaTimeToDeltaX(float t){
        float p = t / xRange;
        return p * T.rect.width;
    }

    float ValueToY(float val) {
        float p = (val - yMin) / (yMax - yMin);
        return (p - T.pivot.y) * T.rect.height;
    }
}
