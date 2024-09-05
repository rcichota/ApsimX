using System;
using System.Collections.Generic;
namespace Models.Crop2ML;
/// <summary>
///
/// </summary>
public class SoilTemperatureRate
{
    private double _rSnowWaterContentRate;
    private double _rSoilSurfaceTemperatureRate;
    private int _rAgeOfSnowRate;

    /// <summary>
    /// </summary>
    public SoilTemperatureRate() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoilTemperatureRate(SoilTemperatureRate toCopy, bool copyAll) // copy constructor
    {
    if (copyAll)
    {

    rSnowWaterContentRate = toCopy.rSnowWaterContentRate;
    rSoilSurfaceTemperatureRate = toCopy.rSoilSurfaceTemperatureRate;
    rAgeOfSnowRate = toCopy.rAgeOfSnowRate;
    }
    }
    /// <summary>
    ///
    /// </summary>
    public double rSnowWaterContentRate
        {
            get { return this._rSnowWaterContentRate; }
            set { this._rSnowWaterContentRate= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double rSoilSurfaceTemperatureRate
        {
            get { return this._rSoilSurfaceTemperatureRate; }
            set { this._rSoilSurfaceTemperatureRate= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public int rAgeOfSnowRate
        {
            get { return this._rAgeOfSnowRate; }
            set { this._rAgeOfSnowRate= value; }
        }
}
