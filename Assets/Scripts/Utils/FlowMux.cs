using System.Collections.Generic;

namespace Utils
{
    public class FlowMux<T>
    {
        private List<FlowShape<T>> flowShapes = new List<FlowShape<T>>();

        public FlowMux() {}

        public FlowMux(FlowShape<T> flowShape)
        {
            flowShapes.Add(flowShape);
        }

        public FlowMux(IEnumerable<FlowShape<T>> flowShapes)
        {
            this.flowShapes.AddRange(flowShapes);
        }

        public void AddShape(FlowShape<T> flowShape)
        {
            flowShapes.Add(flowShape);
        }

        public void Resolve(T shape)
        {
            var (executor, idx) = flowShapes.LookUp(transform => transform.Shape(shape));
            
            if (!idx.Equals(Constants.INVALID))           
            {
                executor.Handler(shape);
            }
        }
    }   
}
