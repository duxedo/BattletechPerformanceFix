﻿using Harmony;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using static BattletechPerformanceFix.Control;

namespace BattletechPerformanceFix
{
    public class NoSalvageSoftlock : Feature
    {
        public void Activate()
        {
            var hap = Control.CheckPatch(AccessTools.Method(typeof(AAR_SalvageChosen), nameof(AAR_SalvageChosen.HasAllPriority))
                                        , "80d43f27b8537a10099fd1ebceb4b6961549f30518c00de53fcf38c27623f7ec");
            Control.harmony.Patch(hap
                                 , new HarmonyMethod(typeof(NoSalvageSoftlock), nameof(NoSalvageSoftlock.HasAllPriority)), null);
        }

        public static bool HasAllPriority(AAR_SalvageChosen __instance, Contract ___contract, AAR_SalvageScreen ___parent, ref bool __result)
        {
            try
            {
                int negotiated = ___contract.FinalPrioritySalvageCount;
                int totalSalvageMadeAvailable = ___parent.TotalSalvageMadeAvailable;
                int count = __instance.PriorityInventory.Count;
                int num = negotiated;
                if (num > totalSalvageMadeAvailable)
                {
                    num = totalSalvageMadeAvailable;
                }
                if (num > 7)
                {
                    num = 7;
                }
                Control.LogDebug("HasAllPriority :negotiated {0} :available {1} :selected {2} :clamped {3}", negotiated, totalSalvageMadeAvailable, count, num);
                __result = count >= num;
                return false;
            } catch (Exception e)
            {
                Control.LogException(e);
                return true;
            }
        }
    }
}