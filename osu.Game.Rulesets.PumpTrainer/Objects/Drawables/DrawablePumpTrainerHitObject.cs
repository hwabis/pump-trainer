// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Reflection.Metadata;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.PumpTrainer.Objects.Drawables
{
    public partial class DrawablePumpTrainerHitObject : DrawableHitObject<PumpTrainerHitObject>
    {
        private const int width = 80;

        public DrawablePumpTrainerHitObject(PumpTrainerHitObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(width);
            Origin = Anchor.TopCentre;

            X = (int) HitObject.Column * width;
        }
 
        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            string textureString;

            switch (HitObject.Column)
            {
                case Column.P1DL:
                case Column.P1DR:
                case Column.P2DL:
                case Column.P2DR:
                    textureString = "DL";
                    break;
                case Column.P1UL:
                case Column.P1UR:
                case Column.P2UL:
                case Column.P2UR:
                    textureString = "UL";
                    break;
                default:
                    textureString = "C";
                    break;
            }

            Sprite sprite = new()
            {
                RelativeSizeAxes = Axes.Both,
                Texture = textures.Get(textureString),
            };

            switch (HitObject.Column)
            {
                case Column.P1UR:
                case Column.P1DR:
                case Column.P2UR:
                case Column.P2DR:
                    Scale = new(-1, 1);
                    break;
            }

            AddInternal(sprite);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset >= 0)
                // todo: implement judgement logic
                ApplyMaxResult();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            const double duration = 1000;

            switch (state)
            {
                case ArmedState.Hit:
                    this.FadeOut(duration, Easing.OutQuint).Expire();
                    break;

                case ArmedState.Miss:

                    this.FadeColour(Color4.Red, duration);
                    this.FadeOut(duration, Easing.InQuint).Expire();
                    break;
            }
        }
    }
}
