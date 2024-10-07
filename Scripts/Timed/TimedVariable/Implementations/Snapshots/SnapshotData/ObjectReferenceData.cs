using System;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class ObjectReferenceData<T> : IEquatable<ObjectReferenceData<T>>
        where T : class
    {
        public T objRef { get; private set; }

        public ObjectReferenceData(T objRef)
        {
            this.objRef = objRef;
        }

        public bool Equals(ObjectReferenceData<T> other)
        {
            if (other == null)
            {
                // Other data is null, but this one isn't. Not the same.
                return false;
            }
            else if (objRef == null)
            {
                if (other.objRef == null)
                {
                    // Both references are null, that means they are equal.
                    return true;
                }
                else
                {
                    // Only this reference is null, not equal.
                    return false;
                }
            }
            else if (other.objRef == null)
            {
                // Only the other reference is null, not equal.
                return false;
            }
            else
            {
                // Nothing is null, do a normal check.
                return objRef.Equals(other.objRef);
            }
        }
    }
}
