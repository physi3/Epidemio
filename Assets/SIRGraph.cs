using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIRGraph : Graphic
{
    public Vector2 unitFrequency;
    public List<float> values;

    float Width { get => rectTransform.rect.width / (unitFrequency.x - 1); }
    float Height { get => rectTransform.rect.height / (unitFrequency.y ); }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();

        if (values.Count == 0)
            return;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        Vector2 baseVector = -0.5f * new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        vertex.position = new Vector2(0, 0) + baseVector; // Create vertex in bottom left corner
        vh.AddVert(vertex); // 0 (2i - 2)
        vertex.position = new Vector2(0, values[0] * Height) + baseVector; // Create second vertex of left edge
        vh.AddVert(vertex); // 1 (2i - 1)

        for (int i = 1; i < values.Count; i++)
        {
            vertex.position = new Vector2(i * Width, 0) + baseVector; // Create vertex for bottom right corner
            vh.AddVert(vertex); // 2i
            vertex.position = new Vector2(i * Width, values[i] * Height) + baseVector; // Create second vertex of right edge
            vh.AddVert(vertex); // 2i + 1

            vh.AddTriangle(2 * i - 2, 2 * i - 1, 2 * i); // Create faces
            vh.AddTriangle(2 * i, 2 * i - 1, 2 * i + 1);
        }
    }

    public void AddValue(float value, bool increaseWidth = false)
    {
        values.Add(value);
        if (increaseWidth)
            unitFrequency += Vector2.right;
        SetVerticesDirty();
    }
}
