using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Shared.DataStructures
{
    public class Projectile
    {
        private string texturePath;

        public Projectile(
            string texturePath,
            float cooldownTime,
            Vector2f initialUnitVelocity,
            List<Vector2f> parentOffsets)
        {
            TexturePath = texturePath;
            CooldownTime = cooldownTime;
            InitialUnitVelocity = initialUnitVelocity;
            ParentOffsets = parentOffsets;
        }

        public string TexturePath
        {
            get => texturePath;
            set
            {
                texturePath = value;
                Texture = new Texture(value);
            }
        }

        [JsonIgnore]
        public Texture Texture { get; set; }

        public float CooldownTime { get; set; }

        public Vector2f InitialUnitVelocity { get; private set; }

        public List<Vector2f> ParentOffsets { get; }
    }
}
