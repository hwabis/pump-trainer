using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerHitObjectGeneratorSettings
    {
        public List<Column> AllowedColumns =
            [Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR];

        /// <summary>
        /// 0 to 1 determining how frequently to generate two notes that are in adjacent columns on the physical dance pad.
        /// One of the notes must be a center panel (aka they do not span across a center panel, aka they must be in the half-doubles region).
        /// Higher means more likely.
        /// Example starting left foot: P1C --> P2UL
        /// Example starting right foot: P2DL --> P1C
        /// NON-example starting left foot: P1DL --> P1DR
        /// </summary>
        public double FarColumnsFrequency = 1;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a horizontal twist. Higher means more likely.
        /// Example starting left foot: UL --> C --> UR
        /// </summary>
        public double HorizontalTwistFrequency = 0;

        /// <summary>
        /// Determines whether to allow generating diagonal twists.
        /// This requires horizontal twists to be on (greater than 0) to have any effect.
        /// Example starting left foot: DL --> C --> UR
        /// </summary>
        public bool AllowDiagonalTwists = false;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal skip. Higher means more likely.
        /// Example starting left foot: DR --> UL
        /// </summary>
        public double DiagonalSkipFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a large horizontal twist. Higher means more likely.
        /// This kind of twist is very rare. Example charts are Uranium D24 and See D20 (those are actually the only charts I know of that have this pattern lol).
        /// Example starting right foot: P1C --> P2UL
        /// </summary>
        public double LargeTwistFrequency = 0;

        public PumpTrainerHitObjectGeneratorSettings()
        {
        }
    }
}
