using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelBounds : MonoBehaviour
{
    public static LevelBounds Instance;

    [SerializeField]
    private PolygonCollider2D m_levelBoundsCollider;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of LevelBounds present. Destroying...");
            Destroy(Instance);
        }

        Instance = this;
    }

    public static Bounds GetLevelBounds()
    {
        if (Instance != null)
        {
            return Instance.m_levelBoundsCollider.bounds;
        }
        else
        {
            Debug.LogError("Attempting to access bounds of an uninitialized Level Bounds object.");
            return default;
        }
    }

    public static bool IsWithinLevelBounds(Vector3 position)
    {
        if (Instance != null)
        {
            var notMaxedY = position.y < Instance.m_levelBoundsCollider.bounds.max.y;
            var notMaxedX = position.x < Instance.m_levelBoundsCollider.bounds.max.x;
            var notMaxed = notMaxedY && notMaxedX;
            return Instance.m_levelBoundsCollider.bounds.Contains(position) && notMaxed;
        }
        else
        {
            Debug.LogError("Attempting to check position with an uninitialized Level Bounds object.");
            return false;
        }
    }

    public static void SetLevelBounds(Vector3 newLevelBounds)
    {
        if (Instance != null)
        {
            Vector2[] newPoints = ConvertVector3ToPointArray(newLevelBounds);
            Instance.m_levelBoundsCollider.points = newPoints;
        }
        else
        {
            Debug.LogError("Attempting to set level bounds on an uninitialized Level Bounds object.");
        }
    }

    private static Vector2[] ConvertVector3ToPointArray(Vector3 bounds)
    {
        var returnArray = new Vector2[4];
        returnArray[0] = new Vector2(0, 0);
        returnArray[1] = new Vector2(0, bounds.y);
        returnArray[2] = new Vector2(bounds.x, bounds.y);
        returnArray[3] = new Vector2(bounds.x, 0);
        return returnArray;
    }
}
