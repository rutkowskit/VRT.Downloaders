namespace VRT.Downloaders.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IDisposable
    {
        private bool _disposedValue;
        public BaseViewModel()
        {
            Disposables = new CompositeDisposable();
        }
        public string Title { get; set; }
        protected CompositeDisposable Disposables { get; }        

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Disposables.Dispose();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
