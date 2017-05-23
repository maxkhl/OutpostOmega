using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OONetwork = OutpostOmega.Network;
using OOServer = OutpostOmega.Server;
using System.Threading;

namespace UnitTest
{
    [TestClass]
    public class NetworkUnitTest
    {
        /* Not working because server will bind the socket and its kind of hard to figure out
         * where to shut the server and where not. So for now I disable this test
         * but it might be handy in the future.
         */

        OOServer.Network.Host Host;
        DataTest dTest;
        [TestMethod]
        [TestCategory("Network&Serialization")]
        public void ServerSetupTest()
        {
            dTest = new DataTest();
            dTest.SimulateTest();
            this.Host = new OOServer.Network.Host("UnitTestServer", dTest.newWorld);
            this.Host.Start();
            this.Host.Shutdown();
        }

        OONetwork.nClient Client;
        [TestMethod]
        [TestCategory("Network&Serialization")]
        public void ClientSetupTest()
        {
            if (Host == null)
                ServerSetupTest();

            Client = new OONetwork.nClient("UnitTestClient");
        }

        [TestMethod]
        [TestCategory("Network&Serialization")]
        public void ClientConnectTest()
        {
            if (Client == null)
                ClientSetupTest();


            this.Host.Start();
            Client.Connect("localhost", Host.Port);
            Client.NewWorldReceived += Client_NewWorldReceived;

            DateTime TimeOut = DateTime.Now;
            while (!Client.Online)
            {
                if ((DateTime.Now - TimeOut).TotalMilliseconds > 10000)
                    throw new Exception("Timout 10s - no connection established");

                Thread.Sleep(1);
            }
        }

        private OutpostOmega.Game.World ReceivedWorld;
        void Client_NewWorldReceived(OutpostOmega.Game.World oldWorld, OutpostOmega.Game.World newWorld)
        {
            ReceivedWorld = newWorld;
        }

        [TestMethod]
        [TestCategory("Network&Serialization DEBUG!")]
        public void ServerClientExchangeTest()
        {
            if (Client == null)
                ClientConnectTest();

            if (!Client.Online)
                throw new Exception("Client not connected");

            // Triggers datasending on the server
            foreach (var hClient in Host.Clients)
                if (hClient.Scope.NeedsUpdate)
                    hClient.Scope.Update();

            // Wait to receive the world
            DateTime TimeOut = DateTime.Now;
            while (ReceivedWorld == null)
            {
                if ((DateTime.Now - TimeOut).TotalMilliseconds > 10000)
                    throw new Exception("Timout 10s - no world received" + Client.Output.ToString());

                Thread.Sleep(1);
            }
        }

        [TestMethod]
        [TestCategory("Network&Serialization DEBUG!")]
        public void ServerClientSimulationTest()
        {
            if (ReceivedWorld == null)
                ServerClientExchangeTest();

            for (int i = 0; i < 10; i++)
            {
                // Update the hosts world
                Host.World.Update(
                    new OutpostOmega.Game.Tools.KeybeardState(),
                    new OutpostOmega.Game.Tools.MouseState(),
                    0.1f);

                // Update the clients scope to trigger sending data
                foreach (var hClient in Host.Clients)
                    if (hClient.Scope.NeedsUpdate)
                        hClient.Scope.Update();

                Client.World.Update(
                    new OutpostOmega.Game.Tools.KeybeardState(),
                    new OutpostOmega.Game.Tools.MouseState(),
                    0.1f);

                Thread.Sleep(1000);
            }
            Thread.Sleep(10000);
        }

        [TestMethod]
        [TestCategory("Network&Serialization DEBUG!")]
        public void ServerClientConsistencyTest()
        {
            if (ReceivedWorld == null)
                ServerClientSimulationTest();

            dTest.CompareWorlds(Host.World, ReceivedWorld);
        }
    }
}
