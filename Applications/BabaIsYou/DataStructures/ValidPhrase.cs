using BabaIsYou.Enums;
using BabaIsYou.Resources;
using CSharpFunctionalExtensions;

namespace BabaIsYou.DataStructures
{
    public class ValidPhrase
    {
        private List<Node> phrase;

        private ValidPhrase(List<Node> phraseNodes)
        {
            phrase = phraseNodes;
        }

        public static Result<ValidPhrase> Create (List<Node> phrase)
        {
            return new ValidPhrase(phrase);
        }

        internal List<Node> GetNodes() => phrase;
    }
}
