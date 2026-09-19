using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class WaveformLineRenderer : MaskableGraphic
{
    public enum WaveType
    {
        ECG,
        SpO2,
        Respiration,
        Flatline
    }

    [Header("Waveform Settings")]
    public WaveType waveType = WaveType.ECG;
    public float thickness = 2f;
    public float speed = 1.5f;
    public float frequency = 1f; 
    public float amplitude = 30f;
    public int resolution = 200;

    private float cycleProgress = 0f;

    private void Update()
    {
        cycleProgress += Time.deltaTime * speed * frequency;
        if (cycleProgress > 1000f) cycleProgress -= 1000f; 
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = GetPixelAdjustedRect();
        float width = rect.width;
        float height = rect.height;

        Vector2[] points = new Vector2[resolution];
        float step = width / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float x = -width / 2f + (i * step);
            
            float normalizedX = (i / (float)(resolution - 1));
            float phase = (normalizedX * 3f + cycleProgress) % 1.0f;

            float y = EvaluateWavePoint(phase) * amplitude;
            points[i] = new Vector2(x, y);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];

            Vector2 dir = (p2 - p1).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

            UIVertex v1 = UIVertex.simpleVert;
            UIVertex v2 = UIVertex.simpleVert;
            UIVertex v3 = UIVertex.simpleVert;
            UIVertex v4 = UIVertex.simpleVert;

            v1.color = color;
            v2.color = color;
            v3.color = color;
            v4.color = color;

            v1.position = p1 - normal;
            v2.position = p1 + normal;
            v3.position = p2 + normal;
            v4.position = p2 - normal;

            int index = vh.currentVertCount;
            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);
            vh.AddVert(v4);

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index);
        }
    }

    private float EvaluateWavePoint(float t)
    {
        switch (waveType)
        {
            case WaveType.ECG:
                float pWave = 0.15f * Mathf.Exp(-Mathf.Pow((t - 0.15f) / 0.03f, 2f));
                float qWave = -0.15f * Mathf.Exp(-Mathf.Pow((t - 0.23f) / 0.015f, 2f));
                float rWave = 1.00f * Mathf.Exp(-Mathf.Pow((t - 0.25f) / 0.012f, 2f));
                float sWave = -0.25f * Mathf.Exp(-Mathf.Pow((t - 0.27f) / 0.015f, 2f));
                float tWave = 0.25f * Mathf.Exp(-Mathf.Pow((t - 0.45f) / 0.06f, 2f));
                return pWave + qWave + rWave + sWave + tWave;

            case WaveType.SpO2:
                float systolic = Mathf.Sin(t * Mathf.PI * 2f) * Mathf.Clamp01(Mathf.Sin(t * Mathf.PI));
                float dicroticNotch = 0.2f * Mathf.Sin((t - 0.35f) * Mathf.PI * 4f);
                return Mathf.Max(0, systolic + dicroticNotch);

            case WaveType.Respiration:
                return Mathf.Sin(t * Mathf.PI * 2f) * 0.5f;

            case WaveType.Flatline:
            default:
                return 0f;
        }
    }
}