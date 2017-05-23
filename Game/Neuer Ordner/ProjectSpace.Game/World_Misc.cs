using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Stuff that doesn't fit anywhere
    /// </summary>
    public partial class World
    {
        public static World CreateTest()
        {
            var world = new OutpostOmega.Game.World(); //Generate World

            //dWorld.World.Debug = true;
            OutpostOmega.Game.turf.Structure.AddFlat(world, 30, 1); //Generate test Structure

            //World.Debug = true;

            /*var suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(2, 2, 0));
            suzanne.Register();

            suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(2, 2, -5));
            suzanne.Model = @"Content\Model\Plane.dae";
            suzanne.Register();*/

            var wrench = new OutpostOmega.Game.GameObjects.Items.Tools.Wrench(world);
            wrench.SetPosition(new Jitter.LinearMath.JVector(4, 1.1f, -2));
            //suzanne.Model = @"Content\Model\Plane.dae";
            wrench.Register();

            //var knife = new OutpostOmega.Game.GameObjects.Items.Tools.Knife(World);
            //knife.SetPosition(new Jitter.LinearMath.JVector(4, 1.1f, -1.5f));

            var Spawner = new OutpostOmega.Game.GameObjects.Items.Devices.Spawner(world);
            Spawner.SetPosition(new Jitter.LinearMath.JVector(5, 2.1f, -1.5f));
            //suzanne.Model = @"Content\Model\Plane.dae";
            Spawner.Register();


            var galaxmachine = new OutpostOmega.Game.GameObjects.Structures.Machines.Vendingmachine(-3, 1, -3, world.Structures[0], world);
            //galaxmachine.PhysicSetPosition(new Jitter.LinearMath.JVector(-2.5f, 1, -4.65f));
            galaxmachine.Register();
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 3, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 3, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 3, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 3, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 2, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 2, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 2, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 2, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 1, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 1, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 1, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 1, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 3, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(0, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 1, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 2, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 3, -4));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-6, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-5, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-4, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-3, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-2, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-1, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-0, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-0, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-0, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-0, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(-0, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 4, -2));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 4, -1));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(1, 4, 0));

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 4, -4));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 4, -3));
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(2, 4, -2));

            new OutpostOmega.Game.GameObjects.Structures.Frame(3, 4, -4, world.Structures[0], world).Register();
            new OutpostOmega.Game.GameObjects.Structures.Frame(3, 4, -3, world.Structures[0], world).Register();

            new OutpostOmega.Game.GameObjects.Structures.Frame(4, 4, -4, world.Structures[0], world).Register();


            new OutpostOmega.Game.GameObjects.Structures.Frame(3, 1, -4, world.Structures[0], world).Register();
            new OutpostOmega.Game.GameObjects.Structures.Window(3, 2, -4, world.Structures[0], world).Register();
            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(3, 3, -4));

            new OutpostOmega.Game.GameObjects.Structures.Frame(4, 1, -4, world.Structures[0], world).Register();
            new OutpostOmega.Game.GameObjects.Structures.Window(4, 2, -4, world.Structures[0], world).Register();
            new OutpostOmega.Game.GameObjects.Structures.Frame(4, 3, -4, world.Structures[0], world).Register();

            world.Structures[0].Add(OutpostOmega.Game.turf.types.turfTypeE.floor, new Jitter.LinearMath.JVector(5, 1, -4));
            new OutpostOmega.Game.GameObjects.Structures.Frame(5, 2, -4, world.Structures[0], world).Register();

            new OutpostOmega.Game.GameObjects.Structures.Frame(6, 1, -4, world.Structures[0], world).Register();


            var airlock = new OutpostOmega.Game.GameObjects.Structures.Machines.Doors.Airlock(0, 1, -4, world.Structures[0], world);
            //airlock.SetPosition(new Jitter.LinearMath.JVector(0.0f, 1, -4.5f));
            airlock.Register();



            /*suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 4, 5));
            suzanne.Register();

            suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 5, 5));
            suzanne.Register();

            suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(-5, 3, -5));
            suzanne.Register();

            suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(-5, 4, -5));
            suzanne.Register();

            suzanne = new OutpostOmega.Game.gObject.structure.Suzanne(World);
            suzanne.PhysicSetPosition(new Jitter.LinearMath.JVector(-5, 5, -5));
            suzanne.Register();*/

            //tmpScene.Initialize();


            return world;
        }

        public GameObjects.Mobs.Mind MakePlayer()
        {
            var mind = new OutpostOmega.Game.GameObjects.Mobs.Mind(this);
            mind.Username = "P1";

            var human = new OutpostOmega.Game.GameObjects.Mobs.CarbonBased.Human(this);
            human.SetPosition(new Jitter.LinearMath.JVector(0, 1, 0));

            mind.Mob = human; //Assign mind to body

            human.Register();

            human.View.MouseOrientation = Jitter.LinearMath.JVector2.Backward;

            mind.Register();

            this.Player = mind; //this mind is the player

            return mind;
        }
    }
}
