using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public class Rigidbody2DData : IEquatable<Rigidbody2DData>
    {
        public Vector2 velocity { get; private set; }


        public Rigidbody2DData(Rigidbody2D rb)
        {
            velocity = rb.velocity;
        }
        public Rigidbody2DData(Vector2 vel)
        {
            velocity = vel;
        }


        public Rigidbody2DData Interpolate(Rigidbody2DData other, float t)
        {
            Vector2 t_intVel = Vector2.LerpUnclamped(velocity, other.velocity, t);
            return new Rigidbody2DData(t_intVel);
        }
        public bool Compare(Rigidbody2DData other, float tolerance = 0.05f)
        {
            float t_xDiff = Mathf.Abs(velocity.x - other.velocity.x);
            float t_yDiff = Mathf.Abs(velocity.y - other.velocity.y);

            return t_xDiff <= tolerance && t_yDiff <= tolerance;
        }

        public override string ToString()
        {
            return $"RB2DData: velocity={velocity}";
        }

        public bool Equals(Rigidbody2DData other) => Compare(other, 0.0f);
    }
}