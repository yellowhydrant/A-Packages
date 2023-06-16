using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NTree<T>
{
    [field: SerializeField] public T data { get; private set; }
    [field: SerializeField] public List<NTree<T>> children { get; private set; }

    public NTree(T data)
    {
        this.data = data;
        children = new List<NTree<T>>();
    }

    public NTree<T> AddChild(T data)
    {
        var child = new NTree<T>(data);
        children.Add(child);
        return child;
    }
}