using System;
using System.Collections;
using System.Collections.Generic;

namespace Shane.Church.Utility
{
	public class CastEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator enumerator;

		public CastEnumerator(IEnumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			enumerator.Reset();
		}

		public T Current
		{
			get { return (T)enumerator.Current; }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}
	}
}
