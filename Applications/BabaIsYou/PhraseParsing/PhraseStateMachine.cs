using BabaIsYou.Enums.Nodes;
using BabaIsYou.Resources;
using CSharpFunctionalExtensions;

public class PhraseStateMachine
{
    public List<Node> Nouns { get; set; }

    public List<Node> AdjectiveVerbs { get; set; }

    public Node NounToBeApplied { get; set; }

    struct StateTransition
    {
        private readonly PhraseState CurrentState;
        private readonly PhraseCommand Command;

        public StateTransition(PhraseState currentState, PhraseCommand command)
        {
            CurrentState = currentState;
            Command = command;
        }
    }

    private Dictionary<StateTransition, (PhraseState, Action<Node>)> transitions;

    public PhraseState CurrentState { get; private set; }

    public PhraseStateMachine()
    {
        Nouns = new List<Node>();
        AdjectiveVerbs = new List<Node>();

        NounToBeApplied = Not_Set.Default;
        CurrentState = PhraseState.Inactive;

        transitions = new Dictionary<StateTransition, (PhraseState, Action<Node>)>
        {
            { new StateTransition(PhraseState.Inactive, PhraseCommand.Noun), (PhraseState.Noun, a => Nouns.Add(NodeTypes.GetObjectNodeFromTextNode(a))) },
            { new StateTransition(PhraseState.Noun, PhraseCommand.Is), (PhraseState.Is, _ => { }) },
            { new StateTransition(PhraseState.Noun, PhraseCommand.On), (PhraseState.On, _ => { }) },
            { new StateTransition(PhraseState.Noun, PhraseCommand.And), (PhraseState.And, _ => { }) },
            { new StateTransition(PhraseState.Is, PhraseCommand.Noun), (PhraseState.Completed, a => NounToBeApplied = NodeTypes.GetObjectNodeFromTextNode(a)) },
            { new StateTransition(PhraseState.On, PhraseCommand.Noun), (PhraseState.Noun, a => Nouns.Add(NodeTypes.GetObjectNodeFromTextNode(a))) },
            { new StateTransition(PhraseState.Is, PhraseCommand.AdjectiveVerb), (PhraseState.Completed, a => AdjectiveVerbs.Add(a)) },
            { new StateTransition(PhraseState.And, PhraseCommand.Noun), (PhraseState.Noun, a => Nouns.Add(NodeTypes.GetObjectNodeFromTextNode(a)))}
        };
    }

    private Maybe<(PhraseState, Action<Node>)> GetNext(PhraseCommand command)
    {
        var transition = new StateTransition(CurrentState, command);

        if (!transitions.TryGetValue(transition, out var nextState))
        {
            return Maybe.None;
        }

        return nextState;
    }

    public Maybe<PhraseState> MoveNext(Node Node)
    {
        var command = NodeTypes.GetCommand(Node);

        var maybeNextState = GetNext(command);

        if (maybeNextState.HasNoValue)
        {
            CurrentState = PhraseState.Inactive;
            return Maybe.None;
        }

        maybeNextState.Value.Item2(Node);
        CurrentState = maybeNextState.Value.Item1;

        return CurrentState;
    }
}