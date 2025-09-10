namespace Regulae.Rql.Messages
{
    using System;
    using System.Collections.Generic;
    using Regulae.Rql;

    internal class MessageContainer : IMessageContainer
    {
        private bool disposedValue;
        private List<Message> messages;

        public MessageContainer()
        {
            this.messages = new List<Message>();
            this.ErrorsCount = 0;
            this.WarningsCount = 0;
        }

        public int ErrorsCount { get; private set; }
        public IReadOnlyList<Message> Messages => this.messages.AsReadOnly();
        public int WarningsCount { get; private set; }

        public void Dispose()
        {
            this.Dispose(disposing: true);
        }

        public void Error(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.AddMessage(message, MessageSeverity.Error, beginPosition, endPosition);
            this.ErrorsCount++;
        }

        public void Warning(string message, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.AddMessage(message, MessageSeverity.Warning, beginPosition, endPosition);
            this.WarningsCount++;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.messages = null;
                }

                this.disposedValue = true;
            }
        }

        private void AddMessage(string message, MessageSeverity severity, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messages.Add(Message.Create(message, beginPosition, endPosition, severity));
        }
    }
}