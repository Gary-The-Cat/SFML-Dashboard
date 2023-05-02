using BabaIsYou.Enums;
using BabaIsYou.Resources;
using SFML.System;

namespace BabaIsYou.PhraseParsing
{
    public static class PhraseParser
    {
        public static List<ValidPhraseComponents> GetPhrases(Node[,] grid)
        {
            // Each phrase must (currently) begin with a noun. This is an optimization and could 
            // be excluded in future if the phrase system changes.
            var nouns = GetNounsFromGrid(grid);

            var phrases = new List<ValidPhraseComponents>();
            var maxX = grid.GetLength(0) - 1;
            var maxY = grid.GetLength(1) - 1;

            var completedGrid = new bool[maxX + 1, maxY + 1];
            var currentPhrase = new List<Vector2u>();
            foreach (var noun in nouns.OrderBy(n => n.X))
            {
                var phraseStateMachine = new PhraseStateMachine();
                currentPhrase.Clear();

                // Check the X axis
                for (uint x = noun.X; x <= maxX; x++)
                {
                    var node = grid[x, noun.Y];

                    if(completedGrid[x, noun.Y])
                    {
                        continue;
                    }

                    phraseStateMachine.MoveNext(node);
                    currentPhrase.Add(new Vector2u(x, noun.Y));

                    if (phraseStateMachine.CurrentState == PhraseState.Completed)
                    {
                        phrases.Add(new ValidPhraseComponents(
                            phraseStateMachine.Nouns,
                            phraseStateMachine.AdjectiveVerbs,
                            phraseStateMachine.NounToBeApplied));

                        currentPhrase.ForEach(n => completedGrid[n.X, n.Y] = true);

                        break;
                    }

                    if (phraseStateMachine.CurrentState == PhraseState.Inactive)
                    {
                        break;
                    }
                }
            }

            completedGrid = new bool[maxX + 1, maxY + 1];
            foreach (var noun in nouns.OrderBy(n => n.Y))
            {
                var phraseStateMachine = new PhraseStateMachine();
                currentPhrase.Clear();

                // Check the X axis
                for (uint y = noun.Y; y <= maxY; y++)
                {
                    var node = grid[noun.X, y];

                    if (completedGrid[noun.X, y])
                    {
                        continue;
                    }

                    phraseStateMachine.MoveNext(node);
                    currentPhrase.Add(new Vector2u(noun.X, y));

                    if (phraseStateMachine.CurrentState == PhraseState.Completed)
                    {
                        phrases.Add(new ValidPhraseComponents(
                            phraseStateMachine.Nouns,
                            phraseStateMachine.AdjectiveVerbs,
                            phraseStateMachine.NounToBeApplied));

                        currentPhrase.ForEach(n => completedGrid[n.X, n.Y] = true);

                        break;
                    }

                    if (phraseStateMachine.CurrentState == PhraseState.Inactive)
                    {
                        break;
                    }
                }
            }

            return phrases;
        }

        private static List<Vector2u> GetNounsFromGrid(Node[,] grid)
        {
            var output = new List<Vector2u>();

            for (uint x = 0; x < grid.GetLength(0); x++)
            {
                for (uint y = 0; y < grid.GetLength(1); y++)
                {
                    if (NodeTypes.IsNodeNoun(grid[x, y]))
                    {
                        output.Add(new Vector2u(x, y));
                    }
                }
            }

            return output;  
        }
    }
}
