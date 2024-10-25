using osu.Framework.Bindables;
using osu.Framework.Localisation;
using System;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModCornersOnSixteenths : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> CornersOnSixteenthsFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.1,
        };

        public override string Name => "[P1Single+] Corners on 1/4s";
        public override string Acronym => "CO";
        public override LocalisableString Description =>
            "90-degree and V-twists across a single panel.\n" +
            "These are normally banned for rhythms 1/4 and faster.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModExcludeP1DL),
            typeof(PumpTrainerModExcludeP1UL),
            typeof(PumpTrainerModExcludeP1C),
            typeof(PumpTrainerModExcludeP1UR),
            typeof(PumpTrainerModExcludeP1DR),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.CornersOnSixteenthRhythmsFrequency = CornersOnSixteenthsFrequency.Value;
        }
    }
}
