using System;

namespace aoc;

[AttributeUsage(AttributeTargets.Parameter)]
public class TemplateAttribute : Attribute
{
    public TemplateAttribute(string template)
    {
        Template = template;
    }

    public string Template { get; }
}