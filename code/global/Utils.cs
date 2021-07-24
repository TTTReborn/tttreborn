using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Globals
{
    public static partial class Utils
    {
        public static List<TTTPlayer> GetPlayers()
        {
            List<TTTPlayer> players = new List<TTTPlayer>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        public static List<TTTPlayer> GetAlivePlayers()
        {
            List<TTTPlayer> players = new List<TTTPlayer>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player && player.LifeState == LifeState.Alive)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        public static List<Client> GetDeadClients()
        {
            List<Client> clients = new List<Client>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player && player.LifeState == LifeState.Dead)
                {
                    clients.Add(client);
                }
            }

            return clients;
        }

        public static List<TTTPlayer> GetConfirmedPlayers()
        {
            List<TTTPlayer> players = new List<TTTPlayer>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player && player.IsConfirmed)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        public static IEnumerable<Client> GetClientsSpectatingPlayer(TTTPlayer player)
        {
            List<Client> clients = new();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer p && p.CurrentPlayer == player)
                {
                    clients.Add(client);
                }
            }

            return clients;
        }

        public static bool HasMinimumPlayers()
        {
            return Client.All.Count >= Gamemode.Game.TTTMinPlayers;
        }

        /// <summary>
        /// Loops through every type derived from the given type and collects non-abstract types.
        /// </summary>
        /// <returns>List of all available types of the given type</returns>
        public static List<Type> GetTypes<T>()
        {
            List<Type> types = new();

            Library.GetAll<T>().ToList().ForEach(t =>
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    types.Add(t);
                }
            });

            return types;
        }

        /// <summary>
        /// Get a derived `Type` of the given type by it's name (`Sandbox.LibraryAttribute`).
        /// </summary>
        /// <param name="name">The name of the `Sandbox.LibraryAttribute`</param>
        /// <returns>Derived `Type` of given type</returns>
        public static Type GetTypeByName<T>(string name)
        {
            foreach (Type type in GetTypes<T>())
            {
                if (GetTypeName(type) == name)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an instance of the given type by the given type `Type`.
        /// </summary>
        /// <param name="type">A derived `Type` of the given type</param>
        /// <returns>Instance of the given type object</returns>
        public static T GetObjectByType<T>(Type type)
        {
            return Library.Create<T>(type);
        }

        /// <summary>
        /// Returns the `Sandbox.LibraryAttribute`'s `Name` of the given `Type`.
        /// </summary>
        /// <param name="type">A `Type` that has a `Sandbox.LibraryAttribute`</param>
        /// <returns>`Sandbox.LibraryAttribute`'s `Name`</returns>
        public static string GetTypeName(Type type)
        {
            return Library.GetAttribute(type).Name;
        }
    }
}
