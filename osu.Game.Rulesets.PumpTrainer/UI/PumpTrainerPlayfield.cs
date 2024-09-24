// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.PumpTrainer.UI
{
    [Cached]
    public partial class PumpTrainerPlayfield : ScrollingPlayfield
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            HitObjectContainer.Margin = new()
            {
                Top = 50,
            };

            AddRangeInternal(new Drawable[]
            {
                new FillFlowContainer()
                {

                },
                HitObjectContainer,
            });
        }
    }
}
