namespace Functional.Type
{
    public class Either<TError, TSuccess> where TError : IError
    {
        private readonly TError error;
        private readonly TSuccess success;
        private bool isError;

        public static implicit operator Either<TError, TSuccess>(TError left) => new Either<TError, TSuccess>(left);
        public static implicit operator Either<TError, TSuccess>(TSuccess right) => new Either<TError, TSuccess>(right);

        public Either(TError error)
        {
            this.error = error;
            isError = true;
        }

        public Either(TSuccess success)
        {
            this.success = success;
            isError = false;
        }

        public void Match(System.Action<TError> errorHandler, System.Action<TSuccess> successHandler)
        {
            if (isError)
            {
                errorHandler(error);
            }
            else
            {
                successHandler(success);
            }
        }

        public T Match<T>(System.Func<TError, T> errorHandler, System.Func<TSuccess, T> successHandler)
        {
            return isError ? errorHandler(error) : successHandler(success);
        }

        public Either<TError, TSuccessOut> Map<TSuccessOut>(System.Func<TSuccess, TSuccessOut> fn) 
        {
            return Match<Either<TError, TSuccessOut>>(
                err => err,
                success => fn(success)
            );
        }
    }
}
