// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.PumpTrainer.Objects;
using osu.Game.Rulesets.PumpTrainer.Objects.Drawables;
using osu.Game.Rulesets.PumpTrainer.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.PumpTrainer.UI
{
    [Cached]
    public partial class DrawablePumpTrainerRuleset : DrawableScrollingRuleset<PumpTrainerHitObject>
    {
        public DrawablePumpTrainerRuleset(PumpTrainerRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Up;
            TimeRange.Value = 2500; // todo but i am so confused what this number actually means
        }

        protected override Playfield CreatePlayfield() => new PumpTrainerPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new PumpTrainerFramedReplayInputHandler(replay);

        public override DrawableHitObject<PumpTrainerHitObject> CreateDrawableRepresentation(PumpTrainerHitObject h) => new DrawablePumpTrainerHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new PumpTrainerInputManager(Ruleset?.RulesetInfo);
    }
}
