using UnityEngine;
using UnityEngine.Assertions;

public class MathUtils
{
    // Get the signed angle between two given angles (in degrees)
    public static float GetDegAngleDifference(float angleFrom, float angleTo)
    {
        return ((((angleTo - angleFrom) + 180f) % 360f) - 180f);
    }

    // Clamp an angle between 
    public static float ClampAngle(float value, float min, float max)
    {
        float result = value % 360f;

        Assert.IsTrue(-360f < min);
        Assert.IsTrue(max < 360f);

        float diff = max - min;

        Assert.IsTrue(0f < diff);
        Assert.IsTrue(diff < 360f);

        float shiftedValue = ((value - min) % 360f);

        if (shiftedValue > diff)
        {
            if (((360f + max) / 2f) < shiftedValue)
            {
                result = min;
            }
            else
            {
                result = max;
            }
        }

        return result;
    }

    // Project 3D vector onto other vector
    public static Vector3 GetVectorProjection(Vector3 vector, Vector3 projectTo)
    {
        return ((Vector3.Dot(vector, projectTo) / (projectTo.magnitude * projectTo.magnitude)) * projectTo);
    }

    // Project 2D vector onto other vector
    public static Vector2 GetVectorProjection(Vector2 vector, Vector2 projectTo)
    {
        return ((Vector2.Dot(vector, projectTo) / (projectTo.magnitude * projectTo.magnitude)) * projectTo);
    }
}
