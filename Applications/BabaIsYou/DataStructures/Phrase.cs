using BabaIsYou.Enums;

namespace BabaIsYou.DataStructures
{
    public class Phrase
    {
        private List<Node> phraseComponents = new List<Node>();

        public void AddPhraseComponent(Node node)
        {
            phraseComponents.Add(node);
        }

        public bool IsCompletePhrase => phraseComponents.Count > 2;

        public List<Node> PhraseComponents => phraseComponents;
    }
}
