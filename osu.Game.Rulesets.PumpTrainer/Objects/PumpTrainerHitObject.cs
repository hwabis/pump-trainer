// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.PumpTrainer.Objects
{
    public class PumpTrainerHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new Judgement();

        public Foot IntendedFoot;
        public Column Column;

        public PumpTrainerHitObject(Column column)
            : base()
        {
            Column = column;
        }

        // does this belong here?
        public Sprite GetAssociatedSprite(TextureStore textures)
        {
            string textureString;

            switch (Column)
            {
                case Column.P1DL:
                case Column.P1DR:
                case Column.P2DL:
                case Column.P2DR:
                    textureString = "DL";
                    break;
                case Column.P1UL:
                case Column.P1UR:
                case Column.P2UL:
                case Column.P2UR:
                    textureString = "UL";
                    break;
                default:
                    textureString = "C";
                    break;
            }

            Sprite sprite = new()
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                Texture = textures.Get(textureString),
            };

            switch (Column)
            {
                case Column.P1UR:
                case Column.P1DR:
                case Column.P2UR:
                case Column.P2DR:
                    sprite.Scale = new(-1, 1);
                    break;
            }

            return sprite;
        }
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
