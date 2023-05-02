using BabaIsYou.DataStructures;
using BabaIsYou.Enums.Nodes;
using SFML.Graphics;

namespace BabaIsYou.Resources
{
    public static class NodeTypes
    {
        public static bool AreNodesText(params Node[] nodes)
        {
            return nodes.All(IsNodeText);
        }

        public static bool IsNodeText(Node node) => node.Match(
            not_set => false,
            noun => true,
            adjective => true,
            conjunction => true,
            objectNode => false);

        public static bool IsNodeConjunction(Node node) => node.Match(
            not_set => false,
            noun => false,
            adjective => false,
            conjunction => true,
            objectNode => false);

        public static bool IsNodeAdjectiveVerb(Node node) => node.Match(
            not_set => false,
            noun => false,
            adjective => true,
            conjunction => false,
            objectNode => false);

        public static bool IsNodeNoun(Node node) => node.Match(
            not_set => false,
            noun => true,
            adjective => false,
            conjunction => false,
            objectNode => false);

        public static bool IsNodeObject(Node node) => node.Match(
            not_set => false,
            noun => false,
            adjective => false,
            conjunction => false,
            objectNode => true);

        // :TODO: should we have the concept of a noun node that can converted to an object?
        public static Node GetObjectNodeFromTextNode(Node nounNode) => nounNode.Match(
            not_set => throw new ArgumentException(),
            noun => (ObjectNode)((int)noun),
            adjective => throw new ArgumentException(),
            conjunction => throw new ArgumentException(),
            objectNode => throw new ArgumentException());

        public static Node GetTextNodeFromObjectNode(Node objectNode) => objectNode.Match(
            not_set => throw new ArgumentException(),
            noun => throw new ArgumentException(),
            adjective => throw new ArgumentException(),
            conjunction => throw new ArgumentException(),
            objectNode => (Noun)((int)objectNode));

        public static PhraseCommand GetCommand(Node nodeType) =>
            nodeType.Match(
            not_set => PhraseCommand.Fault,
            noun => PhraseCommand.Noun,
            adjective => PhraseCommand.AdjectiveVerb,
            conjunction => conjunction switch
            {
                Conjunction.Is => PhraseCommand.Is,
                Conjunction.On => PhraseCommand.On,
                Conjunction.And => PhraseCommand.And,
            },
            objectNode => PhraseCommand.Fault);

        public static IEnumerable<NodeVisual> GetAllNodes()
        {
            // Objects
            yield return GetObjectNode_Baba();
            yield return GetObjectNode_Flag();
            yield return GetObjectNode_Rock();
            yield return GetObjectNode_Wall();
            yield return GetObjectNode_Key();

            // Text
            yield return GetTextNode_Baba();
            yield return GetTextNode_Flag();
            yield return GetTextNode_Rock();
            yield return GetTextNode_Wall();
            yield return GetTextNode_Key();

            yield return GetTextNode_Is();
            yield return GetTextNode_On();
            yield return GetTextNode_And();

            yield return GetTextNode_Push();
            yield return GetTextNode_Stop();
            yield return GetTextNode_Win();
            yield return GetTextNode_You();
            yield return GetTextNode_Sink();
            yield return GetTextNode_Defeat();
            yield return GetTextNode_Hot();
            yield return GetTextNode_Melt();
            yield return GetTextNode_Tele();
            yield return GetTextNode_Open();
            yield return GetTextNode_Shut();
        }

        public static Dictionary<Node, Func<NodeVisual>> NodeTypeToVisualMap
            = new Dictionary<Node, Func<NodeVisual>>
        {
            { ObjectNode.Baba, () => GetObjectNode_Baba() },
            { ObjectNode.Flag, () => GetObjectNode_Flag() },
            { ObjectNode.Rock, () => GetObjectNode_Rock() },
            { ObjectNode.Wall, () => GetObjectNode_Wall() },
            { ObjectNode.Key, () => GetObjectNode_Key() },
            { Noun.Rock, () => GetTextNode_Rock() },
            { Noun.Baba, () => GetTextNode_Baba() },
            { Noun.Flag, () => GetTextNode_Flag() },
            { Noun.Wall, () => GetTextNode_Wall() },
            { Noun.Key, () => GetTextNode_Key() },
            { Conjunction.Is, () => GetTextNode_Is() },
            { Conjunction.On, () => GetTextNode_On() },
            { Conjunction.And, () => GetTextNode_And() },
            { Adjective.Push, () => GetTextNode_Push() },
            { Adjective.Stop, () => GetTextNode_Stop() },
            { Adjective.Win, () => GetTextNode_Win() },
            { Adjective.You, () => GetTextNode_You() },
            { Adjective.Sink, () => GetTextNode_Sink() },
            { Adjective.Defeat, () => GetTextNode_Defeat() },
            { Adjective.Hot, () => GetTextNode_Hot() },
            { Adjective.Melt, () => GetTextNode_Melt() },
            { Adjective.Tele, () => GetTextNode_Tele() },
            { Adjective.Open, () => GetTextNode_Open() },
            { Adjective.Shut, () => GetTextNode_Shut() },
        };

        private static string Nodes => "Resources/NodeImages/";
        private static string Objects => Nodes + "Objects/";
        private static string Text => Nodes + "Text/";
        private static string Nouns => Text + "Nouns/";
        private static string Conjunctions => Text + "Conjunctions/";
        private static string AdjectiveVerbs => Text + "AdjectivesVerbs/";

        #region Objects
        public static NodeVisual GetObjectNode_Baba()
        {
            var sprite = new Sprite(new Texture(Objects + "Baba_Object.png"));
            return new NodeVisual(sprite, ObjectNode.Baba);
        }

        public static NodeVisual GetObjectNode_Flag()
        {
            var sprite = new Sprite(new Texture(Objects + "Flag_Object.png"));
            return new NodeVisual(sprite, ObjectNode.Flag);
        }

        public static NodeVisual GetObjectNode_Key()
        {
            var sprite = new Sprite(new Texture(Objects + "Key.png"));
            return new NodeVisual(sprite, ObjectNode.Key);
        }

        public static NodeVisual GetObjectNode_Rock()
        {
            var sprite = new Sprite(new Texture(Objects + "Rock_Object.png"));
            return new NodeVisual(sprite, ObjectNode.Rock);
        }

        public static NodeVisual GetObjectNode_Wall()
        {
            var sprite = new Sprite(new Texture(Objects + "Wall_Object.png"));
            return new NodeVisual(sprite, ObjectNode.Wall);
        }
        #endregion

        #region Conjunctions
        public static NodeVisual GetTextNode_Is()
        {
            var sprite = new Sprite(new Texture(Conjunctions + "Is_Text.png"));
            return new NodeVisual(sprite, Conjunction.Is);
        }

        public static NodeVisual GetTextNode_On()
        {
            var sprite = new Sprite(new Texture(Conjunctions + "On.png"));
            return new NodeVisual(sprite, Conjunction.On);
        }

        public static NodeVisual GetTextNode_And()
        {
            var sprite = new Sprite(new Texture(Conjunctions + "And_Text.png"));
            return new NodeVisual(sprite, Conjunction.And);
        }
        #endregion

        #region Nouns
        public static NodeVisual GetTextNode_Baba()
        {
            var sprite = new Sprite(new Texture(Nouns + "Baba_Text.png"));
            return new NodeVisual(sprite, Noun.Baba);
        }

        public static NodeVisual GetTextNode_Flag()
        {
            var sprite = new Sprite(new Texture(Nouns + "Flag_Text.png"));
            return new NodeVisual(sprite, Noun.Flag);
        }

        public static NodeVisual GetTextNode_Rock()
        {
            var sprite = new Sprite(new Texture(Nouns + "Rock_Text.png"));
            return new NodeVisual(sprite, Noun.Rock);
        }

        public static NodeVisual GetTextNode_Wall()
        {
            var sprite = new Sprite(new Texture(Nouns + "Wall_Text.png"));
            return new NodeVisual(sprite, Noun.Wall);
        }

        public static NodeVisual GetTextNode_Key()
        {
            var sprite = new Sprite(new Texture(Nouns + "Key.png"));
            return new NodeVisual(sprite, Noun.Key);
        }
        #endregion

        #region AdjectivesVerbs
        public static NodeVisual GetTextNode_You()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "You_Text.png"));
            return new NodeVisual(sprite, Adjective.You);
        }

        public static NodeVisual GetTextNode_Win()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Win_Text.png"));
            return new NodeVisual(sprite, Adjective.Win);
        }

        public static NodeVisual GetTextNode_Stop()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Stop_Text.png"));
            return new NodeVisual(sprite, Adjective.Stop);
        }

        public static NodeVisual GetTextNode_Push()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Push_Text.png"));
            return new NodeVisual(sprite, Adjective.Push);
        }

        public static NodeVisual GetTextNode_Sink()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Sink.png"));
            return new NodeVisual(sprite, Adjective.Sink);
        }

        public static NodeVisual GetTextNode_Defeat()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Defeat.png"));
            return new NodeVisual(sprite, Adjective.Defeat);
        }

        public static NodeVisual GetTextNode_Hot()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Hot.png"));
            return new NodeVisual(sprite, Adjective.Hot);
        }

        public static NodeVisual GetTextNode_Melt()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Melt.png"));
            return new NodeVisual(sprite, Adjective.Melt);
        }

        public static NodeVisual GetTextNode_Tele()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Tele.png"));
            return new NodeVisual(sprite, Adjective.Tele);
        }

        public static NodeVisual GetTextNode_Open()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Open.png"));
            return new NodeVisual(sprite, Adjective.Open);
        }

        public static NodeVisual GetTextNode_Shut()
        {
            var sprite = new Sprite(new Texture(AdjectiveVerbs + "Shut.png"));
            return new NodeVisual(sprite, Adjective.Shut);
        }
        #endregion
    }
}
