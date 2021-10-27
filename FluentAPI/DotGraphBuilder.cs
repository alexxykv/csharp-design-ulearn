using System;
using System.Collections.Generic;
using System.Globalization;

namespace FluentApi.Graph
{
	public abstract class GraphBuilder
    {
		private readonly Graph graph;
		protected GraphBuilder(Graph graph)
        {
			this.graph = graph;
        }
		
		public GraphNodeBuilder AddNode(string name)
		{
			var node = graph.AddNode(name);
			return new GraphNodeBuilder(graph, node);
		}

		public GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode)
		{
			var edge = graph.AddEdge(sourceNode, destinationNode);
			return new GraphEdgeBuilder(graph, edge);
		}

		public string Build() => graph.ToDotFormat();
	}

	public class DotGraphBuilder : GraphBuilder
	{
		private DotGraphBuilder(Graph graph) : base(graph) { }

		public static DotGraphBuilder DirectedGraph(string graphName)
		{
			return new DotGraphBuilder(new Graph(graphName, true, false));
		}

		public static DotGraphBuilder UndirectedGraph(string graphName)
		{
			return new DotGraphBuilder(new Graph(graphName, false, false));
		}
	}

	public class GraphNodeBuilder : GraphBuilder
    {
		private readonly GraphNode node;
		public GraphNodeBuilder(Graph graph, GraphNode node) : base(graph)
        {
			this.node = node;
        }

		public GraphBuilder With(Action<NodeAttributes> applyAttributes)
        {
			applyAttributes(new NodeAttributes(node));
			return this;
        }
	}

	public class GraphEdgeBuilder : GraphBuilder
	{
		private readonly GraphEdge edge;
		public GraphEdgeBuilder(Graph graph, GraphEdge edge) : base(graph)
		{
			this.edge = edge;
		}

		public GraphBuilder With(Action<EdgeAttributes> applyAttributes)
		{
			applyAttributes(new EdgeAttributes(edge));
			return this;
		}
	}

	public class CommonAttributes<TAttributes>
		where TAttributes : CommonAttributes<TAttributes>
    {
        private readonly Dictionary<string, string> attributes;
		protected CommonAttributes(Dictionary<string, string> attributes)
        {
			this.attributes = attributes;
        }

		public TAttributes Color(string colorName)
        {
			attributes["color"] = colorName;
			return (TAttributes)this;
        }

		public TAttributes FontSize(int fontSize)
		{
			attributes["fontsize"] = fontSize.ToString();
			return (TAttributes)this;
		}

		public TAttributes Label(string label)
		{
			attributes["label"] = label;
			return (TAttributes)this;
		}
	}

	public class NodeAttributes : CommonAttributes<NodeAttributes>
    {
		private readonly GraphNode node;
		public NodeAttributes(GraphNode node) : base(node.Attributes)
        {
			this.node = node;
        }

		public NodeAttributes Shape(NodeShape shape)
		{
			node.Attributes["shape"] = shape.ToString().ToLower();
			return this;
		}
	}

	public class EdgeAttributes : CommonAttributes<EdgeAttributes>
    {
		private readonly GraphEdge edge;
		public EdgeAttributes(GraphEdge edge) : base(edge.Attributes)
		{
			this.edge = edge;
		}

		public EdgeAttributes Weight(double weight)
		{
			edge.Attributes["weight"] = weight.ToString(CultureInfo.InvariantCulture);
			return this;
		}
	}

	public enum NodeShape
	{
		Ellipse,
		Box
	}
}
