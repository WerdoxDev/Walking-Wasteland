using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils {
    public static Dictionary<TKey, TValue> CloneDictionary<TKey, TValue>
        (Dictionary<TKey, TValue> original) where TValue : ICloneable {

        Dictionary<TKey, TValue> ret = new(original.Count, original.Comparer);

        foreach (KeyValuePair<TKey, TValue> entry in original)
            ret.Add(entry.Key, (TValue)entry.Value.Clone());

        return ret;
    }

    public static float Remap(float value, float from1, float from2, float to1, float to2) {
        return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
    }
}
