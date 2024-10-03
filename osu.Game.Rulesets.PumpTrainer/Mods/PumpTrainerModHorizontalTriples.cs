using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModHorizontalTriples : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> HorizontalTripleFrequency { get; } = new BindableDouble(1.0)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 1.0,
            Precision = 0.1,
        };

        public override string Name => "Horizontal Triples";
        public override string Acronym => "HHH";
        public override LocalisableString Description =>
            "Three consecutive notes spanning three physical dance pad columns (not note columns) in one direction.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.HorizontalTripleFrequency = HorizontalTripleFrequency.Value;
        }
    }
}
