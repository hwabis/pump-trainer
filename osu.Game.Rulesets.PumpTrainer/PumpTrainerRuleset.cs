// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods;
using osu.Game.Rulesets.PumpTrainer.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.PumpTrainer
{
    public class PumpTrainerRuleset : Ruleset
    {
        public override string Description => "a very pumptrainer ruleset";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawablePumpTrainerRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new PumpTrainerBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new PumpTrainerDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.Automation:
                    return new[] { new PumpTrainerModAutoplay() };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string ShortName => "pumptrainer";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, PumpTrainerAction.Button1),
            new KeyBinding(InputKey.X, PumpTrainerAction.Button2),
        };

        public override Drawable CreateIcon() => new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = ShortName[0].ToString(),
            Font = OsuFont.Default.With(size: 18),
        };

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
