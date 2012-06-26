namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;
	using Routing;

	public abstract class AggregateBase : IAggregate, IEquatable<IAggregate>
	{
		private readonly IList<object> uncommittedEvents = new List<object>();
		private IRouteEvents registeredRoutes;

		public Guid Id { get; protected set; }
		public int Version { get; protected set; }

		protected AggregateBase(IRouteEvents handler)
		{
			RegisteredRoutes = handler ?? new ConventionEventRouter(true, this);
			RegisteredRoutes.Register(this);
		}

		protected IRouteEvents RegisteredRoutes
		{
			get { return registeredRoutes; }
			set
			{
				if (value == null)
				{
					throw new InvalidOperationException("AggregateBase must have an event router to function");
				}

				registeredRoutes = value;
			}
		}

		protected void Register<T>(Action<T> route)
		{
			RegisteredRoutes.Register(route);
		}

		protected void RaiseEvent(object @event)
		{
			((IAggregate) this).ApplyEvent(@event);
			uncommittedEvents.Add(@event);
		}

		protected virtual IMemento GetSnapshot()
		{
			return null;
		}

		void IAggregate.ApplyEvent(object @event)
		{
			RegisteredRoutes.Dispatch(@event);
			Version++;
		}

		IEnumerable<object> IAggregate.GetUncommittedEvents()
		{
			return uncommittedEvents;
		}

		void IAggregate.ClearUncommittedEvents()
		{
			uncommittedEvents.Clear();
		}

		IMemento IAggregate.GetSnapshot()
		{
			var snapshot = GetSnapshot();

			if (snapshot == null)
			{
				return null;
			}

			snapshot.Id = Id;
			snapshot.Version = Version;
			return snapshot;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as IAggregate);
		}

		public virtual bool Equals(IAggregate other)
		{
			return null != other && other.Id == Id;
		}
	}
}