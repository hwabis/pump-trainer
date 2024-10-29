namespace osu.Game.Rulesets.PumpTrainer.Beatmaps
{
    /// <summary>
    /// By default, all the fields in this class are set to maximize the frequency of each pattern.
    /// </summary>
    public class PumpTrainerHitObjectGeneratorSettingsPerHitObject
    {
        /// <summary>
        /// 0 to 1 determining how frequently to generate a corner pattern. Higher means more likely.
        /// Corner patterns include 90 degree patterns and V shape patterns (see examples below).
        /// (Corner patterns are usually banned on sixteenth rhythms.)
        /// Examples:
        /// Starting left: UL, UR, DR (and 7 other variations per singles panel) - 90 degrees across a single panel.
        /// Starting left: UL, DR, UR (and 7 other variations per singles panel) - V shape across a single panel.
        /// </summary>
        public double CornersFrequency = 1;

        /// <summary>
        /// 0 to 1 determining how frequently to generate 3 horizontally adjacent notes that go in one direction (i.e. only left, or only right),
        /// and are not all on the same single pad. Higher means more likely.
        /// Example: P1C --> P1DR --> P2UL
        /// NON-example: P1C --> P2UL --> P2DL (because the notes do not go in one direction)
        /// NON-example: P1C --> P2UL --> P1DR (because the notes do not go in one direction)
        /// NON-example: P1UL --> P1C --> P1UR (because the notes only span a single pad)
        /// NON-example: P1C --> P1UR --> P2C (because the notes are not horizontally adjacent. This case is actually always banned, no matter what mods are on.)
        /// </summary>
        public double HorizontalTripleFrequency = 1;

        public PumpTrainerHitObjectGeneratorSettingsPerHitObject()
        {
        }
    }
}
