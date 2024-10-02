using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModDiagonalSkips : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> DiagonalSkipFrequency { get; } = new BindableDouble(0.53)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.3,
            Precision = 0.1,
        };

        public override string Name => "[AT LEAST S] Diagonal skips";
        public override string Acronym => "DD";
        public override LocalisableString Description => "Crossovers across a single panel, skipping over the center panel.\n";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.DiagonalSkipFrequency = DiagonalSkipFrequency.Value;
        }
    }
}
