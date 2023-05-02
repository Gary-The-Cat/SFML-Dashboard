using System.Collections.Generic;

namespace ShootEmUp.Serialization
{
    public class LevelEnemy
    {
        public LevelEnemy(string name)
        {
            this.EnemyType = name;
            this.DefaultOverrides = new Dictionary<string, object>();
        }

        public string EnemyType { get; set; }

        public Dictionary<string, object> DefaultOverrides { get; set; }
    }
}
