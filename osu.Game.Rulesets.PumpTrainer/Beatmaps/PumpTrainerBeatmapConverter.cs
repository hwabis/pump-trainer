// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerBeatmapConverter : BeatmapConverter<PumpTrainerHitObject>
    {
        public PumpTrainerBeatmapConverterSettings Settings => generator.Settings;
        private NextHitObjectGenerator generator = new();

        public PumpTrainerBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<PumpTrainerHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            yield return generator.GetNextHitObject(original.StartTime, beatmap);

            if (original is IHasDuration hasDuration)
            {
                yield return generator.GetNextHitObject(hasDuration.EndTime, beatmap);
            }
        }
    }
}
