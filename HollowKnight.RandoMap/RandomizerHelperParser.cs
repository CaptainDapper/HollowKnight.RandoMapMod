using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RandoMapMod
{
	public interface Either<E, V>
	{
		void Case(Action<E> ifLeft, Action<V> ifRight);
	}

	public static class Either {
		public sealed class Left<E, V> : Either<E, V>
		{
			public readonly E value;
			public Left(E value)
			{
				this.value = value;
			}

			public void Case(Action<E> ifLeft, Action<V> ifRight)
			{
				ifLeft.Invoke(value);
			}
		}
		public sealed class Right<E, V> : Either<E,V>
		{
			public readonly V value;
			public Right(V value)
			{
				this.value = value;
			}

			public void Case(Action<E> ifLeft, Action<V> ifRight)
			{
				ifRight.Invoke(value);
			}
		}
	}

	public class Location
	{
		public readonly string name;
		public readonly HashSet<String> items;
		public Location(string name)
		{
			this.name = name;
			this.items = new HashSet<String>();
		}
	}

	public class HelperData
	{
		public readonly Dictionary<string, Location> reachable = new Dictionary<string, Location>();
		/// <summary>
		/// "checked" seems to different things for shop locations than for all other locations.
		/// </summary>
		public readonly Dictionary<string, Location> checkedd = new Dictionary<string, Location>();
	}

	class RandomizerHelperParser
	{
		private static readonly DebugLog logger = new DebugLog(nameof(RandomizerHelperParser));

		public Either<string, HelperData> parse(System.IO.StreamReader reader)
		{
			HelperData retVal = new HelperData();
			string line;
			//Read until we see "RECHABLE ITEM LOCATIONS".
			Boolean sawReachableItemLocations = false;
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Equals("REACHABLE ITEM LOCATIONS"))
				{
					sawReachableItemLocations = true;
					break;
				}
			}
			if (!sawReachableItemLocations)
			{
				return new Either.Left<string, HelperData>("Expected to see 'RECHABLE ITEM LOCATIONS' but hit end of file.");
			}
			line = reader.ReadLine();
			if (!Regex.Match(line, @"There are [0-9]+ unchecked reachable locations.", RegexOptions.None).Success)
			{
				return new Either.Left<string, HelperData>($"Expected to see 'There are N unchecked reachable locations.' but got {line}");
			}
			line = reader.ReadLine();
			if (!"".Equals(line))
			{
				return new Either.Left<string, HelperData>($"Expected a blank line but got {line}");
			}
			Boolean sawCheckedItemLocations = false;
			Location currentLocation = null;
			const string itemPrefix = " - ";
			while ((line = reader.ReadLine()) != null)
			{
				if (line.Equals("CHECKED ITEM LOCATIONS"))
				{
					sawCheckedItemLocations = true;
					break;
				}
				else if (line.Equals(""))
				{
					if (currentLocation != null) {
						try
						{
							retVal.reachable.Add(currentLocation.name, currentLocation);
						} catch (ArgumentException e)
						{
							logger.Warn($"Ignoring duplicate entry for locationg {currentLocation.name} (old value = {retVal.reachable[currentLocation.name]}, new value = {currentLocation}) {e.ToString()}");
						}
					}
					currentLocation = null;
				}
				else if (line.StartsWith(itemPrefix))
				{
					currentLocation.items.Add(line.Substring(itemPrefix.Length));
				}
				else
				{
					currentLocation = new Location(line);
				}
			}
			if (!sawCheckedItemLocations)
			{
				return new Either.Left<string, HelperData>("Expected to see 'CHECKED ITEM LOCATIONS' but reached end of file.");
			}
			while ((line = reader.ReadLine()) != null)
			{
				if (Regex.Match(line, @"Generated helper log in [0-9.]+ seconds\.", RegexOptions.None).Success)
				{
					break;
				}
				else if (line.Equals(""))
				{
					if (currentLocation != null) {
						try
						{
							retVal.checkedd.Add(currentLocation.name, currentLocation);
						} catch (ArgumentException e)
						{
							logger.Warn($"Ignoring duplicate entry for locationg {currentLocation.name} (old value = {retVal.reachable[currentLocation.name]}, new value = {currentLocation}) {e.ToString()}");
						}
					}
					currentLocation = null;
				}
				else if (line.StartsWith(itemPrefix))
				{
					currentLocation.items.Add(line.Substring(itemPrefix.Length));
				}
				else
				{
					currentLocation = new Location(line);
				}
			}
			return new Either.Right<string, HelperData>(retVal);
		}
	}
}
