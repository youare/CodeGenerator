using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGenerator
{
    public struct Optional<T>
    {
        public static Optional<T> Empty => new Optional<T>();
        public static Optional<T> Some(T value) => new Optional<T>(value);
        public bool HasValue { get; }
        public bool IsEmpty => !HasValue;
        private readonly T _value;
        private Optional(T value)
        {
            _value = value;
            HasValue = true;
        }
        public T Value
        {
            get
            {
                if (HasValue) return _value;
                throw new InvalidOperationException();
            }
        }
        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }
        public static explicit operator Optional<T>(T value)
        {
            return Some(value);
        }
        public IEnumerable<object> GetEquialityComponents()
        {
            yield return HasValue;
            if (HasValue) yield return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            var other = (Optional<T>)obj;
            if (this.HasValue && other.HasValue)
            {
                return GetEquialityComponents().SequenceEqual(other.GetEquialityComponents());
            }
            else
            {
                return IsEmpty && other.IsEmpty;
            }
        }

        public static bool operator ==(Optional<T> a, Optional<T> b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(Optional<T> a, Optional<T> b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return GetEquialityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 397 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }
        public override string ToString()
        {
            if (HasValue)
            {
                return Value.ToString();
            }
            else
            {
                return $"Optional<{typeof(T).Name}>.Empty";
            }
        }
    }
}
