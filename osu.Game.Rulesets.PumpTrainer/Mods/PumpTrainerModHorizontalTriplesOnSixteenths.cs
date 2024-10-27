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
    public class PumpTrainerModHorizontalTriplesOnSixteenths : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> HorizontalTripleFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.1,
        };

        public override string Name => "[Half-Dbl+] Horiz-3's on 1/4s";
        public override string Acronym => "3H";
        public override LocalisableString Description =>
            "Three consecutive notes spanning three physical columns in a single direction." +
            "These are normally banned for rhythms 1/4 and faster.";
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

            pumpBeatmapConverter.GeneratorSettingsForSixteenthRhythms.HorizontalTripleFrequency = HorizontalTripleFrequency.Value;
        }
    }
}
