using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Mapper;
using System.Collections.Generic;

namespace ShootEmUp.Serialization
{
    public class SerializableEnemy
    {
        // Serialization
        private SerializableEnemy() { }

        public SerializableEnemy(EnemyType enemyType)
        {
            this.EnemyType = enemyType;
            this.Components = new Dictionary<string, object>();
        }

        public EnemyType EnemyType { get; set; }

        public Dictionary<string, object> Components { get; set; }

        public Result MapComponent<T>(T component)
        {
            var componentName = typeof(T).Name;
            if (Components.TryGetValue(componentName, out object componentObject))
            {
                if (componentObject is JObject jObject)
                {
                    var output = jObject.ToObject<T>();

                    ReflectionMapper.CopyProperties(output, component);

                    // :NOTE: Does result automatically wrap a null into a Result.Failure?
                    return Result.Success();
                }
            }

            return Result.Failure<T>("Entity does not have component.");
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static Result<SerializableEnemy> Deserialize(string serializableEnemyJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<SerializableEnemy>(serializableEnemyJson);
            }
            catch
            {
                return Result.Failure<SerializableEnemy>("Invalid input file.");
            }
        }
    }
}
