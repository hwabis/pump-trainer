// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.PumpTrainer.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.PumpTrainer.Objects.Drawables
{
    public partial class DrawablePumpTrainerHitObject : DrawableHitObject<PumpTrainerHitObject>, IKeyBindingHandler<PumpTrainerAction>
    {
        public const int WIDTH = 85;

        private DrawableTopRowHitObject correspondingTopRowHitObject;

        public DrawablePumpTrainerHitObject(PumpTrainerHitObject hitObject, PumpTrainerPlayfield playfield)
            : base(hitObject)
        {
            Size = new Vector2(WIDTH);
            Origin = Anchor.TopCentre;
            Anchor = Anchor.TopCentre;

            // Indexes 0 to 9 so midpoint is 4.5
            X = (float)((int)HitObject.Column - 4.5) * WIDTH;

            correspondingTopRowHitObject = playfield.TopRowHitObjects[(int)HitObject.Column];
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(HitObject.GetAssociatedSprite(textures, false));
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                {
                    ApplyMinResult();
                }

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            ApplyResult(result);
            correspondingTopRowHitObject.FlashOnHit();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    this.FadeOut().Expire();
                    break;

                case ArmedState.Miss:
                    // todo ?
                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<PumpTrainerAction> e)
        {
            if (PumpTrainerKeybindConversions.ACTION_TO_COLUMN[e.Action] == HitObject.Column)
            {
                return UpdateResult(true);
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<PumpTrainerAction> e)
        {
        }
    }
}
