﻿using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace FelixsCallouts.Callouts
{
    [CalloutInfo("GunShotsReported", CalloutProbability.High)]

    class GunShotsReported : Callout
    {

        private Ped Suspect;
        private Blip SuspectBlip;
        private Vector3 Spawnpoint;

        public override bool OnBeforeCalloutDisplayed()
        {

            //Spawnpoint = World.GetRandomPositionOnStreet();
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "GunShots reported";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 WE_HAVE CRIME_GUNFIRE_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", Spawnpoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(Spawnpoint);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", 50, true);
            Suspect.Tasks.Wander();

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            if (Game.LocalPlayer.Character.DistanceTo(Suspect) <= 30f)
            {
                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
            }

            if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
            {
                End();
            }

        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }
            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }

            Game.LogTrivial("FelixsCallouts | GunShots Reported Ended");

        }

    }
}
