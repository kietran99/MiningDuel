namespace Functional.Type
{
    public struct Nothing {}

    public class Option<T>
    {
        public static Option<Nothing> nonValue = new Option<Nothing>();
        private T value;
        private bool isPresent;
        
        public static implicit operator Option<T>(T value) => new Option<T>(value);
        //public static implicit operator Option<T> => new Option<T>();

        public Option(T value)
        {
            this.value = value;
            isPresent = true;
        }

        public Option()
        {
            isPresent = false;
        }

        public void Match(System.Action<T> presentHandler, System.Action absentHandler)
        {
            if (isPresent)
            {
                presentHandler(value);
            }
            else
            {
                absentHandler();
            }
        }

        public U Match<U>(System.Func<T, U> presentHandler, System.Func<U> absentHandler)
        {
            return isPresent ? presentHandler(value) : absentHandler();
        }
    }
}
