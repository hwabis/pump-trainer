﻿using System;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.PumpTrainer.Beatmaps;
using osu.Game.Rulesets.PumpTrainer.Mods.ExcludeColumns;

namespace osu.Game.Rulesets.PumpTrainer.Mods
{
    public class PumpTrainerModBanSinglesTwists : Mod, IApplicableToBeatmapConverter
    {
        [SettingSource("Frequency")]
        public Bindable<double> SinglesTwistFrequency { get; } = new BindableDouble(0)
        {
            MinValue = 0,
            MaxValue = 0.9,
            Default = 0,
            Precision = 0.1,
        };

        public override string Name => "[P1Single+] Beginner";
        public override string Acronym => "B";
        public override LocalisableString Description =>
            "Bans the right foot hitting the left pads and vice versa, in the context of a single pad.";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.DifficultyReduction;
        public override Type[] IncompatibleMods => new Type[]
        {
            typeof(PumpTrainerModBanFarColumns),
            typeof(PumpTrainerModHorizontalTwists),
            typeof(PumpTrainerModCornersOnSixteenths),
            typeof(PumpTrainerModDiagonalSkips),
            typeof(PumpTrainerModHorizontalTriplesOnSixteenths),

            typeof(PumpTrainerModExcludeP1DL),
            typeof(PumpTrainerModExcludeP1UL),
            typeof(PumpTrainerModExcludeP1C),
            typeof(PumpTrainerModExcludeP1UR),
            typeof(PumpTrainerModExcludeP1DR),
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var pumpBeatmapConverter = (PumpTrainerBeatmapConverter)beatmapConverter;

            pumpBeatmapConverter.BeatmapWideGeneratorSettings.SinglesTwistFrequency = SinglesTwistFrequency.Value;
        }
    }
}
