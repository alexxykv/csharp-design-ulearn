using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        private readonly int id;

        public APIObject(int id)
        {
            this.id = id;
            MagicAPI.Allocate(id);
        }

        private bool disposedValue;

        ~APIObject()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MagicAPI.Free(id);
                }

                disposedValue = true;
            }
        }
    }
}
