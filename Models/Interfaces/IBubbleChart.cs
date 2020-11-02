namespace Models.Interfaces
{
    using System;
    using Management;
    using System.Collections.Generic;
    using Core;

    /// <summary>
    /// An interface for a bubble chart.
    /// </summary>
    public interface IBubbleChart : IModel
    {
        /// <summary>
        /// The nodes of the graph.
        /// </summary>
        List<StateNode> Nodes { get; set; }

        /// <summary>
        /// The arcs of the graph.
        /// </summary>
        List<RuleAction> Arcs { get; set; }

        /// <summary>
        /// fixme - can this be refactored out?
        /// </summary>
        string InitialState { get; set; }

        /// <summary>
        /// fixme - can this be refactored out?
        /// </summary>
        bool Verbose { get; set; }
    }
}