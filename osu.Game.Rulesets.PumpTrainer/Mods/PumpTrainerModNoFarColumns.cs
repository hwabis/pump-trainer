using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;
using System;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModNoFarColumns : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> FarColumnsFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0,
            MaxValue = 0.9,
            Default = 0.5,
            Precision = 0.1,
        };

        public override string Name => "[Half-Dbl+] No Far Columns";
        public override string Acronym => "D";
        public override LocalisableString Description =>
            "Reduces the frequency of two physically non-adjacent consecutive notes in the half-doubles region.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModExcludeP1C),
            typeof(PumpTrainerModExcludeP1UR),
            typeof(PumpTrainerModExcludeP1DR),
            typeof(PumpTrainerModExcludeP2DL),
            typeof(PumpTrainerModExcludeP2UL),
            typeof(PumpTrainerModExcludeP2C),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.BeatmapWideGeneratorSettings.FarColumnsFrequency = FarColumnsFrequency.Value;
        }
    }
}
