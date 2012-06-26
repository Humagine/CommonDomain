namespace CommonDomain
{
	using System;
	using System.Collections.Generic;

	public interface IAggregate
	{
		Guid Id { get; }
		int Version { get; }

		void ApplyEvent(object @event);
		IEnumerable<object> GetUncommittedEvents();
		void ClearUncommittedEvents();

		IMemento GetSnapshot();
	}
}