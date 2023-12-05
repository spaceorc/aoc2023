using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class TemplateAttribute : StructureAttribute
{
    public TemplateAttribute(string template)
    {
        Template = template;
    }

    public string Template { get; }
    
    public override string ToString() => $"Template[{Template}], {base.ToString()}";
}
