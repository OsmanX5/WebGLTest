using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect {

    [SerializeField]
    public Color32 m_TopColor = Color.gray;
    [SerializeField]
    public Color32 m_BottomColor = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        List<UIVertex> vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);
        ModifyVertices(vertexList);

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }

    public void ModifyVertices(List<UIVertex> vertexList)
    {
        int count = vertexList.Count;
        float bottomY = vertexList[0].position.y;
        float topY = vertexList[0].position.y;

        for (int i = 1; i < count; i++)
        {
            float y = vertexList[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }

        float uiElementHeight = topY - bottomY;
   
        for (int i = 0; i < count; i++)
        {
            UIVertex uiVertex = vertexList[i];
            uiVertex.color = Color32.Lerp(m_BottomColor, m_TopColor, (uiVertex.position.y - bottomY) / uiElementHeight);

            vertexList[i] = uiVertex;
        }
    }

}
