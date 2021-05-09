using RandoMapMod.Monads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RandoMapMod {
	//<summary>This class handles everything related to Parsing and Data of the RandomizerHelperLog.txt file.</summary>
	[DebugName(nameof(HelperLog))]
	public static class HelperLog {
		#region Statics
		public static DataStore Data { get; private set; }

		public static void NewGame() {
			Data = new DataStore();
		}

		public static void Refresh() {
			//I really like the Either monad, it's really neat! However... We're already try catching the parse function.
			//Couldn't you just make the parse function throw an exception and we can log it out here, like we do right now? -CDpr
			try {
				using StreamReader file = new StreamReader(Path.Combine(Application.persistentDataPath, "RandomizerHelperLog.txt"));

				_Parse(file).Case(errorMessage => {
					DebugLog.Error($"Could not parse RandomizeHelperLog.txt: {errorMessage}");
				}, helperData => {
					Data = helperData;
				});
			} catch (Exception e) {
				DebugLog.Error($"Failed to parse RandomizerHelper data: {e}");
			}
		}
		private static Either<string, DataStore> _Parse(StreamReader reader) {
			DataStore newData = new DataStore();
			string line;
			//Read until we see "REACHABLE ITEM LOCATIONS".
			bool sawReachableItemLocations = false;
			while ((line = reader.ReadLine()) != null) {
				if (line.Equals("REACHABLE ITEM LOCATIONS")) {
					sawReachableItemLocations = true;
					break;
				}
			}
			if (!sawReachableItemLocations) {
				return new Either.Left<string, DataStore>("Expected to see 'RECHABLE ITEM LOCATIONS' but hit end of file.");
			}
			line = reader.ReadLine();
			if (!Regex.Match(line, @"There are [0-9]+ unchecked reachable locations.", RegexOptions.None).Success) {
				return new Either.Left<string, DataStore>($"Expected to see 'There are N unchecked reachable locations.' but got {line}");
			}
			line = reader.ReadLine();
			if (!"".Equals(line)) {
				return new Either.Left<string, DataStore>($"Expected a blank line but got {line}");
			}
			bool sawCheckedItemLocations = false;
			Location currentLocation = null;
			const string itemPrefix = " - ";
			while ((line = reader.ReadLine()) != null) {
				if (line.Equals("CHECKED ITEM LOCATIONS")) {
					sawCheckedItemLocations = true;
					break;
				} else if (line.Equals("")) {
					if (currentLocation != null) {
						try {
							newData.AddReachableLocation(currentLocation.Name, currentLocation);
						} catch (ArgumentException e) {
							DebugLog.Warn($"Ignoring duplicate entry for location {currentLocation.Name} (old value = {newData.GetReachableLocation(currentLocation.Name)}, new value = {currentLocation}) {e}");
						}
					}
					currentLocation = null;
				} else if (line.StartsWith(itemPrefix)) {
					currentLocation.Items.Add(line.Substring(itemPrefix.Length));
				} else {
					currentLocation = new Location(line);
				}
			}
			if (!sawCheckedItemLocations) {
				return new Either.Left<string, DataStore>("Expected to see 'CHECKED ITEM LOCATIONS' but reached end of file.");
			}
			while ((line = reader.ReadLine()) != null) {
				if (Regex.Match(line, @"Generated helper log in [0-9.]+ seconds\.", RegexOptions.None).Success) {
					break;
				} else if (line.Equals("")) {
					if (currentLocation != null) {
						try {
							newData.AddCheckedLocation(currentLocation.Name, currentLocation);
						} catch (ArgumentException e) {
							DebugLog.Warn($"Ignoring duplicate entry for locationg {currentLocation.Name} (old value = {newData.GetCheckedLocation(currentLocation.Name)}, new value = {currentLocation}) {e}");
						}
					}
					currentLocation = null;
				} else if (line.StartsWith(itemPrefix)) {
					currentLocation.Items.Add(line.Substring(itemPrefix.Length));
				} else {
					currentLocation = new Location(line);
				}
			}
			return new Either.Right<string, DataStore>(newData);
		}
		#endregion

		#region Mastercard
		public class Location {
			public readonly string Name;
			public readonly HashSet<string> Items;
			public Location(string name) {
				this.Name = name;
				this.Items = new HashSet<string>();
			}
		}
		
		public class DataStore {
			private readonly Dictionary<string, Location> _checked = new Dictionary<string, Location>();
			private readonly Dictionary<string, Location> _reachable = new Dictionary<string, Location>();

			private readonly HashSet<string> _allCheckedItems = new HashSet<string>();
			private readonly HashSet<string> _allReachableItems = new HashSet<string>();

			public HashSet<string> CheckedShopItems => GetCheckedLocation("Shops")?.Items;
			public HashSet<string> ReachableShopItems => GetReachableLocation("Shops")?.Items;

			public Location GetCheckedLocation(string key) {
				if (_checked.TryGetValue(key, out Location value)) {
					return value;
				} else {
					return null;
				}
			}

			public Location GetReachableLocation(string key) {
				if (_reachable.TryGetValue(key, out Location value)) {
					return value;
				} else {
					return null;
				}
			}

			public void AddCheckedLocation(string name, Location location) {
				_checked.Add(name, location);
				foreach (string item in location.Items) {
					if (!_allCheckedItems.Contains(item)) {
						_allCheckedItems.Add(item);
					}
				}
			}

			public void AddReachableLocation(string name, Location location) {
				_reachable.Add(name, location);
				foreach (string item in location.Items) {
					if (!_allReachableItems.Contains(item)) {
						_allReachableItems.Add(item);
					}
				}
			}

			public bool HasChecked(string itemName) {
				return _allCheckedItems.Contains(itemName.Replace('_', ' '));
			}

			public bool CanReach(string itemName) {
				return _allReachableItems.Contains(itemName);
			}

			public string[] GetReachableCountStrings() {
				return _reachable.Values
				.Where((location) => location.Items.Count > 0)
				.OrderByDescending((location) => location.Items.Count)
				.Select((location) => $"{location.Name} - {location.Items.Count} reachable")
				.ToArray();
			}
		}
		#endregion
	}
}
