using System;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModHorizontalTriples : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> HorizontalTripleFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.1,
        };

        public override string Name => "[Half-Dbl+] Horizontal Triples";
        public override string Acronym => "HHH";
        public override LocalisableString Description =>
            "Three consecutive notes spanning three physical dance pad columns (not note columns) in a single direction.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;
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

            pumpBeatmapConverter.BeatmapWideGeneratorSettings.HorizontalTripleFrequency = HorizontalTripleFrequency.Value;
        }
    }
}
