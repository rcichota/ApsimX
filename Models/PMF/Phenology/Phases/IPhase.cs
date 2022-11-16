
namespace Models.PMF.Phen
{
    using Models.Core;

    /// <summary>Interface for a function</summary>
    public interface IPhase : IModel
    {
        /// <summary>The start</summary>
        string Start { get; }

        /// <summary>The end</summary>
        string End { get; }

        /// <summary>This function returns a non-zero value if the phase target is met today </summary>
        bool DoTimeStep(ref double PropOfDayToUse);

        /// <summary> Fraction of progress through the phase</summary>
        double FractionComplete { get;}

        /// <summary>Resets the phase.</summary>
        void ResetPhase();

    }
}