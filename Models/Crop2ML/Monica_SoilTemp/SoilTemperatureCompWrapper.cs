using APSIM.Shared.Utilities;
using Models.Climate;
using Models.Core;
using Models.Interfaces;
using Models.PMF;
using Models.Soils;
using Models.Surface;
using Models.WaterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Factorial;

namespace Models.Crop2ML.Monica_SoilTemp;

/// <summary>
///  This class encapsulates the SoilTemperatureCompComponent
/// </summary>
[Serializable]
[PresenterName("UserInterface.Presenters.PropertyPresenter")]
[ViewName("UserInterface.Views.PropertyView")]
[ValidParent(ParentType = typeof(Zone))]
[ValidParent(ParentType = typeof(CompositeFactor))]

public class SoilTemperatureCompWrapper :  Model, ISoilTemperature
{
    [Link] Clock clock = null;
    [Link] Weather weather = null;
    [Link] Physical physical = null;
    [Link] Water water = null;
    [Link] Organic organic = null;

    /// <summary>Event invoke when the soil temperature has changed</summary>
    public event EventHandler SoilTemperatureChanged;

    private SoilTemperatureCompState s;
    private SoilTemperatureCompState s1;
    private SoilTemperatureCompRate r;
    private SoilTemperatureCompAuxiliary a;
    private SoilTemperatureCompExogenous ex;
    private SoilTemperatureCompComponent soiltemperaturecompComponent;

    /// <summary>
    ///  The constructor of the Wrapper of the SoilTemperatureCompComponent
    /// </summary>
    public SoilTemperatureCompWrapper()
    {
        s = new SoilTemperatureCompState();
        s1 = new SoilTemperatureCompState();
        r = new SoilTemperatureCompRate();
        a = new SoilTemperatureCompAuxiliary();
        ex = new SoilTemperatureCompExogenous();
        soiltemperaturecompComponent = new SoilTemperatureCompComponent();
    }

    /// <summary>
    ///  The get method of the soilSurfaceTemperature output variable
    /// </summary>
    [Description("soilSurfaceTemperature")]
    [Units("°C")]
    public double soilSurfaceTemperature{ get { return s.soilSurfaceTemperature;}}


    /// <summary>
    ///  The get method of the soilTemperature next day output variable
    /// </summary>
    [Description("soilTemperature next day")]
    [Units("°C")]
    public double[] soilTemperature
    {
        get
        {
            double[] monicaSoilThickness = Enumerable.Repeat(50.0, 44).ToArray();
            double[] result = SoilUtilities.MapConcentration(s.soilTemperature, monicaSoilThickness, physical.Thickness, double.NaN);
            return result;
        }
    }

    /// <summary>
    ///  The get method of the soilSurfaceTemperature output variable
    /// </summary>
    [Description("soilSurfaceTemperature")]
    [Units("°C")]
    public double SoilSurfaceTemperature{ get { return s.soilSurfaceTemperature;}}

    /// <summary>
    ///
    /// </summary>
    public double AverageSoilSurfaceTemperature => SoilSurfaceTemperature;

    /// <summary>
    ///
    /// </summary>
    public double MinimumSoilSurfaceTemperature => double.NaN;

    /// <summary>
    ///
    /// </summary>
    public double MaximumSoilSurfaceTemperature => double.NaN;

    /// <summary>
    ///
    /// </summary>
    public double[] AverageSoilTemperature => soilTemperature;

    /// <summary>
    ///
    /// </summary>
    public double[] MinimumSoilTemperature => Enumerable.Repeat(double.NaN, Value.Length).ToArray();

    /// <summary>
    ///
    /// </summary>
    public double[] MaximumSoilTemperature => Enumerable.Repeat(double.NaN, Value.Length).ToArray();

    /// <summary>
    ///  The get method of the soilTemperature next day output variable
    /// </summary>
    public double[] Value => AverageSoilTemperature;


    /// <summary>
    ///  The Constructor copy of the wrapper of the SoilTemperatureCompComponent
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoilTemperatureCompWrapper(SoilTemperatureCompWrapper toCopy, bool copyAll)
    {
        s = (toCopy.s != null) ? new SoilTemperatureCompState(toCopy.s, copyAll) : null;
        r = (toCopy.r != null) ? new SoilTemperatureCompRate(toCopy.r, copyAll) : null;
        a = (toCopy.a != null) ? new SoilTemperatureCompAuxiliary(toCopy.a, copyAll) : null;
        ex = (toCopy.ex != null) ? new SoilTemperatureCompExogenous(toCopy.ex, copyAll) : null;
        if (copyAll)
        {
            soiltemperaturecompComponent = (toCopy.soiltemperaturecompComponent != null) ? new SoilTemperatureCompComponent(toCopy.soiltemperaturecompComponent) : null;
        }
    }

    /// <summary>
    ///  The Initialization method of the wrapper of the SoilTemperatureCompComponent
    /// </summary>
    public void Init(){
        setExogenous();
        loadParameters();
        soiltemperaturecompComponent.Init(s, s1, r, a, ex);
    }

    /// <summary>
    ///  Load parameters of the wrapper of the SoilTemperatureCompComponent
    /// </summary>
    private void loadParameters()
    {
        double[] monicaSoilThickness = Enumerable.Repeat(50.0, 42).ToArray();
        soiltemperaturecompComponent.dampingFactor = 0.8;
        soiltemperaturecompComponent.timeStep = 1.0;
        soiltemperaturecompComponent.soilMoistureConst = SoilUtilities.MapConcentration(water.Volumetric, physical.Thickness, monicaSoilThickness, double.NaN);
        soiltemperaturecompComponent.baseTemp = 9.5;
        soiltemperaturecompComponent.initialSurfaceTemp = 10.0;
        soiltemperaturecompComponent.densityAir = 1.25;
        soiltemperaturecompComponent.specificHeatCapacityAir = 1005.0;
        soiltemperaturecompComponent.densityHumus = 1300;
        soiltemperaturecompComponent.specificHeatCapacityHumus = 1920;
        soiltemperaturecompComponent.densityWater = 1000;
        soiltemperaturecompComponent.specificHeatCapacityWater = 4192;
        soiltemperaturecompComponent.quartzRawDensity = 2650;
        soiltemperaturecompComponent.specificHeatCapacityQuartz = 750;
        soiltemperaturecompComponent.nTau = 0.65;
        soiltemperaturecompComponent.noOfTempLayers = 44;
        soiltemperaturecompComponent.noOfTempLayersPlus1 = 45;
        soiltemperaturecompComponent.noOfSoilLayers = monicaSoilThickness.Length;
        soiltemperaturecompComponent.soilBulkDensity = SoilUtilities.MapConcentration(physical.BD, physical.Thickness, monicaSoilThickness, double.NaN);
        soiltemperaturecompComponent.saturation = SoilUtilities.MapConcentration(physical.SAT, physical.Thickness, monicaSoilThickness, double.NaN);
        soiltemperaturecompComponent.soilOrganicMatter = SoilUtilities.MapConcentration(organic.Carbon, physical.Thickness, monicaSoilThickness, double.NaN);
        monicaSoilThickness = Enumerable.Repeat(50.0, 44).ToArray();
        soiltemperaturecompComponent.layerThickness = MathUtilities.Divide_Value(monicaSoilThickness, 1000.0);
    }

    /// <summary>
    ///  Set exogenous variables of the wrapper of the SoilTemperatureCompComponent
    /// </summary>
    private void setExogenous()
    {
        ex.tmin = weather.MinT;
        ex.tmax = weather.MaxT;
        ex.globrad = weather.Radn;
        ex.soilCoverage = 0;
        ex.soilSurfaceTemperatureBelowSnow = 0.0;
        ex.hasSnowCover = false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [EventSubscribe("Crop2MLProcess")]
    public void CalculateModel(object sender, EventArgs e)
    {
        if (clock.Today == clock.StartDate)
        {
            Init();
        }

        setExogenous();
        soiltemperaturecompComponent.CalculateModel(s,s1, r, a, ex);
        SoilTemperatureChanged?.Invoke(this, EventArgs.Empty);
    }

}