using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShootEmUp.Serialization
{
    public class GameLevel
    {
        public GameLevel()
        {
            this.Enemies = new List<LevelEnemy>();
        }

        public List<LevelEnemy> Enemies { get; set; }

        public static Result<GameLevel> Deserialize(string levelText)
        {
            try
            {
                return JsonConvert.DeserializeObject<GameLevel>(levelText);
            }
            catch
            {
                return Result.Failure<GameLevel>("The input was not in the corerect format.");
            }
        }
    }
}
