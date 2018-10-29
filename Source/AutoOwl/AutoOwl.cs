using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;

namespace AutoOwl
{

    // from XND - too lazy to keep this in more than one .cs file :P

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            HarmonyInstance h = HarmonyInstance.Create("XeoNovaDan.AutoOwl");

            h.Patch(AccessTools.Method(typeof(PawnComponentsUtility), nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents)),
                postfix: new HarmonyMethod(patchType, nameof(Postfix_PawnComponentsUtility_AddAndRemoveDynamicComponents)));
        }

        public static void Postfix_PawnComponentsUtility_AddAndRemoveDynamicComponents(Pawn pawn)
        {
            if (pawn.story?.traits?.HasTrait(AO_TraitDefOf.NightOwl) == true && pawn.timetable != null)
                pawn.timetable.AdjustForOwl();
        }

    }

    public static class AutoOwlUtility
    {

        public static void AdjustForOwl(this Pawn_TimetableTracker timetable)
        {
            timetable.times = new List<TimeAssignmentDef>(GenDate.HoursPerDay);
            for (int i = 0; i < GenDate.HoursPerDay; i++)
            {
                TimeAssignmentDef item = (i >= 11 && i <= 18) ? TimeAssignmentDefOf.Sleep : TimeAssignmentDefOf.Anything;
                timetable.times.Add(item);
            }

        }

    }

    [DefOf]
    public static class AO_TraitDefOf
    {

        public static TraitDef NightOwl;

    }

    //internal static class Patch_AutoOwl
    //{
    //    private static TraitDef OwlTrait;

    //    private static bool enabled_bool = true;

    //    private static bool enabled
    //    {
    //        get
    //        {
    //            if (enabled_bool == true && OwlTrait == null)
    //            {
    //                OwlTrait = DefDatabase<TraitDef>.GetNamedSilentFail("NightOwl");
    //                enabled_bool = OwlTrait != null;
    //            }

    //            return enabled_bool;
    //        }
    //    }

    //    public static bool IsOwl(this Pawn pawn)
    //    {
    //        if (enabled)
    //            return pawn.story?.traits?.HasTrait(OwlTrait) == true;
    //        else
    //            return false;
    //    }

    //    public static void MakeOwl(this Pawn_TimetableTracker timetable)
    //    {
    //        timetable.times = new List<TimeAssignmentDef>(24);
    //        for (int i = 0; i < 24; i++)
    //        {
    //            TimeAssignmentDef item = (i >= 11 && i <= 18) ? TimeAssignmentDefOf.Sleep : TimeAssignmentDefOf.Anything;
    //            timetable.times.Add(item);
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    //public static class Patch_AutoOwl_GeneratePawn
    //{
    //    public static void Postfix(ref Pawn __result, PawnGenerationRequest request)
    //    {
    //        if (__result is Pawn pawn && pawn.IsOwl() && pawn.timetable != null)
    //        {
    //            pawn.timetable.MakeOwl();
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Pawn_TimetableTracker))]
    //[HarmonyPatch(new[] { typeof(Pawn) })]
    //public static class Patch_AutoOwl_Create_TimetableTracker
    //{
    //    public static void Postfix(ref Pawn_TimetableTracker __instance, Pawn pawn)
    //    {
    //        if (pawn.IsOwl())
    //        {
    //            __instance.MakeOwl();
    //        }
    //    }
    //}
}