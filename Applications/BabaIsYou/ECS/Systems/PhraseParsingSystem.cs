using BabaIsYou.DataStructures;
using BabaIsYou.ECS.Components;
using BabaIsYou.Enums.Nodes;
using BabaIsYou.PhraseParsing;
using BabaIsYou.Resources;
using DefaultEcs;
using DefaultEcs.System;
using SFML.System;
using Shared.Infrastructure;

namespace BabaIsYou.ECS.Systems
{
    public class PhraseParsingSystem : ISystem<StepData>
    {
        private World world;

        public PhraseParsingSystem(World world)
        {
            this.world = world;
        }

        private EntitySet nodeFilter => world.GetEntities()
            .With<IndexPositionComponent>()
            .With<NodeTypeComponent>()
            .With<SpriteComponent>()
            .AsSet();

        private EntitySet phraseFilter => world.GetEntities()
            .With<IndexPositionComponent>()
            .With<NodeTypeComponent>() 
            .AsSet();

        public bool IsEnabled { get; set; }

        public void Update(StepData t)
        {
            var nodes = new List<(Vector2u, Node)>();
            foreach (var node in nodeFilter.GetEntities())
            {
                var positionComponent = node.Get<IndexPositionComponent>();
                var typeComponent = node.Get<NodeTypeComponent>();

                if (!NodeTypes.IsNodeText(typeComponent.Node))
                {
                    continue;
                }

                nodes.Add((positionComponent.Position, typeComponent.Node));
            }

            if (!nodes.Any())
            {
                return;
            }

            uint minX = nodes.Min(n => n.Item1.X);
            uint maxX = nodes.Max(n => n.Item1.X);
            uint minY = nodes.Min(n => n.Item1.Y);
            uint maxY = nodes.Max(n => n.Item1.Y);

            var grid = new Node[maxX + 1, maxY + 1];
            nodes.ForEach(n => grid[n.Item1.X, n.Item1.Y] = n.Item2);

            var phrases = PhraseParser.GetPhrases(grid);

            RemoveAllTransientComponents();

            AddAllTransientTraits(phrases);
        }

        public void Dispose() { }

        private void RemoveAllTransientComponents()
        {
            foreach (var entity in nodeFilter.GetEntities())
            {
                if (entity.Has<YouComponent>())
                {
                    entity.Remove<YouComponent>();
                }

                if (entity.Has<WinComponent>())
                {
                    entity.Remove<WinComponent>();
                }

                if (entity.Has<StopComponent>())
                {
                    entity.Remove<StopComponent>();
                }

                if (entity.Has<SinkComponent>())
                {
                    entity.Remove<SinkComponent>();
                }

                if (entity.Has<DefeatComponent>())
                {
                    entity.Remove<DefeatComponent>();
                }

                if (entity.Has<HotComponent>())
                {
                    entity.Remove<HotComponent>();
                }

                if (entity.Has<MeltComponent>())
                {
                    entity.Remove<MeltComponent>();
                }

                if (entity.Has<TeleComponent>())
                {
                    entity.Remove<TeleComponent>();
                }

                if (entity.Has<PushComponent>() &&
                    entity.Get<PushComponent>().IsTransient)
                {
                    entity.Remove<PushComponent>();
                }
            }
        }

        private void AddAllTransientTraits(List<ValidPhraseComponents> allValidPhrases)
        {
            foreach (var phrase in allValidPhrases.OrderBy(p => !p.IsNounChange))
            {
                if (phrase.IsNounChange)
                {
                    var entitiesToUpdate = new List<Entity>();
                    foreach (var noun in phrase.Nouns)
                    {
                        entitiesToUpdate.AddRange(GetEntitiesOfType(noun));
                    }

                    SetEntitiesType(entitiesToUpdate, phrase.NounToBeApplied);
                }
                if (phrase.IsComponentAttachment)
                {
                    foreach (var noun in phrase.Nouns)
                    {
                        var entities = GetEntitiesOfType(noun);
                        foreach (var adjectiveVerb in phrase.AdjectiveVerbs)
                        {
                            AttachComponentToEntities(entities, adjectiveVerb);
                        }
                    }
                }
            }
        }

        private IEnumerable<Entity> GetEntitiesOfType(Node noun)
        {
            var entities = new List<Entity>();

            // Get all entities that are this node type
            foreach (var node in nodeFilter.GetEntities())
            {
                if (node.Get<NodeTypeComponent>().Node.IsType(noun))
                {
                    entities.Add(node);
                }
            }

            return entities;
        }

        private void SetEntitiesType(IEnumerable<Entity> entities, Node noun)
        {
            // Get all entities that are this node type
            foreach (var entity in entities)
            {
                entity.Get<NodeTypeComponent>().Node = noun;
                entity.Get<SpriteComponent>().Sprite = NodeTypes.NodeTypeToVisualMap[noun]().Visual;
            }
        }

        private void AttachComponentToEntities(IEnumerable<Entity> entities, Node adjectiveVerb)
        {
            foreach (var entity in entities)
            {
                AttachComponentToEntity(entity, adjectiveVerb);
            }
        }

        private void AttachComponentToEntity(Entity entity, Node node)
        {
            Assert.IsTrue(node.IsT2);

            var adjective = node.AsT2;

            switch (adjective)
            {
                case Adjective.You:
                    entity.Set<YouComponent>(new());
                    break;
                case Adjective.Push:
                    entity.Set<PushComponent>(new());
                    break;
                case Adjective.Stop:
                    entity.Set<StopComponent>(new());
                    break;
                case Adjective.Win:
                    entity.Set<WinComponent>(new());
                    break;
                case Adjective.Sink:
                    entity.Set<SinkComponent>(new());
                    break;
                case Adjective.Defeat:
                    entity.Set<DefeatComponent>(new());
                    break;
                case Adjective.Hot:
                    entity.Set<HotComponent>(new());
                    break;
                case Adjective.Melt:
                    entity.Set<MeltComponent>(new());
                    break;
                case Adjective.Tele:
                    entity.Set<TeleComponent>(new());
                    break;
                case Adjective.Open:
                    entity.Set<OpenComponent>(new());
                    break;
                case Adjective.Shut:
                    entity.Set<ShutComponent>(new());
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
