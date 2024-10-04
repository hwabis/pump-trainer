// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.PumpTrainer.Replays
{
    public class PumpTrainerAutoGenerator : AutoGenerator<PumpTrainerReplayFrame>
    {
        public new Beatmap<PumpTrainerHitObject> Beatmap => (Beatmap<PumpTrainerHitObject>)base.Beatmap;

        public PumpTrainerAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new PumpTrainerReplayFrame());

            foreach (PumpTrainerHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new PumpTrainerReplayFrame
                {
                    Time = hitObject.StartTime,
                    Actions = [PumpTrainerKeybindConversions.COLUMN_TO_ACTION[hitObject.Column]],
                });
            }
        }
    }
}
