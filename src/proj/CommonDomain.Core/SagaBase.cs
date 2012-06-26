namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;

	public class SagaBase<TMessage> : ISaga, IEquatable<ISaga>
		where TMessage : class
	{
		private readonly IDictionary<Type, Action<TMessage>> handlers = new Dictionary<Type, Action<TMessage>>();
		private readonly IList<TMessage> uncommitted = new List<TMessage>();
		private readonly IList<TMessage> undispatched = new List<TMessage>();

		public Guid Id { get; protected set; }
		public int Version { get; private set; }

		protected void Register<TRegisteredMessage>(Action<TRegisteredMessage> handler)
			where TRegisteredMessage : class, TMessage
		{
			handlers[typeof (TRegisteredMessage)] = message => handler(message as TRegisteredMessage);
		}

		public void Transition(object message)
		{
			var msg = message as TMessage;
			handlers[message.GetType()](msg);
			uncommitted.Add(msg);
			Version++;
		}

		protected void Dispatch(TMessage message)
		{
			undispatched.Add(message);
		}

		IEnumerable<object> ISaga.GetUncommittedEvents()
		{
			return uncommitted;
		}

		void ISaga.ClearUncommittedEvents()
		{
			uncommitted.Clear();
		}

		IEnumerable<object> ISaga.GetUndispatchedMessages()
		{
			return undispatched;
		}

		void ISaga.ClearUndispatchedMessages()
		{
			undispatched.Clear();
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ISaga);
		}

		public virtual bool Equals(ISaga other)
		{
			return null != other && other.Id == Id;
		}
	}
}