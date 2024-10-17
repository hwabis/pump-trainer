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
        /// Even if this is true, the slider must be at least length 1/4 of a beat for its end to be converted.
        /// (For repeating sliders, each slider end is always converted to hitobjects whether this is true or false.)
        /// </summary>
        public bool CountNormalSliderEnds = true;

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
                // This is a slider. (Even a sliders with no repeats is IHasRepeats)

                int hitObjectsToReturnAfterFirst = hasRepeats.RepeatCount + 1; // +1 for the last hit object

                double durationBetweenHitObjects = (hasRepeats.EndTime - original.StartTime) / hitObjectsToReturnAfterFirst;
                TimingControlPoint currentTimingPoint = beatmap.ControlPointInfo.TimingPointAt(original.StartTime);

                const double rounding_error = 5;

                if (hitObjectsToReturnAfterFirst > 1)
                {
                    // This is a slider with at least one repeat. Apply buzz slider protection:
                    // If the duration between the points is shorter than 1/4 of a beat,
                    // fill the duration with notes 1/4 apart instead of with the original rhythm.

                    if (durationBetweenHitObjects < currentTimingPoint.BeatLength / 4)
                    {
                        durationBetweenHitObjects = currentTimingPoint.BeatLength / 4;
                    }

                    for (double newHitObjectTime = original.StartTime + durationBetweenHitObjects;
                        newHitObjectTime <= hasRepeats.EndTime + rounding_error;
                        newHitObjectTime += durationBetweenHitObjects)
                    {
                        yield return generator.GetNextHitObject(newHitObjectTime, beatmap);
                    }
                }
                else if (CountNormalSliderEnds)
                {
                    // This is a slider with no repeats. Only create a hitobject for the ending of the slider if
                    // the slider length is at least a 1/4 beat.

                    if (durationBetweenHitObjects > currentTimingPoint.BeatLength / 4 - rounding_error)
                    {
                        yield return generator.GetNextHitObject(hasRepeats.EndTime, beatmap);
                    }
                }
            }
        }
    }
}
