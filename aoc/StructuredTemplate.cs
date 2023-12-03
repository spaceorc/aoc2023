using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class)]
    public class StructuredTemplateAttribute : Attribute
    {
        public StructuredTemplateAttribute(string template)
        {
            Template = template;
        }

        public string Template { get; }
    }

    public partial record StructuredTemplate(StructuredTemplate.Token[] Tokens)
    {
        public abstract record Token
        {
            public abstract string CreateRegexSegment();
        }

        public partial record TextToken(string Text) : Token
        {
            public override string CreateRegexSegment()
            {
                var regex = TextTokenRegex();
                return regex.Replace(Text, m => "\\" + m.Value);
            }

            [GeneratedRegex(@"[\\$^()[\]:+*?]", RegexOptions.Compiled | RegexOptions.Singleline)]
            private static partial Regex TextTokenRegex();
        }

        public record ParamToken(string Name, string? Separators, StructuredTemplate? Template) : Token
        {
            public override string CreateRegexSegment()
            {
                return $"(?<{Name}>.*?)";
            }
        }

        public Regex CreateRegex()
        {
            return new Regex(
                "^" + string.Join("", Tokens.Select(x => x.CreateRegexSegment())) + "$"
            );
        }

        public T CreateObject<T>(string text)
        {
            return (T)CreateObject(typeof(T), text);
        }

        public object CreateObject(Type type, string text)
        {
            var match = CreateRegex().Match(text);
            if (!match.Success)
                throw new Exception("Template doesn't match");

            if (type == typeof(long))
                return long.Parse(match.Groups["Value"].Value);
            if (type == typeof(int))
                return int.Parse(match.Groups["Value"].Value);
            if (type == typeof(string))
                return match.Groups["Value"].Value;
            if (type == typeof(char))
            {
                var value = match.Groups["Value"].Value;
                if (value.Length != 1)
                    throw new InvalidOperationException($"Invalid char {value}");
                return value[0];
            }

            if (type.IsArray)
            {
                var paramToken = Tokens.OfType<ParamToken>().Single();
                return CreateParameter(paramToken, type, null, text);
            }

            var constructor = type.GetConstructors().Single(x => x.GetParameters().Any());
            var parameters = new List<object?>();
            for (var paramIndex = 0; paramIndex < constructor.GetParameters().Length; paramIndex++)
            {
                var parameter = constructor.GetParameters()[paramIndex];
                var name = parameter.Name!;
                var paramToken = name == $"item{paramIndex + 1}"
                    ? Tokens.OfType<ParamToken>().ElementAt(paramIndex)
                    : Tokens.OfType<ParamToken>().Single(t => t.Name == name);
                var value = name == $"item{paramIndex + 1}"
                    ? match.Groups[paramIndex + 1].Value
                    : match.Groups[name].Value;
                parameters.Add(CreateParameter(paramToken, parameter.ParameterType, parameter, value));
            }

            return constructor.Invoke(parameters.ToArray());
        }

        private static object CreateParameter(ParamToken paramToken, Type type, ICustomAttributeProvider? customAttributeProvider, string text)
        {
            if (type.IsArray)
            {
                var separators = paramToken.Separators
                                 ?? customAttributeProvider?.GetCustomAttributes(false).OfType<SplitAttribute>().FirstOrDefault()?.Separators
                                 ?? "- ;,:";
                var items = text
                    .Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                var result = Array.CreateInstance(type.GetElementType()!, items.Count);
                for (int i = 0; i < result.Length; i++)
                    result.SetValue(paramToken.Template != null 
                        ? paramToken.Template.CreateObject(type.GetElementType()!, items[i])
                        : CreateValue(type.GetElementType()!, null, items[i]), i);
                return result;
            }

            return paramToken.Template != null 
                ? paramToken.Template.CreateObject(type, text) 
                : CreateValue(type, customAttributeProvider, text);
        }

        private static object CreateValue(Type type, ICustomAttributeProvider? customAttributeProvider, string text)
        {
            var separators = customAttributeProvider?.GetCustomAttributes(false)
                                 .OfType<SplitAttribute>()
                                 .SingleOrDefault()?.Separators
                             ?? type.GetCustomAttribute<SplitAttribute>()?.Separators
                             ?? "- ;,:";
            
            if (type.IsArray)
            {
                var structuredTemplateAttribute = customAttributeProvider?
                                                      .GetCustomAttributes(false)
                                                      .OfType<StructuredTemplateAttribute>()
                                                      .SingleOrDefault()
                                                  ?? type.GetElementType()!.GetCustomAttribute<StructuredTemplateAttribute>();
                if (structuredTemplateAttribute != null)
                {
                    var structuredTemplate = Parse(structuredTemplateAttribute.Template);
                    var items = text
                        .Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();
                    
                    var result = Array.CreateInstance(type.GetElementType()!, items.Count);
                    for (int i = 0; i < result.Length; i++)
                        result.SetValue(structuredTemplate.CreateObject(type.GetElementType()!, items[i]), i);
                    return result;
                }
            }
            
            if (type == typeof(string))
                return text;

            var typeParseMethod =
                type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
            if (typeParseMethod != null)
                return typeParseMethod.Invoke(null, new object?[] { text })!;

            var source = new Queue<string>(text
                .Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)));
            return ReadFrom(type, source);
        }

        private static object ReadFrom(Type type, Queue<string> source)
        {
            if (type == typeof(long))
                return long.Parse(source.Dequeue());
            if (type == typeof(int))
                return int.Parse(source.Dequeue());
            if (type == typeof(string))
                return source.Dequeue();
            if (type == typeof(char))
            {
                var value = source.Dequeue();
                if (value.Length != 1)
                    throw new InvalidOperationException($"Invalid char {value}");
                return value[0];
            }

            var typeParseMethod =
                type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
            if (typeParseMethod != null)
                return typeParseMethod.Invoke(null, new object?[] { source.Dequeue() })!;

            if (type.IsArray)
            {
                var list = new ArrayList();
                while (source.Any())
                    list.Add(ReadFrom(type.GetElementType()!, source));
                var result = Array.CreateInstance(type.GetElementType()!, list.Count);
                for (int i = 0; i < list.Count; i++)
                    result.SetValue(list[i], i);
                return result;
            }

            var constructor = type.GetConstructors().Single(x => x.GetParameters().Any());
            var parameters = new List<object>();
            foreach (var parameter in constructor.GetParameters())
                parameters.Add(ReadFrom(parameter.ParameterType, source));
            return constructor.Invoke(parameters.ToArray());
        }

        public static StructuredTemplate Parse(string template)
        {
            return new StructuredTemplate(Tokenize(template).Select(ParseToken).ToArray());
        }

        private static Token ParseToken(string tokenText)
        {
            if (!tokenText.StartsWith('{'))
                return new TextToken(tokenText);

            Debug.Assert(tokenText.EndsWith('}'));
            tokenText = tokenText[1..^1];

            var nameIdx = tokenText.IndexOf(':');
            if (nameIdx == -1)
                return new ParamToken(tokenText, null, null);
            var name = tokenText[..nameIdx];
            tokenText = tokenText[(nameIdx + 1)..];
            string? separators = null;
            if (tokenText.StartsWith('['))
            {
                var sepIdx = tokenText.IndexOf(']');
                Debug.Assert(sepIdx != -1);
                separators = tokenText[1..sepIdx];
                tokenText = tokenText[(sepIdx + 1)..];
            }

            StructuredTemplate? template = null;
            if (tokenText != "")
                template = Parse(tokenText);

            return new ParamToken(name, separators, template);
        }

        private static IEnumerable<string> Tokenize(string template)
        {
            var counter = 0;
            var start = 0;
            for (int i = 0; i < template.Length; i++)
            {
                if (template[i] == '{')
                {
                    if (counter == 0)
                    {
                        if (i != start)
                        {
                            yield return template.Substring(start, i - start);
                            start = i;
                        }
                    }

                    counter++;
                }
                else if (template[i] == '}')
                {
                    counter--;
                    if (counter == 0)
                    {
                        yield return template.Substring(start, i - start + 1);
                        start = i + 1;
                    }
                }
            }

            if (start != template.Length)
                yield return template.Substring(start, template.Length - start);
        }
    }
}