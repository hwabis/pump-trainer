﻿using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModDiagonalTwists : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> DiagonalTwistFrequency { get; } = new BindableDouble(0.5)
        {
            MinValue = 0.1,
            MaxValue = 1.0,
            Default = 0.5,
            Precision = 0.1,
        };

        public override string Name => "[AT LEAST S] Diagonal twists";
        public override string Acronym => "D";
        public override LocalisableString Description =>
            "Diagonal crossovers across a center panel.\n" +
            "Only has an effect when the panel set spans at least a single, and the \"horizontal twists\" mod is on.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyIncrease;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.Settings.DiagonalTwistFrequency = DiagonalTwistFrequency.Value;
        }
    }
}