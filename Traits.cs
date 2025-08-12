using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Obeliskial_Content;
using UnityEngine;
using static Caldris.CustomFunctions;
using static Caldris.Plugin;
using static Caldris.DescriptionFunctions;
using static Caldris.CharacterFunctions;
using System.Text;
using TMPro;
using Obeliskial_Essentials;
using System.Data.Common;

namespace Caldris
{
    [HarmonyPatch]
    internal class Traits
    {
        // list of your trait IDs

        public static string[] simpleTraitList = ["trait0", "trait1a", "trait1b", "trait2a", "trait2b", "trait3a", "trait3b", "trait4a", "trait4b"];

        public static string[] myTraitList = simpleTraitList.Select(trait => subclassname.ToLower() + trait).ToArray(); // Needs testing

        public static string trait0 = myTraitList[0];
        // static string trait1b = myTraitList[1];
        public static string trait2a = myTraitList[3];
        public static string trait2b = myTraitList[4];
        public static string trait4a = myTraitList[7];
        public static string trait4b = myTraitList[8];

        // public static int infiniteProctection = 0;
        // public static int bleedInfiniteProtection = 0;
        public static bool isDamagePreviewActive = false;

        public static bool isCalculateDamageActive = false;
        public static int infiniteProctection = 0;

        public static string debugBase = "Binbin - Testing " + heroName + " ";


        public static void DoCustomTrait(string _trait, ref Trait __instance)
        {
            // get info you may need
            Enums.EventActivation _theEvent = Traverse.Create(__instance).Field("theEvent").GetValue<Enums.EventActivation>();
            Character _character = Traverse.Create(__instance).Field("character").GetValue<Character>();
            Character _target = Traverse.Create(__instance).Field("target").GetValue<Character>();
            int _auxInt = Traverse.Create(__instance).Field("auxInt").GetValue<int>();
            string _auxString = Traverse.Create(__instance).Field("auxString").GetValue<string>();
            CardData _castedCard = Traverse.Create(__instance).Field("castedCard").GetValue<CardData>();
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            TraitData traitData = Globals.Instance.GetTraitData(_trait);
            List<CardData> cardDataList = [];
            List<string> heroHand = MatchManager.Instance.GetHeroHand(_character.HeroIndex);
            Hero[] teamHero = MatchManager.Instance.GetTeamHero();
            NPC[] teamNpc = MatchManager.Instance.GetTeamNPC();

            if (!IsLivingHero(_character))
            {
                return;
            }
            string traitName = traitData.TraitName;
            string traitId = _trait;


            if (_trait == trait0)
            {
                // if an enemy has Burn, apply Chill 2 to them. If an enemy has Chill, apply Burn 2 to them. 
                LogDebug($"Handling Trait {traitId}: {traitName}");
                for (int i = 0; i < teamNpc.Length; i++)
                {
                    NPC npc = teamNpc[i];
                    if (!IsLivingNPC(npc)) continue;
                    if (npc.HasEffect("burn"))
                    {
                        npc.SetAuraTrait(_character, "chill", 2);
                    }
                    if (npc.HasEffect("chill"))
                    {
                        npc.SetAuraTrait(_character, "burn", 2);
                    }

                }
            }


            else if (_trait == trait2a)
            {
                // trait2a
                // Whenever you apply Chill, deal 2 Fire damage to the target.
                if (_auxString == "chill")
                {
                    LogDebug($"Handling Trait {traitId}: {traitName}");
                    if (IsLivingNPC(_target))
                    {
                        int damageAmount = _character?.DamageWithCharacterBonus(2, Enums.DamageType.Fire, Enums.CardClass.None) ?? 2;
                        _target.IndirectDamage(Enums.DamageType.Fire, damageAmount);
                    }
                }

            }



            else if (_trait == trait2b)
            {
                // trait2b:
                // Stealth on heroes increases All Damage by an additional 15% per charge and All Resistances by an additional 5% per charge.",
                if (_auxString == "burn")
                {
                    LogDebug($"Handling Trait {traitId}: {traitName}");
                    if (IsLivingNPC(_target))
                    {
                        int damageAmount = _character?.DamageWithCharacterBonus(2, Enums.DamageType.Cold, Enums.CardClass.None) ?? 2;
                        _target.IndirectDamage(Enums.DamageType.Cold, damageAmount);
                    }
                }

            }

            else if (_trait == trait4a)
            {
                // trait 4a;

                LogDebug($"Handling Trait {traitId}: {traitName}");
            }

            else if (_trait == trait4b)
            {
                // trait 4b:
                LogDebug($"Handling Trait {traitId}: {traitName}");
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Trait), "DoTrait")]
        public static bool DoTrait(Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardData _castedCard, ref Trait __instance)
        {
            if ((UnityEngine.Object)MatchManager.Instance == (UnityEngine.Object)null)
                return false;
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            if (Content.medsCustomTraitsSource.Contains(_trait) && myTraitList.Contains(_trait))
            {
                DoCustomTrait(_trait, ref __instance);
                return false;
            }
            return true;
        }




    }
}

