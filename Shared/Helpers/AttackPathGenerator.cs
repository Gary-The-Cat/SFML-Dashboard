using SFML.System;
using Shared.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ShootEmUp.Helpers
{
    public static class AttackPathGenerator
    {
        public static List<Vector2f> GetAttackPathA()
        {
            var path = new List<Vector2f>()
            {
                new Vector2f(0, -250), // skip
                new Vector2f(250, 0),
                new Vector2f(0, 250),
                new Vector2f(-250, 0),
                new Vector2f(0, -250),
                new Vector2f(250, 0),
                new Vector2f(0, 250), // Skip
            };

            var length = 1000;

            var splinePath = PathGenerator.GenerateNaturalSplinePath(path, length);

            return splinePath.Skip(length/6).Take(length / 3 * 2).ToList();
        }
    }
}
