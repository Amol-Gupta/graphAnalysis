using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphAnalysis
{
    internal class Node: IComparable<Node>, IEquatable<Node>
    {
        public bool visited { get; set; }
        public string color { get; set; }
        public int index { get; set; }

        int IComparable<Node>.CompareTo(Node? other)
        {
            return this.index - other.index;
        }

        bool IEquatable<Node>.Equals(Node? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;
            return other.index == this.index;
        }
        public Node(int index)
        {
            this.index = index;
            this.visited = false;
            this.color=string.Empty;
        }
        public override string ToString()
        {
            return "Node " + this.index.ToString();
        }
    }
}
