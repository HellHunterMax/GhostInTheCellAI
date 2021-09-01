using System;
using System.Runtime.Serialization;

namespace GhostInTheCellAI.Models
{
    [Serializable]
    internal class ImpossibleActionException : Exception
    {
        public ImpossibleActionException()
        {
        }

        public ImpossibleActionException(string message) : base(message)
        {
        }

        public ImpossibleActionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImpossibleActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}