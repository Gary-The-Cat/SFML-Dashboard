using BabaIsYou.Enums;
using BabaIsYou.Enums.Nodes;

namespace BabaIsYou.PhraseParsing
{
    public class ValidPhraseComponents
    {
        // Remove the NotSet default
        public ValidPhraseComponents(
            List<Node> nouns,
            List<Node> adjectiveVerbs,
            Node nounToBeApplied)
        {
            Nouns = nouns;
            AdjectiveVerbs = adjectiveVerbs;
            NounToBeApplied = nounToBeApplied;
        }

        public List<Node> Nouns { get; set; }

        public List<Node> AdjectiveVerbs { get; set; }

        public Node NounToBeApplied { get; set; }

        public bool IsNounChange => NounToBeApplied.Match(
            not_set => false,
            noun => true,
            adjective => true,
            conjunction => true,
            objectNode => true);

        public bool IsComponentAttachment => !IsNounChange;

        public override string ToString()
        {
            var output = "";
            if (NounToBeApplied.IsNotSet())
            {
                output += $"NounToBeApplied: {NounToBeApplied}, Nouns: ";
                foreach (var noun in Nouns)
                {
                    output += noun.ToString() + ",";
                }
                output += Environment.NewLine;
            }
            else
            {
                output += "Nouns: ";
                foreach (var noun in Nouns)
                {
                    output += noun.ToString() + ",";
                }
                output += Environment.NewLine;

                output += "Adjectives: ";
                foreach (var adjectiveVerb in AdjectiveVerbs)
                {
                    output += adjectiveVerb.ToString() + ",";
                }
            }

            return output;
        }
    }
}
