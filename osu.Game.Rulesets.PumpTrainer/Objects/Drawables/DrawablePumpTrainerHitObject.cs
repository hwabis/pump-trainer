// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.PumpTrainer.Objects.Drawables
{
    public partial class DrawablePumpTrainerHitObject : DrawableHitObject<PumpTrainerHitObject>, IKeyBindingHandler<PumpTrainerAction>
    {
        public const int WIDTH = 85;

        private static Dictionary<PumpTrainerAction, Column> actionToColumn = new()
        {
            { PumpTrainerAction.P1DL, Column.P1DL },
            { PumpTrainerAction.P1UL, Column.P1UL },
            { PumpTrainerAction.P1C, Column.P1C },
            { PumpTrainerAction.P1UR, Column.P1UR },
            { PumpTrainerAction.P1DR, Column.P1DR },
            { PumpTrainerAction.P2DL, Column.P2DL },
            { PumpTrainerAction.P2UL, Column.P2UL },
            { PumpTrainerAction.P2C, Column.P2C },
            { PumpTrainerAction.P2UR, Column.P2UR },
            { PumpTrainerAction.P2DR, Column.P2DR },
        };

        public DrawablePumpTrainerHitObject(PumpTrainerHitObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(WIDTH);
            Origin = Anchor.TopCentre;
            Anchor = Anchor.TopCentre;

            // Indexes 0 to 9 so midpoint is 4.5
            X = (float)((int)HitObject.Column - 4.5) * WIDTH;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(HitObject.GetAssociatedSprite(textures));
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

        public bool OnPressed(KeyBindingPressEvent<PumpTrainerAction> e)
        {
            if (actionToColumn[e.Action] == HitObject.Column)
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
