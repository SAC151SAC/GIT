using UnityEngine;

[System.Serializable]
public class BattlefieldArea
{
    [Header("2Dս������ (X-Y��)")]
    public Vector2 center; // ս������ (X,Y)
    public Vector2 size;   // ս����С (���,�߶�)

    [Header("Gizmos����")]
    public Color gizmoColor = new Color(0, 0, 1, 0.3f); // ��͸����ɫ
    public float gizmoZPosition = 0; // 2D������Gizmo��Z��λ��
    public bool alwaysDrawGizmos = true; // �Ƿ�������ʾGizmos

    // �����Ƿ���ս����
    public bool Contains(Vector3 point)
    {
        // 2D��Ϸʹ��X��Y��
        Vector2 point2D = new Vector2(point.x, point.y);
        Vector2 min = center - size / 2;
        Vector2 max = center + size / 2;

        return point2D.x >= min.x && point2D.x <= max.x &&
               point2D.y >= min.y && point2D.y <= max.y;
    }

    // ���Ƶ���ս����
    public Vector3 Clamp(Vector3 point)
    {
        // ֻ����X��Y�ᣬ����Z�᲻��
        float x = Mathf.Clamp(point.x, center.x - size.x / 2, center.x + size.x / 2);
        float y = Mathf.Clamp(point.y, center.y - size.y / 2, center.y + size.y / 2);

        return new Vector3(x, y, point.z);
    }

    // ����2Dս��Gizmos
    public void DrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // ����2D���ε��ĸ���
        Vector3 bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2, gizmoZPosition);
        Vector3 bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2, gizmoZPosition);
        Vector3 topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2, gizmoZPosition);
        Vector3 topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2, gizmoZPosition);

        // ����������
        Gizmos.DrawCube(
            new Vector3(center.x, center.y, gizmoZPosition),
            new Vector3(size.x, size.y, 0.1f)
        );

        // ���Ʊ߿򣨸������ɼ���
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}


