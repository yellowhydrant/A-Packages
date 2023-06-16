using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ADebugPropertyAttribute : ADebugInputAttribute
{
    protected ADebugPropertyAttribute(string name, int requiredAcceslevel = 0, string description = null) : base(name, requiredAcceslevel, description)
    {
    }
}

public class ADebugInputAttribute : Attribute
{
    public int requiredAccessLevel { get; }
    public string name { get; }
    public string description { get; }

    protected ADebugInputAttribute(string name, int requiredAcceslevel = 0, string description = null)
    {

    }
}
