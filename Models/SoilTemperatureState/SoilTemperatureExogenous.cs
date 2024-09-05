using System;
using System.Collections.Generic;
namespace Models.Crop2ML;
/// <summary>
///
/// </summary>
public class SoilTemperatureExogenous
{
    private double _iAirTemperatureMax;
    private double _iTempMax;
    private double _iAirTemperatureMin;
    private double _iTempMin;
    private double _iGlobalSolarRadiation;
    private double _iRadiation;
    private double _iRAIN;
    private double _iCropResidues;
    private double _iPotentialSoilEvaporation;
    private double _iLeafAreaIndex;
    private double[] _SoilTempArray;
    private double[] _iSoilTempArray;
    private double _iSoilWaterContent;
    private double _iSoilSurfaceTemperature;

    /// <summary>
    ///
    /// </summary>
    public SoilTemperatureExogenous() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoilTemperatureExogenous(SoilTemperatureExogenous toCopy, bool copyAll) // copy constructor
    {
    if (copyAll)
    {

    iAirTemperatureMax = toCopy.iAirTemperatureMax;
    iTempMax = toCopy.iTempMax;
    iAirTemperatureMin = toCopy.iAirTemperatureMin;
    iTempMin = toCopy.iTempMin;
    iGlobalSolarRadiation = toCopy.iGlobalSolarRadiation;
    iRadiation = toCopy.iRadiation;
    iRAIN = toCopy.iRAIN;
    iCropResidues = toCopy.iCropResidues;
    iPotentialSoilEvaporation = toCopy.iPotentialSoilEvaporation;
    iLeafAreaIndex = toCopy.iLeafAreaIndex;
    SoilTempArray = new double[toCopy.SoilTempArray.Length];
            for (int i = 0; i < toCopy.SoilTempArray.Length; i++)
            { SoilTempArray[i] = toCopy.SoilTempArray[i]; }

    iSoilTempArray = new double[toCopy.iSoilTempArray.Length];
            for (int i = 0; i < toCopy.iSoilTempArray.Length; i++)
            { iSoilTempArray[i] = toCopy.iSoilTempArray[i]; }

    iSoilWaterContent = toCopy.iSoilWaterContent;
    iSoilSurfaceTemperature = toCopy.iSoilSurfaceTemperature;
    }
    }
    /// <summary>
    ///
    /// </summary>
    public double iAirTemperatureMax
        {
            get { return this._iAirTemperatureMax; }
            set { this._iAirTemperatureMax= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iTempMax
        {
            get { return this._iTempMax; }
            set { this._iTempMax= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iAirTemperatureMin
        {
            get { return this._iAirTemperatureMin; }
            set { this._iAirTemperatureMin= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iTempMin
        {
            get { return this._iTempMin; }
            set { this._iTempMin= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iGlobalSolarRadiation
        {
            get { return this._iGlobalSolarRadiation; }
            set { this._iGlobalSolarRadiation= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iRadiation
        {
            get { return this._iRadiation; }
            set { this._iRadiation= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iRAIN
        {
            get { return this._iRAIN; }
            set { this._iRAIN= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iCropResidues
        {
            get { return this._iCropResidues; }
            set { this._iCropResidues= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iPotentialSoilEvaporation
        {
            get { return this._iPotentialSoilEvaporation; }
            set { this._iPotentialSoilEvaporation= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iLeafAreaIndex
        {
            get { return this._iLeafAreaIndex; }
            set { this._iLeafAreaIndex= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double[] SoilTempArray
        {
            get { return this._SoilTempArray; }
            set { this._SoilTempArray= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double[] iSoilTempArray
        {
            get { return this._iSoilTempArray; }
            set { this._iSoilTempArray= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iSoilWaterContent
        {
            get { return this._iSoilWaterContent; }
            set { this._iSoilWaterContent= value; }
        }
    /// <summary>
    ///
    /// </summary>
    public double iSoilSurfaceTemperature
        {
            get { return this._iSoilSurfaceTemperature; }
            set { this._iSoilSurfaceTemperature= value; }
        }
}
