namespace CommonDomain
{
	using System;
	using System.Collections.Generic;

	public interface ISaga
	{
		Guid Id { get; }
		int Version { get; }

		void Transition(object message);

		IEnumerable<object> GetUncommittedEvents();
		void ClearUncommittedEvents();

		IEnumerable<object> GetUndispatchedMessages();
		void ClearUndispatchedMessages();
	}
}