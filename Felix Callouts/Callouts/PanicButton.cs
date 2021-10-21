using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using System.Threading;

namespace Felix_Callouts.Callouts
{
    [CalloutInfo("PanicButton", CalloutProbability.High)]
    public class PanicButton : Callout
    {

        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private Vector3 Spawnpoint;
        private float Heading;

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Panic Button Triggered";
            CalloutPosition = Spawnpoint;
            Heading = 66.64632f;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_04 IN_OR_ON_POSITION", Spawnpoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle("POLICE", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped("s_f_y_cop_01", SuspectVehicle.GetOffsetPositionFront(5f), Heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Blue;
            SuspectBlip.IsRouteEnabled = true;

            Game.DisplayHelp("This Callout will not end it self unless you revive the ped or call the coroner", false);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
             
            if (Suspect.IsAlive)
            {
                //SuspectVehicle.IsSirenOn = true;
                Suspect.Health = 0;
            }

            if (!Suspect.Exists())
            {
                End();
            }
;
        }

        public override void End()
        {
            base.End();

            Game.DisplayNotification("Code 4");
            Functions.PlayScannerAudio("SC_CODE4");

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }
            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }
            if (SuspectVehicle.Exists())
            {
                SuspectVehicle.Dismiss();
            }

            Game.LogTrivial("Felix'sCallouts | Panic Button Ended");

        }

    }
}
