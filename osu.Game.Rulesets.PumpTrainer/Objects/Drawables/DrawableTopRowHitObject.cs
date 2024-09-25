using osu.Framework.Allocation;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.PumpTrainer.Objects.Drawables
{
    // todo idk lol
    public partial class DrawableTopRowHitObject : CompositeDrawable
    {
        private PumpTrainerHitObject hitObject;

        public DrawableTopRowHitObject(PumpTrainerHitObject hitObject)
            : base()
        {
            this.hitObject = hitObject;
            Size = new Vector2(DrawablePumpTrainerHitObject.WIDTH);

            Colour = Color4.Gray; // TODO OH MY GOD WHY CAN'T I GET THIS TO BE BLACK AND WHITE
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AddInternal(hitObject.GetAssociatedSprite(textures));
        }
    }
}
