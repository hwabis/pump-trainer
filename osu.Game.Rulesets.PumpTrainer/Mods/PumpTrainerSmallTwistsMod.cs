using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerSmallTwistsMod : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> SmallTwistFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.01,
        };

        public override string Name => "Add small twists";
        public override string Acronym => "ST";
        public override LocalisableString Description => "Feet cross over horizontally across a center panel.";
        public override double ScoreMultiplier => 1;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.SmallTwistFrequency = SmallTwistFrequency.Value;
        }
    }
}
