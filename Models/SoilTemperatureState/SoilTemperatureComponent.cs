using Models.Core;
using Models.Utilities;
using System;
namespace Models.Crop2ML;

/// <summary>
/// 
/// </summary>
public class SoilTemperatureComponent
{
     /// <summary>
     /// 
     /// </summary>
     public SoilTemperatureComponent() { }

    //Declaration of the associated strategies
    SnowCoverCalculator _SnowCoverCalculator = new SnowCoverCalculator();
    STMPsimCalculator _STMPsimCalculator = new STMPsimCalculator();

    /// <summary>
    /// 
    /// </summary>
    public double cCarbonContent
    {
        get
        {
             return _SnowCoverCalculator.cCarbonContent; 
        }
        set
        {
            _SnowCoverCalculator.cCarbonContent = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cAlbedo
    {
        get
        {
             return _SnowCoverCalculator.Albedo; 
        }
        set
        {
            _SnowCoverCalculator.Albedo = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double Albedo
    {
        get
        {
             return _SnowCoverCalculator.Albedo; 
        }
        set
        {
            _SnowCoverCalculator.Albedo = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double[] cSoilLayerDepth
    {
        get
        {
             return _STMPsimCalculator.cSoilLayerDepth; 
        }
        set
        {
            _STMPsimCalculator.cSoilLayerDepth = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cFirstDayMeanTemp
    {
        get
        {
             return _STMPsimCalculator.cFirstDayMeanTemp; 
        }
        set
        {
            _STMPsimCalculator.cFirstDayMeanTemp = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cAverageGroundTemperature
    {
        get
        {
            return _STMPsimCalculator.cAVT;
        }
        set
        {
            _STMPsimCalculator.cAVT = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cAVT
    {
        get
        {
             return _STMPsimCalculator.cAVT;
        }
        set
        {
            _STMPsimCalculator.cAVT = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cAverageBulkDensity
    {
        get
        {
             return _STMPsimCalculator.cABD; 
        }
        set
        {
            _STMPsimCalculator.cABD = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cABD
    {
        get
        {
             return _STMPsimCalculator.cABD; 
        }
        set
        {
            _STMPsimCalculator.cABD = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public double cDampingDepth
    {
        get
        {
             return _STMPsimCalculator.cDampingDepth; 
        }
        set
        {
            _STMPsimCalculator.cDampingDepth = value;
        }
    }

    /// <summary>
    /// </summary>
    public void CalculateModel(SoilTemperatureState s, SoilTemperatureState s1, SoilTemperatureRate r, SoilTemperatureAuxiliary a, SoilTemperatureExogenous ex)
    {
        Albedo = cAlbedo;
        ex.iTempMax = ex.iAirTemperatureMax;
        ex.iTempMin = ex.iAirTemperatureMin;
        ex.iRadiation = ex.iGlobalSolarRadiation;
        ex.iSoilTempArray = s.SoilTempArray;
        cAVT = cAverageGroundTemperature;
        cABD = cAverageBulkDensity;
        _SnowCoverCalculator.CalculateModel(s,s1, r, a, ex);
        ex.iSoilSurfaceTemperature = s.SoilSurfaceTemperature;
        _STMPsimCalculator.CalculateModel(s,s1, r, a, ex);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Init(SoilTemperatureState s, SoilTemperatureState s1, SoilTemperatureRate r, SoilTemperatureAuxiliary a, SoilTemperatureExogenous ex)
    {
        _STMPsimCalculator.Init(s, s1, r, a, ex);
        _SnowCoverCalculator.Init(s, s1, r, a, ex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toCopy"></param>
    public SoilTemperatureComponent(SoilTemperatureComponent toCopy): this() // copy constructor 
    {

        cCarbonContent = toCopy.cCarbonContent;
        cAlbedo = toCopy.cAlbedo;
        Albedo = toCopy.Albedo;
        
            for (int i = 0; i < 100; i++)
            { cSoilLayerDepth[i] = toCopy.cSoilLayerDepth[i]; }
    
        cFirstDayMeanTemp = toCopy.cFirstDayMeanTemp;
        cAverageGroundTemperature = toCopy.cAverageGroundTemperature;
        cAVT = toCopy.cAVT;
        cAverageBulkDensity = toCopy.cAverageBulkDensity;
        cABD = toCopy.cABD;
        cDampingDepth = toCopy.cDampingDepth;
    }
}