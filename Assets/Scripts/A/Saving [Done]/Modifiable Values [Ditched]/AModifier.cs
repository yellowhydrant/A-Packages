using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AModifier
{
    public ACondition condition;
    [Min(0)] public int priority;

    public float ModifyValue(float original)
    {
        if (condition != null && condition.Evaluate())
            return GetModifiedValue(original);
        else 
            return float.NaN;
    }

    abstract protected float GetModifiedValue(float original);
}
