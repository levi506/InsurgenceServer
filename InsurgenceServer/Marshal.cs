using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InsurgenceServer
{
	public static class Marshal
	{
		public static List<string> IgnoreList = new List<string>
		{
			"0A", "20"
		};
		public static Pokemon MarshalLoadPokemon(byte[] arr)
		{
			var o = new Pokemon();
			var barr = new List<byte>();
			foreach (var b in arr)
			{
				var hex = b.ToString("X2");
				if (hex != "3A" && !IgnoreList.Contains(hex))
				{
					barr.Add(b);
				}
				else if (!IgnoreList.Contains(hex))
				{
					var d = InterpretBytes(barr.ToArray());
					barr.Clear();
					if (d == null)
						continue;
					if (d.Datatype == DataTypes.Species)
						o.Species = (short)d.data;
					else if (d.Datatype == DataTypes.ID)
						o.ID = (string)d.data;
					else if (d.Datatype == DataTypes.TrainerID)
						o.TrainerId = (string)d.data;
					else if (d.Datatype == DataTypes.shiny)
						o.Shiny = (bool)d.data;
					else if (d.Datatype == DataTypes.item)
						o.Item = (short)d.data;
					else if (d.Datatype == DataTypes.name)
						o.OT = (string)d.data;
				}
			}
			//Check the second possible way a pokemon can be shiny
			if (o.Shiny == false)
			{
				try
				{
					uint _trid = uint.Parse(o.TrainerId, System.Globalization.NumberStyles.HexNumber);
					uint _id = uint.Parse(o.ID, System.Globalization.NumberStyles.HexNumber);
					var numa = _id ^ _trid;
					var numb = numa & 0xFFFF;
					var numc = (numa >> 16) & 0xFFFF;
					var numd = numb ^ numc;
					o.Shiny = numd < 16;
				}
				catch (Exception e)
				{
                    Logger.ErrorLog.Log(e);
                    o.Shiny = false;
					Console.WriteLine(e);
				}
			}

			return o;
		}
		private static DataHolder InterpretBytes(byte[] arr)
		{
			if (arr.Count() < 5) return null;
			var name = System.Text.Encoding.Default.GetString(arr);
			if (name.Contains("@species"))
			{
				var l = arr.Length - 10;
				var amount = (int)Math.Ceiling((float)l / 2f);
				var a = arr.Skip(arr.Length - l + (l - amount)).Take(amount).ToArray();
				if (a.Length == 1)
				{
					var li = a.ToList();
					li.Add(0);
					a = li.ToArray();
				}
				var i = BitConverter.ToInt16(a, 0);
				if (l - amount == 0)
					i -= 5;
				return new DataHolder { Datatype = DataTypes.Species, data = i };
			}
			if (name.Contains("@personalID"))
			{
				var a = arr.Skip(arr.Length - 4).Take(4).ToArray();
				a = a.Reverse().ToArray();
				var h = BitConverter.ToString(a).Replace("-", "");
				return new DataHolder { Datatype = DataTypes.ID, data = h };
			}
			if (name.Contains("@trainerID"))
			{
				var a = arr.Skip(arr.Length - 4).Take(4).ToArray();
				a = a.Reverse().ToArray();
				var h = BitConverter.ToString(a).Replace("-", "");
				return new DataHolder { Datatype = DataTypes.TrainerID, data = h };
			}
			if (name.Contains("@shinyflag"))
			{
				if (arr.Last().ToString("X2") == "46")
					return new DataHolder { Datatype = DataTypes.shiny, data = false };
				else if (arr.Last().ToString("X2") == "54")
					return new DataHolder { Datatype = DataTypes.shiny, data = true };
			}
			if (name.Contains("@item") && !name.Contains("@itemInitial") && !name.Contains("@itemRecycle"))
			{
				var l = arr.Length - 6;
				var amount = (int)Math.Ceiling((float)l / 2f);
				var a = arr.Skip(arr.Length - l + (l - amount)).Take(amount).ToArray();
				if (a.Length == 1)
				{
					var li = a.ToList();
					li.Add(0);
					a = li.ToArray();
				}
				var i = BitConverter.ToInt16(a, 0);
				if (i == 28009 || i == 0)
					return null;
				if (l - amount == 0)
					i -= 5;
				return new DataHolder { Datatype = DataTypes.item, data = i };
			}
			if (name.Contains("@ot\""))
			{
				var n = name.Replace("@ot\"", "").Replace(" ","").Replace("\t","".Replace("\\b","").Replace("\b",""));
				return new DataHolder { Datatype = DataTypes.name, data = n };
			}
			return null;
		}
		private class DataHolder
		{
			public DataTypes Datatype { get; set; }
			public object data { get; set; }
		}
		private enum DataTypes
		{
			Null = 0, Species, ID, TrainerID, shiny, item, name
		}
	}
	public class Pokemon
	{
		public short Species;
		public string TrainerId;
		public string ID;
		public bool Shiny;
		public short Item;
		public string OT;
	}
}

