using BabaIsYou.DataStructures;
using BabaIsYou.Enums;
using BabaIsYou.Enums.Nodes;
using CSharpFunctionalExtensions;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BabaIsYou.Resources
{
    internal class LevelData
    {
        public GridConfiguration GridConfiguration { get; init; }
        public List<LevelNode> Nodes { get; init; }

        private LevelData(GridConfiguration config)
        {
            this.Nodes = new List<LevelNode>();
            this.GridConfiguration = config;
        }

        public static Result<LevelData> CreateFromCsv(string[] lines)
        {
            if (lines.Length == 0)
            {
                return Result.Failure<LevelData>(
                    "No lines were provided. Please check file structure and try again.");
            }

            var configurationLine = lines[0];
            var configResult = TryGetWidthHeightFromLine(configurationLine);

            if (configResult.IsFailure)
            {
                return Result.Failure<LevelData>(configResult.Error);
            }

            var level = new LevelData(new GridConfiguration() 
            { 
                CellHeight = 64,
                CellWidth = 64,
                GridIndexSize = new Vector2u(configResult.Value.Item1, configResult.Value.Item2)
            });

            // Load all level objects
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineElements = line.Split(',').Select(e => e.Trim()).ToArray();
                if (lineElements.Length != 4)
                {
                    return Result.Failure<LevelData>(
                        $"An error occurred while reading line {i}, please ensure it is in the correct format and try again.");
                }

                if (!(uint.TryParse(lineElements[0], out var x) && uint.TryParse(lineElements[1], out var y)))
                {
                    return Result.Failure<LevelData>(
                        $"An error occurred while reading line {i}, please ensure your x and y values are positive integers.");
                }

                if (x >= configResult.Value.Item1 || y >= configResult.Value.Item2)
                {
                    return Result.Failure<LevelData>(
                        $"An error occurred while reading line {i}, please ensure your x and y are within the defined level grid size.");
                }

                Node nodeType;
                try
                {
                    nodeType = NodeParser.FromString(lineElements[2]);
                }
                catch
                {
                    return Result.Failure<LevelData>(
                        $"An error occurred while reading line {i}, that node type is unrecognised.");
                }

                Direction direction;
                try
                {
                    direction = (Direction)Enum.Parse(typeof(Direction), lineElements[3]);
                }
                catch
                {
                    return Result.Failure<LevelData>(
                        $"An error occurred while reading line {i}, that direction is unrecognised.");
                }

                level.Nodes.Add(new LevelNode()
                {
                    IndexPosition = new Vector2u(x, y),
                    NodeType = nodeType,
                    Direction = direction,
                });
            }

            return level;
        }

        public static Result<LevelData> CreateFromLevelContents(
            GridConfiguration config,
            List<LevelNode> nodes)
        {
            // Perform Validation
            if(config.GridIndexSize.X <= 0 || config.GridIndexSize.Y <= 0)
            {
                return Result.Failure<LevelData>("The grid sizze must be positive on both axis.");
            }

            if(nodes.Count == 0)
            {
                return Result.Failure<LevelData>("Your level must contain at least one node.");
            }

            return new LevelData(config)
            {
                Nodes = nodes.ToList(),
            };
        }

        public static string[] CreateLevelFile(LevelData level)
        {
            var file = new string[level.Nodes.Count + 1];
            file[0] = $"{level.GridConfiguration.GridIndexSize.X},{level.GridConfiguration.GridIndexSize.Y}";

            for (int i = 0; i < level.Nodes.Count; i++)
            {
                var node = level.Nodes[i];
                file[i + 1] = $"{node.IndexPosition.X}, {node.IndexPosition.Y}, {node.NodeType}, {node.Direction}";
            }

            return file.ToArray();
        }

        private static Result<(uint, uint)> TryGetWidthHeightFromLine(string configurationLine)
        {
            var conigSplit = configurationLine.Split(',');

            if (conigSplit.Length == 0)
            {
                return Result.Failure<(uint, uint)>(
                    "No configuration was was provided. Please check file structure and try again.");
            }

            if (conigSplit.Length != 2)
            {
                return Result.Failure<(uint, uint)>(
                    "The configuration line must only consist of 'width,height'. Please check file structure and try again.");
            }

            if (uint.TryParse(conigSplit[0], out var width) && uint.TryParse(conigSplit[1], out var height))
            {
                if (width > 0 && height > 0)
                {
                    return (width, height);
                }
            }

            return Result.Failure<(uint, uint)>(
                "The values provided for 'width,height' must both be positive integers. Please check file structure and try again.");
        }
    }
}
