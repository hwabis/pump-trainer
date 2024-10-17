// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerBeatmapConverter : BeatmapConverter<PumpTrainerHitObject>
    {
        public PumpTrainerBeatmapConverterSettings Settings => generator.Settings;
        private NextHitObjectGenerator generator = new();

        /// <summary>
        /// For osu! sliders with no repeats, this represents whether to convert the end of the slider to a hitobject.
        /// (For repeating sliders, each slider end is always converted to hitobjects whether this is true or false.)
        /// </summary>
        public bool IgnoreNormalSliderEnds = false;

        public PumpTrainerBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<PumpTrainerHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (Settings.AllowedColumns.Count <= 1)
            {
                yield break;
            }

            yield return generator.GetNextHitObject(original.StartTime, beatmap);

            if (original is IHasRepeats hasRepeats)
            {
                // This is a slider. (Even sliders with no repeats counts as IHasRepeats)

                int hitObjectsToReturnAfterFirst = hasRepeats.RepeatCount + 1; // +1 for the last hit object

                if (hitObjectsToReturnAfterFirst > 1)
                {
                    // This is a slider with at least one repeat

                    double durationBetweenPoints = (hasRepeats.EndTime - original.StartTime) / hitObjectsToReturnAfterFirst;

                    // Buzz slider protection!
                    // If the duration between the points is too small (e.g. the beatmap has a 1/6 or 1/8 buzz slider instead of the typical 1/4 (blue tick)),
                    // fill the duration with notes 1/4 apart instead of with the original rhythm.
                    TimingControlPoint currentTimingPoint = beatmap.ControlPointInfo.TimingPointAt(original.StartTime);

                    if (durationBetweenPoints < currentTimingPoint.BeatLength / 4)
                    {
                        durationBetweenPoints = currentTimingPoint.BeatLength / 4;
                    }

                    const double rounding_error = 5;

                    for (double newHitObjectTime = original.StartTime + durationBetweenPoints;
                        newHitObjectTime <= hasRepeats.EndTime + rounding_error;
                        newHitObjectTime += durationBetweenPoints)
                    {
                        yield return generator.GetNextHitObject(newHitObjectTime, beatmap);
                    }
                }
                else if (!IgnoreNormalSliderEnds)
                {
                    // This is a slider with no repeats, and the mod for ignoring is slider ends is off

                    yield return generator.GetNextHitObject(hasRepeats.EndTime, beatmap);
                }
            }
        }
    }
}
