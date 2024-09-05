using System;
using System.Collections.Generic;
using Models.Core;
namespace Models.Crop2ML;
/// <summary>
///
/// </summary>
[ValidParent(ParentType = typeof(Zone))]
public class SoilTemperatureState : Model
{
    private double[] _rSoilTempArrayRate;
    private double[] _pSoilLayerDepth;
    private double _pInternalAlbedo;
    private double _SnowWaterContent;
    private double _SoilSurfaceTemperature;
    private int _AgeOfSnow;
    private double[] _SoilTempArray;

    /// <summary>
    /// Constructor
    /// </summary>
    public SoilTemperatureState() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoilTemperatureState(SoilTemperatureState toCopy, bool copyAll) // copy constructor
    {
    if (copyAll)
    {

    rSoilTempArrayRate = new double[toCopy.rSoilTempArrayRate.Length];
            for (int i = 0; i < toCopy.rSoilTempArrayRate.Length; i++)
            { rSoilTempArrayRate[i] = toCopy.rSoilTempArrayRate[i]; }

    pSoilLayerDepth = new double[toCopy.pSoilLayerDepth.Length];
            for (int i = 0; i < toCopy.pSoilLayerDepth.Length; i++)
            { pSoilLayerDepth[i] = toCopy.pSoilLayerDepth[i]; }

    pInternalAlbedo = toCopy.pInternalAlbedo;
    SnowWaterContent = toCopy.SnowWaterContent;
    SoilSurfaceTemperature = toCopy.SoilSurfaceTemperature;
    AgeOfSnow = toCopy.AgeOfSnow;
    SoilTempArray = new double[toCopy.SoilTempArray.Length];
            for (int i = 0; i < toCopy.SoilTempArray.Length; i++)
            { SoilTempArray[i] = toCopy.SoilTempArray[i]; }

    }
    }
    /// <summary>
    ///
    /// </summary>
    public double[] rSoilTempArrayRate
        {
            get { return this._rSoilTempArrayRate; }
            set { this._rSoilTempArrayRate= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double[] pSoilLayerDepth
        {
            get { return this._pSoilLayerDepth; }
            set { this._pSoilLayerDepth= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double pInternalAlbedo
        {
            get { return this._pInternalAlbedo; }
            set { this._pInternalAlbedo= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double SnowWaterContent
        {
            get { return this._SnowWaterContent; }
            set { this._SnowWaterContent= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double SoilSurfaceTemperature
        {
            get { return this._SoilSurfaceTemperature; }
            set { this._SoilSurfaceTemperature= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public int AgeOfSnow
        {
            get { return this._AgeOfSnow; }
            set { this._AgeOfSnow= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double[] SoilTempArray
        {
            get { return this._SoilTempArray; }
            set { this._SoilTempArray= value; }
        }
}
