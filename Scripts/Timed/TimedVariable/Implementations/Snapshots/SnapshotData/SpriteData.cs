using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class SpriteData : IEquatable<SpriteData>
    {
        public Sprite sprite { get; private set; }

        public SpriteData(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public bool Equals(SpriteData other)
        {
            if (other == null)
            {
                // Other data is null, but this one isn't. Not the same.
                return false;
            }
            else if (sprite == null)
            {
                if (other.sprite == null)
                {
                    // Both sprite are null, that means they are equal.
                    return true;
                }
                else
                {
                    // Only this sprite is null, not equal.
                    return false;
                }
            }
            else if (other.sprite == null)
            {
                // Only the other sprite is null, not equal.
                return false;
            }
            else
            {
                // Nothing is null, do a normal check.
                return sprite.Equals(other.sprite);
            }
        }
    }
}