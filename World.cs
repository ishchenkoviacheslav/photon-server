﻿
using ExitGames.Threading;
using System.Collections.Generic;
using System.Threading;

namespace TestPhotonLib
{
    public class World
    {
        public static readonly World Inctance = new World();
        public List<UnityClient> Clients { get; private set; }
        private readonly ReaderWriterLockSlim readWriteLock;

        public World()
        {
            Clients = new List<UnityClient>();
            readWriteLock = new ReaderWriterLockSlim();
        }

        public UnityClient TryGetByName(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                return Clients.Find(n=>n.Charactername.Equals(name));
            }
        }

        public bool IsContain(string name)
        {
            using (ReadLock.TryEnter(this.readWriteLock,1000))
            {
                return Clients.Exists((n) => n.Equals(name));
            }
        }

        public void AddClient(UnityClient client)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Add(client);
            }
        }
        public void RemoveClient(UnityClient client)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                Clients.Remove(client);
            }
        }
        ~World()
        {
            readWriteLock.Dispose();
        }
    }
}
