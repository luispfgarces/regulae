namespace Regulae
{
    using System;

    internal sealed class ObjectContentContainer : IContentContainer
    {
        private readonly object content;

        public ObjectContentContainer(object content)
        {
            this.content = content;
        }

        public TContent GetContentAs<TContent>()
        {
            try
            {
                return (TContent)this.content;
            }
            catch (InvalidCastException ice)
            {
                throw new ContentTypeException($"Cannot cast content to provided type as {nameof(TContent)}: {typeof(TContent).FullName}", ice);
            }
        }
    }
}