// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Objects
{
    public class PumpTrainerHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new Judgement();

        public Foot IntendedFoot;
        public Column Column;
    }

    public enum Column
    {
        P1DL,
        P1UL,
        P1C,
        P1UR,
        P1DR,
        P2DL,
        P2UL,
        P2C,
        P2UR,
        P2DR,
    }

    public enum Foot
    {
        Left,
        Right,
    }
}
