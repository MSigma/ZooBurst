using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZooBurst.View.Activities;

namespace ZooBurst.View.Graphics
{
    public abstract class Renderable
    {
        public Renderable Parent { get; private set; }
        public float Alpha { get; private set; }
        public Vector2 Position => (Parent != null ? Parent.Position + LocalPosition : LocalPosition);
        public Vector2 LocalPosition { get; set; }

        protected List<Renderable> Children;
        protected List<Activity> RunningActivities;

        protected Renderable()
        {
            Alpha = 1.0F;

            RunningActivities = new List<Activity>();
            Children = new List<Renderable>();
            LocalPosition = Vector2.Zero;
        }

        public void Clear() => Children.Clear();
        public void AddActivity(Activity action) => RunningActivities.Add(action);
        public void Detach() => Parent.RemoveChild(this);

        public void AddChild(Renderable child)
        {
            if (child == null)
                return;

            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(Renderable child)
        {
            Children.Remove(child);
            child.Parent = null;
        }

        public void SetAlpha(float alpha)
        {
            Alpha = alpha;

            foreach (var child in Children.Where(child => child.Alpha > alpha))
                child.SetAlpha(alpha);
        }

        public virtual void Update(float delta)
        {
            for (var i = 0; i < RunningActivities.Count; i++)
            {
                if (RunningActivities[i].Run(this, delta))
                    RunningActivities.RemoveAt(i--);
            }

            for (var i = 0; i < Children.Count; i++)
                Children[i]?.Update(delta);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (var i = 0; i < Children.Count; i++)
                Children[i]?.Draw(gameTime, spriteBatch);
        }
    }
}