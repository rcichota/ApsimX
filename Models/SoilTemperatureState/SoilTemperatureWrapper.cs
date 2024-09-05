using APSIM.Shared.Utilities;
using Models.Climate;
using Models.Core;
using Models.Interfaces;
using Models.PMF;
using Models.Soils;
using Models.Surface;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Models.Crop2ML;

/// <summary>
/// 
/// </summary>
[Serializable]
[PresenterName("UserInterface.Presenters.PropertyPresenter")]
[ViewName("UserInterface.Views.PropertyView")]
[ValidParent(ParentType = typeof(Zone))]
public class SoilTemperatureWrapper : Model
{
    [Link] Clock clock = null;
    [Link] Weather weather = null;
    [Link] IPlant crop = null;
    [Link] SurfaceOrganicMatter SurfaceOM = null;
    [Link] IPhysical soilPhysical = null;
    [Link] Organic soilOrganic = null;
    [Link] ISoilWater waterBalance = null;

    private SoilTemperatureState s;
    private SoilTemperatureState s1;
    private SoilTemperatureRate r;
    private SoilTemperatureAuxiliary a;
    private SoilTemperatureExogenous ex;
    private SoilTemperatureComponent soiltemperatureComponent;
    private STMPsimCalculator sTMPsimCalculator;
    private SnowCoverCalculator snowCoverCalculator;

    /// <summary>
    /// 
    /// </summary>
    public SoilTemperatureWrapper()
    {
        s = new SoilTemperatureState();
        s1 = new SoilTemperatureState();
        r = new SoilTemperatureRate();
        a = new SoilTemperatureAuxiliary();
        ex = new SoilTemperatureExogenous();
        soiltemperatureComponent = new SoilTemperatureComponent();
        sTMPsimCalculator = new STMPsimCalculator();
        snowCoverCalculator = new SnowCoverCalculator();
    }

    /// <summary>
    /// Gets and sets the initial value for damping depth of soil
    /// </summary>
    [Description("Initial value for damping depth of soil")]
    [Units("m")]
    //[Crop2ML(datatype="double", min=1.5, max=20.0, default=6.0, parametercategory="constant", inputtype="parameter")]
    public double cDampingDepth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double[] rSoilTempArrayRate{ get { return s.rSoilTempArrayRate;}}

    /// <summary>
    /// 
    /// </summary>
    public double SnowWaterContent{ get { return s.SnowWaterContent;}}

    /// <summary>
    /// 
    /// </summary>
    public double SoilSurfaceTemperature{ get { return s.SoilSurfaceTemperature;}}

    /// <summary>
    /// 
    /// </summary>
    public int AgeOfSnow{ get { return s.AgeOfSnow;}}

    /// <summary>
    /// 
    /// </summary>
    public double[] SoilTempArray{ get { return s.SoilTempArray;}}

    /// <summary>
    /// 
    /// </summary>
    public double rSnowWaterContentRate{ get { return r.rSnowWaterContentRate;}}

    /// <summary>
    /// 
    /// </summary>
    public double rSoilSurfaceTemperatureRate{ get { return r.rSoilSurfaceTemperatureRate;}}

    /// <summary>
    /// 
    /// </summary>
    public int rAgeOfSnowRate{ get { return r.rAgeOfSnowRate;}}

    /// <summary>
    /// 
    /// </summary>
    public double SnowIsolationIndex{ get { return a.SnowIsolationIndex;}}


    /// <summary>
    /// 
    /// </summary>
    public SoilTemperatureWrapper(SoilTemperatureWrapper toCopy, bool copyAll) : this()
    {
        s = (toCopy.s != null) ? new SoilTemperatureState(toCopy.s, copyAll) : null;
        r = (toCopy.r != null) ? new SoilTemperatureRate(toCopy.r, copyAll) : null;
        a = (toCopy.a != null) ? new SoilTemperatureAuxiliary(toCopy.a, copyAll) : null;
        ex = (toCopy.ex != null) ? new SoilTemperatureExogenous(toCopy.ex, copyAll) : null;
        if (copyAll)
        {
            soiltemperatureComponent = (toCopy.soiltemperatureComponent != null) ? new SoilTemperatureComponent(toCopy.soiltemperatureComponent) : null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Init()
    {
        setExogenous();
        ex.iSoilTempArray = new double[soilPhysical.Thickness.Length];
        loadParameters();
        soiltemperatureComponent.Init(s, s1, r, a, ex);
    }

    private void loadParameters()
    {
        soiltemperatureComponent.cCarbonContent = soilOrganic.Carbon[0];
        soiltemperatureComponent.cAlbedo = waterBalance.Salb;
        soiltemperatureComponent.Albedo = soiltemperatureComponent.cAlbedo;
        soiltemperatureComponent.cSoilLayerDepth = MathUtilities.Divide_Value(soilPhysical.Thickness, 1000.0);
        soiltemperatureComponent.cFirstDayMeanTemp = weather.MeanT;
        soiltemperatureComponent.cAverageGroundTemperature = weather.Tav;
        soiltemperatureComponent.cAVT = weather.Tav;
        soiltemperatureComponent.cAverageBulkDensity = MathUtilities.Multiply(soilPhysical.BD, soilPhysical.Thickness).Sum() / soilPhysical.Thickness.Sum();
        soiltemperatureComponent.cABD = soiltemperatureComponent.cAverageBulkDensity;
        soiltemperatureComponent.cDampingDepth = cDampingDepth;
    }

    private void setExogenous()
    {
        ex.iAirTemperatureMax = weather.MaxT;
        ex.iAirTemperatureMin = weather.MinT;
        ex.iGlobalSolarRadiation = weather.Radn;
        ex.iRAIN = weather.Rain;
        ex.iCropResidues = SurfaceOM.Wt;
        ex.iPotentialSoilEvaporation = waterBalance.Eos;
        ex.iLeafAreaIndex = (crop as Plant).LAI;
        ex.iSoilWaterContent = waterBalance.SWmm.Sum();
    }

    [EventSubscribe("Crop2MLProcess")]
    private void CalculateModel(object sender, EventArgs e)
    {
        if (clock.Today == clock.StartDate)
        {
            Init();
        }

        setExogenous();
        soiltemperatureComponent.CalculateModel(s, s1, r, a, ex);
    }

}