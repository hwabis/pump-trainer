// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

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
            Anchor = Anchor.TopCentre;

            // Indexes 0 to 9 so midpoint is 4.5
            X = (float)((int)HitObject.Column - 4.5) * width;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(HitObject.ToSprite(textures));
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset >= 0)
                // todo: implement judgement logic
                ApplyMaxResult();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    this.FadeOut().Expire(); // todo i guess
                    break;

                case ArmedState.Miss:
                    // todo ?
                    break;
            }
        }
    }
}
