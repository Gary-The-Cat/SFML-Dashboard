using SFML.System;
using Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.CollisionData.CollisionShapes
{
    public class Circle : IShape
    {
        public float Radius { get; private set; }

        public Vector2f Center { get; private set; }

        public Circle()
        {
            Radius = 0;
            Center = new Vector2f(0, 0);
        }

        public Circle(float x, float y, float radius)
        {
            this.Radius = radius;
            Center = new Vector2f(x, y);
        }

        public void SetTopLeft(float x, float y)
        {
            Center = new Vector2f(x, y) + GetHalfSize();
        }

        public void SetPosition(float x, float y)
        {
            Center = new Vector2f(x, y);
        }

        public void SetPosition(Vector2f position)
        {
            this.Center = position;
        }

        public Vector2f GetPosition()
        {
            return Center;
        }

        public Vector2f GetUpperLeftPosition()
        {
            return new Vector2f(Center.X - Radius, Center.Y - Radius);
        }

        public void Move(Vector2f delta)
        {
            this.Center += delta;
        }

        public float GetRadius()
        {
            return Radius;
        }

        public void SetRadius(float radius)
        {
            this.Radius = radius;
        }

        public bool Contains(Vector2f point)
        {
            return Center.DistanceSquared(point) < Radius * Radius;
        }

        public Vector2f GetHalfSize()
        {
            return new Vector2f(Radius, Radius);
        }
    }
}
