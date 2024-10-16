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
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;
using osu.Game.Rulesets.PumpTrainer.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.PumpTrainer
{
    public class PumpTrainerRuleset : Ruleset
    {
        public override string Description => "Pump It Up!";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawablePumpTrainerRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new PumpTrainerBeatmapConverter(beatmap, this);

        // todo yeah no this is impossible
        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new PumpTrainerDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new PumpTrainerModBanSinglesTwists(),
                        new PumpTrainerModBanFarColumns(),
                        new PumpTrainerModIgnoreOsuSliderEnds(),
                        new PumpTrainerModHalfTime(),
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new PumpTrainerModHorizontalTriples(),
                        new PumpTrainerModHorizontalTwists(),
                        new PumpTrainerModDiagonalTwists(),
                        new PumpTrainerModDiagonalSkips(),
                        new PumpTrainerModDoubleTime(),
                    };

                case ModType.Conversion:
                    return new Mod[]
                    {
                        new PumpTrainerModExcludeP1DL(),
                        new PumpTrainerModExcludeP1UL(),
                        new PumpTrainerModExcludeP1C(),
                        new PumpTrainerModExcludeP1UR(),
                        new PumpTrainerModExcludeP1DR(),
                        new PumpTrainerModExcludeP2DL(),
                        new PumpTrainerModExcludeP2UL(),
                        new PumpTrainerModExcludeP2C(),
                        new PumpTrainerModExcludeP2UR(),
                        new PumpTrainerModExcludeP2DR(),
                    };

                case ModType.Automation:
                    return new[] { new PumpTrainerModAutoplay() };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override string ShortName => "pumptrainer";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, PumpTrainerAction.P1DL),
            new KeyBinding(InputKey.Q, PumpTrainerAction.P1UL),
            new KeyBinding(InputKey.S, PumpTrainerAction.P1C),
            new KeyBinding(InputKey.E, PumpTrainerAction.P1UR),
            new KeyBinding(InputKey.C, PumpTrainerAction.P1DR),
            new KeyBinding(InputKey.V, PumpTrainerAction.P2DL),
            new KeyBinding(InputKey.R, PumpTrainerAction.P2UL),
            new KeyBinding(InputKey.G, PumpTrainerAction.P2C),
            new KeyBinding(InputKey.Y, PumpTrainerAction.P2UR),
            new KeyBinding(InputKey.N, PumpTrainerAction.P2DR),
        };

        // TODO make an actual icon like an arrow or something.
        // Apparently according to the tau developers, there's no good way to do the resource texture store stuff (???)
        // so that's my excuse for putting this off
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
