// 
//     Take Command!
// 
//     Copyright (C) 2015 Sean McDougall
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TakeCommand
{

    public class TakeCommand : ModuleCommand
    {
        // Keep track of all the command seats that need to be emptied (shared across all instances)
        public static List<Part> allCommandSeats = new List<Part>();

        // Variables to store the escape hatch and collider
        GameObject escapeHatch = null;
        BoxCollider escapeHatchCollider = null;

        // Name of the Kerbal who belongs in this seat
        private string myKerbal;

        // Whether or not the Kerbal has been ejected and should now be boarded
        private bool boardKerbal = false;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {

                if (escapeHatch == null)
                {
                    escapeHatch = new GameObject("EscapeHatch");
                    escapeHatch.tag = "Airlock";
                    escapeHatch.layer = 21;
                    escapeHatch.transform.parent = this.part.transform;
                    escapeHatch.transform.localEulerAngles = new Vector3(0, 0, 0);
                    escapeHatch.transform.localPosition = new Vector3(0, 0, 0);

                    escapeHatch.AddComponent<BoxCollider>();
                    escapeHatchCollider = escapeHatch.GetComponent<BoxCollider>();
                    escapeHatchCollider.size = new Vector3(0.25f, 0.25f, 0.25f);
                    escapeHatchCollider.isTrigger = true;
                    
                    this.part.airlock = escapeHatch.transform;
                    print("[TakeCommand] added escape hatch to " + this.part.name + " (" + this.part.GetInstanceID() + ")");

                    // Disable it for now until we need it
                    escapeHatch.collider.enabled = true;
                }

            }
            base.OnStart(state);
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && vessel.HoldPhysics == false)
            {
                // Make sure controls are unlocked (workaround for compatibility issue with Kerbal Joint Reinforcement)
                if (InputLockManager.GetControlLock("KJRLoadLock") != ControlTypes.ALL_SHIP_CONTROLS)
                {
                    if (this.part.protoModuleCrew.Count > 0 && allCommandSeats.Count == 0)
                    {
                        print("[TakeCommand] populating seat list");
                        foreach (Part p in vessel.parts)
                        {
                            if (p.Modules.OfType<TakeCommand>().Any())
                            {
                                if (p.protoModuleCrew.Count > 0)
                                {
                                    allCommandSeats.Add(p);
                                }
                            }
                        }
                        print("[TakeCommand] found " + allCommandSeats.Count + " occupied seats");
                    }
                    if (boardKerbal == false)
                    {
                        if (this.part.protoModuleCrew.Count > 0 && allCommandSeats.First().GetInstanceID() == this.part.GetInstanceID())
                        {
                            // Time to eject this crew member
                            ProtoCrewMember kerbal = this.part.protoModuleCrew.Single();
                            print("[TakeCommand] ejecting " + kerbal.name + " from " + this.part.GetInstanceID());
                            escapeHatch.collider.enabled = true;
                            if (FlightEVA.fetch.spawnEVA(kerbal, this.part, escapeHatch.transform))
                            {
                                myKerbal = "kerbalEVA (" + kerbal.name + ")";
                                boardKerbal = true;
                                escapeHatch.collider.enabled = false;
                            }
                            else
                            {
                                print("[TakeCommand] error ejecting " + kerbal.name);
                            }
                        }
                    }
                    else
                    {
                        // Check and wait until the ejected Kerbal is the active vessel before proceeding
                        if (FlightGlobals.ActiveVessel.name == myKerbal)
                        {
                            KerbalEVA kerbal = FlightGlobals.ActiveVessel.GetComponent<KerbalEVA>();

                            if (kerbal.fsm.Started == true)
                            {
                                allCommandSeats.Remove(allCommandSeats.First());
                                boardKerbal = false;

                                print("[TakeCommand]  seating " + kerbal.name + " in " + this.part.GetInstanceID());
                                this.part.Modules.OfType<KerbalSeat>().Single().BoardSeat();
                            }
                        }
                    }
                }
            }
            base.OnUpdate();
        }

    }

}