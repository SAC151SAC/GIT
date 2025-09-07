using UnityEngine;

[System.Serializable]
public class BattlefieldArea
{
    [Header("2D战场设置 (X-Y轴)")]
    public Vector2 center; // 战场中心 (X,Y)
    public Vector2 size;   // 战场大小 (宽度,高度)

    [Header("Gizmos设置")]
    public Color gizmoColor = new Color(0, 0, 1, 0.3f); // 半透明蓝色
    public float gizmoZPosition = 0; // 2D场景中Gizmo的Z轴位置
    public bool alwaysDrawGizmos = true; // 是否总是显示Gizmos

    // 检查点是否在战场内
    public bool Contains(Vector3 point)
    {
        // 2D游戏使用X和Y轴
        Vector2 point2D = new Vector2(point.x, point.y);
        Vector2 min = center - size / 2;
        Vector2 max = center + size / 2;

        return point2D.x >= min.x && point2D.x <= max.x &&
               point2D.y >= min.y && point2D.y <= max.y;
    }

    // 限制点在战场内
    public Vector3 Clamp(Vector3 point)
    {
        // 只限制X和Y轴，保持Z轴不变
        float x = Mathf.Clamp(point.x, center.x - size.x / 2, center.x + size.x / 2);
        float y = Mathf.Clamp(point.y, center.y - size.y / 2, center.y + size.y / 2);

        return new Vector3(x, y, point.z);
    }

    // 绘制2D战场Gizmos
    public void DrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // 计算2D矩形的四个角
        Vector3 bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2, gizmoZPosition);
        Vector3 bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2, gizmoZPosition);
        Vector3 topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2, gizmoZPosition);
        Vector3 topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2, gizmoZPosition);

        // 绘制填充矩形
        Gizmos.DrawCube(
            new Vector3(center.x, center.y, gizmoZPosition),
            new Vector3(size.x, size.y, 0.1f)
        );

        // 绘制边框（更清晰可见）
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}


