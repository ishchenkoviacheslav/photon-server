﻿using ExitGames.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestPhotonLib
{
    public class Chat
    {
        public static readonly Chat Instance = new Chat();
        private List<String> Messages { get; set; }
        private readonly ReaderWriterLockSlim readWriteLock;
        private const int MAX_RECENT_MESSAGES = 10;
        private const int MAX_BUFFERED_MESSAGES = 1000;
        public Chat()
        {
            Messages = new List<string>();
            readWriteLock = new ReaderWriterLockSlim();
        }
        ~Chat()
        {
            readWriteLock.Dispose();
        }
        public List<string> GetResentMesseges()
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                List<string> list = new List<string>(MAX_RECENT_MESSAGES);
                if (Messages.Count == 0)
                    return list;
                if (Messages.Count <= MAX_RECENT_MESSAGES)
                {
                    list.AddRange(Messages);
                    return list;
                }
                return Messages.Skip(Messages.Count - MAX_RECENT_MESSAGES).ToList();
            }
        }
        public void AddMessage(string message)
        {
            using (ReadLock.TryEnter(this.readWriteLock, 1000))
            {
                Messages.Add(message);
                if (Messages.Count > MAX_BUFFERED_MESSAGES)
                    Messages = Messages.Skip(Messages.Count - MAX_RECENT_MESSAGES).ToList();
            }
        }
    }
}
