using System;

[AttributeUsage(AttributeTargets.Method)]
public class ADebugMethodAttribute : ADebugInputAttribute
{
    protected ADebugMethodAttribute(string name, int requiredAcceslevel = 0, string description = null) : base(name, requiredAcceslevel, description)
    {
    }
}
