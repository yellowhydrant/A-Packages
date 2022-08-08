using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindableNodeData
{
    public Dictionary<Object, Object> bindings { get; set; }
}
