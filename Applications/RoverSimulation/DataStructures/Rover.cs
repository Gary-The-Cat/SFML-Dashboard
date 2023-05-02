using SFML.Graphics;
using SFML.System;
using Shared.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoverSimulation.DataStructures
{
    public class Rover
    {
        private EasingWorker xWorker;
        private EasingWorker yWorker;
        private RectangleShape rover;
        public Vector2f Position { get; private set; }

        public Rover(Vector2f cellSize)
        {
            this.rover = new RectangleShape(cellSize);
            this.rover.Texture = new Texture("Resources\\Rover.png");
            this.rover.Origin = cellSize / 2;
            this.rover.Scale = new Vector2f(0.7f, 0.7f);
        }

        public void Update(float dt)
        {
            xWorker?.OnUpdate(dt);
            yWorker?.OnUpdate(dt);

            this.rover.Position = Position;
        }

        public void OnRender(RenderTarget target)
        {
            target.Draw(this.rover);
        }

        public void SetDesiredXPosition(float finish, float transitionTime = 1.5f)
        {
            xWorker = new EasingWorker(
                       Easings.EaseOutQuint,
                       value =>
                       {
                           Position = new Vector2f((float)value, Position.Y);
                       },
                       transitionTime,
                       Position.X,
                       finish);
        }

        public void SetDesiredYPosition(float finish, float transitionTime = 1.5f)
        {
            yWorker = new EasingWorker(
                      Easings.EaseOutQuint,
                      value =>
                      {
                          Position = new Vector2f(Position.X, (float)value);
                      },
                      transitionTime,
                      Position.Y,
                      finish);
        }
    }
}
