using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public struct PumpTrainerHitObjectSettings
    {
        public List<Column> AllowedColumns = new();

        /// <summary>
        /// 0 to 1 determining how frequently to generate crossovers.
        /// A cross over would be like: starting left foot: UL, C, UR
        /// </summary>
        public double SmallCrossOverFrequency = 0;

        public PumpTrainerHitObjectSettings()
        {
        }
    }
}
