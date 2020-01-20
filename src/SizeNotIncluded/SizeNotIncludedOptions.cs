using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using System;

namespace SizeNotIncluded
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public class SizeNotIncludedOptions : POptions.SingletonOptions<SizeNotIncludedOptions>
    {
		public static Vector2I Smallest = new Vector2I(8, 12);
		public static Vector2I Small = new Vector2I(10, 14);
		public static Vector2I Medium = new Vector2I(12, 18);
		public static Vector2I Large = new Vector2I(14, 20);
		public static Vector2I Default = new Vector2I(16, 24);
		public static Vector2I SmallestTall = new Vector2I(6, 14);
		public static Vector2I SmallTall = new Vector2I(8, 18);
		public static Vector2I MediumTall = new Vector2I(10, 20);
		public static Vector2I LargeTall = new Vector2I(12, 24);
		public static Vector2I DefaultTall = new Vector2I(14, 26);

		public static float ChunkSize = 16f;

		private Vector2I Settings()
		{
			switch(MapType)
			{
				case SizeNotIncludedMapType.Smallest:
					return Smallest;
				case SizeNotIncludedMapType.Small:
					return Small;
				case SizeNotIncludedMapType.Medium:
					return Medium;
				case SizeNotIncludedMapType.Large:
					return Large;
				case SizeNotIncludedMapType.Default:
					return Default;
				case SizeNotIncludedMapType.SmallestTall:
					return SmallestTall;
				case SizeNotIncludedMapType.SmallTall:
					return SmallTall;
				case SizeNotIncludedMapType.MediumTall:
					return MediumTall;
				case SizeNotIncludedMapType.LargeTall:
					return LargeTall;
				case SizeNotIncludedMapType.DefaultTall:
					return DefaultTall;
			}
			return Default;
		}

		public bool IsSmall()
		{
			switch (MapType)
			{
				case SizeNotIncludedMapType.Smallest:
				case SizeNotIncludedMapType.Small:
				case SizeNotIncludedMapType.Medium:
				case SizeNotIncludedMapType.SmallestTall:
				case SizeNotIncludedMapType.SmallTall:
				case SizeNotIncludedMapType.MediumTall:
					return true;
			}
			return false;
		}

		public float NeutroniumBorder(ProcGenGame.Border __instance)
		{
			switch (MapType)
			{
				case SizeNotIncludedMapType.Smallest:
				case SizeNotIncludedMapType.Small:
				case SizeNotIncludedMapType.Medium:
				case SizeNotIncludedMapType.SmallestTall:
				case SizeNotIncludedMapType.SmallTall:
				case SizeNotIncludedMapType.MediumTall:
					return 1.5f;
			}
			return __instance.width;
		}

		// smaller biome border so it doesn't take up most of the map
		public float BiomeBorder(ProcGenGame.Border __instance)
		{
			switch (MapType)
			{
				case SizeNotIncludedMapType.Smallest:
				case SizeNotIncludedMapType.Small:
				case SizeNotIncludedMapType.Medium:
				case SizeNotIncludedMapType.SmallestTall:
				case SizeNotIncludedMapType.SmallTall:
				case SizeNotIncludedMapType.MediumTall:
					return 0.5f;
			}
			return __instance.width;
		}

		public float XSize()
		{
			return ChunkSize * Settings().x;
		}

		public float YSize()
		{
			return ChunkSize * Settings().y;
		}

		public float XScale()
		{
			return Default.x / (float) Settings().x;
		}

		public float YScale()
		{
			return Default.y / (float)Settings().y;
		}

		public float Density()
		{
			return XScale() * YScale();
		}

		public float DensityCapped(float maxDensity)
		{
			return Math.Min(Density(), maxDensity);
		}

		[Option("Map Type", "The type of small map. Description for each contains map size and percentage size compared to default map.")]
		[JsonProperty]
		public SizeNotIncludedMapType MapType { get; set; }

		public SizeNotIncludedOptions()
		{
			MapType = SizeNotIncludedMapType.Smallest;
		}

		public override string ToString()
		{
			return $"SizeNotIncludedOptions[mode={MapType}]";
		}

		public enum SizeNotIncludedMapType
		{
			[Option("Smallest", "128x192 size, 25% area")]
			Smallest,
			[Option("Small", "160x224 size, 40% area")]
			Small,
			[Option("Medium", "192x288 size, 55% area")]
			Medium,
			[Option("Large", "224x320 size, 75% area")]
			Large,
			[Option("Default", "256x384 size, 100% area")]
			Default,
			[Option("Smallest Tall", "96x224 size, 25% area")]
			SmallestTall,
			[Option("Small Tall", "128x288 size, 40% area")]
			SmallTall,
			[Option("Medium Tall", "160x320 size, 55% area")]
			MediumTall,
			[Option("Large Tall", "192x384 size, 75% area")]
			LargeTall,
			[Option("Default Tall", "224x416 size, 100% area")]
			DefaultTall,
		}
	}
}
