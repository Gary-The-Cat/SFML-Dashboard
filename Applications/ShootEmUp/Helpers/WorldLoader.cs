using ShootEmUp.Serialization;
using System.Collections.Generic;
using System.IO;

namespace ShootEmUp.Helpers
{
    public static class WorldLoader
    {
        public static Dictionary<string, SerializableEnemy> LoadEnemyFiles(string enemyDirectory)
        {
            var output = new Dictionary<string, SerializableEnemy>();

            var enemyFiles = Directory.GetFiles(enemyDirectory);
            foreach (var file in enemyFiles)
            {
                var enemyFileText = File.ReadAllText(file);
                var enemy = SerializableEnemy.Deserialize(enemyFileText);
                if (enemy.IsSuccess)
                {
                    output.Add(enemy.Value.EnemyType.ToString(), enemy.Value);
                }
            }

            return output;
        }

        public static List<GameLevel> LoadLevelFiles(string levelDirectory)
        {
            var output = new List<GameLevel>();

            var levelFiles = Directory.GetFiles(levelDirectory);
            foreach (var file in levelFiles)
            {
                var gameLevelText = File.ReadAllText(file);
                var gameLevelResult = GameLevel.Deserialize(gameLevelText);
                if (gameLevelResult.IsSuccess)
                {
                    output.Add(gameLevelResult.Value);
                }
            }

            return output;
        }
    }
}
