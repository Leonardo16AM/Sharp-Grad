using System.Collections.Generic;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class ValueBase<TType>(TType data, string name, params ValueBase<TType>[] childs)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public TType Grad = TType.Zero;
        public TType Data = data;
        public readonly ValueBase<TType>[] Childrens = childs;
        public readonly string Name = name;
        protected virtual void Backward() { }
        public void Backpropagate()
        {
            Grad = TType.One;
            List<ValueBase<TType>> TopOSort = [];
            HashSet<ValueBase<TType>> Visited = [];
            DFS(TopOSort, Visited);
            for (int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward();
            }
        }

        void DFS(List<ValueBase<TType>> TopOSort, HashSet<ValueBase<TType>> Visited)
        {
            Visited.Add(this);
            foreach (var child in Childrens)
            {
                if (!Visited.Contains(child))
                    child.DFS(TopOSort, Visited);
            }
            TopOSort.Add(this);
        }
    }
}