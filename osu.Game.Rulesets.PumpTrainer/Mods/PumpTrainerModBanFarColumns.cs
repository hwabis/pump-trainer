using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using System;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModBanFarColumns : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> FarColumnsFrequency { get; } = new BindableDouble(0.3)
        {
            MinValue = 0,
            MaxValue = 0.9,
            Default = 0.3,
            Precision = 0.1,
        };

        public override string Name => "[AT LEAST HALF-D] Ban Far Columns";
        public override string Acronym => "D";
        public override LocalisableString Description =>
            "Reduces the frequency of two consecutive notes that are not in physically adjacent columns.\n" +
            "Only has an effect on at least half-doubles.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModDiagonalSkips),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.FarColumnsFrequency = FarColumnsFrequency.Value;
        }
    }
}
