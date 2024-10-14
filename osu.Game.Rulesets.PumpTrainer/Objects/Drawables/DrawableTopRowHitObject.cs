using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.PumpTrainer.Objects.Drawables
{
    public partial class DrawableTopRowHitObject : CompositeDrawable
    {
        private PumpTrainerHitObject hitObject;

        public DrawableTopRowHitObject(PumpTrainerHitObject hitObject)
            : base()
        {
            this.hitObject = hitObject;
            Size = new Vector2(DrawablePumpTrainerHitObject.WIDTH);

            Colour = Color4.Gray;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(hitObject.GetAssociatedSprite(textures, true));
        }

        public void FlashOnHit()
        {
            this.FlashColour(Color4.White, 250, Easing.In);
        }
    }
}
