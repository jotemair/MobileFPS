using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUtils
{
    public static T GetComponentInDirectChildren<T>(Transform root)
    {
        T result = default;

        int childCount = root.childCount;
        for (int idx = 0; ((null == result) && (idx < childCount)); ++idx)
        {
            result = root.GetChild(idx).GetComponent<T>();
        }

        return result;
    }

    public static List<T> GetComponentsInDirectChildren<T>(Transform root)
    {
        List<T> result = new List<T>();

        int childCount = root.childCount;
        for (int idx = 0; idx < childCount; ++idx)
        {
            T component = root.GetChild(idx).GetComponent<T>();
            if (null != component)
            {
                result.Add(component);
            }
        }

        return result;
    }
}
