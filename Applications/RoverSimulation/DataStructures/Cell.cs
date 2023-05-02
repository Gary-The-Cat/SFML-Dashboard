using SFML.Graphics;
using SFML.System;
using Shared.Maths;
using System;

namespace RoverSimulation.DataStructures
{
    public class Cell
    {
        public bool HasBeenVisited { get; private set; }

        private EasingWorker rWorker;
        private EasingWorker gWorker;
        private EasingWorker bWorker;
        private RectangleShape cellVisual;
        private RectangleShape fuelVisual;
        private RectangleShape goldVisual;
        private Color originalColor;
        private Texture fuelTexture;
        private Texture depletedFuelTexture;
        private Texture goldTexture;
        private Texture depletedGoldTexture;
        private bool isInitialized;
        
        private float normalizedGold;
        private float normalizedFuel;

        public float FuelValue { get; set; }

        public float GoldValue { get; set; }

        public Vector2f Centroid { get; set; }

        public Cell(
            Vector2f size,
            Color color,
            Vector2f position,
            Texture fuelTexture,
            Texture goldTexture,
            Texture depletedFuelTexture,
            Texture depletedGoldTexture)
        {
            this.fuelTexture = fuelTexture;
            this.goldTexture = goldTexture;
            this.depletedFuelTexture = depletedFuelTexture;
            this.depletedGoldTexture = depletedGoldTexture;

            this.originalColor = color;

            this.Centroid = position + size / 2;

            cellVisual = new RectangleShape(size)
            {
                Position = Centroid,
                Origin = size / 2,
                FillColor = color
            };

            fuelVisual = new RectangleShape(cellVisual.Size)
            {
                Position = new Vector2f(
                    cellVisual.Position.X + cellVisual.Size.X / 2,
                    cellVisual.Position.Y + cellVisual.Size.Y / 2),
                Origin = new Vector2f(
                    cellVisual.Size.X,
                    cellVisual.Size.Y),
                Texture = fuelTexture,
                Scale = new Vector2f(0, 0)
            };

            goldVisual = new RectangleShape(cellVisual.Size)
            {
                Position = new Vector2f(
                    cellVisual.Position.X - cellVisual.Size.X / 2,
                    cellVisual.Position.Y - cellVisual.Size.Y / 2),
                Texture = goldTexture,
                Scale = new Vector2f(0, 0)
            };
        }

        public void OnRender(RenderTarget target)
        {
            target.Draw(cellVisual);

            if(isInitialized)
            {
                target.Draw(fuelVisual);
                target.Draw(goldVisual);
            }
        }

        public void OnUpdate(float dt)
        {
            rWorker?.OnUpdate(dt);
            gWorker?.OnUpdate(dt);
            bWorker?.OnUpdate(dt);
        }

        public void Initialize(
            float normalizedFuel,
            float normalizedGold,
            float fuelValue,
            float goldValue)
        {
            this.isInitialized = true;
            this.normalizedFuel = normalizedFuel;
            this.normalizedGold = normalizedGold;
            this.FuelValue = fuelValue;
            this.GoldValue = goldValue;

            var scale = normalizedFuel * 0.5f;
            fuelVisual.Scale = new Vector2f(scale, scale);

            scale = normalizedGold * 0.5f;
            goldVisual.Scale = new Vector2f(scale, scale);
        }

        public void SetVisited()
        {
            HasBeenVisited = true;

            SetDesiredRValue(255 - ((normalizedFuel + normalizedGold) * 128), 0.8f);
            SetDesiredGValue((normalizedFuel + normalizedGold) * 128, 0.8f);
            SetDesiredBValue(0, 0.8f);

            normalizedGold = 0;
            normalizedFuel = 0;

            fuelVisual.Texture = depletedFuelTexture;
            goldVisual.Texture = depletedGoldTexture;
        }

        public void Reset()
        {
            HasBeenVisited = false;
            cellVisual.FillColor = this.originalColor;

            cellVisual.OutlineThickness = 0;
            cellVisual.Scale = new Vector2f(1, 1);

            fuelVisual.Texture = fuelTexture;
            goldVisual.Texture = goldTexture;
        }

        public void MarkPath()
        {
            if (HasBeenVisited)
            {
                SetDesiredRValue(originalColor.R, 0.8f);
                SetDesiredGValue(originalColor.G, 0.8f);
                SetDesiredBValue(originalColor.B, 0.8f);
            }
            else
            {
                SetVisited();
            }
        }
        private void SetDesiredRValue(float finish, float transitionTime)
        {
            rWorker = new EasingWorker(
                       Easings.EaseOutQuint,
                       value =>
                       {
                           cellVisual.FillColor = new Color(
                                   (byte)value,
                                   cellVisual.FillColor.G,
                                   cellVisual.FillColor.B);
                       },
                       transitionTime,
                       cellVisual.FillColor.R,
                       finish);
        }

        private void SetDesiredGValue(float finish, float transitionTime)
        {
            gWorker = new EasingWorker(
                       Easings.EaseOutQuint,
                       value =>
                       {
                           cellVisual.FillColor = new Color(
                                   cellVisual.FillColor.R,
                                   (byte)value,
                                   cellVisual.FillColor.B);
                       },
                       transitionTime,
                       cellVisual.FillColor.G,
                       finish);
        }

        private void SetDesiredBValue(float finish, float transitionTime)
        {
            bWorker = new EasingWorker(
                       Easings.EaseOutQuint,
                       value =>
                       {
                           cellVisual.FillColor = new Color(
                                   cellVisual.FillColor.R,
                                   cellVisual.FillColor.G,
                                   (byte)value);
                       },
                       transitionTime,
                       cellVisual.FillColor.B,
                       finish);
        }
    }
}
