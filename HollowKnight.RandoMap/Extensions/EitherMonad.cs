using System;

namespace RandoMapMod.Monads {
	public interface Either<E, V> {
		void Case(Action<E> ifLeft, Action<V> ifRight);
	}

	public static class Either {
		public sealed class Left<E, V> : Either<E, V> {
			public readonly E Value;
			public Left(E value) {
				this.Value = value;
			}

			public void Case(Action<E> ifLeft, Action<V> ifRight) {
				ifLeft.Invoke(Value);
			}
		}
		public sealed class Right<E, V> : Either<E, V> {
			public readonly V Value;
			public Right(V value) {
				this.Value = value;
			}

			public void Case(Action<E> ifLeft, Action<V> ifRight) {
				ifRight.Invoke(Value);
			}
		}
	}
}
