using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using Shared.Core;
using Shared.Helpers;
using Shared.Maths;
using Shared.Maths.Spline;
using Shared.ScreenConfig;
using System.Collections.Generic;

namespace SplineDemo.Screens
{
    public class SplineDemoScreen : Screen
    {
        private List<Vector2f> splinePath;
        float currentTime;
        float totalDuration;
        CircleShape circle;

        public override void InitializeScreen()
        {
            base.InitializeScreen();

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

            this.splinePath = PathGenerator.GenerateNaturalSplinePath(path, 1000);

            var text = JsonConvert.SerializeObject(this.splinePath, Formatting.Indented);

            //this.splinePath.AddRange(PathGenerator.GenerateNaturalSplinePath(path, 300));

            circle = new CircleShape(20) { FillColor = new Color(0x42, 0x85, 0xf4), Origin = new Vector2f(20, 20) };
            totalDuration = 2;
            currentTime = 0;
        }

        public override void OnUpdate(float deltaT)
        {
            base.OnUpdate(deltaT);

            currentTime += deltaT;
            currentTime = currentTime > totalDuration ? currentTime - totalDuration : currentTime;
            var index = (int)((currentTime / totalDuration) * this.splinePath.Count);
            circle.Position = this.splinePath[index];
        }

        public override void OnRender(RenderTarget target)
        {
            base.OnRender(target);

            target.Draw(circle);
        }
    }
}
