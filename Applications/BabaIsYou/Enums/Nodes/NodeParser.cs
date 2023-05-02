namespace BabaIsYou.Enums.Nodes;

internal static class NodeParser
{
    internal static Node FromString(string text)
    {
        var components = text.Split(':');
        var (type, instance) = (components[0].Trim(), components[1].Trim());

        return type switch
        {
            "BabaIsYou.Enums.Nodes.Noun" => (Noun)Enum.Parse(typeof(Noun), instance),
            "BabaIsYou.Enums.Nodes.Adjective" => (Adjective)Enum.Parse(typeof(Adjective), instance),
            "BabaIsYou.Enums.Nodes.Conjunction" => (Conjunction)Enum.Parse(typeof(Conjunction), instance),
            "BabaIsYou.Enums.Nodes.ObjectNode" => (ObjectNode)Enum.Parse(typeof(ObjectNode), instance),
            "BabaIsYou.Enums.Nodes.Not_Set" => (Not_Set)Enum.Parse(typeof(Not_Set), instance),
            _ => throw new ArgumentException(),
        };
    }
}
