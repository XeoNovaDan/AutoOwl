using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace AutoOwl
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		private static readonly TraitDef NightOwl = DefDatabase<TraitDef>.GetNamed("NightOwl");

		private static readonly Queue<Pawn> PawnsToCheck = new Queue<Pawn>();

		private static void AdjustForNightOwlSchedule(Pawn pawn)
		{
			if (pawn?.IsFreeColonist ?? false)
			{
				if ((pawn.story?.traits?.HasTrait(NightOwl) ?? false) && pawn.timetable != null)
				{
					pawn.timetable.times = new List<TimeAssignmentDef>(GenDate.HoursPerDay);

					for (int i = 0; i < GenDate.HoursPerDay; i++)
					{
						TimeAssignmentDef assignment;

						if (i >= 11 && i <= 18)
						{
							assignment = TimeAssignmentDefOf.Sleep;
						}

						else
						{
							assignment = TimeAssignmentDefOf.Anything;
						}

						pawn.timetable.times.Add(assignment);
					}
				}
			}
		}

		[HarmonyPatch(typeof(Pawn))]
		[HarmonyPatch(nameof(Pawn.SetFaction))]
		public static class Patch_Pawn
		{
			public static void Postfix(Pawn __instance, Faction newFaction)
			{
				if (newFaction == Faction.OfPlayerSilentFail)
				{
					PawnsToCheck.Enqueue(__instance);
				}
			}
		}

		[HarmonyPatch(typeof(Thing))]
		[HarmonyPatch(nameof(Thing.SetFactionDirect))]
		public static class Patch_PawnGenerator
		{
			public static void Postfix(Thing __instance, Faction newFaction)
			{
				if (__instance is Pawn pawn && newFaction == Faction.OfPlayerSilentFail)
				{
					PawnsToCheck.Enqueue(pawn);
				}
			}
		}

		[HarmonyPatch(typeof(GameComponentUtility))]
		[HarmonyPatch(nameof(GameComponentUtility.GameComponentUpdate))]
		public static class Patch_GameComponentUtility
		{
			public static void Postfix()
			{
				if (PawnsToCheck.Count > 0)
				{
					AdjustForNightOwlSchedule(PawnsToCheck.Dequeue());
				}
			}
		}

		static HarmonyPatches()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("XeoNovaDan.AutoOwl");

			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
