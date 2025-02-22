﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
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
        public PumpTrainerHitObjectGenerator HitObjectGenerator = new();

        // Initialized with values that minimize the per-(hit object) patterns
        public PumpTrainerHitObjectGeneratorSettingsPerHitObject GeneratorSettingsForSixteenthRhythms = new()
        {
            CornersFrequency = 0,
            HorizontalTripleFrequency = 0,
        };

        private double timeOfPreviousPumpHitObject = 0;
        private const double rounding_error = 5; // Use this rounding error "generously" for '<=' and '>=', and "not generously" for '<' and '>'

        public PumpTrainerBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<PumpTrainerHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (HitObjectGenerator.Settings.AllowedColumns.Count <= 1)
            {
                yield break;
            }

            yield return getNextHitObject(original.StartTime, beatmap);

            if (original is IHasRepeats hasRepeats)
            {
                // This is a slider. (Even a slider with no repeats is IHasRepeats)

                int hitObjectsToReturnAfterFirst = hasRepeats.RepeatCount + 1; // +1 for the last hit object

                double durationBetweenHitObjects = (hasRepeats.EndTime - original.StartTime) / hitObjectsToReturnAfterFirst;

                TimingControlPoint currentTimingPoint = beatmap.ControlPointInfo.TimingPointAt(original.StartTime);

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
                        yield return getNextHitObject(newHitObjectTime, beatmap);
                    }
                }
                else if (durationBetweenHitObjects >= currentTimingPoint.BeatLength / 4 - rounding_error)
                {
                    // This is a slider with no repeats, and lasts at least a 1/4 beat (same as buzz slider protection).

                    double hitObjectTimeForSliderEnd = hasRepeats.EndTime;

                    if (durationBetweenHitObjects > currentTimingPoint.BeatLength / 2 + rounding_error)
                    {
                        // The slider is longer than 1/2 a beat.
                        // Round down to the nearest 1/4.

                        double newEndTime = original.StartTime;

                        while (newEndTime < hitObjectTimeForSliderEnd)
                        {
                            newEndTime += currentTimingPoint.BeatLength / 2;
                        }

                        hitObjectTimeForSliderEnd = newEndTime - currentTimingPoint.BeatLength / 2;
                    }
                    else if (durationBetweenHitObjects > currentTimingPoint.BeatLength / 4 + rounding_error
                        && durationBetweenHitObjects < currentTimingPoint.BeatLength / 2 - rounding_error)
                    {
                        // The slider length is longer than 1/4 but shorter than 1/2, so it has to be 3/8 or something.
                        // Round down to the nearest 1/4.
                        // Good test map: https://osu.ppy.sh/beatmapsets/929924#osu/2073258

                        double newEndTime = original.StartTime;

                        while (newEndTime < hitObjectTimeForSliderEnd)
                        {
                            newEndTime += currentTimingPoint.BeatLength / 4;
                        }

                        hitObjectTimeForSliderEnd = newEndTime - currentTimingPoint.BeatLength / 4;
                    }

                    yield return getNextHitObject(hitObjectTimeForSliderEnd, beatmap);
                }
            }
        }

        private PumpTrainerHitObject getNextHitObject(double pumpHitObjectTime, IBeatmap beatmap)
        {
            double lengthOfSixteenthRhythm = beatmap.ControlPointInfo.TimingPointAt(pumpHitObjectTime).BeatLength / 4;

            PumpTrainerHitObjectGeneratorSettingsPerHitObject perHitObjectSettingsToUse =
                pumpHitObjectTime - timeOfPreviousPumpHitObject <= lengthOfSixteenthRhythm + rounding_error ?
                GeneratorSettingsForSixteenthRhythms : new();

            timeOfPreviousPumpHitObject = pumpHitObjectTime;

            return HitObjectGenerator.GetNextHitObject(pumpHitObjectTime, beatmap, perHitObjectSettingsToUse);
        }
    }
}
