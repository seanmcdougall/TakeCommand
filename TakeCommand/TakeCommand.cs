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

        // Whether processing is complete and the module can disable itself
        private bool tcComplete = false;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (this.part.protoModuleCrew.Count > 0)
                {
                    if (allCommandSeats.Count == 0)
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
                    }
                }
                else
                {
                    // No crew left to eject, disable the module
                    tcComplete = true;
                    
                }
            }
            base.OnStart(state);
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && vessel.Landed && vessel.HoldPhysics == false)
            {
                // Make sure controls are unlocked (workaround for compatibility issue with Kerbal Joint Reinforcement)
                if (InputLockManager.GetControlLock("KJRLoadLock") != ControlTypes.ALL_SHIP_CONTROLS)
                {
                    if (boardKerbal == false)
                    {
                        if (this.part.protoModuleCrew.Count > 0 && allCommandSeats.First().GetInstanceID() == this.part.GetInstanceID())
                        {
                            // Time to eject this crew member
                            ProtoCrewMember kerbal = this.part.protoModuleCrew.Single();
                            print("[TakeCommand] ejecting " + kerbal.name + " from " + this.part.GetInstanceID());
                            if (FlightEVA.fetch.spawnEVA(kerbal, this.part, escapeHatch.transform))
                            {
                                myKerbal = "kerbalEVA (" + kerbal.name + ")";
                                boardKerbal = true;
                                escapeHatch.collider.enabled = false;
                                this.part.airlock = null;
                                DestroyObject(escapeHatchCollider);
                                DestroyObject(escapeHatch);
                                print("[TakeCommand] removed escape hatch from " + this.part.name + " (" + this.part.GetInstanceID() + ")");
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
                                tcComplete = true;

                                print("[TakeCommand]  seating " + kerbal.name + " in " + this.part.GetInstanceID());
                                this.part.Modules.OfType<KerbalSeat>().Single().BoardSeat();
                            }
                        }
                    }
                }
            }
            base.OnUpdate();
        }

        public void LateUpdate()
        {
            // Disable this module after all other processing is complete
            if (tcComplete)
            {
                print("[TakeCommand] deactivating module for " + this.part.GetInstanceID());
                PartModule m = this.part.Modules.OfType<TakeCommand>().Single();
                m.enabled = false;
                m.isEnabled = false;
            }
        }

    }

}