using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SpiritWorld.Events {

	/// <summary>
	/// An event system with channels, to be extended for each needed use.
	/// </summary>
	public abstract class EventSystem<ChannelList> : IEventSystem<ChannelList>
	where ChannelList : struct, Enum {

		/// <summary>
		/// all observers currently listening
		/// </summary>
		List<IObserver> allListeners;

		/// <summary>
		/// All the listeners who are subscribed to a specific channel are stored here.
		/// </summary>
		List<IObserver>[] listenersByChannel;

		/// <summary>
		/// All the listeners who are subscribed every notification, even channel specific ones
		/// </summary>
		List<IObserver> superListeners;

		/// <summary>
		/// Debug Mode.
		/// </summary>
		bool debugMode = false;

		/// <summary>
		/// Constructor
		/// </summary>
		public EventSystem() {
			allListeners = new List<IObserver>();
			superListeners = new List<IObserver>();
			/// Set up the channels based on the enum
			int channelCount = Enum.GetValues(typeof(ChannelList)).Length;
			listenersByChannel = new List<IObserver>[channelCount];
			for (int i = 0; i < channelCount; i++) {
				listenersByChannel[i] = new List<IObserver>();
			}
		}

		///// PUBLIC FUNCTIONS

		/// <summary>
		/// Subscribe to the listener list.
		/// If the channel is left null then just sibscribe to all.
		/// </summary>
		public void subscribe(IObserver newListener, ChannelList? channelToSubscribeTo = null) {
			allListeners.Add(newListener);
			if (channelToSubscribeTo != null) {
				subscribeToChannel(newListener, Convert.ToInt32(channelToSubscribeTo));
			}
		}

		/// <summary>
		/// Subscribe to the listener list.
		/// If the channel is left null then just sibscribe to all.
		/// </summary>
		public void subscribeToAll(IObserver newListener) {
			allListeners.Add(newListener);
			superListeners.Add(newListener);
		}

		/// <summary>
		/// Notify all listening observers of an event
		/// </summary>
		/// <param name="event">The event to notify all listening observers of</param>
		/// <param name="origin">(optional) the osurce of the event</param>
		public void notifyAllOf(IEvent @event, bool sendAsync = false) {
			if (sendAsync) {
				new Thread(() => {
					notifyAllOf(@event);
				}) { Name = $"{@event.name} - Messenger" }.Start();
			} else {
				notifyAllOf(@event);
			}
		}

		/// <summary>
		/// Notify all listening observers of an event
		/// </summary>
		/// <param name="event">The event to notify all listening observers of</param>
		/// <param name="channelToNotify">The channel to notify</param>
		/// <param name="sendAsync">(optional)Whether to send asyncly in a thread</param>
		/// <param name="origin">(optional) the osurce of the event</param>
		public void notifyChannelOf(IEvent @event, ChannelList channelToNotify, bool sendAsync = false) {
			if (sendAsync) {
				new Thread(() => {
					notifyChannelOf(@event, channelToNotify);
				}) { Name = $"{@event.name} - Messenger" }.Start();
			} else {
				notifyChannelOf(@event, channelToNotify);
			}
		}

		///// SUB FUNCTIONS

		/// <summary>
		/// Wrapper for adding to the channel subscriber list.
		/// </summary>
		/// <param name="newListener"></param>
		/// <param name="channelToSubscribeTo"></param>
		void subscribeToChannel(IObserver newListener, int channelToSubscribeTo) {
			if (channelToSubscribeTo < listenersByChannel.Length && channelToSubscribeTo > 0) {
				listenersByChannel[channelToSubscribeTo].Add(newListener);
			} else ThrowMissingChannelException(channelToSubscribeTo);
		}

		/// <summary>
		/// Notify all listening observers of an event
		/// </summary>
		/// <param name="event">The event to notify all listening observers of</param>
		/// <param name="origin">(optional) the osurce of the event</param>
		void notifyAllOf(IEvent @event) {
			if (debugMode) {
				Debug.Log($"Notifiying ALL of {@event.name}");
			}
			foreach (IObserver observer in allListeners) {
				observer.notifyOf(@event);
			}
		}

		/// <summary>
		/// Notify all listening observers of an event
		/// </summary>
		/// <param name="event">The event to notify all listening observers of</param>
		/// <param name="origin">(optional) the osurce of the event</param>
		void notifyChannelOf(IEvent @event, ChannelList channelToNotify) {
			int channelNumber = Convert.ToInt32(channelToNotify);
			if (channelNumber < listenersByChannel.Length && channelNumber > 0) {
				if (debugMode) {
					Debug.Log($"Notifiying channel: {channelToNotify} of {@event.name}");
				}
				foreach (IObserver observer in listenersByChannel[channelNumber]) {
					observer.notifyOf(@event);
				}
			} else ThrowMissingChannelException(channelNumber);

			// also let superlisteners know
			notifySuperListenersOf(@event);
		}

		/// <summary>
		/// Notify all the super listeners
		/// </summary>
		/// <param name="event">The event to notify all listening observers of</param>
		void notifySuperListenersOf(IEvent @event) {
			if (debugMode) {
				Debug.Log($"Notifiying Super-Listeners of {@event.name}");
			}
			foreach (IObserver observer in superListeners) {
				observer.notifyOf(@event);
			}
		}

		/// <summary>
		/// Throw a missing channel exception
		/// </summary>
		/// <param name="missingChannel"></param>
		static void ThrowMissingChannelException(int missingChannel) {
			throw new System.IndexOutOfRangeException($"Event System does not have a chanel {missingChannel}!");
		}
	}
}

