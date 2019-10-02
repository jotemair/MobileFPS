using UnityEngine;
using UnityEngine.Assertions;

public class MathUtils
{
    // Get the signed angle between two given angles (in degrees)
    public static float GetDegAngleDifference(float angleFrom, float angleTo)
    {
        return ((((angleTo - angleFrom) + 180f) % 360f) - 180f);
    }

    // Clamp an angle between two values, taking into account overflow
    public static float ClampAngle(float value, float min, float max)
    {
        // Take the modulo of the input, we add 720 and take modulo again so that the result is guaranteed to be positive
        float result = ((value % 360f) + 720f) % 360f;

        // Make sure that the min and max values are in the appropriate range
        Assert.IsTrue(-360f < min);
        Assert.IsTrue(max < 360f);

        float diff = max - min;

        // Min should be less than max, so the difference should be positive, and the difference should be less than a full circle
        Assert.IsTrue(0f < diff);
        Assert.IsTrue(diff < 360f);

        // We shift the value by min, so that we can clamp between 0 degrees and a positive value less than 360
        float shiftedValue = (((value - min) % 360f) + 720f) % 360f;

        // If the shifted value is greater than the difference between min and max, it's outside the clamping range
        if (shiftedValue > diff)
        {
            // We check if the shifted value that's outside the clamp range is closer to 0 or diff on the circle
            if (((360f + diff) / 2f) < shiftedValue)
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
