using System;
using System.Collections.Generic;
using Models.Core;
namespace Models.Crop2ML;
/// <summary>
///
/// </summary>
[ValidParent(ParentType = typeof(Zone))]
public class SoilTemperatureAuxiliary : Model
{
    private double _SnowIsolationIndex;

    /// <summary>
    /// Constructor
    /// </summary>
    public SoilTemperatureAuxiliary() { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoilTemperatureAuxiliary(SoilTemperatureAuxiliary toCopy, bool copyAll) // copy constructor
    {
    if (copyAll)
    {

    SnowIsolationIndex = toCopy.SnowIsolationIndex;
    }
    }
    /// <summary>
    ///
    /// </summary>
    public double SnowIsolationIndex
        {
            get { return this._SnowIsolationIndex; }
            set { this._SnowIsolationIndex= value; }
        }
}
