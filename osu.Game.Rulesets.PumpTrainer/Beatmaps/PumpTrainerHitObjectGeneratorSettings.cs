using System.Collections.Generic;
using osu.Game.Rulesets.PumpTrainer.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    public class PumpTrainerHitObjectGeneratorSettings
    {
        public List<Column> AllowedColumns =
            [Column.P1DL, Column.P1UL, Column.P1C, Column.P1UR, Column.P1DR, Column.P2DL, Column.P2UL, Column.P2C, Column.P2UR, Column.P2DR];

        /// <summary>
        /// 0 to 1 determining how frequently to generate a singles "twist" that's not a horizontal twist. Higher means more likely.
        /// Example starting left foot: C --> UR --> DR
        /// Example starting left foot: C --> DR --> UR
        /// NON-example starting left foot: UL --> C --> UR (this is a horizontal twist)
        /// tl;dr When this is 0, you get something resembling the very ending of Flavor Step D19.
        /// </summary>
        public double SinglesTwistFrequency = 1;

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
        /// 0 to 1 determining how frequently to generate a diagonal twist. Higher means more likely.
        /// Example starting left foot: DL --> C --> UR
        /// </summary>
        public double DiagonalTwistFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate a diagonal skip. Higher means more likely.
        /// Example starting left foot: DR --> UL
        /// </summary>
        public double DiagonalSkipFrequency = 0;

        /// <summary>
        /// 0 to 1 determining how frequently to generate 3 adjacent notes that span unique columns on the physical dance pad,
        /// and go in one direction (i.e. only left, or only right).
        /// The only exception to this are triples that are all within 1 singles pad.
        /// Higher means more likely. For example, P1UR and P1DR are on the same unique column on the physical dance pad.
        /// Example: P1C --> P1DR --> P2UL
        /// Example: P2UL --> P1DR --> P1C
        /// NON-example: P1C --> P2UL --> P2DL (because the physical columns are not unique)
        /// NON-example: P1C --> P2UL --> P1DR (because the notes do not go in one direction)
        /// NON-example: P1UL --> P1C --> P1UR (because the notes only span a singles pad)
        /// </summary>
        public double HorizontalTripleFrequency = 0;

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
