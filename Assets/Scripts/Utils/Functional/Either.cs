namespace Functional.Type
{
    public class Either<TL, TR>
    {
        private readonly TL left;
        private readonly TR right;
        private bool isLeft;

        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);
        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);

        public Either(TL left)
        {
            this.left = left;
            isLeft = true;
        }

        public Either(TR right)
        {
            this.right = right;
            isLeft = false;
        }

        public void Match(System.Action<TL> leftHandler, System.Action<TR> rightHandler)
        {
            if (isLeft)
            {
                leftHandler(left);
            }
            else
            {
                rightHandler(right);
            }
        }

        public T Match<T>(System.Func<TL, T> leftHandler, System.Func<TR, T> rightHandler)
        {
            return isLeft ? leftHandler(left) : rightHandler(right);
        }
    }
}
