using System;

namespace Cycler
{
    public interface ICycler<T>
    {
        T Current { get; }
        
        Action<T> OnCycle { get; set; }

        void LoadElements(T[] elements);
        void GoNext();
        void GoPrev();
    }
}