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
    public class PumpTrainerModBanFarColumns : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> FarColumnsFrequency { get; } = new BindableDouble(0)
        {
            MinValue = 0,
            MaxValue = 0.9,
            Default = 0,
            Precision = 0.1,
        };

        public override string Name => "[Half-Dbl+] Ban Far Columns";
        public override string Acronym => "D";
        public override LocalisableString Description =>
            "Bans two consecutive notes in the half-doubles region that are not in physically adjacent columns.";
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
