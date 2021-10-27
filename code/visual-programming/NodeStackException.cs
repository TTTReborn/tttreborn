using System;

namespace TTTReborn.VisualProgramming
{
    public class NodeStackException : Exception
    {
        public NodeStackException() : base() { }
        public NodeStackException(string message) : base(message) { }
        public NodeStackException(string message, Exception inner) : base(message, inner) { }
    }
}
