using System;
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

		private static void GenerateNightOwlSchedule(Pawn pawn)
		{
			if (pawn.story?.traits?.HasTrait(NightOwl) ?? false && pawn.timetable != null)
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

		// Modifies pawns after they've been generated
		[HarmonyPatch(typeof(PawnGenerator))]
		[HarmonyPatch("GeneratePawn")]
		[HarmonyPatch(new Type[] { typeof(PawnGenerationRequest) })]
		public static class Patch_PawnGenerator
		{
			public static void Postfix(Pawn __result)
			{
				if (__result != null)
				{
					GenerateNightOwlSchedule(__result);
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
