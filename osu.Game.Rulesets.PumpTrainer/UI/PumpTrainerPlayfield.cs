// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.PumpTrainer.Objects;
using osu.Game.Rulesets.PumpTrainer.Objects.Drawables;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.PumpTrainer.UI
{
    [Cached]
    public partial class PumpTrainerPlayfield : ScrollingPlayfield
    {
        public IReadOnlyList<DrawableTopRowHitObject> TopRowHitObjects;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Margin = new()
            {
                Top = 50,
            };

            AddRangeInternal(new Drawable[]
            {
                new FillFlowContainer()
                {
                    Direction = FillDirection.Horizontal,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    AutoSizeAxes = Axes.Both,
                    Children = TopRowHitObjects =
                    [
                        new DrawableTopRowHitObject(new(Column.P1DL)),
                        new DrawableTopRowHitObject(new(Column.P1UL)),
                        new DrawableTopRowHitObject(new(Column.P1C)),
                        new DrawableTopRowHitObject(new(Column.P1UR)),
                        new DrawableTopRowHitObject(new(Column.P1DR)),
                        new DrawableTopRowHitObject(new(Column.P2DL)),
                        new DrawableTopRowHitObject(new(Column.P2UL)),
                        new DrawableTopRowHitObject(new(Column.P2C)),
                        new DrawableTopRowHitObject(new(Column.P2UR)),
                        new DrawableTopRowHitObject(new(Column.P2DR)),
                    ]
                },
                HitObjectContainer,
            });
        }
    }
}
