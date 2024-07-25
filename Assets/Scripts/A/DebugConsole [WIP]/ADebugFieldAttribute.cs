using System;

[AttributeUsage(AttributeTargets.Field)]
public class ADebugFieldAttribute : ADebugInputAttribute
{
    protected ADebugFieldAttribute(string name, int requiredAcceslevel = 0, string description = null) : base(name, requiredAcceslevel, description)
    {
    }
}
