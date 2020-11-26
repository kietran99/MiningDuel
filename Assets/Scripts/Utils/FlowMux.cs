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

        public FlowMux(FlowShape<T>[] flowShapes)
        {
            this.flowShapes.AddRange(flowShapes);
        }

        public void AddShape(FlowShape<T> flowShape)
        {
            flowShapes.Add(flowShape);
        }

        public void Resolve(T shape)
        {
            var executor = flowShapes.LookUp(transform => transform.CanApply(shape)).item;
            executor.Transform(shape);
        }
    }   
}
